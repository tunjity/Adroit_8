namespace Adroit_v8.MongoConnections.Models
{
    public class MongoDbContext
    {
    }
    public interface IMongoDbSettings
    {
        string DatabaseName { get; set; }
        string ConnectionURI { get; set; }
    }

    public class MongoDbSettings : IMongoDbSettings
    {
        public string DatabaseName { get; set; }
        public string ConnectionURI { get; set; }
    }
}
