namespace Adroit_v8.MongoConnections.Models
{
    [BsonCollection("AdroitBridgeLoanDocumentationDoc")]
    public class DocumentationDoc : BaseDtoII
    {
        public string DocumentationUniqueId { get; set; }
        public string DocumentationDocumentString{ get; set; }
    }
}
