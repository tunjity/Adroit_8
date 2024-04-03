
using MongoDB.Bson.Serialization.Attributes;

namespace Adroit_v8.MongoConnections.CustomerCentric
{
    [BsonIgnoreExtraElements]
    [BsonCollection("MobileAppWalletToBankTransferCollection")]
    public class CustomerCentricWalleToBankTransfer : CustomerCentricBaseDtoII
    {
        public string TransactionId { get; set; }
        public string TransactionReference { get; set; }
        public long PayerCustomerId { get; set; }
        public string PayerWalletAccountNumber { get; set; }
        public string BeneficiaryAccountNumber { get; set; }
        public string BankCode { get; set; }
        public string Amount { get; set; }
        public string TransactionDescription { get; set; }
        public bool IsSavedAsBeneficiary { get; set; }
        public object TransactionPin { get; set; }
        public bool IsProcessed { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public string TransferType { get; set; }
    } 
    [BsonIgnoreExtraElements]
    [BsonCollection("MobileAppWalletToBankTransferStatusCollection")]
    public class CustomerCentricWalleToBankTransferStatus : CustomerCentricBaseDtoII
    {
        public string TransactionId { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
    }
}
