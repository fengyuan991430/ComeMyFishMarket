using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComeMyFishMarket.Models
{
    public class FeedbackEntity : TableEntity
    {
        public FeedbackEntity() { }

        //pkey = UserID (customer), rkey = MarketOrderID
        public FeedbackEntity(string CustomerID, string OrderID)
        {
            this.PartitionKey = CustomerID;
            this.RowKey = OrderID;
        }

        //other properties
        public string Feedback_Content { get; set; }
        public DateTime Feedback_Date { get; set; }
        public string Feedback_Reaction { get; set; }
        public string SellerID { get; set; }


    }
}
