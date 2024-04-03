using System;
using System.Collections.Generic;

namespace Adroit_v8.Model;

public partial class FixedDepositStatus
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string UniqueId { get; set; } = null!;

    public int Isdeleted { get; set; }

    public int? Status { get; set; }
}
public partial class SavingsStatus
{
    public int Id { get; set; }

    public string? StatusName { get; set; }
}
public partial class FixDepositStatus
{
    public int Id { get; set; }

    public string? StatusName { get; set; }
}
public partial class EscrowStatus
{
    public int Id { get; set; }

    public string? StatusName { get; set; }
}
public partial class BillsPaymentStatus
{
    public int Id { get; set; }

    public string? StatusName { get; set; }
}
