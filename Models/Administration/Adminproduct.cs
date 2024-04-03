using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Adroit_v8.Models.Administration;

public partial class Adminproduct :BaseEntity
{

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

    [NotMapped]
    public List<ProductLateFee> ProductLateFees{get;set;}
    [NotMapped]
    public List<ProductLoanProcessingFee> ProductLoanProcessingFees{get;set;}

}
