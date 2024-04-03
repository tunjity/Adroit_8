using System;
using System.Collections.Generic;

namespace Adroit_v8.Model;

public partial class FixedDepositDailyProgress
{
    public long ProgressId { get; set; }

    public long? CustomerId { get; set; }

    public long? FixdepositSetupId { get; set; }

    public decimal? AmountBeforeInterest { get; set; }

    public decimal? InterestAmount { get; set; }

    public decimal? AmountAfterInterest { get; set; }

    public DateOnly? DateLastUpdate { get; set; }

    public DateTimeOffset? DateCreated { get; set; }

    public string? ClientCode { get; set; }
}
