using System;
using System.Collections.Generic;

namespace Adroit_v8.Model;

public partial class FixedDepositAmountRange1
{
    public int Id { get; set; }

    public string? FromAmount { get; set; }

    public string? ToAmount { get; set; }

    public string UniqueId { get; set; } = null!;

    public int Isdeleted { get; set; }

    public int? Status { get; set; }
}
