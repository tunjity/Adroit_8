namespace Adroit_v8.Models.UtilityModel;

public partial class FixedDepositStatus : BaseEntity
{
    public string? Name { get; set; }

}

public class FixedDepositStatusFm
{
    public string? Name { get; set; }
        public int? Status { get; set; }

}