namespace Adroit_v8.MongoConnections.UnderWriterModel
{
    [BsonCollection("UnderWriterRegularLoanInterestRate")]
    public class AdministrationRegularLoanInterestRate : BaseDtoII
    {
        public string InterestRate { get; set; }
        public decimal LoanAmountFrom { get; set; }
        public decimal LoanAmountTo { get; set; }
        public string EmploymentTypeId { get; set; }
    }
    public class AdminRegularLoanInterestRatePost
    {
        public string InterestRate { get; set; }
        public decimal LoanAmountFrom { get; set; }
        public decimal LoanAmountTo { get; set; }
        public string EmploymentTypeId { get; set; }
    }

    public class AdminRegularLoanInterestRatePut
    {
        public string UniqueId { get; set; }
        public string InterestRate { get; set; }
        public decimal LoanAmountFrom { get; set; }
        public decimal LoanAmountTo { get; set; }
        public string EmploymentTypeId { get; set; }
    }
    public class RegularLoanInterestRateGet
    {
        public decimal LoanAmount { get; set; }
        public string EmploymentTypeId { get; set; }
    }
}
