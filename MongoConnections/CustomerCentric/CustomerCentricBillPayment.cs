using MongoDB.Bson.Serialization.Attributes;
using System.Security.Cryptography;

namespace Adroit_v8.MongoConnections.CustomerCentric
{
    [BsonIgnoreExtraElements]
    [BsonCollection("MobileAppBillsPaymentCollection")]
    public class CustomerCentricPayment : CustomerCentricBaseDtoII
    {
            public long CustomerId { get; set; }
            public string BillersId { get; set; }
            public string BillersName { get; set; }
            public string PaymentCode { get; set; }
            public string CategoryId { get; set; }
            public string CategoryName { get; set; }
            public string CustomerBillsPaymentId { get; set; }
            public string MobileNumber { get; set; }
            public string Amount { get; set; }
            public string Pin { get; set; }
            public int IsDeleted { get; set; }
            public int IsActive { get; set; }
            public int IsCompleted { get; set; }
            public string TransactionReference { get; set; }
            public string Status { get; set; }
       
    }
}
