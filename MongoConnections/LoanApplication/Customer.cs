using Adroit_v8.MongoConnections.CRM;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Adroit_v8.MongoConnections.LoanApplication
{
    [BsonIgnoreExtraElements]
    [BsonCollection("RegularLoan")]
    public class RegularLoan : BaseDto
    {
        public int CustomerId { get; set; }
        public int LoanDuration { get; set; }
        public string LoanDurationValue { get; set; }
        public string ApplicantNumber { get; set; }
        public string LoanCategory { get; set; }
        public decimal InterestRate { get; set; }
        public string? CustomerRef { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public decimal LoanAmount { get; set; }
        public string EmploymentType { get; set; }
        public string Bank { get; set; }
        public string LoanApplicationId { get; set; }
        public string DocumentPassword { get; set; }
        public string BankStatementOfAccount { get; set; }
        public string NameOnCard { get; set; }
        public string CardNumber { get; set; }
        public string ExpiryDate { get; set; }
        public bool IsAcceptedOfferLetter { get; set; }
        public string CVV { get; set; }
        public string CardPin { get; set; }
        public string BankName { get; set; }
        public string BankAccount { get; set; }
        public string AccountHolderName { get; set; }
        public string JobTitle { get; set; }
        public string Interest { get; set; }
        public string EmployerName { get; set; }
        public string WorkEmail { get; set; }
        public string EmployerAddress { get; set; }
        public string State { get; set; }
        public string LGA { get; set; }
        public string StartDate { get; set; }
        public decimal GrossSalaryOrIncome { get; set; }
        public string BusinessType { get; set; }
        public string BusinessName { get; set; }
        public string BusinessAddress { get; set; }
        public string BusinessAge { get; set; }
        public int Status { get; set; }
        public string ApplicationChannel { get; set; }
        public string StageName { get; set; }
        public byte[] EncryptedCardDetails { get; set; }
    }

    [BsonIgnoreExtraElements]
    [BsonCollection("RegularLoanRepaymentPlan")]
    public class RegularLoanRepaymentPlan : BaseDto
    {
        public long CustomerId { get; set; }
        public string LoanApplicationId { get; set; }
        public DateTime MonthlyRepaymentDate { get; set; }
        public string MonthlyRepaymentAmount { get; set; }
        public string InterestRate { get; set; }
        public string LoanAmount { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
    }
    public class RegularLoanRepaymentReturnModel
    {
        public DateTime repaymentDate { get; set; }
        public string principal { get; set; }
        public string Interest { get; set; }
        public string TotalPayment { get; set; }
    }

    [BsonCollection("RegularLoanDisbursement")]
    public class RegularLoanDisbursement : BaseDtoII
    {
        public long CustomerId { get; set; }
        public int RetryCount { get; set; }
        public string DisbursedTo { get; set; }
        public string Description { get; set; }
        //public string EncryptedCardDetails { get; set; }
        public int GenderId { get; set; }
        public bool IsClosed { get; set; }
        public bool Treated { get; set; }
        public string LoanType { get; set; }
        public string LoanApplicationId { get; set; }
        public string GenderName { get; set; }
        public string LoanTenor { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal LoanAmountWithInterest { get; set; }
        public byte[] Bvn { get; set; }
        public PaymentDetails DisbursementAccountDetail { get; set; }
        public List<PaymentDetails> RepaymentAccountDetail { get; set; }
        public List<PaymentDetails> CustomerAccountDetail { get; set; }
        public int EmploymentTypeId { get; set; }
        public string EmploymentType { get; set; }
        public List<PhoneNumber> CustomerPhoneNumber { get; set; }
        public List<carddetail> Repaymentcarddetail { get; set; }
        public List<DisError> DisbursementError { get; set; }
        public List<ClientEmploymentHistory> WorkDetail { get; set; }
        public List<ClientEmploymentHistory> EmployerInformation { get; set; }
        public List<ClientNextOfKin> NextOfKinDetail { get; set; }
        public string CustomerEmail { get; set; }
        public string? CustomerNIN { get; set; }
        public string? FacebookId { get; set; }
        public string? WhatsappNumber { get; set; }
        public string? LinkedinId { get; set; }
        public bool DisbursementStatus { get; set; } = false;
        public DateTime DateApproved { get; set; }
        public DateTime DateDisbursed { get; set; }
        public string OfficeAddress { get; set; }
        public string HomeAddress { get; set; }
        public string NearestBusstop { get; set; }
        public List<LoanRepaymentSchedule> LoanRepaymentSchedule { get; set; }
        public string ChannelId { get; set; }
        public string ChannelName { get; set; }
        public string InterestRate { get; set; }
        public byte[] EncryptedCardDetails { get; set; }
    }
    public class PaymentDetails
    {
        public int BankId { get; set; }
        public string BankName { get; set; }
        public string BankCode { get; set; }
        public string BankAccount { get; set; }
        public string AccountHolderName { get; set; }
    }
    public class LoanRepaymentSchedule
    {
        public string MonthlyRepaymentLoanAmount { get; set; }
        public string InterestRate { get; set; }
        public string LoanApplicationId { get; set; }
        public DateTime LoanRepaymentDate { get; set; }
    }

    public class RepaymentAPI
    {
        public string adroitUserId { get; set; }
        public int loanCustomerId { get; set; }
        public string loanApplicationId { get; set; }
        public string loanType { get; set; }
        public string DisbursementId { get; set; }
        public string clientId { get; set; }
        public string createdBy { get; set; }
    }

    public class PhoneNumber
    {
        public string Numbers { get; set; }
    }
    public class DisError
    {
        public string Items { get; set; }
    }

    public class carddetail
    {
        public string NameOnCard { get; set; }
        public byte[] CardNumber { get; set; }
        public string ExpiryDate { get; set; }
        public byte[] CVV { get; set; }
        public byte[] CardPin { get; set; }
    }
    [BsonCollection("LoanTopUp")]
    public class LoanTopUpStepOne : BaseDto
    {
        public int CustomerId { get; set; }
        public int LoanDuration { get; set; }
        public decimal LoanAmount { get; set; }
        public string ApplicantNumber { get; set; }
    }

    public class RegularLoanStepFive : BaseDto
    {
        public int CustomerId { get; set; }
        public string LoanApplicationId { get; set; }
        public string Bvn { get; set; }
    }
    //[BsonIgnoreExtraElements]
    [BsonCollection("RegularLoanReAssignment")]
    public class RegularLoanReAssignment : BaseDto
    {
        public string LoanApplicationId { get; set; }
        public string AssigneruserId { get; set; }
        public string AssigneeUserId { get; set; }
        public string LoanReAssignStatus { get; set; }
    }
    [BsonCollection("RegularLoanStepSix")]
    public class RegularLoanStepSix : BaseDto
    {
        public int CustomerId { get; set; }
        public string LoanApplicationId { get; set; }
        public string DocumentPassword { get; set; }
        public string BankStatementOfAccount { get; set; }
    }
    public class RegularLoanStepThree : BaseDto
    {
        public int CustomerId { get; set; }
        public string LoanApplicationId { get; set; }
        public string NameOnCard { get; set; }
        public string CardNumber { get; set; }
        public string ExpiryDate { get; set; }
        public string CVV { get; set; }
        public string CardPin { get; set; }
    }


    public class RegularLoanStepFour : BaseDto
    {
        public int CustomerId { get; set; }
        public string LoanApplicationId { get; set; }
        public string BankName { get; set; }
        public string BankAccount { get; set; }
        public string AccountHolderName { get; set; }
    }
    [BsonCollection("RegularLoanComment")]
    public class RegularLoanComment : BaseDto
    {
        public int CustomerId { get; set; }
        public string LoanApplicationId { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
    }
    [BsonCollection("RegularLoanRequestedDocument")]
    public class RegularLoanRequestedDocument : BaseDto
    {
        public int CustomerId { get; set; }
        public string LoanApplicationId { get; set; }
        public string DocName { get; set; }
    }
    [BsonCollection("RegularLoanSupportingDocumentsGuarantorForm")]
    public class RegularLoanSupportingDocumentsGuarantorForm : BaseDto
    {
        public int CustomerId { get; set; }
        public string LoanApplicationId { get; set; }
        public string GuarantorForm { get; set; }
    }
    [BsonCollection("RegularLoanSupportingDocumentsOtherForms")]
    public class RegularLoanSupportingDocumentsOtherForms : BaseDto
    {
        public int CustomerId { get; set; }
        public string LoanApplicationId { get; set; }
        public string OtherForms { get; set; }
    }
    [BsonCollection("RegularLoanReasonToDecline")]
    public class RegularLoanReasonToDecline : BaseDto
    {
        public string LoanApplicationId { get; set; }
        public string Reasons { get; set; }
        public string Comment { get; set; }
        public string LoanCategory { get; set; }
    }

    [BsonCollection("RegularLoanAdjustment")]
    public class RegularLoanAdjustment : BaseDto
    {
        public string LoanApplicationId { get; set; }
        public string Description { get; set; }
        public string AdjustedTenor { get; set; }
        public string AdjustedAmount { get; set; }
        public string LoanCategory { get; set; }
    }
    //RegularLoanStageHolder
    [BsonCollection("RegularLoanStageHolder")]
    public class RegularLoanStageHolder : BaseDto
    {
        public int CustomerId { get; set; }
        public string LoanApplicationId { get; set; }
        public int Stage { get; set; }
    }
    public class RegularLoanStepTwo : BaseDto
    {
        public int CustomerId { get; set; }
        public int EmploymentType { get; set; }
        public string LoanApplicationId { get; set; }
        public string JobTitle { get; set; }
        public string EmployerName { get; set; }
        public string WorkEmail { get; set; }
        public string EmployerAddress { get; set; }
        public string State { get; set; }
        public string LGA { get; set; }
        public string StartDate { get; set; }
        public decimal GrossSalaryOrIncome { get; set; }
        public string BusinessType { get; set; }
        public string BusinessName { get; set; }
        public string BusinessAddress { get; set; }
        public string BusinessAge { get; set; }
    }
}
