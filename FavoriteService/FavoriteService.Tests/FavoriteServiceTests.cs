using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using Shop.FavoriteService;
using Shop.FavoriteService.Dto;
using Xunit.Abstractions;

namespace FavoriteService.Tests;

public class FavoriteServiceTests(FavoriteServiceFixture favoriteService, ITestOutputHelper outputHelper): IClassFixture<FavoriteServiceFixture>
{
    private const string Secret = "XCAP05H6LoKvbRRa/QkqLNMI7cOHguaRyHzyg7n5qEkGjQmtBhz4SzYh4Fqwjyi3KJHlSXKPwVu2+bXr6CtpgQ==";

    private HttpClient CreateAuthorizedClient()
    {
        var svc = new FavoriteServiceFixture();
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
                new Claim(ClaimTypes.Sid, FakeFavoriteData.UserIds.Last().ToString("D"))
            ]),
            Expires = DateTime.UtcNow.AddMinutes(30),
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
        var client = favoriteService.CreateClient();
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
        var client = favoriteService.CreateClient();
        var response = await client.GetAsync("/");
        response.Should().HaveStatusCode(HttpStatusCode.Unauthorized);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().BeNullOrEmpty();
    }
    
    [Fact]
    [Trait("Category", "Security")]
    public async Task Service_Should_Return_Content_If_Token_Valid()
    {
        var client = favoriteService.CreateClient();
        var token = GenerateToken("John");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await client.GetAsync("/");
        response.Should().BeSuccessful();
        
        var content = await response.Content.ReadFromJsonAsync<FavoriteProductListResponse>();
        content.Should().NotBeNull();
    }
    
    [Fact]
    public async Task Service_Should_Increase_Items_Count()
    {
        // Arrange
        var client = CreateAuthorizedClient();
        
        var response = await client.GetAsync("/");
        response.Should().BeSuccessful();
        
        var content = await response.Content.ReadFromJsonAsync<FavoriteProductListResponse>();
        content.Should().NotBeNull();
        var itemsBeforeUpdate = content!.Items.Count();
        
        // Act
        response = await client.PostAsJsonAsync("/", new  AddFavoriteProductRequest
        {
            ProductId = FakeFavoriteData.ProductIds.Last()
        });
        
        // Assert
        response.Should().BeSuccessful();
        response.Should().HaveStatusCode(HttpStatusCode.Created);
        
        response = await client.GetAsync("/");
        response.Should().BeSuccessful();
        
        content = await response.Content.ReadFromJsonAsync<FavoriteProductListResponse>();
        content.Should().NotBeNull();
        content!.Items.Should()
            .NotBeEmpty()
            .And.Contain(x => x.ProductId == FakeFavoriteData.ProductIds.Last())
            .And.HaveCount(itemsBeforeUpdate + 1);
    }
    
    [Fact]
    public async Task Service_Should_Decrease_Items_Count()
    {
        // Arrange
        var client = CreateAuthorizedClient();
        
        var response = await client.GetAsync("/");
        response.Should().BeSuccessful();
        
        var content = await response.Content.ReadFromJsonAsync<FavoriteProductListResponse>();
        content.Should().NotBeNull();
        var itemsBeforeUpdate = content!.Items.Count();
        var productId = content.Items.First().ProductId;
        
        // Act
        response = await client.DeleteAsync($"/{productId:D}");
        
        // Assert
        response.Should().BeSuccessful();
        response.Should().HaveStatusCode(HttpStatusCode.OK);
        
        response = await client.GetAsync("/");
        response.Should().BeSuccessful();
        
        content = await response.Content.ReadFromJsonAsync<FavoriteProductListResponse>();
        content.Should().NotBeNull();
        content!.Items.Should()
            .NotContain(x => x.ProductId == productId)
            .And.HaveCount(itemsBeforeUpdate - 1);
    }
    
    [Fact]
    public async Task Service_Should_On_Delete_Product_Return_404_If_ProductId_Not_Exists()
    {
        var client = CreateAuthorizedClient();
        
        var response = await client.DeleteAsync($"/{Guid.NewGuid():D}");
        response.Should().HaveStatusCode(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task Service_Should_Return_Not_Allowed_If_ProductId_Not_Valid()
    {
        var client = CreateAuthorizedClient();
        
        var response = await client.GetAsync("/AAAA-BBBB-CCCC");
        response.Should().HaveStatusCode(HttpStatusCode.MethodNotAllowed);
    }
    
    [Fact]
    public async Task Service_Should_Set_Items_Count_To_Zero()
    {
        // Arrange
        var client = CreateAuthorizedClient();
        
        var response = await client.GetAsync("/");
        response.Should().BeSuccessful();
        
        var content = await response.Content.ReadFromJsonAsync<FavoriteProductListResponse>();
        content.Should().NotBeNull();
        content!.Items.Should().NotBeEmpty();
        
        // Act
        response = await client.DeleteAsync("/");
        
        // Assert
        response.Should().BeSuccessful();
        response.Should().HaveStatusCode(HttpStatusCode.OK);
        
        response = await client.GetAsync("/");
        response.Should().BeSuccessful();
        
        content = await response.Content.ReadFromJsonAsync<FavoriteProductListResponse>();
        content.Should().NotBeNull();
        content!.Items.Should().BeEmpty();
    }
}