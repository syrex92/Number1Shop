using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersService.Interfaces
{
    public interface IProductInfo
    {
        public Guid Id { get; set; }
        public int StockQuantity { get; set; }
        public string ImageUrl { get; set; }
        public string ProductTitle { get; set; }
        public string ProductDescription { get; set; }
        public string ProductCategory { get; set; }
        public int Price { get; set; }
        public int Article { get; set; }
    }
}