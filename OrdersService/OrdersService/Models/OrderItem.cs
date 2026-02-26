using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OrdersService.Models
{
    public class OrderItem
    {
        [Key]
        [JsonIgnore]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid Product { get; set; }

        [JsonInclude]
        public virtual string Name {
            get { return "Тестовый товар №" + new Random().Next(); }
        }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public int Cost { get; set; }

        [Required]
        [JsonIgnore]
        public Guid OrderId { get; set; }
        [JsonIgnore]
        public Order? Order { get; set; }
    }

}