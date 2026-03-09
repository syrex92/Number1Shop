using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrdersService.Data;
using OrdersService.Interfaces;
using OrdersService.Models;
using OrdersService.Services;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IStorageService _storageService;
    private readonly ICatalogService _catalogService;
    private readonly IUiNotificationPublisher _notifications;

    public OrdersController(AppDbContext db, IStorageService storageService, ICatalogService catalogService, IUiNotificationPublisher notifications)
    {
        _db = db;
        _storageService = storageService;
        _catalogService = catalogService;
        _notifications = notifications;
    }

    [Authorize(AuthenticationSchemes = "Bearer")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Order orderCreate)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid);
        if (userIdClaim == null)
        {
            return Unauthorized("User ID claim not found.");
        }
        
        orderCreate.Id = Guid.NewGuid();
        orderCreate.UserId = Guid.Parse(userIdClaim.Value);
        orderCreate.CreatedAt = DateTime.UtcNow;

        orderCreate.OrderNumber = await _db.Orders.CountAsync() + 1;

        var address = orderCreate.DeliveryAddress;

        orderCreate.DeliveryAddress = await _db.Addresses.FirstOrDefaultAsync(c =>
            c.Appartment == address.Appartment &&
            c.City == address.City &&
            c.Country == address.Country &&
            c.House == address.House &&
            c.PostalCode == address.PostalCode &&
            c.Street == address.Street
        ) ?? address;

        orderCreate.DeliveryAddressId = address.Id;
        
        for (int i = 0; i < orderCreate.Items.Count; i++)
        {
            var stockInfo = await _storageService.GetStockInfo(orderCreate.Items[i].Product);
            orderCreate.Items[i].Cost = stockInfo.PurchasePrice;
        }

        Guid reservationId;

        try
        {
            reservationId = await _storageService.Reserve(orderCreate);            
        } catch (Exception ex)
        {
            await _notifications.PublishAsync(
                userId: orderCreate.UserId.ToString(),
                type: "stock.reservation_failed",
                title: "Не удалось зарезервировать товары",
                message: ex.Message,
                data: new { orderId = orderCreate.Id, orderNumber = orderCreate.OrderNumber });
            return BadRequest("Failed to reserve items: " + ex.Message);
        }

        try
        {
            await _storageService.ConfirmReservation(orderCreate.Id, reservationId);
        }
        catch (Exception ex)
        {
            await _storageService.CancelReservation(reservationId);
            await _notifications.PublishAsync(
                userId: orderCreate.UserId.ToString(),
                type: "stock.confirm_failed",
                title: "Не удалось подтвердить резерв",
                message: ex.Message,
                data: new { orderId = orderCreate.Id, orderNumber = orderCreate.OrderNumber, reservationId });
            return BadRequest("Failed to confirm reservation: " + ex.Message + ". Reservation has been cancelled. Reservation ID: " + reservationId);
        }

        _db.Orders.Add(orderCreate);
        await _db.SaveChangesAsync();

        await _notifications.PublishAsync(
            userId: orderCreate.UserId.ToString(),
            type: "order.created",
            title: "Заказ создан",
            message: $"Заказ №{orderCreate.OrderNumber} успешно создан.",
            data: new { orderId = orderCreate.Id, orderNumber = orderCreate.OrderNumber });

        return Created();
    }

    [Authorize(AuthenticationSchemes = "Bearer")]
    [HttpGet]
    public async Task<IActionResult> GetAll(int page = 1, int pageSize = 20)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid);
        if (userIdClaim == null)
        {
            return Unauthorized("User ID claim not found.");
        }
        var userId = Guid.Parse(userIdClaim.Value);

        var query = _db.Orders
            .Include(o => o.DeliveryAddress)
            .Include(o => o.Items)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt);

        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return Ok(new OrderListResponse { Total = total, Page = page, PageSize = pageSize, Data = items });
    }

    [Authorize(AuthenticationSchemes = "Bearer")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid);
        if (userIdClaim == null)
        {
            return Unauthorized("User ID claim not found.");
        }
        var userId = Guid.Parse(userIdClaim.Value);

        var order = await _db.Orders
            .Include(o => o.DeliveryAddress)
            .Include(o => o.Items)
            .Where(o => o.UserId == userId)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return NotFound();

        foreach (var item in order.Items)
        {
            var productInfo = await _catalogService.GetById(item.Product);
            item.Name = productInfo.ProductTitle;
        }

        return Ok(order);
    }

    [Authorize(AuthenticationSchemes = "Bearer")]
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] OrderUpdate orderUpdate)
    {
        var order = await _db.Orders
            .Include(o => o.DeliveryAddress)
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return NotFound();

        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid);
        if (userIdClaim == null)
        {
            return Unauthorized("User ID claim not found.");
        }
        var userId = Guid.Parse(userIdClaim.Value);

        if (order.UserId != userId) return Forbid();

        var prevStatus = order.Status;
        if (orderUpdate.Status.HasValue) order.Status = orderUpdate.Status;

        await _db.SaveChangesAsync();

        if (orderUpdate.Status.HasValue)
        {
            await _notifications.PublishAsync(
                userId: order.UserId.ToString(),
                type: "order.status_changed",
                title: "Статус заказа изменён",
                message: $"Статус заказа №{order.OrderNumber}: {prevStatus} → {order.Status}",
                data: new { orderId = order.Id, orderNumber = order.OrderNumber, from = prevStatus, to = order.Status });
        }
        return Ok();
    }

    [Authorize(AuthenticationSchemes = "Bearer")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order == null) return NotFound();

        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid);
        if (userIdClaim == null)
        {
            return Unauthorized("User ID claim not found.");
        }
        var userId = Guid.Parse(userIdClaim.Value);

        if (order.UserId != userId) return Forbid();

        _db.Orders.Remove(order);
        await _db.SaveChangesAsync();

        await _notifications.PublishAsync(
            userId: order.UserId.ToString(),
            type: "order.cancelled",
            title: "Заказ отменён",
            message: $"Заказ №{order.OrderNumber} отменён.",
            data: new { orderId = order.Id, orderNumber = order.OrderNumber });

        return Ok();
    }
}