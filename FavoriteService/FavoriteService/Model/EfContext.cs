using Microsoft.EntityFrameworkCore;

namespace Shop.FavoriteService.Model;

internal class EfContext(DbContextOptions<EfContext> options) : DbContext(options)
{
    public static void SeedData(DbContext context)
    {
        context.Set<FavoriteList>().AddRange(FakeFavoriteData.FavoriteList);
        context.SaveChanges();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .Entity<FavoriteList>()
            .HasMany(x => x.Items)
            .WithOne();

        modelBuilder
            .Entity<FavoriteList>()
            .Navigation(x => x.Items)
            .AutoInclude();
    }
}