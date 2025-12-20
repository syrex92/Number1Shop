using System.Text.Json.Serialization;

namespace Shop.Core.Messages;

public abstract record MessageBase
{
}

public record ProductMessage : MessageBase
{
    [JsonInclude]
    public Guid ProductId { get; set; }
    
    [JsonInclude]
    public int QuantityInStock { get; set; }
    
    [JsonInclude]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ProductEventType EventType { get; set; }

    // private ProductMessage()
    // {
    //     ProductId = Guid.Empty;
    //     QuantityInStock = 0;
    //     EventType = ProductEventType.QuantityChanged;
    // }
}

public record CatalogProductMessage : MessageBase
{
    [JsonInclude]
    public Guid ProductId { get; set; }

    [JsonInclude]
    public long Article { get; set; }

    [JsonInclude]
    public int Price { get; set; }

    [JsonInclude]
    public string Title { get; set; }

    [JsonInclude]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CatalogProductEventType EventType { get; set; }
}

public enum ProductEventType
{
    QuantityChanged,
    ProductAddedToStock,
    ProductRemovedFromStock,
}

public enum CatalogProductEventType
{
    ProductChangedInCatalog,
    ProductAddedToCatalog,
    ProductRemovedFromCatalog,
}