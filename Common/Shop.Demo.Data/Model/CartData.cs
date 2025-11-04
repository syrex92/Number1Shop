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

    public List<string> ProductImages { get; set; } = [];
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

