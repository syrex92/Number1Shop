using Microsoft.EntityFrameworkCore;
using StorageService.Models;

namespace StorageService.Repository
{
    public class EfContext(DbContextOptions<EfContext> options) : DbContext(options)
    {
        public DbSet<StockItem> StockItems { get; set; }
        public DbSet<StockReservation> StockReservations { get; set; }
        public DbSet<ReservationDetail> ReservationDetails { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public static void SeedData(DbContext context)
        {
            context.Set<Supplier>().AddRange(FakeStorageData.Suppliers);
            context.Set<StockItem>().AddRange(FakeStorageData.Stocks);
            context.SaveChanges();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<StockItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.ProductId).IsUnique(false);
                entity.HasIndex(e => e.SupplierId).IsUnique(false);

                entity.Property(e => e.SKU).HasMaxLength(100);
                entity.Property(e => e.PurchasePrice).HasPrecision(18, 2);
            });

            // Конфигурация StockReservation
            modelBuilder.Entity<StockReservation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.OrderId).IsUnique();
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.ExpiresAt);

                entity.HasMany(e => e.Details)
                    .WithOne()
                    .HasForeignKey(d => d.ReservationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Конфигурация ReservationDetail
            modelBuilder.Entity<ReservationDetail>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.ReservationId);
                entity.HasIndex(e => e.ProductId);
                entity.HasIndex(e => e.StockItemId);
            });

            // Конфигурация Supplier
            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasIndex(e => e.INN).IsUnique();
                entity.HasIndex(e => e.IsActive);

                entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.INN).HasMaxLength(12);
                entity.Property(e => e.MinimumOrderAmount).HasPrecision(18, 2);
            });
        }
    }
}
