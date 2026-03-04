using Microsoft.EntityFrameworkCore;
using StorageService.DTO;
using StorageService.Interfaces;
using StorageService.Models;
using StorageService.Repository;

namespace StorageService.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly EfContext _context;
        private readonly ILogger<InventoryService> _logger;

        public InventoryService(EfContext context, ILogger<InventoryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<StockAvailabilityResponse> CheckAvailabilityAsync(Guid productId, int requestedQuantity)
        {
            var stockItem = await _context.StockItems.FirstOrDefaultAsync(x => x.ProductId == productId);

            if (stockItem == null)
            {
                return new StockAvailabilityResponse
                {
                    IsAvailable = false,
                    AvailableQuantity = 0,
                    ProductId = productId,
                    RequestedQuantity = requestedQuantity,
                };
            }

            // Вычитаем уже зарезервированные товары
            var reservationDetails = await _context.ReservationDetails.ToListAsync();

            var reservedQuantity = reservationDetails?.Where(rd => rd.ProductId == productId
                          && !rd.IsConfirmed
                          && !rd.IsCancelled).Sum(rd => rd.Quantity) ?? 0;

            var available = stockItem.TotalQuantity - reservedQuantity;

            return new StockAvailabilityResponse
            {
                IsAvailable = available >= requestedQuantity,
                AvailableQuantity = available,
                ProductId = productId,
                RequestedQuantity = requestedQuantity,
            };
        }

        public async Task<ReservationResponse> ReserveStockAsync(ReservationRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Устанавливаем срок действия 30 минут часа от текущего момента UTC
                var expiresAt = DateTime.UtcNow.AddMinutes(30);

                var reservation = new StockReservation
                {
                    OrderId = request.OrderId,
                    ExpiresAt = expiresAt,
                    Status = ReservationStatus.Reserved
                };

                var reservedItems = new List<ReservedItem>();
                var reservationDetails = new List<ReservationDetail>();

                foreach (var item in request.Items)
                {
                    // Проверяем доступность
                    var availability = await CheckAvailabilityAsync(item.ProductId, item.Quantity);
                    if (!availability.IsAvailable)
                    {
                        await transaction.RollbackAsync();

                        return new ReservationResponse
                        {
                            Success = false,
                            ErrorMessage = $"Недостаточно товара {item.ProductId}. " +
                                          $"Запрошено: {item.Quantity}, доступно: {availability.AvailableQuantity}"
                        };
                    }

                    // Находим товар на складе
                    var stockItem = await _context.StockItems
                        .FirstOrDefaultAsync(s => s.ProductId == item.ProductId
                                               && s.Condition == StockItemCondition.New);

                    if (stockItem == null)
                    {
                        await transaction.RollbackAsync();
                        return new ReservationResponse
                        {
                            Success = false,
                            ErrorMessage = $"Товар {item.ProductId} не найден на складе"
                        };
                    }

                    // Создаем деталь резервирования
                    var detail = new ReservationDetail
                    {
                        ProductId = item.ProductId,
                        StockItemId = stockItem.Id,
                        Quantity = item.Quantity,
                    };

                    reservationDetails.Add(detail);
                    reservedItems.Add(new ReservedItem
                    {
                        ProductId = item.ProductId,
                        ReservedQuantity = item.Quantity,
                        ReservationDetailId = detail.Id
                    });
                }

                reservation.Details = reservationDetails;
                _context.StockReservations.Add(reservation);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Резервирование {ReservationId} создано для заказа {OrderId}",
                    reservation.Id, request.OrderId);

                return new ReservationResponse
                {
                    Success = true,
                    ReservationId = reservation.Id,
                    ExpiresAt = reservation.ExpiresAt,
                    ReservedItems = reservedItems
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Ошибка при резервировании для заказа {OrderId}", request.OrderId);
                return new ReservationResponse
                {
                    Success = false,
                    ErrorMessage = $"Внутренняя ошибка сервера: {ex.Message}"
                };
            }
        }

        public async Task<PurchaseConfirmationResponse> ConfirmPurchaseAsync(PurchaseConfirmationRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Находим резервирование
                var reservation = await _context.StockReservations
                    .Include(r => r.Details)
                    .FirstOrDefaultAsync(r => r.OrderId == request.OrderId
                                           && r.Status == ReservationStatus.Reserved);

                if (reservation == null)
                {
                    return new PurchaseConfirmationResponse
                    {
                        Success = false,
                        OrderId = request.OrderId,
                        ErrorMessage = "Резервирование не найдено или уже обработано"
                    };
                }

                // Проверяем срок действия
                if (reservation.ExpiresAt < DateTime.UtcNow)
                {
                    // Отменяем просроченное резервирование
                    reservation.Status = ReservationStatus.Expired;
                    await _context.SaveChangesAsync();

                    return new PurchaseConfirmationResponse
                    {
                        Success = false,
                        OrderId = request.OrderId,
                        ErrorMessage = "Срок резервирования истек"
                    };
                }

                // Обновляем товары на складе
                foreach (var detail in reservation.Details)
                {
                    var stockItem = await _context.StockItems.FindAsync(detail.StockItemId);
                    if (stockItem != null)
                    {
                        // Списываем товар
                        stockItem.TotalQuantity -= detail.Quantity;
                        stockItem.UpdatedAt = DateTime.UtcNow;
                    }

                    // Помечаем деталь как подтвержденную
                    detail.IsConfirmed = true;
                    detail.ConfirmedAt = DateTime.UtcNow;
                }

                // Обновляем статус резервирования
                reservation.Status = ReservationStatus.Confirmed;
                reservation.ConfirmedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Покупка подтверждена для заказа {OrderId}, резервирование {ReservationId}",
                    request.OrderId, reservation.Id);

                // Формируем ответ
                var confirmedItems = reservation.Details.Select(d => new ConfirmedItem
                {
                    ProductId = d.ProductId,
                    Quantity = d.Quantity,
                }).ToList();

                return new PurchaseConfirmationResponse
                {
                    Success = true,
                    OrderId = request.OrderId,
                    ConfirmedItems = confirmedItems
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Ошибка при подтверждении покупки для заказа {OrderId}", request.OrderId);
                return new PurchaseConfirmationResponse
                {
                    Success = false,
                    OrderId = request.OrderId,
                    ErrorMessage = $"Внутренняя ошибка сервера: {ex.Message}"
                };
            }
        }

        public async Task<bool> CancelReservationAsync(Guid reservationId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var reservation = await _context.StockReservations
                    .Include(r => r.Details)
                    .FirstOrDefaultAsync(r => r.Id == reservationId&& r.Status == ReservationStatus.Reserved);

                if (reservation == null)
                    return false;

                // Помечаем как отмененное
                reservation.Status = ReservationStatus.Cancelled;
                reservation.CancelledAt = DateTime.UtcNow;

                // Помечаем детали как отмененные
                foreach (var detail in reservation.Details)
                {
                    detail.IsCancelled = true;
                    detail.CancelledAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Резервирование {ReservationId} отменено", reservationId);
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Ошибка при отмене резервирования {ReservationId}", reservationId);
                return false;
            }
        }

        public async Task<StockItem?> GetStockItemAsync(Guid productId)
        {
            return await _context.StockItems
                .FirstOrDefaultAsync(s => s.ProductId == productId);
        }
    }
}
