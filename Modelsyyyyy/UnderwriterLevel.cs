using System;
using System.Collections.Generic;

namespace Adroit_v8.Modelsyyyyy;

public partial class UnderwriterLevel
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public decimal? MinimuimAmount { get; set; }

    public decimal? MaximuimAmount { get; set; }

    public string? Loanrange { get; set; }

    public DateTime Datecreated { get; set; }

    public int? Createdby { get; set; }

    public string UniqueId { get; set; } = null!;

    public int Isdeleted { get; set; }
}
