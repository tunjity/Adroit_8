namespace Adroit_v8.MongoConnections.Models
{
    [BsonCollection("AdroitBridgeLoanDocSetup")]
    public class DocSetup : BaseDtoII
    {
        public string DocName { get; set; }
        public string Status { get; set; }
    }
    public class DocSetupFm
    {
        public string Name { get; set; }
        public string CreatedBy{ get; set; }
        public string Status { get; set; }
    }
    public class DocSetupUpdateFm
    {
        public string UniqueId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
    }
}
