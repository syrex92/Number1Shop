using System.Net.Http.Json;
using System.Security.Claims;
using FluentAssertions;
using Shop.CartService.Dto;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using Shop.CartService;
using Xunit.Abstractions;

namespace CartService.Tests;

public class CartServiceTests(CartServiceFixture cartService, ITestOutputHelper outputHelper): IClassFixture<CartServiceFixture>
{
    private static readonly string Secret = "XCAP05H6LoKvbRRa/QkqLNMI7cOHguaRyHzyg7n5qEkGjQmtBhz4SzYh4Fqwjyi3KJHlSXKPwVu2+bXr6CtpgQ==";

    private HttpClient CreateAuthorizedClient()
    {
        var svc = new CartServiceFixture();
        var client = svc.CreateClient();
        var token = GenerateToken("John");
        outputHelper.WriteLine("--- TOKEN ---");
        outputHelper.WriteLine(token);
        outputHelper.WriteLine("--- TOKEN ---");
        
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private static string GenerateToken(string username)
    {
        var key = Convert.FromBase64String(Secret);
        var securityKey = new SymmetricSecurityKey(key);
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Sid, FakeCartData.UserIds.Last().ToString("D"))
            ]),
            Expires = DateTime.UtcNow.AddDays(30),
            SigningCredentials = new SigningCredentials(securityKey,
                SecurityAlgorithms.HmacSha256Signature)
        };
  	     
        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateJwtSecurityToken(descriptor);
        return handler.WriteToken(token);
    }
    
    [Fact]
    [Trait("Category", "Security")]
    public async Task Service_Should_Return_401_If_Token_Not_Valid()
    {
        var client = cartService.CreateClient();
        var token = "invalid_token";
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await client.GetAsync("/");
        response.Should().HaveStatusCode(HttpStatusCode.Unauthorized);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().BeNullOrEmpty();
    }
    
    [Fact]
    [Trait("Category", "Security")]
    public async Task Service_Should_Return_401_If_Token_Not_Set()
    {
        var client = cartService.CreateClient();
        var response = await client.GetAsync("/");
        response.Should().HaveStatusCode(HttpStatusCode.Unauthorized);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().BeNullOrEmpty();
    }
    
    [Fact]
    [Trait("Category", "Security")]
    public async Task Service_Should_Return_Content_If_Token_Valid()
    {
        var client = cartService.CreateClient();
        var token = GenerateToken("John");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await client.GetAsync("/");
        response.Should().BeSuccessful();
        
        var content = await response.Content.ReadFromJsonAsync<CartResponse>();
        content.Should().NotBeNull();
    }
    
    [Fact]
    public async Task Service_Should_Update_Items_Count()
    {
        // Arrange
        var client = CreateAuthorizedClient();
        
        var response = await client.GetAsync("/");
        response.Should().BeSuccessful();
        
        var content = await response.Content.ReadFromJsonAsync<CartResponse>();
        content.Should().NotBeNull();
        var itemsBeforeUpdate = content!.Items.Count();
        
        // Act
        response = await client.PutAsJsonAsync("/", new CartUpdateProductRequest
        {
            ProductId = FakeCartData.ProductIds.First(),
            Quantity = itemsBeforeUpdate + 1
        });
        
        // Assert
        response.Should().BeSuccessful();
        
        response = await client.GetAsync("/");
        response.Should().BeSuccessful();
        
        content = await response.Content.ReadFromJsonAsync<CartResponse>();
        content.Should().NotBeNull();
        content!.Items.Should()
            .NotBeEmpty()
            .And.Contain(x => x.ProductId == FakeCartData.ProductIds.First())
            .And.Subject.Single(x => x.ProductId == FakeCartData.ProductIds.First()).Quantity.Should()
            .Be(itemsBeforeUpdate + 1);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Service_Should_Remove_Product_If_Quantity_Is_Zero_Or_Negative(int quantity)
    {
        var client = CreateAuthorizedClient();
        
        var response = await client.PutAsJsonAsync("/", new CartUpdateProductRequest
        {
            ProductId = FakeCartData.ProductIds.First(),
            Quantity = quantity
        });
        
        response.Should().BeSuccessful();
        
        response = await client.GetAsync("/");
        response.Should().BeSuccessful();
        
        var content = await response.Content.ReadFromJsonAsync<CartResponse>();
        content.Should().NotBeNull();
        content!.Items.Should().NotBeEmpty();
        content.Items.Should().NotContain(x => x.ProductId == FakeCartData.ProductIds.First());
    }
    
    [Fact]
    public async Task Service_Should_Clear_Cart()
    {
        // Arrange
        var client = CreateAuthorizedClient();
        
        // Act
        var response = await client.DeleteAsync("/");
        response.Should().BeSuccessful();
        
        // Assert
        response = await client.GetAsync("/");
        response.Should().BeSuccessful();
        
        var content = await response.Content.ReadFromJsonAsync<CartResponse>();
        content.Should().NotBeNull();
        content!.Items.Should().BeEmpty("После очистки в корзине не должно оставаться товаров");
    }
}