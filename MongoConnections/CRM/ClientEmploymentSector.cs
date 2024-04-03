namespace Adroit_v8.MongoConnections.CRM
{
    [BsonCollection("AdroitCRMClientEmploymentSector")]
    public class ClientEmploymentSector : BaseDtoII
    {
        public string HasBVN { get; set; }
        public string EmploymentSector { get; set; }
    }
}
