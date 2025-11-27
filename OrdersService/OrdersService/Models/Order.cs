using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OrdersService.Models
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        [Required]
        [JsonIgnore]
        public Guid DeliveryAddressId { get; set; }

        [Required]
        public Address DeliveryAddress { get; set; } = null!;

        public List<OrderItem> Items { get; set; } = new();

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