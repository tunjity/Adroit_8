
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using static MongoDB.Driver.WriteConcern;
using System.Security;
using System.ComponentModel.DataAnnotations.Schema;

namespace Adroit_v8
{
    public class AuthDto
    {
        public string email { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string IsOtpVerified { get; set; }
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
    public class NewPermissionReturnObject
    {
        public bool status { get; set; }
        public string message { get; set; }
        public int statusCode { get; set; }
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
        public decimal PreviousLoanBalance { get; set; }
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
    public class LoanTransaction
    {
        public string LoanAmount { get; set; }
        public string LoanApplicationId { get; set; }
        public string LoanRepaymentId { get; set; }
        public string Tenor { get; set; }
        public string Status { get; set; }
        public bool IsBankDebit { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime TransactionDate { get; set; }
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

    public class ManualRepayment
    {
        public long CustomerId { get; set; }
        public string RepaymentId { get; set; }
        public string LoanApplicationId { get; set; }
        public string Amount { get; set; }
    }
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
public class UserApplicationPagesAndPermission
{
    public dynamic Modules { get; set; }
    public dynamic Pages { get; set; }
    public dynamic Permission { get; set; }
}

public class ApplicationPermissionGetDTo
{

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string ApplicationPermissionId { get; set; }
    public string ApplicationPageId { get; set; }
    public string ApplicationId { get; set; }
    public string ApplicationRoleName { get; set; }
    public string ApplicationPageName { get; set; }
    public int Status { get; set; }
    public int IsDeleted { get; set; }
    public DateTime DateCreated { get; set; }
    public bool CanView { get; set; }
    public bool CanAdd { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanRemove { get; set; }
    public bool CanApprove { get; set; }
    public bool CanReject { get; set; }
    public bool CanDecline { get; set; }
    public bool CanAssign { get; set; }
    public bool CanReAssign { get; set; }
    public bool CanReview { get; set; }
    public bool CanAdjust { get; set; }
    public bool CanComment { get; set; }
    public bool CanDownload { get; set; }
    public bool CanUpload { get; set; }
    public bool CanSearch { get; set; }
    public bool CanDisburse { get; set; }
    public bool CanReturn { get; set; }
    public bool CanDecide { get; set; }
    public bool CanEditRepayment { get; set; }
}

public class ApplicationPermissionGetDTo1
{
    public ObjectId Id { get; set; }
    public string ApplicationPermissionId { get; set; }
    public string ApplicationRoleId { get; set; }
    public string ApplicationName { get; set; }
    public string ApplicationPageId { get; set; }
    public string ApplicationId { get; set; }
    public string ApplicationRoleName { get; set; }
    public string ApplicationPageName { get; set; }
    public int Status { get; set; }
    public int IsDeleted { get; set; }
    public DateTime DateCreated { get; set; }
    public bool CanView { get; set; }
    public bool CanAdd { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanRemove { get; set; }
    public bool CanApprove { get; set; }
    public bool CanReject { get; set; }
    public bool CanDecline { get; set; }
    public bool CanAssign { get; set; }
    public bool CanReAssign { get; set; }
    public bool CanReview { get; set; }
    public bool CanAdjust { get; set; }
    public bool CanComment { get; set; }
    public bool CanDownload { get; set; }
    public bool CanUpload { get; set; }
    public bool CanSearch { get; set; }
    public bool CanDisburse { get; set; }
    public bool CanReturn { get; set; }
    public bool CanDecide { get; set; }
    public bool CanEditRepayment { get; set; }
}

[BsonIgnoreExtraElements]
[BsonCollection("SSO_ApplicationRoleCollection")]
public class ApplicationRoleGetDTO : BaseDtoII
{
    public string ApplicationRoleId { get; set; }
    public string ApplicationId { get; set; }
    public string ApplicationModuleId { get; set; }
    public string ApplicationRoleName { get; set; }
    public string ApplicationRoleCode { get; set; }
    public string ApplicationRoleDescription { get; set; }
}
public class ApplicationGetModuleDTO : BaseDtoII
{
    public string ApplicationId { get; set; }
    public string ApplicationName { get; set; }
    public string ApplicationModuleId { get; set; }
    public string ApplicationModuleName { get; set; }
    public string ApplicationModuleCode { get; set; }
    public string ApplicationModuleDescription { get; set; }
}

[BsonIgnoreExtraElements]
[BsonCollection("SSO_ApplicationPageCollection")]
public class ApplicationPage : BaseDtoII
{
    public string ApplicationId { get; set; }
    public string ApplicationModuleId { get; set; }
    public string ApplicationPageId { get; set; }
    public string ApplicationPageName { get; set; }
    public string ApplicationPageCode { get; set; }
    public string ApplicationPageDescription { get; set; }
    [NotMapped]
    public List<ApplicationPermissionGetDto1> permissions { get; } = new List<ApplicationPermissionGetDto1>();
}
public class ApplicationPageII
{
    public string ApplicationId { get; set; }
    public string ApplicationModuleId { get; set; }
    public string ApplicationPageId { get; set; }
    public string ApplicationPageName { get; set; }
    public string ApplicationPageCode { get; set; }
    public string ApplicationPageDescription { get; set; }
    public List<ApplicationPermissionGetDto1II> permissions { get; } = new List<ApplicationPermissionGetDto1II>();
}


[BsonIgnoreExtraElements]
[BsonCollection("SSO_ApplicationModuleCollection")]
public class ApplicationGetModuleDTO1 : BaseDtoII
{
    public string ApplicationId { get; set; }
    public string ApplicationName { get; set; }
    public string ApplicationModuleId { get; set; }
    public string ApplicationModuleName { get; set; }
    public string ApplicationModuleCode { get; set; }
    public string ApplicationModuleDescription { get; set; }
    public dynamic ApplicationPages { get; set; }
    [NotMapped]
    public List<ApplicationPage> forms { get; } = new();
}

[BsonIgnoreExtraElements]
[BsonCollection("SSO_ApplicationsCollection")]
public class ApplicationGetDTO : BaseDtoII
{
    public string ApplicationId { get; set; }
    public string ApplicationName { get; set; }
    public string ApplicationCode { get; set; }
    public int Status { get; set; }
    public string Description { get; set; }
}

[BsonIgnoreExtraElements]
[BsonCollection("SSO_ApplicationPermission")]
public class ApplicationPermissionGetDto1 : BaseDtoII
{
    public string ApplicationId { get; set; }
    public string ApplicationRoleId { get; set; }
    public string ApplicationPageId { get; set; }
    public string ApplicationPermissionId { get; set; }
    public bool CanView { get; set; }
    public bool CanAdd { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanRemove { get; set; }
    public bool CanApprove { get; set; }
    public bool CanReject { get; set; }
    public bool CanDecline { get; set; }
    public bool CanAssign { get; set; }
    public bool CanReAssign { get; set; }
    public bool CanReview { get; set; }
    public bool CanAdjust { get; set; }
    public bool CanComment { get; set; }
    public bool CanDownload { get; set; }
    public bool CanUpload { get; set; }
    public bool CanSearch { get; set; }
    public bool CanDisburse { get; set; }
    public bool CanReturn { get; set; }
    public bool CanDecide { get; set; }
    public bool CanEditRepayment { get; set; }
}
public class ApplicationPermissionGetDto1II
{
    public string ApplicationId { get; set; }
    public string ApplicationRoleId { get; set; }
    public string ApplicationPageId { get; set; }
    public string ApplicationPermissionId { get; set; }
    public bool CanView { get; set; }
    public bool CanAdd { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanRemove { get; set; }
    public bool CanApprove { get; set; }
    public bool CanReject { get; set; }
    public bool CanDecline { get; set; }
    public bool CanAssign { get; set; }
    public bool CanReAssign { get; set; }
    public bool CanReview { get; set; }
    public bool CanAdjust { get; set; }
    public bool CanComment { get; set; }
    public bool CanDownload { get; set; }
    public bool CanUpload { get; set; }
    public bool CanSearch { get; set; }
    public bool CanDisburse { get; set; }
    public bool CanReturn { get; set; }
    public bool CanDecide { get; set; }
    public bool CanEditRepayment { get; set; }
}
public class PermissionReturnObject
{
    public string ModuleName { get; set; }
    public List<ApplicationPageII> Forms { get; set; } = new();
}
public class PermissionReturn
{
    public List<PermissionReturnObject> modules { get; set; } = new();
}
[BsonIgnoreExtraElements]
[BsonCollection("SSO_UserApplicationRoleCollection")]
public class UserApplicationRoleGetDTO : BaseDtoII
{
    public string UserApplicationRoleId { get; set; }
    public string ApplicationId { get; set; }
    public string UserId { get; set; }
    public string ApplicationRoleId { get; set; }
    public int Status { get; set; }
    //public int IsDeleted { get; set; }
    public string Description { get; set; }
    // public DateTime DateCreated { get; set; }
}

[BsonIgnoreExtraElements]
[BsonCollection("SSO_ApplicationsCollection")]
public class ApplicationGetPageDTO : BaseDtoII
{
    public string ApplicationPageId { get; set; }
    public string ApplicationId { get; set; }
    public string ApplicationName { get; set; }
    public string ApplicationModuleId { get; set; }
    public string ApplicationModuleName { get; set; }
    public string ApplicationPageName { get; set; }
    public string ApplicationPageCode { get; set; }
    public string ApplicationPageDescription { get; set; }
    public int Status { get; set; }
}


public class APIResponseforAllWithoutData
{
    public string id { get; set; }
    public bool status { get; set; }
    public string message { get; set; }
    public int statusCode { get; set; }
}
public class APIResponseforAll : APIResponseforAllWithoutData
{
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
