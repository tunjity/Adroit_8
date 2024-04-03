using System;
using System.Collections.Generic;

namespace Adroit_v8.Model;

public partial class FixedDepositAmountRange
{
    public long Id { get; set; }

    public decimal? FromAmount { get; set; }

    public decimal? ToAmount { get; set; }

    public int? CreatedBy { get; set; }

    public DateOnly? DateCreated { get; set; }

    public DateOnly? DateLastUpdated { get; set; }
}
