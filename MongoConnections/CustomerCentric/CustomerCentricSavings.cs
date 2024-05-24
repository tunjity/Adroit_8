using Adroit_v8.MongoConnections.LoanApplication;
using MongoDB.Bson.Serialization.Attributes;

namespace Adroit_v8.MongoConnections.CustomerCentric
{
    [BsonIgnoreExtraElements]
    [BsonCollection("MobileAppSavingsCollection")]
    public class CustomerCentricSavings : CustomerCentricBaseDtoII
    {
        public long CustomerId { get; set; }
        public string Purpose { get; set; }
        public int SavingsFrequencyId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Amount { get; set; }
        public decimal TargetAmount { get; set; }
        public bool IsProcessed { get; set; }
        public bool IsActive { get; set; }
        public bool EnableSmsNotification { get; set; }
        public bool EnableEmailNotification { get; set; }
        public bool SaveOnWeekend { get; set; } = false;
    }
    public class restuctureReturn
    {
        public LoanRestructingResponse CusDetail { get; set; }
        public List<LoanRepaymentSchedule> repaymentSchedule { get; set; }
        public List<LoanRepaymentSchedule> oldrepaymentSchedule { get; set; }
    }


    [BsonIgnoreExtraElements]
    [BsonCollection("RegularLoanRestructureTemp")]
    public class RegularLoanRestructureTemp : CustomerCentricBaseDtoII
    {
        public string LoanApplicationId { get; set; }
        public long CustomerId { get; set; }
        public int TenorId { get; set; }
        public bool DisbursementStatus { get; set; }
        public int Status { get; set; }
        public string TenorValue { get; set; }
        public int InitialTenorId { get; set; }
        public string InitialTenorValue { get; set; }
        public string Comment { get; set; }
        public string LoanAmount { get; set; }
        public string LoanRestructureServiceCharge { get; set; }
        public byte[] EncryptedCardDetails { get; set; }
        public string BankStatementOfAccount { get; set; }
        public object RepaymentPlan { get; set; }
    }
    [BsonIgnoreExtraElements]
    [BsonCollection("RegularLoanRestructure")]
    public class RegularLoanRestructure : CustomerCentricBaseDtoII
    {
        public string CurrentLoanApplicationId { get; set; }
        public string LoanApplicationId { get; set; }
        public long CustomerId { get; set; }
        public int TenorId { get; set; }
        public bool DisbursementStatus { get; set; }
        public int Status { get; set; }
        public string TenorValue { get; set; }
        public int InitialTenorId { get; set; }
        public string InitialTenorValue { get; set; }
        public string Comment { get; set; }
        public string LoanAmount { get; set; }
        public string LoanRestructureServiceCharge { get; set; }
        public byte[] EncryptedCardDetails { get; set; }
        public string BankStatementOfAccount { get; set; }
        public object RepaymentPlan { get; set; }
    }

    [BsonIgnoreExtraElements]
    [BsonCollection("RegularLoanRestructureRepaymentPlan")]
    public class RegularLoanRestructureRepaymentPlan : CustomerCentricBaseDtoII
    {
        public decimal PrincipalAmount { get; set; }
        public decimal Interest { get; set; }
        public decimal Fee { get; set; }
        public decimal PaymentDue { get; set; }
        public string UniqueId { get; set; }
        public int Isdeleted { get; set; }
        public string ClientId { get; set; }
        public long CustomerId { get; set; }
        public string LoanApplicationId { get; set; }
        public DateTime MonthlyRepaymentDate { get; set; }
        public string MonthlyRepaymentAmount { get; set; }
        public string InterestRate { get; set; }
        public string LoanAmount { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
    }

