namespace OrdersService.DataAccess
{
    public class Order
    {
        public int Id { get; set; }
        public Dictionary<Product, int> Products { get; }
        public User User { get; set; }
    }
}
