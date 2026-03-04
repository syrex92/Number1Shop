namespace OrdersService.Interfaces
{
    public interface ICatalogService
    {
        Task<IProductInfo> GetById(Guid productId);
        Task<string> GetTitleByIdWithCache(Guid productId);
    }
}