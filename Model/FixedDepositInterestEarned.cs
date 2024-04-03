using System;
using System.Collections.Generic;

namespace Adroit_v8.Model;

public partial class FixedDepositInterestEarned
{
    public long InterestEarnedId { get; set; }

    public long? FixedDepositSetupId { get; set; }

    public decimal? InterestAmountSoFar { get; set; }

    public DateOnly? DateLastUpdated { get; set; }

    public int? TotalNumberOfDays { get; set; }

    public long? CustomerId { get; set; }

    public string? ClientCode { get; set; }

    public DateTimeOffset? DateCreated { get; set; }
}
