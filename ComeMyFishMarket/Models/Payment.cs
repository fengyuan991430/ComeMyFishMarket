using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComeMyFishMarket.Models
{
    public class Payment
    {
        public int PaymentID { get; set; }
        public string PaymentDescription { get; set; }        
        public double TotalAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string CustomerID { get; set; }
        public string ReceivedBy { get; set; }
    }
}
