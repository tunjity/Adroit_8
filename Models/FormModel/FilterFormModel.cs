using System.ComponentModel.DataAnnotations;

namespace Adroit_v8.Models.FormModel
{
    public class FilFormModel:PaginationWithOutFilterModel
    {
        public string? ApplicationId { get; set; }
        public int Det { get; set; }
        public int Category { get; set; }
        public int Status { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Channel { get; set; }
        public string? ApplicantName { get; set; }
        public string? EmailAddress { get; set; }
        public string? CustomerReference { get; set; }
        public string? Bvn { get; set; }
        public string? LoanCategory { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }  

    }

    public class ForDownload
    {
        public int Det { get; set; }
        public DateTime StartDate { get; set; }
    }
    public class FilFormModelIn:FilFormModel
    { public string FilterDet { get; set; }
    }

    public class DropDownDetail{
        public int Id { get; set; }
        public string Name { get; set; }
        public string TransactionId { get; set; }
    }   

}