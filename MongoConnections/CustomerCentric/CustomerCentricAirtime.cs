using MongoDB.Bson.Serialization.Attributes;
using System.Security.Cryptography;

namespace Adroit_v8.MongoConnections.CustomerCentric
{

    [BsonIgnoreExtraElements]
    [BsonCollection("MobileAppAirtimeLogCollection")]
    public class CustomerCentricAirtime : CustomerCentricBaseDtoII
    {
        public long CustomerId { get; set; }
        public string BillerId { get; set; }
        public string BillersName { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string PaymentCode { get; set; }
        public string MobileOperatorsRef { get; set; }
        public string MobileOperatorName { get; set; }
        public string MobileNumber { get; set; }
        public string Amount { get; set; }
        public string Pin { get; set; }
        public int IsDeleted { get; set; }
        public int IsActive { get; set; }
        public int IsCompleted { get; set; }
        public string TransactionReference { get; set; }
        public string Status { get; set; }
    }
    [BsonIgnoreExtraElements]
    [BsonCollection("MobileAppP2PLoanRequestCollection")]
    public class CustomerCentricP2p : CustomerCentricBaseDtoII
    {
        public string P2PLoanRequestId { get; set; }
        public long BorrowerCustomerId { get; set; }
        public long LenderCustomerId { get; set; }
        public string LenderLoanInterest { get; set; }
        public string LenderMonthlyReceivable { get; set; }
        public string MonthlyInterest { get; set; }
        public string MonthlyNetInterest { get; set; }
        public string ProcessingFee { get; set; }
        public string RepaymentCharges { get; set; }
        public bool ApprovalStatus { get; set; }
        public string Amount { get; set; }
        public int LoanTenor { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime DateDisbursed { get; set; }

    }
    [BsonIgnoreExtraElements]
    [BsonCollection("MobileAppP2PLoanRequestMonthlyRepaymentCollection")]
    public class MobileAppP2PLoanRequestMonthlyRepaymentCollection : CustomerCentricBaseDtoII
    {
        public string MonthlyRepaymentId { get; set; }
        public string P2PLoanRequestId { get; set; }
        public long BorrowerCustomerId { get; set; }
        public long LenderCustomerId { get; set; }
        public string LoanAmount { get; set; }
        public string TotalRepaymentAmount { get; set; }
        public string MonthlyRepaymentAmount { get; set; }
        public string RepaymentStatus { get; set; }
        public DateTime RepaymentDate { get; set; }
        public DateTime ActualRepaymentDate { get; set; }
        public int DebitCounter { get; set; }
    }
    public class CustomerCentricP2pResponse
    {
        public string P2PLoanRequestId { get; set; }
        public string LenderName { get; set; }
        public string LenderEmailAddress { get; set; }
        public string LenderPhoneNumber { get; set; }
        public string BorrowerName { get; set; }
        public string BorrowerEmailAddress { get; set; }
        public string BorrowerPhoneNumber { get; set; }
        public string Amount { get; set; }
        public string Tenor { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Status { get; set; }
    }

    [BsonIgnoreExtraElements]
    [BsonCollection("MobileAppDataBundlesLogCollection")]
    public class CustomerCentricData : CustomerCentricBaseDtoII
    {
        public long CustomerId { get; set; }
        public string BillerId { get; set; }
        public string BillersName { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string PaymentCode { get; set; }
        public string MobileOperatorsRef { get; set; }
        public string MobileOperatorName { get; set; }
        public string MobileNumber { get; set; }
        public string Amount { get; set; }
        public string Pin { get; set; }
        public int IsDeleted { get; set; }
        public int IsActive { get; set; }
        public int IsCompleted { get; set; }
        public string TransactionReference { get; set; }
        public string Status { get; set; }
    }

    [BsonIgnoreExtraElements]
    [BsonCollection("MobileAppLoanOfferCollection")]
    public class CustomerCentricLoanOffer : CustomerCentricBaseDtoII
    {
        public long CustomerId { get; set; }
        public string BillerId { get; set; }
        public string BillersName { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string PaymentCode { get; set; }
        public string MobileOperatorsRef { get; set; }
        public string MobileOperatorName { get; set; }
        public string MobileNumber { get; set; }
        public string Amount { get; set; }
        public string Pin { get; set; }
        public int IsDeleted { get; set; }
        public int IsActive { get; set; }
        public int IsCompleted { get; set; }
        public string TransactionReference { get; set; }
        public string Status { get; set; }
    }
}