    [BsonIgnoreExtraElements]
    [BsonCollection("LoanTopUp")]
    public class LoanTopUp : CustomerCentricBaseDtoII
    {
        public decimal PreviousLoanBalance { get; set; }
        public string InitialLoanAmount { get; set; }
        public bool DisbursementStatus { get; set; }
        public string TopUpAmount { get; set; }
        public string NewLoanAmount { get; set; }
        public decimal NewLoanTopUpAmount { get; set; }
        public string NewLoanTopUpTenor { get; set; }
        public string UniqueId { get; set; }
        public string Comment { get; set; }
        public int Isdeleted { get; set; }
        public string ClientId { get; set; }
        public long CustomerId { get; set; }
        public int LoanDuration { get; set; }
        public string LoanDurationValue { get; set; }
        public string LoanAmount { get; set; }
        public string ApplicantNumber { get; set; }
        public string LoanApplicationId { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public string Interest { get; set; }
        public string InterestRate { get; set; }
        public List<CustomerCardDetails> EncryptedCardDetails { get; set; }

        public string BankStatementOfAccount { get; set; }
        public string CurrentLoanApplicationId { get; set; }
    }

    [BsonIgnoreExtraElements]
    [BsonCollection("RegularLoanTopUpRepaymentPlan")]
    public class RegularLoanTopUpRepaymentPlan : CustomerCentricBaseDtoII
    {
        public decimal PrincipalAmount { get; set; }
        public decimal Interest { get; set; }
        public decimal Fee { get; set; }
        public decimal PaymentDue { get; set; }
        public string UniqueId { get; set; }
        public int Isdeleted { get; set; }
        public string ClientId { get; set; }
        public long CustomerId { get; set; }
        public string LoanApplicationId { get; set; }
        public DateTime MonthlyRepaymentDate { get; set; }
        public string MonthlyRepaymentAmount { get; set; }
        public string InterestRate { get; set; }
        public string LoanAmount { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
    }

    [BsonIgnoreExtraElements]
    [BsonCollection("MobileAppLoanOfferBidsCollection")]
    public class LoanBidding : CustomerCentricBaseDtoII
    {
        public string BidId { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public string LoanOfferStatus { get; set; }
        public string LoanOfferId { get; set; }
        public long BiddersCustomerId { get; set; }
        public long CustomerId { get; set; }
        public string BiddersComment { get; set; }
        public bool IsAccepted { get; set; }
        public DateTime DateAccepted { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Biddersrepaymentschedule[] BiddersRepaymentSchedule { get; set; }
        public string ClientId { get; set; }
        public string CreatedBy { get; set; }
        public string LenderName { get; set; }
        public string LenderPhoneNumber { get; set; }
        public string LenderEmailAddress { get; set; }
        public string BiddersName { get; set; }
        public string BiddersEmailAddress { get; set; }
        public string BiddersPhoneNumber { get; set; }
        public string LoanAmount { get; set; }
        public string Tenor { get; set; }
    }

    public class Biddersrepaymentschedule
    {
        public string LoanOfferRepaymentId { get; set; }
        public string LoanOfferId { get; set; }
        public long CustomerId { get; set; }
        public int RepaymentStage { get; set; }
        public string LoanAmount { get; set; }
        public string MonthlyLoanRepaymentAmount { get; set; }
        public string InterestRate { get; set; }
        public DateTime RepaymentDate { get; set; }
        public DateTime ActualRepaymentDate { get; set; }
        public DateTime DateCreated { get; set; }
        public int DebitCounter { get; set; }
        public string ClientId { get; set; }
        public string CreatedBy { get; set; }
        public bool IsRecovered { get; set; }
    }


    [BsonIgnoreExtraElements]
    [BsonCollection("MobileAppLoanOfferCollection")]
    public class LoanOffer : CustomerCentricBaseDtoII
    {
        public string ClientId { get; set; }
        public string CreatedBy { get; set; }
        public string LoanOfferId { get; set; }
        public long CustomerId { get; set; }
        public string LoanAmount { get; set; }
        public string InterestRateId { get; set; }
        public string InterestRate { get; set; }
        public int LoanTenorId { get; set; }
        public string LoanTenor { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsPosted { get; set; }
        public bool IsHidden { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
    }
}
