using System.ComponentModel.DataAnnotations.Schema;

namespace Adroit_v8.MongoConnections.Models
{
    [BsonCollection("AdroitBridgeLoanDisbursed")]
    public class Disbursement : BaseDtoII
    {
        public string Surname { get; set; }
        public string Firstname { get; set; }
        public string Status { get; set; }
        public string Middlename { get; set; }
        public string PHONENO { get; set; }
        public string EmailAddress { get; set; }
        public string HouseNo { get; set; }
        public string StreetName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string DOB { get; set; }
        public string BVN { get; set; }
        public string IdNo { get; set; }
        public string IdDateIssued { get; set; }
        public string TransferAmount { get; set; }
        public string PreferredNaration { get; set; }
        public string Gender { get; set; }
        public string RepaymentDate { get; set; }
        public DateTime StartDate { get;  set; }
    } 
    [BsonCollection("AdroitBridgeLoanDisbursementNew")]
    public class DisbursementNew : BaseDtoII
    {
        public string Surname { get; set; }
        public string Firstname { get; set; }
        public string Status { get; set; }
        public string Middlename { get; set; }
        public string PHONENO { get; set; }
        public string EmailAddress { get; set; }
        public string HouseNo { get; set; }
        public string StreetName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string DOB { get; set; }
        public string BVN { get; set; }
        public string IdNo { get; set; }
        public string IdDateIssued { get; set; }
        public string TransferAmount { get; set; }
        public string PreferredNaration { get; set; }
        public string Gender { get; set; }
        public string RepaymentDate { get; set; }
        [NotMapped]
        public string? DocumentationStage { get; set; }
        [NotMapped]
        public DisbursementComment? comments { get; set; }
        public DateTime StartDate { get;  set; }

    }
    public class DisbursementFmFromReturnToProcess : DisbursementFm
    {
        public string UniqueId { get; set; }
    }
    public class DisbursementFmForReturn : DisbursementFm
    {
        public string UniqueId { get; set; }
        public string Comments { get; set; }
    }
    public class DisbursementFm
    {
        public DateTime? StartDate { get; set; }
       // public string StartDateString { get; set; }
        public string Gender { get; set; }
        public string RepaymentDate { get; set; }
        public string Surname { get; set; }
        public string Firstname { get; set; }
        public string Middlename { get; set; }
        public string Email { get; set; }
        public string PHONENO { get; set; }
        public string HouseNo { get; set; }
        public string StreetName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string DOB { get; set; }
        public string BVN { get; set; }
        public string IdNo { get; set; }
        public string IdDateIssued { get; set; }
        public string TransferAmount { get; set; }
        public string PreferredNaration { get; set; }
    }
    public class DisbursementFmUpload
    {
        public IFormFile UploadedExcel { get; set; }
    }
    public class DisbursementFmUploadDate : DisbursementFmUpload
    {
        public DateTime StartDate { get; set; }
    }
}
