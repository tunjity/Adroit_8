namespace Adroit_v8.MongoConnections.Models
{
    [BsonCollection("AdroitBridgeLoanGeneralSetUpDisbursementStatus")]
    public class GeneralSetUpDisbursementStatus : BaseDtoII
    {

        public string DocName { get; set; }
        public string Status { get; set; }
    }
}
