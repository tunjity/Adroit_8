namespace Adroit_v8.MongoConnections.Models
{
    [BsonCollection("AdroitBridgeLoanGeneralSetUpTenor")]
    public class GeneralSetUpTenor : BaseDtoII
    {

        public string Name { get; set; }
        public string Status { get; set; }
    }  
    [BsonCollection("AdroitBridgeLoanGeneralSetUpInterestRate")]
    public class GeneralSetUpInterestRate : BaseDtoII
    {

        public string Name { get; set; }
        public string Status { get; set; }
    }
}
