
using Adroit_v8.Models.FormModel;
using Adroit_v8.MongoConnections;
using Adroit_v8.Service;
using AutoMapper;
using MongoDB.Driver;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using static Adroit_v8.Config.Helper;
using static Adroit_v8.EnumFile.EnumHelper;

namespace Adroit_v8.Controllers.Administration
{
    [Route("api/Administration/[controller]")]
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
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("GetStaffLoan")]
        public Task<IActionResult> GetStaffLoan([FromQuery] FilFormModel obj)
        {
            var r = new ReturnObject(); bool eget = false;
            try
            {
                IQueryable<StaffLoanModel>? query = null;
                IQueryable<StaffLoanModel>? fquery = null;
                // var rec = new List<MainResponse>();
                switch (obj.Det)
                {
                    case 1:
                        query = _repo.AsQueryable().Where(o => o.LoanStatusId == (int)StaffLoanStatus.Pending
                        && o.DateCreated > obj.StartDate
                        && o.DateCreated < obj.EndDate.AddDays(1)//add 1 day to make sure it includes the end date
                        );
                        if (query.Any())
                        {
                            switch (obj)
                            {
                                case { Channel: not null, ApplicantName: null, ApplicationId: null, Status: 0, EmailAddress: null, PhoneNumber: null }:

                                    break;
                                case { Channel: null, ApplicantName: not null, ApplicationId: null, Status: 0, EmailAddress: null, PhoneNumber: null }:
                                    query = query.Where(o => o.FirstName.Contains(obj.ApplicantName) || o.LastName.Contains(obj.ApplicantName));
                                    break;
                                case { Channel: null, ApplicantName: null, ApplicationId: not null, Status: 0, EmailAddress: null, PhoneNumber: null }:
                                    query = query.Where(o => o.UniqueId == obj.ApplicationId);
                                    break;
                                case { Channel: null, ApplicantName: null, ApplicationId: null, Status: not 0, EmailAddress: null, PhoneNumber: null }:
                                    query = query.Where(o => o.LoanStatusId == obj.Status);
                                    break;
                                case { Channel: null, ApplicantName: null, ApplicationId: null, Status: 0, EmailAddress: not null, PhoneNumber: null }:
                                    query = query.Where(o => o.OfficialEmail == obj.EmailAddress);
                                    break;
                                default:
                                    Console.WriteLine("Unknown object");
                                    break;
                            };
                        }
                        break;
                    case 2:
                        query = _repo.AsQueryable().Where(o => o.LoanStatusId == (int)StaffLoanStatus.Pending);
                        break;
                    default:
                        break;
                }

                if (query.Any())
                {
                    foreach (var item in query)
                    {
                        item.StatusName = Enum.TryParse(item.LoanStatusId.ToString(), out StaffLoanStatus value).ToString();
                    }
                    fquery = query.Skip((obj.PageNumber - 1) * obj.PasgeSize)
                            .Take(obj.PasgeSize);

                    eget = true;
                }
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully": "No Record Found";
                r.data = fquery.ToList();
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
        [Route("GetStaffDisbursedLoan")]
        public Task<IActionResult> GetStaffDisbursedLoan([FromQuery] FilFormModel obj)
        {
            var r = new ReturnObject(); bool eget = false;
            try
            {
                IQueryable<StaffLoanModel>? query = null;
                IQueryable<StaffLoanModel>? fquery = null;
                // var rec = new List<MainResponse>();
                switch (obj.Det)
                {
                    case 1:
                        query = _repo.AsQueryable().Where(o => o.LoanStatusId == (int)StaffLoanStatus.Disbursed
                        && o.DateCreated > obj.StartDate
                        && o.DateCreated < obj.EndDate.AddDays(1)//add 1 day to make sure it includes the end date
                        );
                        if (query.Any())
                        {
                            switch (obj)
                            {
                                case { Channel: not null, ApplicantName: null, ApplicationId: null, Status: 0, EmailAddress: null, PhoneNumber: null }:

                                    break;
                                case { Channel: null, ApplicantName: not null, ApplicationId: null, Status: 0, EmailAddress: null, PhoneNumber: null }:
                                    query = query.Where(o => o.FirstName.Contains(obj.ApplicantName) || o.LastName.Contains(obj.ApplicantName));
                                    break;
                                case { Channel: null, ApplicantName: null, ApplicationId: not null, Status: 0, EmailAddress: null, PhoneNumber: null }:
                                    query = query.Where(o => o.UniqueId == obj.ApplicationId);
                                    break;
                                case { Channel: null, ApplicantName: null, ApplicationId: null, Status: not 0, EmailAddress: null, PhoneNumber: null }:
                                    query = query.Where(o => o.LoanStatusId == obj.Status);
                                    break;
                                case { Channel: null, ApplicantName: null, ApplicationId: null, Status: 0, EmailAddress: not null, PhoneNumber: null }:
                                    query = query.Where(o => o.OfficialEmail == obj.EmailAddress);
                                    break;
                                default:
                                    break;
                            };
                        }
                        break;
                    case 2:
                        query = _repo.AsQueryable().Where(o => o.LoanStatusId == (int)StaffLoanStatus.Disbursed);
                        break;
                    default:
                        break;
                }

                if (query.Any())
                {
                    foreach (var item in query)
                    {
                        item.StatusName = Enum.TryParse(item.LoanStatusId.ToString(), out StaffLoanStatus value).ToString();
                    }
                    fquery = query.Skip((obj.PageNumber - 1) * obj.PasgeSize)
                            .Take(obj.PasgeSize);

                    eget = true;
                }
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = fquery.ToList();
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