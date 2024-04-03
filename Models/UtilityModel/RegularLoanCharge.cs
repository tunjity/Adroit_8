using Adroit_v8.MongoConnections.CRM;

namespace Adroit_v8.Models.UtilityModel
{
    public class AdminRegularLoanCharge : BaseEntity
    {
        public string EmploymentTypeId { get; set; }
        public Boolean IsPercentage { get; set; }
        public string ChargeAmount { get; set; }
        public decimal LoanAmountFrom { get; set; }
        public decimal LoanAmountTo { get; set; }
        public int LoanTenorid { get; set; }
    }
    public class RegularLoanCharge : BaseEntity
    {
        public string EmploymentTypeId { get; set; }
        public Boolean IsPercentage { get; set; }
        public string ChargeAmount { get; set; }
        public decimal LoanAmountFrom { get; set; }
        public decimal LoanAmountTo { get; set; }
        public int LoanTenorid { get; set; }
    }
    public class RegularLoanChargePost
    {
        public string EmploymentTypeId { get; set; }
        public Boolean IsPercentage { get; set; }
        public string ChargeAmount { get; set; }
        public decimal LoanAmountFrom { get; set; }
        public decimal LoanAmountTo { get; set; }
        public int LoanTenorid { get; set; }
    public int? Status { get; set; }
    }
    public class RegularLoanChargeUpdate
    {
        public int Id { get; set; }
        public string EmploymentTypeId { get; set; }
        public Boolean IsPercentage { get; set; }
        public string ChargeAmount { get; set; }
        public decimal LoanAmountFrom { get; set; }
        public decimal LoanAmountTo { get; set; }
        public int LoanTenorid { get; set; }
        public int? Status { get; set; }
    }
}
