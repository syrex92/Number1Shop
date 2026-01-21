using CatalogService.Interfaces;
using System.IO;

namespace CatalogService.Services
{
    public class LocalImageStorage : IImageStorage
    {
        private readonly IWebHostEnvironment _env;
        private readonly string _folderPath;

        public LocalImageStorage(IWebHostEnvironment env)
        {
            _env = env ?? throw new ArgumentNullException(nameof(env));

            // Если WebRootPath null (бывает в Unit-тестах), используем текущую папку

            var root = _env.WebRootPath;
            if (_env.WebRootPath == null)
            {
                root = "wwwroot";
                Directory.CreateDirectory(root);
            }

            _folderPath = Path.Combine(root, "images", "products");
        }

        public void DeleteFile(string filePath)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(filePath)}";
            var fullPath = Path.Combine(_folderPath, fileName);
            if (File.Exists(fullPath)) { File.Delete(fullPath); }
        }

        public async Task<string> SaveAsync(IFormFile file)
        {
            if (!Directory.Exists(_folderPath))
                Directory.CreateDirectory(_folderPath);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var fullPath = Path.Combine(_folderPath, fileName);

            await using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/images/products/{fileName}";
        }
    }
}
