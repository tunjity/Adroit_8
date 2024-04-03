namespace Adroit_v8.Models.UtilityModel;

public partial class FixedDepositTenor : BaseEntity
{
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public string? Days { get; set; }
    public int? Status { get; set; }
}
public  class FixedDepositTenorFM
{
    public int? Status { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public string? Days { get; set; }
}
public  class FixedDepositTenorUpdateFM
{ public int Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public string? Days { get; set; }
    public int? Status { get; set; }
}