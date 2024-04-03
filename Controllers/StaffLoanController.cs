
using Adroit_v8.Models.FormModel;
using Adroit_v8.MongoConnections;
using Adroit_v8.Service;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Newtonsoft.Json;
using static Adroit_v8.Config.Helper;
using static Adroit_v8.EnumFile.EnumHelper;

namespace Adroit_v8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StaffLoanController : AuthController
    {
        private string errMsg = "Unable to process request, kindly try again";
        private readonly IMongoRepository<StaffLoanModel> _repo;
        private readonly IMongoRepository<StaffLoanInterestRateModel> _repoC;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IMongoCollection<StaffLoanModel> _CustomerLoanStage;
        public StaffLoanController(IMapper mapper, IConfiguration config, IMongoRepository<StaffLoanModel> repo,
            IMongoRepository<StaffLoanInterestRateModel> repoC, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _mapper = mapper;
            _config = config;
            _repo = repo;
            _repoC = repoC;
            string? connectionURI = _config.GetSection("MongoDB").GetSection("ConnectionURI").Value;
            string? databaseName = _config.GetSection("MongoDB").GetSection("DatabaseName").Value;
            MongoClient client = new MongoClient(connectionURI);
            IMongoDatabase database = client.GetDatabase(databaseName);
            _CustomerLoanStage = database.GetCollection<StaffLoanModel>("StaffLoan");
        }
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("GetStaffLoanType")]
        public Task<IActionResult> GetStaffLoanType()
        {
            var res = new Dictionary<int, string>();
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                foreach (var value in Enum.GetValues(typeof(StaffLoanType)))
                {
                    res.Add((int)value, ((StaffLoanType)value).ToString());
                }
                r.data = res.ToList();
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
        [Route("GetStaffLoanApprovalStatus")]
        public Task<IActionResult> GetStaffLoanApprovalStatus()
        {
            var res = new Dictionary<int, string>();
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                foreach (var value in Enum.GetValues(typeof(StaffLoanStatus)))
                {
                    res.Add((int)value, ((StaffLoanStatus)value).ToString());
                }
                r.data = res.ToList();
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
        [Route("UpdateStaffLoanApprovalStatus")]
        public async Task<IActionResult> UpdateStaffLoanApprovalStatus([FromBody] StaffLoanUpdateStatus obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            var RequestTime = DateTime.UtcNow;
            string actionUrl = $"{ControllerContext.RouteData.Values["controller"]}/{ControllerContext.RouteData.Values["action"]}";
            try
            {

                var formalStage = await _CustomerLoanStage.Find(o => o.UniqueId == obj.UniqueId).FirstOrDefaultAsync();
                formalStage.LoanStatusId = obj.LoanStatusId;
                _repo.ReplaceOne(formalStage);
                _ = LogService_Old.LoggerCreateAsync(JsonConvert.SerializeObject(obj), actionUrl, RequestTime, JsonConvert.SerializeObject(obj), "", (int)ServiceLogLevel.Information);
                return Ok(r);
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
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("addStaffLoan")]
        public async Task<IActionResult> AddStaffLoan([FromBody] StaffLoanCreateModel obj)
        {

            var RequestTime = DateTime.UtcNow;
            string actionUrl = $"{ControllerContext.RouteData.Values["controller"]}/{ControllerContext.RouteData.Values["action"]}";
            try
            {
                var mda = _mapper.Map<StaffLoanModel>(obj);
                mda.UniqueId = Guid.NewGuid().ToString();
                mda.LoanStatusId = (int)StaffLoanStatus.Pending;
                var res = await _repo.InsertOneAsync(mda);
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
        [Route("GetEndDate")]
        public IActionResult GetEndDate([FromQuery] int tenor, string startDate)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            r.data = Convert.ToDateTime(startDate).AddMonths(tenor);
            return Ok(r);
        }
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("GetStaffLoanInterestRate")]
        public Task<IActionResult> GetStaffLoanInterestRate()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoC.AsQueryable().ToList();
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
        [Route("ViewStaffLoan/{loanId}")]
        public Task<IActionResult> ViewStaffLoan([FromRoute] string loanId)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                var ret = _repo.FindById(loanId);
                var rS = getScheduleAndAmount(ret.InterestRate, ret.LoanAmount, ret.LoanTenorid);
                var res = new

                {
                    staffId = ret.StaffId,
                    firstname = ret.FirstName,
                    lastname = ret.LastName,
                    PersonalEmail = ret.PersonalEmail,
                    OfficialEmail = ret.OfficialEmail,
                    PhoneNumber = ret.PhoneNumber,
                    LoanAmount = ret.LoanAmount,
                    Status = Enum.TryParse(ret.LoanStatusId.ToString(), out StaffLoanStatus value),
                    purpose = ret.Purpose,
                    repaymentSchedule = rS
                };
                r.data = res;
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


        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("AddStaffLoanInterestRate")]
        public async Task<IActionResult> AddStaffLoanInterestRate([FromBody] StaffLoanInterestRatenCreateModel obj)
        {
            var RequestTime = DateTime.UtcNow;
            string actionUrl = $"{ControllerContext.RouteData.Values["controller"]}/{ControllerContext.RouteData.Values["action"]}";
            try
            {
                var mda = _mapper.Map<StaffLoanInterestRateModel>(obj);
                mda.UniqueId = Guid.NewGuid().ToString();
                var res = await _repoC.InsertOneAsync(mda);
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

    }
}