using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComeMyFishMarket.Models
{
    public class ShoppingCart
    {
        public int ShoppingCartID { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public string ProductImage { get; set; }
        public string CustomerId { get; set; }
        public string SellerId { get; set; }
    }
}
