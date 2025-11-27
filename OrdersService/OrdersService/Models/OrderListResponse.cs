namespace OrdersService.Models;

public class OrderListResponse
{
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<Order> Data { get; set; } = new();
}