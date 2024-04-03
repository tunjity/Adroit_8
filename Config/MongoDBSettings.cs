namespace Adroit_v8.Config
{
    public class MongoDBSettings
    {
        public string ConnectionURI { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string MobileDatabaseName { get; set; } = null!;
        public string ApplicationName { get; set; } = null!;
        public string ApplicationCollectionName { get; set; } = null!;
        public string UserCollectionName { get; set; } = null!;
        public string RoleCollectionName { get; set; } = null!;
    }
}
