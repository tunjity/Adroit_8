namespace Adroit_v8.Models.UtilityModel
{
    public class FixedDepositInterestRate : BaseEntity
    {
        public string InterestRate { get; set; }
        public decimal LoanAmountFrom { get; set; }
        public decimal LoanAmountTo { get; set; }
       public int FixedDepositTenor { get; set; }
    }
    public class FixedDepositInterestRatePost
    {
        public string InterestRate { get; set; }
        public int FixedDepositTenor { get; set; }
        public decimal LoanAmountFrom { get; set; }
        public decimal LoanAmountTo { get; set; }
    public int? Status { get; set; }
    }   
    public class FixedDepositInterestRateUpdate
    {
        public int Id { get; set; }
         public string InterestRate { get; set; }
        public decimal LoanAmountFrom { get; set; }
        public decimal LoanAmountTo { get; set; }
        public int FixedDepositTenor { get; set; }
        public int? Status { get; set; }
    }
}
