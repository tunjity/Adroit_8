using Adroit_v8.MongoConnections.UnderWriterModel;
using Adroit_v8.MongoConnections;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Adroit_v8.Controllers.Administration
{
    [Route("api/Administration/[controller]")]
    [ApiController]
    [Authorize]
    public class ManageController :AuthController
    {
        private IMongoRepository<Manage> _repoManage;
        public ManageController(IMongoRepository<Manage> repoManage, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _repoManage = repoManage;
        }
        #region Manage
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("addManage")]
        public Task<IActionResult> AddManage([FromBody] AdminManagePost obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Saved Successfully";
            try
            {
                Manage RegularLoanTenor = new()
                {
                    UniqueId = Guid.NewGuid().ToString(),
                    FirstName = obj.FirstName,
                    LastName = obj.LastName,
                    MiddleName = obj.MiddleName,
                    EmailAddress = obj.EmailAddress,
                    PhoneNumber = obj.PhoneNumber,
                    Level = obj.Level,
                };
                _repoManage.InsertOne(RegularLoanTenor);

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
        [Route("getallManage")]
        public Task<IActionResult> GetAllManage([FromQuery] PaginationWithOutFilterModel obj)
        {
            var r = new ReturnObject(); bool eget = false;
            try
            {
                var query = _repoManage.AsQueryable();
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
        [Route("getManagebyuniqueid/id")]
        public Task<IActionResult> GetManage(string id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoManage.FindById(id);
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
        [Route("deleteManagebyuniqueid/id")]
        public Task<IActionResult> DeleteManage(string id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Deleted Successfully";
            try
            {
                _repoManage.DeleteById(id);
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
        [Route("updateManage")]
        public Task<IActionResult> UpdateManage([FromBody] AdminManagePut obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record updated Successfully";
            try
            {
                var retChecker = _repoManage.FindById(obj.UniqueId);
                if (retChecker is null)
                {
                    r.status = false;
                    r.message = "Record Not Found";
                }
                else
                {
                    retChecker.FirstName = obj.FirstName;
                    retChecker.UniqueId = obj.UniqueId;
                    retChecker.PhoneNumber = obj.PhoneNumber;
                    retChecker.LastName = obj.LastName;
                    retChecker.EmailAddress = obj.EmailAddress;
                    retChecker.MiddleName = obj.MiddleName;
                    retChecker.Level = obj.Level;
                    _repoManage.ReplaceOne(retChecker);
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
