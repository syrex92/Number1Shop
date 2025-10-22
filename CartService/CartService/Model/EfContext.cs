using Microsoft.EntityFrameworkCore;

namespace Shop.CartService.Model;

internal class EfContext(DbContextOptions<EfContext> options) : DbContext(options)
{
    public static void SeedData(DbContext context)
    {
        context.Set<Cart>().AddRange(FakeCartData.Carts);
        context.SaveChanges();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .Entity<Cart>()
            .HasMany(x => x.CartItems)
            .WithOne();

        modelBuilder
            .Entity<Cart>()
            .Navigation(x => x.CartItems)
            .AutoInclude();
    }
}