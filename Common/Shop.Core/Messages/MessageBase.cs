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

public enum ProductEventType
{
    QuantityChanged,
    ProductAddedToStock,
    ProductRemovedFromStock,
}