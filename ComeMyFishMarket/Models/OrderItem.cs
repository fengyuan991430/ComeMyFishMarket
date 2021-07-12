using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComeMyFishMarket.Models
{
    public class OrderItem
    {
        public int OrderItemID { get; set; }
        public string ItemName { get; set; }
        public int ItemQuantity { get; set; }
        public double TotalPrice { get; set; }
        public int ItemID { get; set; }
        public string UserID { get; set; }
        public int OrderID { get; set; }
    }
}
