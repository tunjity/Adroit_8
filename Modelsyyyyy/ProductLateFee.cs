using System;
using System.Collections.Generic;

namespace Adroit_v8.Modelsyyyyy;

public partial class ProductLateFee
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string LateFeeType { get; set; } = null!;

    public decimal FixedPrice { get; set; }

    public string LateFeePrincipal { get; set; } = null!;

    public string FeeFrequency { get; set; } = null!;

    public string GracePeriod { get; set; } = null!;

    public string UniqueId { get; set; } = null!;

    public int Isdeleted { get; set; }
}
