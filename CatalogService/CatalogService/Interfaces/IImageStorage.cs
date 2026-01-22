namespace CatalogService.Interfaces
{
    public interface IImageStorage
    {
        Task<string> SaveAsync(IFormFile file);

        void DeleteFile(string filePath);
    }
}
