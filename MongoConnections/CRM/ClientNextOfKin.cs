namespace Adroit_v8.MongoConnections.CRM
{
    [BsonCollection("AdroitCRMClientNextOfKin")]

    public class ClientNextOfKin : BaseDtoII
    {
        public string TitleId { get; set; }
        public string CustomerId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string AltPhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string PermanentAddress { get; set; }
    }
    public class ClientNextOfKinUpdateFM: ClientNextOfKinFM
    {
        public string UniqueId { get; set; }
    }  
    public class ClientNextOfKinFM
    {
        public string TitleId { get; set; }
        public string CustomerId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string AltPhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string PermanentAddress { get; set; }
    }
}
