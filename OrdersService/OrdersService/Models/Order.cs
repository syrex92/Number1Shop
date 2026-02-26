using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;

namespace OrdersService.Models
{
    public class Order
    {
        [Key]
        [SwaggerIgnore]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [SwaggerIgnore]
        public Guid UserId { get; set; }

        [Required]
        [SwaggerIgnore]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        [Required]
        [JsonIgnore]
        public Guid DeliveryAddressId { get; set; }

        [Required]
        public Address DeliveryAddress { get; set; } = null!;

        public List<OrderItem> Items { get; set; } = new();

        [JsonInclude]
        public virtual int OrderNumber
        {
            get
            {
                return (new Random()).Next();
            }
        }

        [JsonInclude]
        public virtual int Cost
        {
            get
            {
                return Items.Sum(item => item.Quantity * item.Cost );
            }
        }

        public OrderStatus? Status { get; set; } = OrderStatus.New;
    }
    public class OrderUpdate
    {
        public OrderStatus? Status { get; set; } = OrderStatus.New;
    }
    public enum OrderStatus
    {
        New,
        Processing,
        Shipping,
        Delivered,
        Cancelled
    }
}