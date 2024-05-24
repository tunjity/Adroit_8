using Adroit_v8.MongoConnections;
using Newtonsoft.Json;

namespace Adroit_v8.Models.FormModel
{
    public class RegularLoanCommentFormModelToGet
    {
        public string LoanApplicationId { get; set; }
        public string LoanCategory { get; set; }
    }

    public class RegularLoanManualDis
    {
        public string LoanApplicationId { get; set; }
        public string ProcessedBy { get; set; }
        public string Comment { get; set; }
        public int AmountPaid { get; set; }
    }  public class RegularLoanManualDisII : RegularLoanManualDis
    {
        public string ClientId { get; set; }
    }
    public class UpdateWithBankStatement : RegularLoanCommentFormModelToGet
    {
        public decimal AdjustedAmount { get; set; }
        public string AdjustedTenor { get; set; }
        public string? Comment { get; set; }
        public IFormFile? BankStatement { get; set; }
    }

    public class CommentFormModelToGet : RegularLoanCommentFormModelToGet
    {
        public string Comment { get; set; }
        public int Status { get; set; }
    }
    public class DisturbFormModelToGet : RegularLoanCommentFormModelToGet
    {
        public string Description { get; set; }
    }
    public class RegularLoanCommentFormModelDecision : RegularLoanCommentFormModelToGet
    {
        public int cusId { get; set; }
    }
    public class RegularLoanCommentFormModelAdjust : RegularLoanCommentFormModelToGet
    {
        public string Description { get; set; }
        public string AdjustedTenor { get; set; }
        public string AdjustedAmount { get; set; }
        public List<string> Comments { get; set; }
    }
    public class Reassignment : RegularLoanCommentFormModelToGet
    {
        public string AssigneruserId { get; set; }
        public string AssigneeUserId { get; set; }
    }
    public class RegularLoanReasonTodeclineFormModel : RegularLoanCommentFormModelToGet
    {
        public List<UtilityReasonFormModel> Reasons { get; set; }
        public string Comment { get; set; }
    }
    public class RegularLoanCommentFormModel : RegularLoanCommentFormModelToGet
    {
        public string Description { get; set; }
    }
    public class RegularLoanSupportingDocFormModel : RegularLoanCommentFormModelToGet
    {
        public List<IFormFile> OtherForms { get; set; }
    }
    public class RegularLoanSupportingDocGuarantorFormFormModel : RegularLoanCommentFormModelToGet
    {
        public IFormFile GuarantorForm { get; set; }
    }
    public class RegularLoanRequestedSupportingDocFormModel : RegularLoanCommentFormModelToGet
    {
        public string DocName { get; set; }
        public int CustomerId { get; set; }
    }

    public class UtilityBankUpdateFormModel : UtilityFormModel
    {
        public int Id { get; set; }
        public string BankCode { get; set; }
    }
    public class UtilityBankFormModel : UtilityFormModel
    {
        public string BankCode { get; set; }
    }
    public class UtilityFormModelAdmin
    {
        public string Name { get; set; }
        //   public string CreatedBy { get; set; }
        public int StatusID { get; set; }
    }
    public class UtilityReasonFormModel
    {
        public string Name { get; set; }
    }
    public class GeneralTemplate
    {
        public int NotificationType { get; set; }
        public dynamic TemplateModel { get; set; }
    }

    public class AcceptOfferLetterMail
    {
        public int NotificationType { get; set; }
        public string CustomerName { get; set; }
        public string EmailTo { get; set; }
        public string LoanApplicationId { get; set; }
        public decimal LoanAmount { get; set; }
        public int LoanTenor { get; set; }
        public string AcceptanceUrl { get; set; }
    }
    public class UtilityReasonFormModelFoUpda
    {
        public string Name { get; set; }
        public string UniqueId { get; set; }
    }
    public class UtilityFormModel
    {
        public string Name { get; set; }
        public int StatusID { get; set; }
    }
    public class UtilityFormModelForStateAndLga : UtilityFormModel
    {
        public string DetId { get; set; }
    }
    public class UtilityModificationFormModel : UtilityFormModel
    {
        public int Id { get; set; }
    }
    public class AdminProductModificationFormModel : AdminproductFormMode
    {
        public int Id { get; set; }
    }
}