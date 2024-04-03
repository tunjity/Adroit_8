namespace Adroit_v8.Models.UtilityModel;

public partial class FixedDepositAmountRange : BaseEntity
{
    public string? FromAmount { get; set; }
    public string? ToAmount { get; set; }
}
public class FixedDepositAmountRangeFm
{
    public string? FromAmount { get; set; }
    public string? ToAmount { get; set; }
    public int? Status { get; set; }
}
public class FixedDepositAmountRangeUpdateFm
{
    public int Id { get; set; }
    public string? FromAmount { get; set; }
    public string? ToAmount { get; set; }
    public int? Status { get; set; }
}