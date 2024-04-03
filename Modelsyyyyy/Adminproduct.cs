using System;
using System.Collections.Generic;

namespace Adroit_v8.Modelsyyyyy;

public partial class Adminproduct
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? Status { get; set; }

    public decimal? Minimuimamount { get; set; }

    public decimal? Maximuimamount { get; set; }

    public string? InterestRate { get; set; }

    public int? Tenor { get; set; }

    public DateOnly? Startdate { get; set; }

    public bool? AsEndDate { get; set; }

    public DateOnly? Enddate { get; set; }

    public DateTime Datecreated { get; set; }

    public string UniqueId { get; set; } = null!;

    public int Isdeleted { get; set; }
}
