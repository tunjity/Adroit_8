using Adroit_v8.MongoConnections.Models;
using Adroit_v8.MongoConnections;
using Adroit_v8.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Adroit_v8.EnumFile.EnumHelper;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace Adroit_v8.Controllers.BridgeLoan
{
    [Route("api/BridgeLoan/[controller]")]
    [ApiController]
    [Authorize]
    public class GeneralSetUpFacilityTypeController :AuthController
    {
        private readonly IMongoRepository<GeneralSetUpFacilityType> _repo;
        string errMsg = "Unable to process request, kindly try again";
       // string _CreatedBy = "0";
        public GeneralSetUpFacilityTypeController(IMongoRepository<GeneralSetUpFacilityType> repo, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _repo = repo;
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("get")]
        public async Task<IActionResult> GetAll()
        {
            var r = new ReturnObject();
            try
            {
                var res = Get();
                r.status = true;
                r.message = "Record Found Successfully";
                r.data = res;
                return Ok(r);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ReturnObject
                {
                    status = false,
                    message =ex.Message
                });
            }
        }
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getallvalid")]
        public async Task<IActionResult> GetAllValid()
        {
            var r = new ReturnObject();
            try
            {

                var res = Get().Where(o => o.IsDeleted == 0).ToList();
                r.status = true;
                r.message = "Record Found Successfully";
                r.data = res;
                return Ok(r);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ReturnObject
                {
                    status = false,
                    message =ex.Message
                });
            }
        }

        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("add")]
        public async Task<IActionResult> Add([FromBody] DocSetupFm obj)
        {
            var RequestTime = DateTime.UtcNow;
            string actionUrl = $"{ControllerContext.RouteData.Values["controller"]}/{ControllerContext.RouteData.Values["action"]}";
            try
            {
                GeneralSetUpFacilityType la = new()
                {
                    DocName = obj.Name,
                    Status = obj.Status,
                    UniqueId = Guid.NewGuid().ToString(),
                    CreatedBy = obj.CreatedBy
                };
                var res = await _repo.InsertOneAsync(la);
                _ = LogService_Old.LoggerCreateAsync(JsonConvert.SerializeObject(obj), actionUrl, RequestTime, JsonConvert.SerializeObject(res), "", (int)ServiceLogLevel.Information);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _ = LogService_Old.LoggerCreateAsync(JsonConvert.SerializeObject(obj), actionUrl, RequestTime, "", ex.InnerException.ToString(), (int)ServiceLogLevel.Error);
                return StatusCode(StatusCodes.Status500InternalServerError, new ReturnObject
                {
                    status = false,
                    message =ex.Message
                });
            }
        }
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("byuniqueid/{id}")]
        public async Task<IActionResult> GetById([FromRoute] string id)
        {
            try
            {
                var res = _repo.FindById(id);
                if (res != null)
                    res.StatusName = Enum.GetName(typeof(GeneralSetUpEnum), Convert.ToInt32(res.Status));
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ReturnObject
                {
                    status = false,
                    message =ex.Message
                });
            }
        }
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("Update")]
        public async Task<IActionResult> Update([FromBody] DocSetupUpdateFm obj)
        {
            var RequestTime = DateTime.UtcNow;
            string actionUrl = $"{ControllerContext.RouteData.Values["controller"]}/{ControllerContext.RouteData.Values["action"]}";
            try
            {
                var pcu = _repo.FindById(obj.UniqueId);
                if (pcu != null)
                {
                    pcu.Status = obj.Status;
                    pcu.DocName = obj.Name;
                    _repo.ReplaceOne(pcu);
                }
                else
                {
                    _ = LogService_Old.LoggerCreateAsync(JsonConvert.SerializeObject(obj), actionUrl, RequestTime, JsonConvert.SerializeObject(pcu), "", (int)ServiceLogLevel.Warning);
                    return StatusCode(StatusCodes.Status500InternalServerError, new ReturnObject
                    {
                        status = false,
                        message = "Record not Valid"
                    });
                }
                _ = LogService_Old.LoggerCreateAsync(JsonConvert.SerializeObject(obj), actionUrl, RequestTime, JsonConvert.SerializeObject(pcu), "", (int)ServiceLogLevel.Information);
                return Ok(new ReturnObject
                {
                    status = true,
                    message = "Record Update Successfully"
                });
            }
            catch (Exception ex)
            {
                _ = LogService_Old.LoggerCreateAsync(JsonConvert.SerializeObject(obj), actionUrl, RequestTime, "", ex.InnerException.ToString(), (int)ServiceLogLevel.Warning);
                return StatusCode(StatusCodes.Status500InternalServerError, new ReturnObject
                {
                    status = false,
                    message =ex.Message
                });
            }
        }
        [HttpDelete]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("deletebyuniqueid/id")]
        public Task<IActionResult> Deletebyuniqueid(string id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Deleted Successfully";
            try
            {
                _repo.DeleteById(id);
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

        [NonAction]
        private List<GeneralSetUpFacilityType> Get()
        {
            var rec = _repo.AsQueryable();
            foreach (var r in rec)
                if (r.Status != null)
                    r.StatusName = Enum.GetName(typeof(GeneralSetUpEnum), Convert.ToInt32(r.Status));
            return rec.ToList();
        }
    }
}
