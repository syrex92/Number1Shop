using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CartService.Tests;

public class CartServiceFixture : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        base.ConfigureWebHost(builder);
    }
}

// public class InMemoryRepository : IRepository
// {
//     private readonly List<Cart> _carts;
//
//     public InMemoryRepository(List<Cart> carts)
//     {
//         _carts = carts;
//     }
//
//     public Task<Cart?> GetById(Guid addRequestUserId)
//     {
//         return Task.FromResult(_carts.FirstOrDefault(x => x.Id == addRequestUserId));
//     }
//
//     public Task<Cart> Add(Cart cart)
//     {
//         return Task.FromResult(_carts.FirstOrDefault(x => x.Id == cart.Id));
//     }
//
//     public Task Update(Cart cart)
//     {
//         throw new NotImplementedException();
//     }
// }