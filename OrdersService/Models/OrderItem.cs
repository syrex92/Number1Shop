using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrdersService.Models
{
    public class OrderItem
    {

        [Required]
        public Guid Product { get; set; }

        [Required]
        public int Quantity { get; set; }
    }

}