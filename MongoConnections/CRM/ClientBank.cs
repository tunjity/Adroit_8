namespace Adroit_v8.MongoConnections.CRM
{
    [BsonCollection("AdroitCRMClientBank")]
    public class ClientBank:BaseDtoII
    {
        public string BankId { get; set; }
        public string CustomerId { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
    } 
    public class ClientBankFM
    {
        public string BankId { get; set; }
        public string CustomerId { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
    }
    public class ClientBankUpdateFM: ClientBankFM
    {
        public string UniqueId { get; set; }
    }
}
