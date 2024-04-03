using System;
using System.Collections.Generic;

namespace Adroit_v8.Model;

public partial class FixedDepositLiquidation
{
    public long Id { get; set; }

    public long? CustomerId { get; set; }

    public long? FixedDepositSetupId { get; set; }

    public decimal? Fee { get; set; }

    public decimal? AmountBeforeLiquidation { get; set; }

    public decimal? LiquidationAmount { get; set; }

    public decimal? AmountAfterLiquidation { get; set; }

    public string? LiquidationPurpose { get; set; }

    public string? LiquidationComment { get; set; }

    public DateOnly? LiquidationDate { get; set; }

    public DateTimeOffset? DateCreated { get; set; }

    public decimal? Balance { get; set; }

    public string? LiquidationReferenceId { get; set; }
}
