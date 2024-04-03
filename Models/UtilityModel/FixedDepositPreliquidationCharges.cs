namespace Adroit_v8.Models.UtilityModel;

public partial class FixedDepositPreliquidationCharges : BaseEntity
{
    public string? FromAmount { get; set; }
    public string? ToAmount { get; set; }
    public string? IsPercentage { get; set; }
    public string? AmountCharge { get; set; }
}
public class FixedDepositPreliquidationChargesFm
{
    public string? FromAmount { get; set; }
    public string? ToAmount { get; set; }
    public string? IsPercentage { get; set; }
    public string? AmountCharge { get; set; }
    public int? Status { get; set; }
}
public class FixedDepositPreliquidationChargesUpdateFm
{   public int Id { get; set; }
    public string? FromAmount { get; set; }
    public string? ToAmount { get; set; }
    public string? IsPercentage { get; set; }
    public string? AmountCharge { get; set; }
        public int? Status { get; set; }
}