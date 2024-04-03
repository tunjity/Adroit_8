using System;
using System.Collections.Generic;

namespace Adroit_v8.Model;

public partial class RegularLoanCharge
{
    public int Id { get; set; }

    public string EmploymentTypeId { get; set; } = null!;

    public bool IsPercentage { get; set; }

    public string ChargeAmount { get; set; } = null!;

    public decimal LoanAmountFrom { get; set; }

    public decimal LoanAmountTo { get; set; }

    public int LoanTenorid { get; set; }

    public string UniqueId { get; set; } = null!;

    public int Isdeleted { get; set; }

    public int? Status { get; set; }
}
