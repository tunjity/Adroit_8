namespace Adroit_v8.MongoConnections.Models
{
    [BsonCollection("AdroitBridgceLoanDisbursementComment")]
    public class DisbursementComment : BaseDtoII
    {
        public string DisbursementUniqueId { get; set; }
        public string Comment { get; set; }
    }
   
}
