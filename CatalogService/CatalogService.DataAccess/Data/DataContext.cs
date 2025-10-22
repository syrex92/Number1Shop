using CatalogService.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.DataAccess.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> opts) : base(opts) { }

        public const string DefaultDefaultSchema = "PRODUCTS";

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(DefaultDefaultSchema);

            modelBuilder.Entity<Product>(b =>
            {
                b.HasKey(a => a.Id);
                b.Property(a => a.Name).IsRequired();
                b.Property(a => a.Price).IsRequired();
                b.Property(a => a.CreatedAt).IsRequired();
                b.Property(a => a.UpdatedAt);
                b.Property(a => a.Price).IsRequired();
                b.Property(a => a.Description);
                b.HasMany(a => a.ProductImages).WithOne(pi => pi.Product).HasForeignKey(pi => pi.ProductId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ProductImage>(b =>
            {
                b.HasKey(a => a.Id);
                b.Property(a => a.ImageUrl).IsRequired();
            });


            modelBuilder.Entity<Category>(b =>
            {
                b.HasKey(a => a.Id);
                b.Property(a => a.Name).IsRequired();
                b.HasMany(c => c.Products).WithOne(p => p.Category).HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.Restrict);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
