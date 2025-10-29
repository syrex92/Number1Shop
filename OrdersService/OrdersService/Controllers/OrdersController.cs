using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrdersService.Data;
using OrdersService.Models;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _db;

    public OrdersController(AppDbContext db) => _db = db;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Order orderCreate)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId");
        if (userIdClaim == null)
        {
            return Unauthorized("User ID claim not found.");
        }
        orderCreate.UserId = Guid.Parse(userIdClaim.Value);
        orderCreate.CreatedAt = DateTime.UtcNow;

        _db.Orders.Add(orderCreate);
        await _db.SaveChangesAsync();

        return Created();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(int page = 1, int pageSize = 20)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId");
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

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId");
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

        return Ok(order);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Order orderUpdate)
    {
        var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == id);
        if (order == null) return NotFound();

        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId");
        if (userIdClaim == null)
        {
            return Unauthorized("User ID claim not found.");
        }
        var userId = Guid.Parse(userIdClaim.Value);

        if (order.UserId != userId) return Forbid();

        if (!string.IsNullOrEmpty(orderUpdate.Status)) order.Status = orderUpdate.Status;

        await _db.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order == null) return NotFound();

        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId");
        if (userIdClaim == null)
        {
            return Unauthorized("User ID claim not found.");
        }
        var userId = Guid.Parse(userIdClaim.Value);

        if (order.UserId != userId) return Forbid();

        _db.Orders.Remove(order);
        await _db.SaveChangesAsync();
        return Ok();
    }
}