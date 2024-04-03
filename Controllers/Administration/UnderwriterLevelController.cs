using Adroit_v8.Models.Administration;
using Adroit_v8.Models.FormModel;
using Microsoft.AspNetCore.Authorization;

namespace Adroit_v8.Controllers.Administration
{
    [Route("api/Administration/[controller]")]
    [ApiController]
    [Authorize]
    public class UnderwriterLevelController : AuthController
    {
        private readonly CreditWaveContext _context;
        private IGenericRepository<UnderwriterLevel> _repo;
        private string errMsg = "Unable to process request, kindly try again";
        string clientId = "";

        public UnderwriterLevelController(
            IGenericRepository<UnderwriterLevel> repo,
            CreditWaveContext context,
            IHttpContextAccessor httpContextAccessor
        )
            : base(httpContextAccessor)
        {
            _repo = repo;
            _context = context;
            clientId = GetAuthData().ClientId;
        }

        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("addUnderwriterLevels")]
        public IActionResult AddUnderwriterLevel([FromBody] UnderwriterLevelFormModel obj)
        {
            UnderwriterLevel? ret = new();
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Saved Successfully";
            try
            {
                ret = _context.UnderwriterLevels.FirstOrDefault(o =>
                    o.Name.ToLower() == obj.Name.ToLower()
                    && o.clientid == clientId
                    && o.Isdeleted == 0
                );
                if (ret != null)
                {
                    r.status = false;
                    r.message = "Record Already Exist";
                }
                else
                {
                    UnderwriterLevel Adminproduct =
                        new()
                        {
                            UniqueId = Guid.NewGuid().ToString(),
                            Name = obj.Name,
                            MaximuimAmount = obj.MaximuimAmount,
                            MinimuimAmount = obj.MinimuimAmount,
                            Datecreated = DateTime.UtcNow
                        };
                    _repo.Insert(Adminproduct);
                }
                return Ok(r);
            }
            catch (Exception ex)
            {
                return (
                    StatusCode(
                        StatusCodes.Status500InternalServerError,
                        new ReturnObject { status = false, message = ex.Message }
                    )
                );
            }
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getallUnderwriterLevels")]
        public IActionResult GetAllUnderwriterLevels([FromQuery] PaginationWithOutFilterModel obj)
        {
            var r = new ReturnObject();
            bool eget = false;
            try
            {
                var query = _repo.GetAll();
                var fneRes = query.Skip((obj.PageNumber - 1) * obj.PasgeSize).Take(obj.PasgeSize);
                if (query.Any())
                    eget = true;
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = fneRes.ToList();
                r.recordCount = query.Count();
                r.recordPageNumber = obj.PageNumber;
                return Ok(r);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ReturnObject { status = false, message = ex.Message }
                );
            }
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getall")]
        public IActionResult GetAll()
        {
            var r = new ReturnObject();
            try
            {
                r.data = _repo.GetAll();
                return Ok(r);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ReturnObject { status = false, message = ex.Message }
                );
            }
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getallvalidUnderwriterLevels")]
        public IActionResult GetAllValidUnderwriterLevels(
            [FromQuery] PaginationWithOutFilterModel obj
        )
        {
            var r = new ReturnObject();
            bool eget = false;
            try
            {
                var query = _repo.GetAllIsValid();
                var fneRes = query.Skip((obj.PageNumber - 1) * obj.PasgeSize).Take(obj.PasgeSize);
                if (query.Any())
                    eget = true;
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = fneRes.ToList();
                r.recordCount = query.Count();
                r.recordPageNumber = obj.PageNumber;
                return Ok(r);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ReturnObject { status = false, message = ex.Message }
                );
            }
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getUnderwriterLevelbyid/id")]
        public IActionResult GetUnderwriterLevels(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repo.Get(id);
                return Ok(r);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ReturnObject { status = false, message = ex.Message }
                );
            }
        }

        [HttpDelete]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("deleteUnderwriterLevelbyid/id")]
        public Task<IActionResult> DeleteUnderwriterLevels(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Deleted Successfully";
            try
            {
                var rec = _repo.Get(id);
                _repo.SoftDelete(rec);
                return Task.FromResult<IActionResult>(Ok(r));
            }
            catch (Exception ex)
            {
                return Task.FromResult<IActionResult>(
                    StatusCode(
                        StatusCodes.Status500InternalServerError,
                        new ReturnObject { status = false, message = ex.Message }
                    )
                );
            }
        }

        [HttpPut]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("updateUnderwriterLevel")]
        public Task<IActionResult> UpdateUnderwriterLevel(
            [FromBody] UnderwriterLevelFormModelUpdate obj
        )
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record updated Successfully";
            try
            {
                var retChecker = _context.UnderwriterLevels.FirstOrDefault(o =>
                    o.Id == obj.Id && o.Isdeleted == 0
                );
                if (retChecker is null)
                {
                    r.status = false;
                    r.message = "Record Not Found";
                }
                else
                {
                    retChecker.Name = obj.Name;
                    retChecker.MaximuimAmount = obj.MaximuimAmount;
                    retChecker.MinimuimAmount = obj.MinimuimAmount;
                    _repo.Update(retChecker);
                }
                return Task.FromResult<IActionResult>(Ok(r));
            }
            catch (Exception ex)
            {
                return Task.FromResult<IActionResult>(
                    StatusCode(
                        StatusCodes.Status500InternalServerError,
                        new ReturnObject { status = false, message = ex.Message }
                    )
                );
            }
        }
    }
}
