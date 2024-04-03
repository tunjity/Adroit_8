using System.Net.Mail;

namespace Adroit_v8.MongoConnections.CRM
{
    [BsonCollection("AdroitCRMClientEmploymentHistory")]
    public class ClientEmploymentHistory : BaseDtoII
    {
        public string OrganizationId { get; set; }
        public string CustomerId { get; set; }
        public string StateId { get; set; }
        public string LgaId { get; set; }
        public string Address { get; set; }
        public string NearestLandmark { get; set; }
        public string PhoneNumber { get; set; }
        public string StaffId { get; set; }
        public string JobRole { get; set; }
        public string EmploymentTypeId { get; set; }
        public string DateOfEmployment { get; set; }
        public string EmailAddress { get; set; }
        public string SalaryRange { get; set; }
        public string SalaryPaymentDay { get; set; }
    }


    public class ClientEmploymentHistoryFormModel
    {
        public string OrganizationId { get; set; }
        public string CustomerId { get; set; }
        public string StateId { get; set; }
        public string LgaId { get; set; }
        public string Address { get; set; }
        public string NearestLandmark { get; set; }
        public string PhoneNumber { get; set; }
        public string StaffId { get; set; }
        public string JobRole { get; set; }
        public string EmploymentTypeId { get; set; }
        public string DateOfEmployment { get; set; }
        public string EmailAddress { get; set; }
        public string SalaryRange { get; set; }
        public string SalaryPaymentDay { get; set; }

    }
    public class ClientEmploymentHistoryUpdateFormModel : ClientEmploymentHistoryFormModel
    {
        public string UniqueId { get; set; }
    }
}
