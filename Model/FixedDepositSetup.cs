using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Adroit_v8.Model;

public partial class FixedDepositSetup
{
    public long SetupId { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? MaturityDate { get; set; }

   // public DateTimeOffset  DateCreated { get; set; }

    public int? Status { get; set; }

    public decimal? DepositAmount { get; set; }

    public decimal? InterestRate { get; set; }

    public int? FixedDepositTenorId { get; set; }

    public long? CustomerId { get; set; }
    [NotMapped]
    public string? StatusName { get; set; }
    [NotMapped]
    public string? StringInterestRate => InterestRate.ToString();
    public string? ClientId { get; set; }

    public string? Purpose { get; set; }

    public string? Description { get; set; }
    public string? Reason { get; set; }

    public string? ReferenceId { get; set; }
}
