using System;
using System.Collections.Generic;

namespace Adroit_v8.Model;

public partial class FixedDepositTenor1
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Code { get; set; }

    public string? Description { get; set; }

    public string? Days { get; set; }

    public string UniqueId { get; set; } = null!;

    public int Isdeleted { get; set; }

    public int? Status { get; set; }
}
