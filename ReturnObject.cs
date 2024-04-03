
namespace Adroit_v8
{
    public class AuthDto
    {
        public string email { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string ClientId { get; set; }
        public string ApplicationId { get; set; }
        public string CreatedBy { get; set; }
    }
    public class ReturnObject
    {
        public string id { get; set; }
        public bool status { get; set; }
        public string message { get; set; }
        public int statusCode { get; set; }
        public int recordCount { get; set; }
        public int recordPageNumber { get; set; }
        public dynamic data { get; set; }
    }
    public class CustomerCentricResponseEscrow
    {
        public string TransactionReference { get; set; }
        public string SellerName { get; set; }
        public string SellerEmailAddress { get; set; }
        public string SellerPhoneNumber { get; set; }
        public string BuyerName { get; set; }
        public string BuyerEmailAddress { get; set; }
        public string BuyerPhoneNumber { get; set; }
        public string TransactionDate { get; set; }
        public string Status { get; set; }
    }
    public class CustomerCentricResponse
    {
        public long CustomerId { get; set; }
        public string CustomerRef { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
        public string Bvn { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public DateTimeOffset DateCreated { get; set; }
    }  
    public class CustomerCentricLoanBiddingResponse
    {
        public string LenderPhoneNumber { get; set; }
        public string LenderEmailAddress { get; set; }
        public string LenderName { get; set; } 
        public string BorrowerPhoneNumber { get; set; }
        public string BorrowerEmailAddress { get; set; }
        public string BorrowerName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
        public string Bvn { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public DateTimeOffset DateCreated { get; set; }
    }
    public class LoanRestructingResponse
    {
        public int InitialTenorId { get; set; }
        public string InitialTenorValue { get; set; }
        public string LoanApplicationId { get; set; }
        public string PhoneNumber { get; set; }
        public string CustomerRef { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
        public string Bvn { get; set; }
        public int TenorId { get; set; }
        public string TenorValue { get; set; }
        public string LoanAmount { get; set; }
        public int Status { get; set; }
        public DateTime DateSubmitted { get; set; }
    }  
    public class LoanTopUpResponse
    {
        public decimal NewLoanTopUpAmount { get; set; }
        public string NewLoanTopUpTenor { get; set; }
        public string StatusName { get; set; }
        public string Comment { get; set; }
        public string LoanApplicationId { get; set; }
        public string CurrentLoanApplicationId { get; set; }
        public string PhoneNumber { get; set; }
        public string CustomerRef { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
        public string Bvn { get; set; }
        public string LoanAmount { get; set; }
        public string Gender { get; set; }
        public string InitialLoanAmount { get; set; }
        public string TopUpAmount { get; set; }
        public string NewLoanAmount { get; set; }
        public string Tenor { get; set; }
        public string Status { get; set; }
        // public int Status { get; set; }
        public DateTime DateSubmitted { get; set; }
    }
    public class CustomerCentricResponseForView
    {
        public string EmailAddress { get; set; }
        public string FullName { get; set; }
        public string DateOfBirth { get; set; }
        public string Bvn { get; set; }
        public string PhoneNumber { get; set; }
        public dynamic ListItem { get; set; }
    }
    public class FilterUpdateModelWithDesciption : FilterUpdateModel
    {
        public string? Description { get; set; }
    }
    public class FilterUpdateModel
    {
        public string EntityId { get; set; }
        public string Status { get; set; }
    }
    public class PaginationModel : PaginationWithOutFilterModel
    {
        public string? SearchType { get; set; }
        public string? SearchName { get; set; }
    }
    public class CustomerCentricFilter : PaginationModel
    {
        public int Det { get; set; }
        public int Status { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
    public class PaginationWithOutFilterModel
    {
        public int PasgeSize { get; set; }
        public int PageNumber { get; set; }
    }
    public class FilterModel
    {
        public string SearchBy { get; set; }
    }
    public class FilterBy
    {
        public int Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
    public class APIResponseforAll
    {
        public string id { get; set; }
        public bool status { get; set; }
        public string message { get; set; }
        public int statusCode { get; set; }
        public dynamic data { get; set; }
    }
    public class APIResponseWithToken
    {
        public string id { get; set; }
        public bool status { get; set; }
        public string message { get; set; }
        public int statusCode { get; set; }
        public data data { get; set; }
    }

    public class data
    {

        public string userId { get; set; }
        public string clientId { get; set; }
        public string applicationId { get; set; }
        public string email { get; set; }
        public string description { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string token { get; set; }
        public string tokenExpirationTime { get; set; }
    }
}