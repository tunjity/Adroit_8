namespace Adroit_v8.MongoConnections.Models
{
    [BsonCollection("AdroitBridgeLoanGeneralSetUpFacilityType")]
    public class GeneralSetUpFacilityType : BaseDtoII
    {

        public string DocName { get; set; }
        public string Status { get; set; }
    }
}
