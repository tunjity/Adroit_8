using static Adroit_v8.Config.Helper;

using static Adroit_v8.EnumFile.EnumHelper;
namespace Adroit_v8.Repository
{
    public interface ICRMGenericRepository<T> where T : BaseEntityForCRM
    {
        //IEnumerable<T> GetAllIsValid();
        IEnumerable<T> GetAll();

        T Get(int id);

        void Insert(T entity);

        void Update(T entity);
        void SoftDelete(T entity);

        void Delete(T entity);
    }

    public class CRMGenericRepository<T> : ICRMGenericRepository<T> where T : BaseEntityForCRM
    {
        public IHttpContextAccessor _httpContextAccessor;

        private readonly CreditWaveContext _context;
        private DbSet<T> entities;
        private string errorMessage = string.Empty;
        AuthDto auth = new AuthDto();

        public CRMGenericRepository(CreditWaveContext context, IHttpContextAccessor httpContextAccessor)
        {
            //in case of controller with many controller with many repo
            if (auth.ClientId == null)
            {
                _httpContextAccessor = httpContextAccessor; 
                auth.IsOtpVerified = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "IsOtpVerified").Value.ToString();

                auth.ClientId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ClientId") != null ? _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ClientId").Value : "";
                auth.FirstName = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "FirstName")?.Value;
                auth.LastName = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "LastName")?.Value;
                auth.ApplicationId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ApplicationId")?.Value;
                auth.email = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "email") != null ? _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "email").Value : "";
                auth.UserName = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserName").Value;
                auth.UserId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
                auth.CreatedBy = $"{auth.UserName}, {auth.FirstName} {auth.LastName}| {auth.UserId}";
            }
            entities = context.Set<T>();
            _context = context;
        }

        public IEnumerable<T> GetAll()
        {
            var rec = entities.AsEnumerable().Where(o => o.ClientId == auth.ClientId);
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
            entity.ClientId = auth.ClientId;
            entity.CreatedBy = auth.CreatedBy;

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


        public void SoftDelete(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            //entity.Isdeleted = 1;
            _context.SaveChanges();
        }
    }
}
