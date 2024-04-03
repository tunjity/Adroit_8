namespace Adroit_v8.MongoConnections.Models
{
    [BsonCollection("AdroitBridgeLoanDocumentationStatus")]
    public class DocumentationStatus : BaseDtoII
    {
       
            public string DocName { get; set; }
            public string Status { get; set; }
    }
}
