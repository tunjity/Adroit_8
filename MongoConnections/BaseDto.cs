using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Adroit_v8.MongoConnections
{

    public interface IBaseDto
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public ObjectId Id { get; set; }
        public string CreatedBy { get; set; }
        public string ClientId { get; set; }
        public string UniqueId { get; set; }
        public DateTime DateCreated { get; set; }
    }
    public interface IBaseDtoForCC
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public ObjectId Id { get; set; }
        public DateTime DateCreated { get; set; }
    }
    public interface AdroitIBaseDto
    {
        public ObjectId Id { get; set; }
        public string UniqueId { get; set; }
        public int Isdeleted { get; set; }
        public string? CreatedBy { get; set; }
        public string? ClientId { get; set; }
        public DateTime DateCreated { get; set; }
    }
    public abstract class BaseDto : AdroitIBaseDto
    {
        public ObjectId Id { get; set; }
        public string UniqueId { get; set; } = null!;
        public int Isdeleted { get; set; } = 0;
        public string? ClientId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
    public abstract class BaseDtoII : IBaseDto
    {
        public ObjectId Id { get; set; }
        public string UniqueId { get; set; }
        [NotMapped]
        public string StatusName { get; set; }
        public string CreatedBy { get; set; }
        public string ClientId { get; set; }
        public int IsDeleted { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
    public abstract class CustomerCentricBaseDtoII : IBaseDtoForCC
    {
        public ObjectId Id { get; set; }
        public DateTime DateCreated { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class BsonCollectionAttribute : Attribute
    {
        public string CollectionName { get; }

        public BsonCollectionAttribute(string collectionName)
        {
            CollectionName = collectionName;
        }
    }

}
