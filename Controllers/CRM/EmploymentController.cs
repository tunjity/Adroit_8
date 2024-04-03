using Adroit_v8.MongoConnections.Models;
using Adroit_v8.MongoConnections;
using Adroit_v8.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Adroit_v8.Config.Helper;
using static Adroit_v8.EnumFile.EnumHelper;
using Adroit_v8.MongoConnections.CRM;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace Adroit_v8.Controllers.CRM
{
    [Route("api/CRM/[controller]")]
    [ApiController]
    [Authorize]
    public class EmploymentController :AuthController
    {
        private readonly IMongoRepository<ClientEmploymentHistory> _repo;
        private readonly IMongoRepository<CustomerStageHolder> _repoCustomerStageHolder;
        string errMsg = "Unable to process request, kindly try again";
        string _CreatedBy = "0";
        public EmploymentController(IMongoRepository<ClientEmploymentHistory> repo, 
            IMongoRepository<CustomerStageHolder> repoCustomerStageHolder, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _repo = repo;
            _repoCustomerStageHolder = repoCustomerStageHolder;
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("get")]
        public async Task<IActionResult> GetAll([FromQuery] PaginationWithOutFilterModel obj)
        {
            var r = new ReturnObject(); bool eget = false;
            try
            {
                var query = _repo.AsQueryable(); var fneRes = query.Skip((obj.PageNumber - 1) * obj.PasgeSize)
                    .Take(obj.PasgeSize);
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
                return StatusCode(StatusCodes.Status500InternalServerError, new ReturnObject
                {
                    status = false,
                    message = ex.Message
                });
            }
        }


        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("add")]
        public async Task<IActionResult> Add([FromBody] ClientEmploymentHistoryFormModel obj)
        {
            var RequestTime = DateTime.UtcNow;
            string actionUrl = $"{ControllerContext.RouteData.Values["controller"]}/{ControllerContext.RouteData.Values["action"]}";
            try
            {
                ClientEmploymentHistory la = new()
                {
                    OrganizationId = obj.OrganizationId,
                    CustomerId = obj.CustomerId,
                    StateId = obj.StateId,
                    LgaId = obj.LgaId,
                    Address = obj.Address,
                    StaffId = obj.StaffId,
                    JobRole = obj.JobRole,
                    EmploymentTypeId = obj.EmploymentTypeId,
                    DateOfEmployment = obj.DateOfEmployment,
                    EmailAddress = obj.EmailAddress,
                    SalaryRange = obj.SalaryRange,
                    SalaryPaymentDay = obj.SalaryPaymentDay,
                    NearestLandmark = obj.NearestLandmark,
                    PhoneNumber = obj.PhoneNumber,
                    UniqueId = Guid.NewGuid().ToString(),
                    CreatedBy = _CreatedBy
                };
                var res = await _repo.InsertOneAsync(la);
                CustomerStageHolder st = new()
                {
                    CustomerId = obj.CustomerId,
                    StageId = CustomerRegistrationStage.StageEmploymentInfo.ToString()
                };
                _repoCustomerStageHolder.InsertOne(st);
                _ = LogService_Old.LoggerCreateAsync(JsonConvert.SerializeObject(obj), actionUrl, RequestTime, JsonConvert.SerializeObject(res), "", (int)ServiceLogLevel.Information);
                return Ok(res);
            }
            catch (Exception ex)
            {
                _ = LogService_Old.LoggerCreateAsync(JsonConvert.SerializeObject(obj), actionUrl, RequestTime, "", ex.InnerException.ToString(), (int)ServiceLogLevel.Error);
                return StatusCode(StatusCodes.Status500InternalServerError, new ReturnObject
                {
                    status = false,
                    message = ex.Message
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
                return Ok(_repo.FindById(id));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ReturnObject
                {
                    status = false,
                    message = ex.Message
                });
            }
        }
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("Update")]
        public async Task<IActionResult> Update([FromBody] ClientEmploymentHistoryUpdateFormModel obj)
        {
            var RequestTime = DateTime.UtcNow;
            string actionUrl = $"{ControllerContext.RouteData.Values["controller"]}/{ControllerContext.RouteData.Values["action"]}";
            try
            {
                var pcu = _repo.FindById(obj.UniqueId);
                if (pcu != null)
                {
                    pcu = new()
                    {
                        Id = pcu.Id,
                        ClientId = pcu.ClientId,
                        UniqueId = obj.UniqueId,
                        OrganizationId = obj.OrganizationId,
                        CustomerId = obj.CustomerId,
                        StateId = obj.StateId,
                        LgaId = obj.LgaId,
                        Address = obj.Address,
                        StaffId = obj.StaffId,
                        JobRole = obj.JobRole,
                        EmploymentTypeId = obj.EmploymentTypeId,
                        DateOfEmployment = obj.DateOfEmployment,
                        EmailAddress = obj.EmailAddress,
                        SalaryRange = obj.SalaryRange,
                        SalaryPaymentDay = obj.SalaryPaymentDay,
                        NearestLandmark = obj.NearestLandmark,
                        PhoneNumber = obj.PhoneNumber
                    };
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
                    message = ex.Message
                });
            }
        }
    }
}

