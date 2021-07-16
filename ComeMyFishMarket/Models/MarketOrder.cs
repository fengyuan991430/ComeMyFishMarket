using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComeMyFishMarket.Models
{
    public class MarketOrder
    {
        public int MarketOrderID { get; set; }
        public string OrderDescription { get; set; }
        public double TotalAmount { get; set; }
        public string OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public string UserID { get; set; }
        public string GetFeedback { get; set; }
    }
}
