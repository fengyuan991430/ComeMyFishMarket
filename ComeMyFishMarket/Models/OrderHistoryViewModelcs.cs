using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComeMyFishMarket.Models
{
    public class OrderHistoryViewModelcs
    {
        public int OrderID { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public DateTime OrderDate { get; set; }
        public string SellerID { get; set; }
        public double TotalPrice { get; set; }
        public int ItemID { get; set; }
    }
}
