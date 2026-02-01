using System.Globalization;
using System.Reflection;
using Shop.Demo.Data.Model;

namespace Shop.Demo.Data;

public static class ShopFakeData
{
    private static Csv _csv = new();

    private static void LoadProductsCsv()
    {
        try
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "data",
                "products.csv");
            var lines = File.ReadAllLines(path);
            var isHeader = true;

            foreach (var line in lines)
            {
                if (isHeader)
                {
                    _csv.SetHeader(line.Split(';'));
                    isHeader = false;
                    continue;
                }

                _csv.AddValues(line.Split(';'));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to load products csv: " + e);
        }

        Console.WriteLine($"Loaded products csv: {_csv.RowCount}");
        Console.WriteLine($"Loaded products columns: {_csv.ColumnCount}");

        //UpdateKeys();
    }

    /*
    private static void UpdateKeys()
    {
        for (int i = 0; i < _csv.RowCount; i++)
        {
            _csv.Set("product_id", i, Guid.NewGuid().ToString("D"));
        }

        for (int catId = 1; catId <= 5; catId++)
        {
            var categories = _csv.ColumnValues($"cat{catId}").Distinct()
                .ToDictionary(x => x, _ => Guid.NewGuid().ToString("D"));
            for (int i = 0; i < _csv.RowCount; i++)
            {
                var c = _csv.Cell($"cat{catId}", i);
                if (string.IsNullOrEmpty(c))
                    continue;
                _csv.Set($"cat{catId}_id", i, categories[c]);
            }
        }

        var result = new List<string>();
        result.Add(_csv.Columns.Aggregate("", (c, n) => c + n + ";").Trim(';'));

        for (int i = 0; i < _csv.RowCount; i++)
        {
            result.Add(_csv.Row(i).Aggregate("", (c, n) => c + n + ";").Trim(';'));
        }

        File.WriteAllLines("products_updated.csv", result);
        Console.WriteLine("saved");
    }
    */

    static ShopFakeData()
    {
        LoadProductsCsv();

        Users = GenerateUsers();
        Categories = GenerateCategories();
        Products = GenerateProducts();
        Carts = GenerateCartData();
        Suppliers = GenerateSuppliers();
        Stocks = GenerateStocks();
    }

    public static List<CategoryData> Categories { get; }
    public static List<ProductData> Products { get; }
    public static List<CartData> Carts { get; }
    public static List<UserData> Users { get; }
    public static List<StockData> Stocks { get; }
    public static List<SupplierData> Suppliers { get; }

    private static List<UserData> GenerateUsers()
    {
        return
        [
            new()
            {
                Id = Guid.Parse("90C5C68E-01E1-4C80-908E-726151CA96BD"),
                Email = "user@shop.ru",
                PasswordHash = "",
                UserName = "user@shop.ru",
                Roles = ["user"]
            },

            new()
            {
                Id = Guid.Parse("747E2232-60F7-4BAD-B28F-6ADF1CBB8FB6"),
                Email = "admin@shop.ru",
                PasswordHash = "",
                UserName = "admin@shop.ru",
                Roles = ["admin"]
            }
        ];
    }

    private static string GetProductImages(int rowId)
    {
        var images = new List<string>();

        for (int i = 1; i <= 4; i++)
        {
            var img = _csv.Cell($"image{i}", rowId);
            if (string.IsNullOrEmpty(img))
                continue;
            images.Add(img);
        }

        return images[0];
    }

    private static Guid[] GetCategories(int rowId)
    {
        var catId = _csv.Cell($"cat1_id", rowId);
        if (string.IsNullOrEmpty(catId) || !Guid.TryParse(catId, out var cat))
            return [];

        return [cat];
    }

    private static List<ProductData> GenerateProducts()
    {
        var products = new List<ProductData>();
        for (int i = 0; i < _csv.RowCount; i++)
        {
            var product = new ProductData
            {
                Id = Guid.Parse(_csv.Cell("product_id", i)),
                Name = _csv.Cell("name", i),
                Description = _csv.Cell("desc_full", i),
                Price = (int)double.Parse(_csv.Cell("price", i), CultureInfo.InvariantCulture),
                ProductImageUrl = GetProductImages(i),
                Categories = GetCategories(i),
                StockQuantity = (int)double.Parse(_csv.Cell("stock", i), CultureInfo.InvariantCulture),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            products.Add(product);
        }

        return products;
    }


    private static List<StockData>? GenerateStocks()
    {
        var index = Random.Shared.Next(0, 1);
        return Products.Select(x => new StockData()
        {
            ProductId = x.Id,
            TotalQuantity = x.StockQuantity,
            ManufactureDate = DateTime.Today,
            PurchasePrice = x.Price - (x.Price / 2),
            SupplierId = Suppliers[index].Id
        }).ToList();
    }

    private static List<SupplierData>? GenerateSuppliers()
    {
        return new List<SupplierData>
            {
                new SupplierData
                {
                    Id = Guid.NewGuid(),
                    Name = "ТехноПоставка ООО",
                    ContactPerson = "Иванов Иван",
                    Email = "info@techno.ru",
                    Phone = "+79991234567",
                    Address = "ул. Техническая, 123",
                    City = "Москва",
                    Country = "Россия",
                    INN = "7701234567",
                    BankDetails = "АО \"Банк\", р/с 40702810000000012345",
                    AverageDeliveryDays = 5,
                    MinimumOrderAmount = 10000m,
                    IsActive = true
                },
                new SupplierData
                {
                    Id = Guid.NewGuid(),
                    Name = "ЭлектроТорг",
                    ContactPerson = "Петрова Мария",
                    Email = "order@electro.ru",
                    Phone = "+79997654321",
                    Address = "пр. Электронный, 45",
                    City = "Санкт-Петербург",
                    Country = "Россия",
                    INN = "7812345678",
                    AverageDeliveryDays = 7,
                    MinimumOrderAmount = 5000m,
                    IsActive = true
                }
            };
    }

    private static List<CategoryData> GenerateCategories()
    {
        var categories = new List<CategoryData>();


        for (int i = 0; i < _csv.RowCount; i++)
        {
            var id = Guid.Parse(_csv.Cell("cat1_id", i));

            if (categories.Any(x => x.Id == id))
                continue;

            var data = new CategoryData
            {
                Id = id,
                Name = _csv.Cell("cat1", i),
            };

            categories.Add(data);
        }

        return categories;
    }

    private static List<CartData> GenerateCartData()
    {
        var carts = new List<CartData>();

        foreach (var user in Users)
        {
            var cart = new CartData
            {
                Id = user.Id,
                CartItems = GetRandomProducts()
            };

            carts.Add(cart);
        }

        return carts;
    }

    private static List<CartItemData> GetRandomProducts()
    {
        var items = new List<CartItemData>();
        var productsCount = Random.Shared.Next(3, 10);
        for (int i = 0; i < productsCount; i++)
        {
            var item = new CartItemData
            {
                ProductId = Products[i].Id,
                Quantity = Random.Shared.Next(1, Products[i].StockQuantity),
                QuantityInStock = Products[i].StockQuantity
            };
            items.Add(item);
        }
        return items;
    }
}