using Microsoft.AspNetCore.Mvc;
using StorageService.DTO;
using StorageService.Interfaces;
using StorageService.Models;
using System.Security.Claims;

namespace StorageService.Handlers
{
    public static class InventoryRequestsHandler
    {
        // Проверка доступности товара
        public static async Task<IResult> CheckAvailability(Guid productId, [FromQuery] int quantity, IInventoryService inventoryService, HttpContext httpContext)
        {
            try
            {
                var response = await inventoryService.CheckAvailabilityAsync(productId, quantity);
                return Results.Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: $"Internal server error: {ex.Message}",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        // Резервирование товара
        public static async Task<IResult> ReserveStock([FromBody] ReservationRequest request, IInventoryService inventoryService, HttpContext httpContext)
        {
            try
            {
                // Получаем ID пользователя из токена
                var userId = GetUserIdFromClaims(httpContext);
                if (userId == Guid.Empty)
                    return Results.Unauthorized();

                var response = await inventoryService.ReserveStockAsync(request);

                if (!response.Success)
                    return Results.BadRequest(new { error = response.ErrorMessage });

                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: $"Internal server error: {ex.Message}",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        // Подтверждение покупки
        public static async Task<IResult> ConfirmPurchase([FromBody] PurchaseConfirmationRequest request, IInventoryService inventoryService, HttpContext httpContext)
        {
            try
            {
                var userId = GetUserIdFromClaims(httpContext);
                if (userId == Guid.Empty)
                    return Results.Unauthorized();

                var response = await inventoryService.ConfirmPurchaseAsync(request);

                if (!response.Success)
                    return Results.BadRequest(new { error = response.ErrorMessage });

                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: $"Internal server error: {ex.Message}",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        // Отмена резервирования
        public static async Task<IResult> CancelReservation(Guid reservationId, IInventoryService inventoryService, HttpContext httpContext)
        {
            try
            {
                var userId = GetUserIdFromClaims(httpContext);
                if (userId == Guid.Empty)
                    return Results.Unauthorized();

                var success = await inventoryService.CancelReservationAsync(reservationId);

                if (!success)
                    return Results.NotFound(new { error = "Reservation not found or already processed" });

                return Results.Ok(new { message = "Reservation cancelled successfully" });
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: $"Internal server error: {ex.Message}",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        // Получение информации о товаре на складе
        public static async Task<IResult> GetStockItem(Guid productId, IInventoryService inventoryService, HttpContext httpContext)
        {
            try
            {
                var userId = GetUserIdFromClaims(httpContext);
                if (userId == Guid.Empty)
                    return Results.Unauthorized();

                var stockItem = await inventoryService.GetStockItemAsync(productId);

                if (stockItem == null)
                    return Results.NotFound(new { error = "Stock item not found" });

                return Results.Ok(stockItem);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: $"Internal server error: {ex.Message}",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        // Health check
        public static IResult CheckHealth()
        {
            return Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow });
        }

        // Вспомогательный метод для получения ID пользователя из claims
        private static Guid GetUserIdFromClaims(HttpContext httpContext)
        {
            var userIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value;

            if (Guid.TryParse(userIdClaim, out var userId))
                return userId;

            return Guid.Empty;
        }
    }
}

  