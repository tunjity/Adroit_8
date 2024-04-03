using System;
using System.Collections.Generic;

namespace Adroit_v8.Model;

public partial class FixedDepositSetupTopUp
{
    public long SetupTopUpId { get; set; }

    public long? SetupId { get; set; }

    public decimal? TopUpAmount { get; set; }

    public DateOnly? TopUpDate { get; set; }

    public DateTimeOffset? DateCreated { get; set; }

    public int? IsDeleted { get; set; }

    public decimal? PrincipalBeforeTopUp { get; set; }

    public decimal? PrincipalAfterTopUp { get; set; }

    public decimal? InterestRate { get; set; }

    public int? TenorId { get; set; }

    public string? TopUpPurpose { get; set; }

    public string? TopUpDescription { get; set; }

    public int? IsActivated { get; set; }

    public long? CustomerId { get; set; }

    public string? ClientCode { get; set; }
}
