using MongoDB.Bson.Serialization.Attributes;

namespace Adroit_v8.MongoConnections.CustomerCentric
{
  
    [BsonIgnoreExtraElements]
    [BsonCollection("MobileAppEscrowPaymentCollection")]
    public class ConsumerCentricEscrow : CustomerCentricBaseDtoII
    {
        public long BuyerId { get; set; }
        public string TransactionReference { get; set; }
        public string BuyerFullName { get; set; }
        public long SellerId { get; set; }
        public string SellerFullName { get; set; }
        public DateTime TransactionDate { get; set; }
        public string BuyOrSell { get; set; }
        public string TransactionAmount { get; set; }
        public string TransactionDescription { get; set; }
        public string ProcessingFee { get; set; }
        public int TransactionStatus { get; set; }
        public string TransactionStatusName { get; set; }
        public string ReversedBy { get; set; }
        public DateTime ReversedDate { get; set; }
        public DateTime DateCompleted { get; set; }
        public string CompletedBy { get; set; }
    }
}
