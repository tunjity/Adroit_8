using System;
using System.Collections.Generic;

namespace Adroit_v8.Model;

public partial class FixedDepositJob
{
    public long Id { get; set; }

    public DateOnly? LastDateRun { get; set; }

    public int? IsForcedRerun { get; set; }

    public string? FixDepositJobCode { get; set; }

    public DateTimeOffset? DateCreated { get; set; }
}
