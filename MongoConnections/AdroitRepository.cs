using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Adroit_v8.MongoConnections
{
    public interface IAdroitRepository<TBaseDto> where TBaseDto : AdroitIBaseDto
    {
        IQueryable<TBaseDto> AsQueryable();

        // IQueryable<Customer> GetCustomersForChecker();
        IEnumerable<TBaseDto> FilterBy(
            Expression<Func<TBaseDto, bool>> filterExpression);

        IEnumerable<TProjected> FilterBy<TProjected>(
            Expression<Func<TBaseDto, bool>> filterExpression,
            Expression<Func<TBaseDto, TProjected>> projectionExpression);

        TBaseDto FindOne(Expression<Func<TBaseDto, bool>> filterExpression);

        Task<TBaseDto> FindOneAsync(Expression<Func<TBaseDto, bool>> filterExpression);

        TBaseDto FindById(string id);

        Task<TBaseDto> FindByIdAsync(string id);

        ReturnObject InsertOne(TBaseDto document);

        Task<ReturnObject> InsertOneAsync(TBaseDto document);

        void InsertMany(ICollection<TBaseDto> documents);

        Task<ReturnObject> InsertManyAsync(ICollection<TBaseDto> documents);
        void ReplaceOne(TBaseDto document);
        Task ReplaceOneAsync(TBaseDto document);
        void DeleteOne(Expression<Func<TBaseDto, bool>> filterExpression);
        Task DeleteOneAsync(Expression<Func<TBaseDto, bool>> filterExpression);
        void DeleteById(string id);

        Task DeleteByIdAsync(string id);

        void DeleteMany(Expression<Func<TBaseDto, bool>> filterExpression);

        Task DeleteManyAsync(Expression<Func<TBaseDto, bool>> filterExpression);
    }

    public class AdroitRepository<TBaseDto> : IAdroitRepository<TBaseDto>
    where TBaseDto : AdroitIBaseDto
    {
        public IHttpContextAccessor _httpContextAccessor;
        private readonly IMongoCollection<TBaseDto> _collection;
        private readonly IConfiguration _config;

        AuthDto auth = new AuthDto();
        public AdroitRepository(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            //in case of controller with many repo
            if (auth.ClientId == null)
            {
                _httpContextAccessor = httpContextAccessor;
                auth.ClientId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ClientId") != null ? _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ClientId").Value : "";
                auth.FirstName = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "FirstName")?.Value;
                auth.LastName = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "LastName")?.Value;
                auth.ApplicationId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ApplicationId")?.Value;
                auth.email = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "email") != null ? _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "email").Value : "";
                auth.UserName = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserName").Value;
                auth.UserId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
                auth.CreatedBy = $"{auth.UserName}, {auth.FirstName} {auth.LastName}| {auth.UserId}";
            }

            _config = config;

            string? connectionURI = _config.GetSection("MongoDB").GetSection("ConnectionURI").Value;
            string? databaseName = _config.GetSection("MongoDB").GetSection("MobileDatabaseName").Value;

            MongoClient client = new(connectionURI);
            var database = client.GetDatabase(databaseName);
            _collection = database.GetCollection<TBaseDto>(GetCollectionName(typeof(TBaseDto)));
        }

        protected string? GetCollectionName(Type documentType)
        {
            return ((BsonCollectionAttribute)documentType.GetCustomAttributes(
                    typeof(BsonCollectionAttribute),
                    true)
                .FirstOrDefault())?.CollectionName;
        }

        public virtual IQueryable<TBaseDto> AsQueryable()
        {
            return _collection.AsQueryable().Where(o => o.ClientId == auth.ClientId);
        }

        public virtual IEnumerable<TBaseDto> FilterBy(
            Expression<Func<TBaseDto, bool>> filterExpression)
        {
            return _collection.Find(filterExpression).ToEnumerable();
        }

        public virtual IEnumerable<TProjected> FilterBy<TProjected>(
            Expression<Func<TBaseDto, bool>> filterExpression,
            Expression<Func<TBaseDto, TProjected>> projectionExpression)
        {
            return _collection.Find(filterExpression).Project(projectionExpression).ToEnumerable();
        }

        public virtual TBaseDto FindOne(Expression<Func<TBaseDto, bool>> filterExpression)
        {
            return _collection.Find(filterExpression).FirstOrDefault();
        }

        public virtual Task<TBaseDto> FindOneAsync(Expression<Func<TBaseDto, bool>> filterExpression)
        {
            return Task.Run(() => _collection.Find(filterExpression).FirstOrDefaultAsync());
        }

        public virtual TBaseDto FindById(string id)
        {
            var objectId = new ObjectId(id);
            var filter = Builders<TBaseDto>.Filter.Eq(doc => doc.Id, objectId);
            return _collection.Find(filter).SingleOrDefault();
        }

        public virtual Task<TBaseDto> FindByIdAsync(string id)
        {
            return Task.Run(() =>
            {
                var filter = Builders<TBaseDto>.Filter.Eq(doc => doc.UniqueId, id);
                return _collection.Find(filter).SingleOrDefaultAsync();
            });
        }
        public virtual async Task<ReturnObject> InsertOneAsync(TBaseDto document)
        {
            try
            {

            document.ClientId = auth.ClientId;
            document.CreatedBy = auth.CreatedBy;
            await Task.Run(() => _collection.InsertOneAsync(document));
            return new ReturnObject { status = true, statusCode = 200, message = "Record Saved Successfully." };
            }
            catch (Exception ex)
            {
                return new ReturnObject { status = false, statusCode = 500, message = ex.Message  };
            }
        }

        public void InsertMany(ICollection<TBaseDto> documents)
        {
            Parallel.ForEach(documents, (document) =>
            {
                document.ClientId = auth.ClientId; document.CreatedBy = auth.CreatedBy;
            });
            _collection.InsertMany(documents);
        }

        public virtual async Task<ReturnObject> InsertManyAsync(ICollection<TBaseDto> documents)
        {
            await _collection.InsertManyAsync(documents);
            return new ReturnObject { status = true, statusCode = 200, message = "Record Saved Successfully." };
        }

        public void ReplaceOne(TBaseDto document)
        {
            var filter = Builders<TBaseDto>.Filter.Eq(doc => doc.Id, document.Id);
            _collection.FindOneAndReplace(filter, document);
        }

        public virtual async Task ReplaceOneAsync(TBaseDto document)
        {
            var filter = Builders<TBaseDto>.Filter.Eq(doc => doc.Id, document.Id);
            await _collection.FindOneAndReplaceAsync(filter, document);
        }

        public void DeleteOne(Expression<Func<TBaseDto, bool>> filterExpression)
        {
            _collection.FindOneAndDelete(filterExpression);
        }

        public Task DeleteOneAsync(Expression<Func<TBaseDto, bool>> filterExpression)
        {
            return Task.Run(() => _collection.FindOneAndDeleteAsync(filterExpression));
        }

        public void DeleteById(string id)
        {
            var objectId = new ObjectId(id);
            var filter = Builders<TBaseDto>.Filter.Eq(doc => doc.Id, objectId);
            _collection.FindOneAndDelete(filter);
        }

        public Task DeleteByIdAsync(string id)
        {
            return Task.Run(() =>
            {
                var objectId = new ObjectId(id);
                var filter = Builders<TBaseDto>.Filter.Eq(doc => doc.Id, objectId);
                _collection.FindOneAndDeleteAsync(filter);
            });
        }

        public void DeleteMany(Expression<Func<TBaseDto, bool>> filterExpression)
        {
            _collection.DeleteMany(filterExpression);
        }

        public Task DeleteManyAsync(Expression<Func<TBaseDto, bool>> filterExpression)
        {
            return Task.Run(() => _collection.DeleteManyAsync(filterExpression));
        }

        ReturnObject IAdroitRepository<TBaseDto>.InsertOne(TBaseDto document)
        {
            document.ClientId = auth.ClientId;
            document.CreatedBy = auth.CreatedBy;
            _collection.InsertOne(document);
            return new ReturnObject { status = true, statusCode = 200, message = "Record Saved Successfully." };
        }
    }
}
