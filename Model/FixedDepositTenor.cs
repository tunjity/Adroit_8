using System;
using System.Collections.Generic;

namespace Adroit_v8.Model;

public partial class FixedDepositTenor
{
    public int TenorId { get; set; }

    public string? TenorName { get; set; }

    public string? TenorCode { get; set; }

    public string? TenorDescription { get; set; }

    public int? CreatedBy { get; set; }

    public DateTimeOffset? DateCreated { get; set; }

    public int? IsActive { get; set; }

    public int? IsDeleted { get; set; }

    public int? TenorDays { get; set; }
}
