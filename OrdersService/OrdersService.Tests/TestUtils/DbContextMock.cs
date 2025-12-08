using Microsoft.EntityFrameworkCore;
using Moq;
using OrdersService.Data;
using OrdersService.Models;

namespace OrdersService.Tests.TestUtils;

public static class DbContextMock
{
    public static Mock<AppDbContext> Create()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"OrdersDb_{Guid.NewGuid()}")
            .Options;

        var dbContextMock = new Mock<AppDbContext>(options);
        return dbContextMock;
    }
}