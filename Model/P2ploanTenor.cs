using System;
using System.Collections.Generic;

namespace Adroit_v8.Model;

public partial class P2ploanTenor
{
    public int TenorId { get; set; }

    public string? Tenor { get; set; }

    public bool? IsActive { get; set; }

    public DateOnly? DateCreated { get; set; }
}
