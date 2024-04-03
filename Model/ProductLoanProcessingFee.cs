using System;
using System.Collections.Generic;

namespace Adroit_v8.Model;

public partial class ProductLoanProcessingFee
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public bool IsOptInProcessingFee { get; set; }

    public bool IsFixedPrice { get; set; }

    public decimal FixedPrice { get; set; }

    public decimal Principal { get; set; }

    public string UniqueId { get; set; } = null!;

    public int Isdeleted { get; set; }

    public int? Status { get; set; }
}
