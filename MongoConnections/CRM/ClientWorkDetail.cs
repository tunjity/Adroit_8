namespace Adroit_v8.MongoConnections.CRM
{
    [BsonCollection("AdroitCRMClientWorkDetail")]
    public class ClientWorkDetail : BaseDtoII
    {
        public string StaffId { get; set; }
        public string CustomerId { get; set; }
        public string JobRole { get; set; }
        public string EmploymentTypeId { get; set; }
        public string DateOfEmployment { get; set; }
        public string EmailAddress { get; set; }
        public string SalaryPayDay { get; set; }
        public string SalaryRange { get; set; }
    }
}
