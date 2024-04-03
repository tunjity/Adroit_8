using System;
using System.Collections.Generic;

namespace Adroit_v8.Model;

public partial class AdminRegularLoanInterestRate
{
    public int Id { get; set; }

    public string CustomerId { get; set; } = null!;

    public string InterestRate { get; set; } = null!;

    public decimal LoanAmountFrom { get; set; }

    public decimal LoanAmountTo { get; set; }

    public string EmploymentTypeId { get; set; } = null!;

    public string UniqueId { get; set; } = null!;

    public int Isdeleted { get; set; }

    public int? Status { get; set; }
}
