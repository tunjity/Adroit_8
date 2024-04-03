using System.Security.Claims;
using static Adroit_v8.Config.Helper;
using static Adroit_v8.EnumFile.EnumHelper;

namespace Adroit_v8.Repository
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        IEnumerable<T> GetAllIsValid();
        IEnumerable<T> GetAll();

        T Get(int id);

        void Insert(T entity);

        void Update(T entity);
        void SoftDelete(T entity);

        void Delete(T entity);
    }

    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        public IHttpContextAccessor _httpContextAccessor;

        private readonly CreditWaveContext _context;
        private DbSet<T> entities;
        private string errorMessage = string.Empty;
        AuthDto auth = new AuthDto();

        public GenericRepository(CreditWaveContext context, IHttpContextAccessor httpContextAccessor)
        {
            //in case of controller with many repo
            if (auth.ClientId == null)
            {
                _httpContextAccessor = httpContextAccessor;
                var k = _httpContextAccessor.HttpContext.User.Claims.ToList();
                auth.ClientId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ClientId") != null ? _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ClientId").Value : "";
                auth.FirstName = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "FirstName")?.Value;
                auth.LastName = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "LastName")?.Value;
                auth.ApplicationId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ApplicationId")?.Value;
                auth.email = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "email") != null ? _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "email").Value : "";
                auth.UserName = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserName").Value;
                auth.UserId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
                auth.CreatedBy = $"{auth.UserName}, {auth.FirstName} {auth.LastName}| {auth.UserId}";
            }

            _context = context;
            entities = context.Set<T>();
        }

        public IEnumerable<T> GetAll()
        {
            var rec = entities.AsEnumerable().Where(o => o.clientid == auth.ClientId && o.Isdeleted ==0);
            foreach (var r in rec)
                if (r.Status != null)
                    r.StatusName = Enum.GetName(typeof(GeneralSetUpEnum), Convert.ToInt32(r.Status));
            return rec;
        }

        public T Get(int id)
        {
            return entities.FirstOrDefault(s => s.Id == id);
        }

        public void Insert(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            entity.clientid = auth.ClientId;
            entity.Createdby = auth.CreatedBy;
            entities.Add(entity);
            _context.SaveChanges();
        }

        public void Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            _context.SaveChanges();
        }

        public void Delete(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Remove(entity);
            _context.SaveChanges();
        }

        public IEnumerable<T> GetAllIsValid()
        {
            var rec = entities.Where(o => o.Isdeleted == 0 && o.clientid == auth.ClientId).AsEnumerable();
            foreach (var r in rec)
                if (r.Status != null)
                    r.StatusName = Enum.GetName(typeof(GeneralSetUpEnum), Convert.ToInt32(r.Status));
            return rec;
        }

        public void SoftDelete(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entity.Isdeleted = 1;
            _context.SaveChanges();
        }
    }
}