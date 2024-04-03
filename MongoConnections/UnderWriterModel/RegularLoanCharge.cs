using Adroit_v8.MongoConnections.CRM;

namespace Adroit_v8.MongoConnections.UnderWriterModel
{
    [BsonCollection("UnderWriterRegularLoanCharge")]
    public class AdministrationRegularLoanCharge : BaseDtoII
    {
        public string CustomerId { get; set; }
        public string EmploymentTypeId { get; set; }
        public Boolean IsPercentage { get; set; }
        public string ChargeAmount  { get; set; }
        public decimal LoanAmountFrom { get; set; }
        public decimal LoanAmountTo { get; set; }
        public int LoanTenorid  { get; set; }
    }
    public class AdminRegularLoanChargePost
    {
        public string EmploymentTypeId { get; set; }
        public Boolean IsPercentage { get; set; }
        public string ChargeAmount  { get; set; }
        public decimal LoanAmountFrom  { get; set; }
        public decimal LoanAmountTo  { get; set; }
        public int LoanTenorid  { get; set; }
    } 
    public class GetAdminRegularLoanCharge
    {
        public string EmploymentTypeId { get; set; }
        public decimal LoanAmount { get; set; }
        public int LoanTenorid  { get; set; }
    }
    public class AdminRegularLoanChargePut
    {
        public string UniqueId { get; set; }
        public string EmploymentTypeId { get; set; }
        public Boolean IsPercentage { get; set; }
        public string ChargeAmount { get; set; }
        public decimal LoanAmountFrom { get; set; }
        public decimal LoanAmountTo { get; set; }
        public int LoanTenorid { get; set; }
    }
    public class RegularLoanChargeGet
    {
        public string EmploymentTypeId { get; set; }
        public decimal LoanAmount  { get; set; }
        public int LoanTenorid  { get; set; }
    }
}
