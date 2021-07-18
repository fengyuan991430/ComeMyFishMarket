using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComeMyFishMarket.Models
{
    public class WalletHistory
    {
        public int ID { get; set; }
        public string HistoryDesc { get; set; }
        public string HistoryAmount { get; set; }
        public DateTime HistoryDate { get; set; }
        public string UserID { get; set; }
    }
}
