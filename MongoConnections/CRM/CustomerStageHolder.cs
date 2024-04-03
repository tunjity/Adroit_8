namespace Adroit_v8.MongoConnections.CRM
{
    [BsonCollection("AdroitCRMCustomerStageHolder")]
    public class CustomerStageHolder : BaseDtoII
    {
        public string StageId { get; set; }
        public string CustomerId { get; set; }
    }
}
