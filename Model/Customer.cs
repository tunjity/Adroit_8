using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Adroit_v8.Model;

public partial class Customer
{
    public long Id { get; set; }

    public int? TitleId { get; set; }

    public string? FirstName { get; set; }
    public string? ClientId { get; set; }

    public string? MiddleName { get; set; }

    public string? LastName { get; set; }

    public int? GenderId { get; set; }

    public string? DateOfBirth { get; set; }

    public int? MaritalStatusId { get; set; }

    public int? NumberOfDependent { get; set; }

    public int? EducationalLevelId { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Bvn { get; set; }

    public string? AlternativePhoneNumber { get; set; }

    public string? EmailAddress { get; set; }

    public string? Nin { get; set; }

    public string? CustomerRef { get; set; }

    public int? IsDeleted { get; set; }

    public int? IsActive { get; set; }

    public int? IsBlackListedCustomer { get; set; }

    public DateOnly? DateCreated { get; set; }

    public int? RegistrationStageId { get; set; }

    public int? RegistrationChannelId { get; set; }


    public int? HasValidBvn { get; set; }

    public int? Status { get; set; }

    public string? EscrowAccountNumber { get; set; }
    [NotMapped]
    public string CustomerCentricStatus { get; set; }

    public string? FacebookId { get; set; }

    public string? WhatsappNumber { get; set; }

    public string? LinkedinId { get; set; }
}




