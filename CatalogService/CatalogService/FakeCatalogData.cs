using CatalogService.Core.Domain.Entities;
using CatalogService.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Shop.Demo.Data;

namespace CatalogService;

/// <summary>
/// Фейковые данные для тестирования
/// </summary>
public class FakeCatalogData
{
    /// <summary>
    /// Sync seeding
    /// </summary>
    /// <param name="context"></param>
    /// <param name="seed"></param>
    public static void SeedData(DbContext context, bool seed)
    {
        if (!seed)
            return;
        
        context.Set<Category>().AddRange(ShopFakeData.Categories.Select(x => new Category { Id = x.Id, Name = x.Name }));
        context.SaveChanges();
        
        context.Set<Product>().AddRange(ShopFakeData.Products.Select(x => 
            new Product
            {
                Id = x.Id, 
                Name = x.Name,
                Price = x.Price,
                Description = x.Description,
                Category = context.Set<Category>().Single(z => z.Id == x.Categories.FirstOrDefault()),
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,    
                StockQuantity = x.StockQuantity,
                Article = x.Article,
                CategoryId = x.Categories.FirstOrDefault(),
                ProductImages = x.ProductImages.Select(s => new ProductImage
                {
                    Id = Guid.NewGuid(),
                    ImageUrl = s
                }).ToList()
            }));
        context.SaveChanges();
        
        // context.Set<Category>().AddRange(ShopFakeData.Categories.Select(x => new Category { Id = x.Id, Name = x.Name }));
        // context.SaveChanges();
        
        
    }


    /// <summary>
    /// Async seeding
    /// </summary>
    /// <param name="context"></param>
    /// <param name="seed"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static Task SeedDataAsync(DbContext context, bool seed, CancellationToken token)
    {
        return Task.Run(() => SeedData(context, seed), token);
    }
}