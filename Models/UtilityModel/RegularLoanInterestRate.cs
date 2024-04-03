namespace Adroit_v8.Models.UtilityModel
{
    public class RegularLoanInterestRate : BaseEntity
    {
        public string InterestRate { get; set; }
        public decimal LoanAmountFrom { get; set; }
        public decimal LoanAmountTo { get; set; }
        public string EmploymentTypeId { get; set; }
    }
    public class AdminRegularLoanInterestRate : BaseEntity
    {
        public string CustomerId { get; set; }
        public string InterestRate { get; set; }
        public decimal LoanAmountFrom { get; set; }
        public decimal LoanAmountTo { get; set; }
        public string EmploymentTypeId { get; set; }
    }
    public class RegularLoanInterestRatePost
    {
        public string InterestRate { get; set; }
        public decimal LoanAmountFrom { get; set; }
        public decimal LoanAmountTo { get; set; }
        public string EmploymentTypeId { get; set; }
    public int? Status { get; set; }
    }   
    public class RegularLoanInterestRateUpdate
    {
        public int Id { get; set; }
         public string InterestRate { get; set; }
        public decimal LoanAmountFrom { get; set; }
        public decimal LoanAmountTo { get; set; }
        public string EmploymentTypeId { get; set; } 
         public int? Status { get; set; }
    }
}
