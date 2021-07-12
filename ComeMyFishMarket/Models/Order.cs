using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComeMyFishMarket.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public string OrderDescription { get; set; }
        public double TotalAmount { get; set; }
        public string OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public string UserID { get; set; }
        public string FyTest { get; set; }
        //command
        public DateTime OrderDate_TEST { get; set; }
    }
}
