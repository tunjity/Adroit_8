using System.Collections;

namespace Adroit_v8.Models.FormModel
{
    public class AdministrationFormModel
    {
        public string? Name { get; set; }
    }

    public class UnderwriterLevelFormModel : AdministrationFormModel
    {
        public decimal? MinimuimAmount { get; set; }

        public decimal? MaximuimAmount { get; set; }
    } 
    public class UnderwriterLevelFormModelUpdate : UnderwriterLevelFormModel
    {
        public int Id { get; set; }
    }

    public class AdminproductFormMode
    {
        public string? Name { get; set; }

        public int? Status { get; set; }

        public decimal? Minimuimamount { get; set; }

        public decimal? Maximuimamount { get; set; }

        public string? InterestRate { get; set; }

        public int? Tenor { get; set; }

        public DateOnly? Startdate { get; set; }

        public bool? AsEndDate { get; set; }
        public DateOnly? Enddate { get; set; }
        public string LateFeeType { get; set; }
        public decimal FixedPrice { get; set; }
        public string LateFeePrincipal { get; set; }
        public string FeeFrequency { get; set; }
        public string GracePeriod { get; set; }
        public bool IsOptInProcessingFee { get; set; }
        public bool IsFixedPrice { get; set; }
        public decimal Principal { get; set; }
    }
}
