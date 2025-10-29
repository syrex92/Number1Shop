using System.Security.Claims;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrdersService.Data;
using OrdersService.Models;
using Shouldly;

namespace OrdersService.Tests;

public class OrdersControllerTests : IDisposable
{
    private readonly IFixture _fixture;
    private readonly AppDbContext _dbContext;
    private readonly OrdersController _controller;
    private readonly Guid _userId;

    public OrdersControllerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        
        // Configure AutoFixture to handle circular references
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        // Configure AutoFixture for circular references
        _fixture.Customize<Order>(composer => composer
            .Without(o => o.DeliveryAddress)
            .Without(o => o.Items));
            
        _fixture.Customize<Address>(composer => composer
            .Without(a => a.Order));
            
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"OrdersDb_{Guid.NewGuid()}")
            .Options;
            
        _dbContext = new AppDbContext(options);
        
        // Setup controller with database context
        _controller = new OrdersController(_dbContext);
        
        // Setup user context
        _userId = Guid.NewGuid();
        var claims = new List<Claim>
        {
            new Claim("userId", _userId.ToString())
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
    }
    
    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    [Fact]
    public async Task Create_WithValidOrder_ReturnsCreatedResult()
    {
        // Arrange
        var order = _fixture.Build<Order>()
            .Without(o => o.Id)
            .Without(o => o.UserId)
            .Without(o => o.CreatedAt)
            .With(o => o.DeliveryAddress, _fixture.Create<Address>())
            .Create();

        // Act
        var result = await _controller.Create(order);

        // Assert
        var createdResult = result.ShouldBeOfType<CreatedResult>();
        var savedOrder = await _dbContext.Orders.FirstOrDefaultAsync();
        savedOrder.ShouldNotBeNull();
        savedOrder.UserId.ShouldBe(_userId);
        savedOrder.CreatedAt.ShouldNotBe(default);
    }

    [Fact]
    public async Task GetAll_ReturnsOrdersForUser()
    {
        // Arrange
        var orders = new List<Order>();
        for (int i = 0; i < 3; i++)
        {
            var address = _fixture.Create<Address>();
            var order = _fixture.Build<Order>()
                .With(o => o.UserId, _userId)
                .With(o => o.DeliveryAddress, address)
                .With(o => o.DeliveryAddressId, address.Id)
                .Create();

            address.Order = order;
            orders.Add(order);
        }

        _dbContext.Orders.AddRange(orders);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.ShouldBeOfType<OkObjectResult>();
        var response = okResult.Value.ShouldBeOfType<OrderListResponse>();
        response.Total.ShouldBe(3);
        response.Data.Count.ShouldBe(3);
    }

    [Fact]
    public async Task GetById_WithExistingOrder_ReturnsOrder()
    {
        // Arrange
        var order = _fixture.Build<Order>()
            .With(o => o.UserId, _userId)
            .With(o => o.DeliveryAddress, _fixture.Create<Address>())
            .Create();
            
        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _controller.GetById(order.Id);

        // Assert
        var okResult = result.ShouldBeOfType<OkObjectResult>();
        var returnedOrder = okResult.Value.ShouldBeOfType<Order>();
        returnedOrder.Id.ShouldBe(order.Id);
        returnedOrder.UserId.ShouldBe(_userId);
    }

    [Fact]
    public async Task GetById_WithNonexistentOrder_ReturnsNotFound()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        // Act
        var result = await _controller.GetById(orderId);

        // Assert
        result.ShouldBeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Update_WithValidOrder_ReturnsOkResult()
    {
        // Arrange
        var order = _fixture.Build<Order>()
            .With(o => o.UserId, _userId)
            .With(o => o.DeliveryAddress, _fixture.Create<Address>())
            .Create();
            
        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

    var updateOrder = new Order { Status = OrderStatus.Processing };

        // Act
        var result = await _controller.Update(order.Id, updateOrder);

        // Assert
        result.ShouldBeOfType<OkResult>();
        var updatedOrder = await _dbContext.Orders.FindAsync(order.Id);
        updatedOrder.ShouldNotBeNull();
    updatedOrder.Status.ShouldBe(OrderStatus.Processing);
    }

    [Fact]
    public async Task Update_WithNonexistentOrder_ReturnsNotFound()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        // Act
        var result = await _controller.Update(orderId, new Order());

        // Assert
        result.ShouldBeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Delete_WithExistingOrder_ReturnsOkResult()
    {
        // Arrange
        var order = _fixture.Build<Order>()
            .With(o => o.UserId, _userId)
            .With(o => o.DeliveryAddress, _fixture.Create<Address>())
            .Create();
            
        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _controller.Delete(order.Id);

        // Assert
        result.ShouldBeOfType<OkResult>();
        var deletedOrder = await _dbContext.Orders.FindAsync(order.Id);
        deletedOrder.ShouldBeNull();
    }

    [Fact]
    public async Task Delete_WithNonexistentOrder_ReturnsNotFound()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        // Act
        var result = await _controller.Delete(orderId);

        // Assert
        result.ShouldBeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Delete_WithOrderFromDifferentUser_ReturnsForbid()
    {
        // Arrange
        var order = _fixture.Build<Order>()
            .With(o => o.UserId, Guid.NewGuid()) // Different user ID
            .With(o => o.DeliveryAddress, _fixture.Create<Address>())
            .Create();
            
        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _controller.Delete(order.Id);

        // Assert
        result.ShouldBeOfType<ForbidResult>();
        var orderStillExists = await _dbContext.Orders.FindAsync(order.Id);
        orderStillExists.ShouldNotBeNull();
    }
}