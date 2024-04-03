namespace Adroit_v8.MongoConnections.UnderWriterModel
{
    [BsonCollection("UnderWriterManage")]
    public class Manage : BaseDtoII
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string Level { get; set; }
    }
    public class AdminManagePost
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string Level { get; set; }
    }
    public class AdminManagePut
    {
        public string UniqueId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string Level { get; set; }
    }
}
