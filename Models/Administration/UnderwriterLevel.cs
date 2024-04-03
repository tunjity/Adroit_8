
namespace Adroit_v8.Models.Administration;

public partial class UnderwriterLevel : BaseEntity
{
    public string? Name { get; set; }

    public decimal? MinimuimAmount { get; set; }

    public decimal? MaximuimAmount { get; set; }

    public string? Loanrange { get; set; }

    public DateTime Datecreated { get; set; }

}