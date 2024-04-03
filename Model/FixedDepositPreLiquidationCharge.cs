using System;
using System.Collections.Generic;

namespace Adroit_v8.Model;

public partial class FixedDepositPreLiquidationCharge
{
    public long Id { get; set; }

    public int? FixedDepositAmountRangeId { get; set; }

    public bool? IsPercentage { get; set; }

    public decimal? AmountCharge { get; set; }
}
