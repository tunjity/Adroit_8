using Adroit_v8.Models.Administration;
using Adroit_v8.Models.FormModel;
using Microsoft.AspNetCore.Authorization;

namespace Adroit_v8.Controllers.Administration
{
    [Route("api/Administration/[controller]")]
    [ApiController]
    [Authorize]
    public class LoanTenorController : AuthController
    {
        private IGenericRepository<LoanTenor> _repoLoanTenor;
        private readonly CreditWaveContext _context;
        string clientId = "";
        public LoanTenorController(IGenericRepository<LoanTenor> repoLoanTenor, CreditWaveContext context, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _repoLoanTenor = repoLoanTenor;
            _context = context;
            clientId = GetAuthData().ClientId; 
        }

        #region LoanTenors
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("addLoanTenors")]
        public Task<IActionResult> AddLoanTenor([FromBody] UtilityFormModelAdmin obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Saved Successfully";
            try
            {
                var ret = _context.LoanTenors.FirstOrDefault(o => o.Name.ToLower() == obj.Name.ToLower() && o.clientid == clientId && o.Isdeleted == 0);
                if (ret != null)
                {
                    r.status = false;
                    r.message = "Record Already Exist";
                }
                else
                {
                    LoanTenor RegularLoanTenor = new LoanTenor { UniqueId = Guid.NewGuid().ToString(), Name = obj.Name, Status = obj.StatusID };
                    _repoLoanTenor.Insert(RegularLoanTenor);
                }
                return Task.FromResult<IActionResult>(Ok(r));
            }
            catch (Exception ex)
            {
                return Task.FromResult<IActionResult>(StatusCode(StatusCodes.Status500InternalServerError, new ReturnObject
                {
                    status = false,
                    message = ex.Message
                }));
            }
        }
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getallLoanTenors")]
        public Task<IActionResult> GetAllLoanTenors([FromQuery] PaginationWithOutFilterModel obj)
        {
            var r = new ReturnObject(); bool eget = false;
            try
            {
                var query = _repoLoanTenor.GetAll().Where(o => o.Isdeleted == 0);
                var fneRes = query.Skip((obj.PageNumber - 1) * obj.PasgeSize)
                   .Take(obj.PasgeSize);
                if (query.Any())
                    eget = true;
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = fneRes.ToList();
                r.recordCount = query.Count();
                r.recordPageNumber = obj.PageNumber;
                return Task.FromResult<IActionResult>(Ok(r));
            }
            catch (Exception ex)
            {
                return Task.FromResult<IActionResult>(StatusCode(StatusCodes.Status500InternalServerError, new ReturnObject
                {
                    status = false,
                    message = ex.Message
                }));
            }
        }
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getall")]
        public Task<IActionResult> GetAll()
        {
            var r = new ReturnObject();
            try
            {
                r.data = _repoLoanTenor.GetAll().Where(o => o.Isdeleted == 0);
                return Task.FromResult<IActionResult>(Ok(r));
            }
            catch (Exception ex)
            {
                return Task.FromResult<IActionResult>(StatusCode(StatusCodes.Status500InternalServerError, new ReturnObject
                {
                    status = false,
                    message = ex.Message
                }));
            }
        }
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getallvalidLoanTenors")]
        public Task<IActionResult> GetAllValidLoanTenors([FromQuery] PaginationWithOutFilterModel obj)
        {
            var r = new ReturnObject(); bool eget = false;
            try
            {
                var query = _repoLoanTenor.GetAllIsValid();
                var fneRes = query.Skip((obj.PageNumber - 1) * obj.PasgeSize)
                     .Take(obj.PasgeSize);
                if (query.Any())
                    eget = true;
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = fneRes.ToList();
                r.recordCount = query.Count();
                r.recordPageNumber = obj.PageNumber;
                return Task.FromResult<IActionResult>(Ok(r));
            }
            catch (Exception ex)
            {
                return Task.FromResult<IActionResult>(StatusCode(StatusCodes.Status500InternalServerError, new ReturnObject
                {
                    status = false,
                    message = ex.Message
                }));
            }
        }
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getLoanTenorbyid/id")]
        public Task<IActionResult> GetLoanTenors(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoLoanTenor.Get(id);
                return Task.FromResult<IActionResult>(Ok(r));
            }
            catch (Exception ex)
            {
                return Task.FromResult<IActionResult>(StatusCode(StatusCodes.Status500InternalServerError, new ReturnObject
                {
                    status = false,
                    message = ex.Message
                }));
            }
        }
        [HttpDelete]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("deleteLoanTenorbyid/id")]
        public Task<IActionResult> DeleteLoanTenors(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Deleted Successfully";
            try
            {
                var rec = _repoLoanTenor.Get(id);
                _repoLoanTenor.SoftDelete(rec);
                return Task.FromResult<IActionResult>(Ok(r));
            }
            catch (Exception ex)
            {
                return Task.FromResult<IActionResult>(StatusCode(StatusCodes.Status500InternalServerError, new ReturnObject
                {
                    status = false,
                    message = ex.Message
                }));
            }
        }
        [HttpPut]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("updateLoanTenor")]
        public Task<IActionResult> UpdateLoanTenor([FromBody] UtilityModificationFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record updated Successfully";
            try
            {
                var retChecker = _repoLoanTenor.Get(obj.Id);
                if (retChecker is null)
                {
                    r.status = false;
                    r.message = "Record Not Found";
                }
                else
                {
                    retChecker.Name = obj.Name;
                    retChecker.Status = obj.StatusID;
                    _repoLoanTenor.Update(retChecker);
                }

                return Task.FromResult<IActionResult>(Ok(r));
            }
            catch (Exception ex)
            {
                return Task.FromResult<IActionResult>(StatusCode(StatusCodes.Status500InternalServerError, new ReturnObject
                {
                    status = false,
                    message = ex.Message
                }));
            }
        }
        #endregion
    }
}
