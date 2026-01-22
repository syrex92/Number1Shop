using CatalogService.Interfaces;

namespace CatalogService.Services
{
    public class LocalImageStorage : IImageStorage
    {
        private readonly string _webRootPath;
        private const string ImagesFolder = "images/products";

        public LocalImageStorage(IWebHostEnvironment env)
        {
            var webRoot = env.WebRootPath;

            if (string.IsNullOrEmpty(webRoot) || !Directory.Exists(webRoot))
            {
                webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                Directory.CreateDirectory(webRoot);
            }

            _webRootPath = webRoot;

            Directory.CreateDirectory(Path.Combine(_webRootPath, "images", "products"));
        }

        public async Task<string> SaveAsync(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";

            var physicalPath = Path.Combine(_webRootPath, ImagesFolder, fileName);

            using var stream = new FileStream(physicalPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"{ImagesFolder}/{fileName}";
        }

        public void DeleteFile(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return;

            var relativePath = imageUrl.TrimStart('/')
                                       .Replace('/', Path.DirectorySeparatorChar);

            var physicalPath = Path.Combine(_webRootPath, relativePath);

            if (File.Exists(physicalPath))
            {
                File.Delete(physicalPath);
            }
        }
    }
}
