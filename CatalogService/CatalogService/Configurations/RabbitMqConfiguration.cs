namespace CatalogService.Configurations
{
    public class RabbitMqConfiguration
    {
        public string Host { get; set; }
        public ushort Port { get; set; }
        public string VHost { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string ProductsFromCatalogQueue { get; set; }
    }
}
