namespace Shop.Demo.Data.Model;

public class CartData
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public List<CartItemData> CartItems { get; set; } = [];
}

public class CartItemData
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public int QuantityInStock { get; set; }
}

public class ProductData
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public string Description { get; set; } = string.Empty;

    //public Guid CategoryId { get; set; }

    public Guid[] Categories { get; set; } = [];

    public int Price { get; set; }

    public int StockQuantity { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long Article {  get; set; }

    public string? ProductImageUrl { get; set; }
}

public class CategoryData
{
    public Guid Id { get; set; }

    public required string Name { get; set; }
}

public class UserData
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = [];
}

public class StockData
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProductId { get; set; }
    public int TotalQuantity { get; set; }               
    public string SKU { get; set; } = string.Empty;     
    public DateTime? ManufactureDate { get; set; }       
    public decimal PurchasePrice { get; set; }        
    public Guid? SupplierId { get; set; }              
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

public class SupplierData
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? INN { get; set; }               
    public string? BankDetails { get; set; }
    public int AverageDeliveryDays { get; set; } = 7;
    public decimal? MinimumOrderAmount { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

