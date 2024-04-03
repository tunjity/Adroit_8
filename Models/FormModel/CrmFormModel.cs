using System.ComponentModel.DataAnnotations;

namespace Adroit_v8.Models.FormModel
{
    public class CrmFormModelForEdit 
    {
        public int CusId { get; set; }
        public int TitleId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public int GenderId { get; set; }
        public string DOB { get; set; }
        public int MaritalStatusId { get; set; }
        public int NoOfDependantId { get; set; }
        public int EducationLevelId { get; set; }
        public string PhoneNumber { get; set; }
        public string AltPhoneNumber { get; set; }
        public string Email { get; set; }
    }

    public class CrmFormModel
    {
        public string HasBVN { get; set; }
        [Required]
        public string Bvn { get; set; }
        public string EmploymentSector { get; set; }
        public int TitleId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public int GenderId { get; set; }
        public string DOB { get; set; }
        public int MaritalStatusId { get; set; }
        public int NoOfDependantId { get; set; }
        public int EducationLevelId { get; set; }
        public string PhoneNumber { get; set; }
        public string AltPhoneNumber { get; set; }
        public string Email { get; set; }
    }


}
