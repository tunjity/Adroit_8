namespace Adroit_v8.MongoConnections.LoanApplication
{
    [BsonCollection("LoanApplicationLoanStatus")]
    public class LoanStatus : BaseDtoII
    {
        public string Name { get; set; }
        public string Status { get; set; }
    }
    public class LoanStatusFm
    {
        public string Name { get; set; }
        public string CreatedBy { get; set; }
        public string Status { get; set; }
    }
    public class LoanStatusUpdateFm
    {
        public string UniqueId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
    }
}
