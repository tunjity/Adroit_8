namespace Adroit_v8.MongoConnections.Models
{
    [BsonCollection("AdroitBridgeLoanGeneralSetUpDocumentationStage")]
    public class GeneralSetUpDocumentationStage : BaseDtoII
    {

        public string DocName { get; set; }
        public string Status { get; set; }
    }
}
