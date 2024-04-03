using System;
using System.Collections.Generic;

namespace Adroit_v8.Model;

public partial class Nationality
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? Status { get; set; }

    public string UniqueId { get; set; } = null!;

    public int Isdeleted { get; set; }
}
