using Adroit_v8.Config;
using Adroit_v8.Models.FormModel;
using Adroit_v8.MongoConnections.LoanApplication;
using Adroit_v8.Service;
using static Adroit_v8.EnumFile.EnumHelper;
using static Adroit_v8.Config.Helper;
using MongoDB.Driver;
using Nancy.Json;
using Newtonsoft.Json;
using Adroit_v8.MongoConnections.CRM;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Adroit_v8.MongoConnections.CustomerCentric;
using Adroit_v8.Model;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace Adroit_v8.Controllers.LoanUnderwriting
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ApprovalController : AuthController
    {
        private readonly IAdroitRepository<RegularLoan> _repo;
        private readonly IMongoRepository<RegularLoanDisbursement> _repoLD;
        private readonly IMongoRepository<ClientEmploymentHistory> _reo;
        private readonly IMongoRepository<ClientNextOfKin> _repoNK;
        private readonly CreditWaveContext _context;
        private readonly AdroitDbContext _db;
        private readonly ICustomerCentricRepository<RegularLoanRestructure> _repoRS;
        private readonly ICustomerCentricRepository<LoanTopUp> _repoTP;
        private readonly IAdroitRepository<RegularLoanRepaymentPlan> _repoRegularLoanRepaymentPlan;
        private readonly IAdroitRepository<RegularLoanStepSix> _repoDoc;
        private readonly IAdroitRepository<RegularLoanComment> _repoComment;
        private readonly IAdroitRepository<RegularLoanReasonToDecline> _repoRTD;
        private readonly IAdroitRepository<RegularLoanAdjustment> _repoRLA;
        private readonly IMongoCollection<RegularLoan> _customerLoan;
        private readonly IMongoCollection<RegularLoanDisbursement> _customerDet;
        private readonly IConfiguration _config;
        private readonly IFilterRepository _repoF;
        AuthDto auth = new AuthDto();
        string errMsg = "Unable to process request, kindly try again";
        public ApprovalController(IAdroitRepository<RegularLoan> repo, AdroitDbContext db, ICustomerCentricRepository<LoanTopUp> repoTP, ICustomerCentricRepository<RegularLoanRestructure> repoRS, IFilterRepository repoF, IMongoRepository<RegularLoanDisbursement> repoLD,
         IMongoRepository<ClientEmploymentHistory> reo, IMongoRepository<ClientNextOfKin> repoNK, CreditWaveContext context, IConfiguration config, IAdroitRepository<RegularLoanAdjustment> repoRLA,
         IAdroitRepository<RegularLoanReasonToDecline> repoRTD, IAdroitRepository<RegularLoanRepaymentPlan> repoRegularLoanRepaymentPlan, IAdroitRepository<RegularLoanComment> repoComment, IAdroitRepository<RegularLoanStepSix> repoDoc,
         IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
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
            _repo = repo;
            _db = db;
            _repoTP = repoTP;
            _repoRS = repoRS;
            _repoF = repoF;
            _repoLD = repoLD;
            _reo = reo;
            _repoNK = repoNK;
            _context = context;
            _config = config;
            _repoRLA = repoRLA;
            _repoRTD = repoRTD;
            _repoRegularLoanRepaymentPlan = repoRegularLoanRepaymentPlan;
            _repoComment = repoComment;
            string? connectionURI = _config.GetSection("MongoDB").GetSection("ConnectionURI").Value;
            string? databaseName = _config.GetSection("MongoDB").GetSection("MobileDatabaseName").Value;
            MongoClient client = new MongoClient(connectionURI);
            IMongoDatabase database = client.GetDatabase(databaseName);
            _customerLoan = database.GetCollection<RegularLoan>("RegularLoan");
            _customerDet = database.GetCollection<RegularLoanDisbursement>("RegularLoanDisbursement");
            _repoDoc = repoDoc;
        }
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getbyloantype")]
        public async Task<IActionResult> GetAllByLoanType([FromQuery] FilFormModel obj)
        {
            var r = new ReturnObject();
            bool eget = false;
            try
            {
                var fneRes = new object();
                List<LoanTopUp>? qy = null;
                List<LoanTopUpResponse>? resI = null;
                List<RegularLoanRestructure>? query = null;
                var genders = _db.Genders.ToList();
                var allSav = _db.Customers.ToList();

                switch (obj.Category)
                {
                    case 1:
                        switch (obj.Det)
                        {
                            case 1:
                                qy = _repoTP.AsQueryable().Where(o => o.Status == (int)AdroitLoanApplicationStatus.Approved
                                && (o.DateCreated > obj.StartDate && o.DateCreated < obj.EndDate.AddDays(1))).ToList();
                                if (qy.Any())
                                {

                                    allSav = allSav.Where(o => qy.Select(o => o.CustomerId).Contains(o.Id)).ToList();
                                    resI = GetList(qy, allSav, genders);

                                    switch (obj)
                                    {
                                        case { Channel: null, ApplicantName: not null, ApplicationId: null, Status: 0, EmailAddress: null, PhoneNumber: null }:
                                            resI = resI.Where(o => o.FirstName.Contains(obj.ApplicantName) || o.LastName.Contains(obj.ApplicantName)).ToList();
                                            break;
                                        case { Channel: null, ApplicantName: null, ApplicationId: not null, Status: 0, EmailAddress: null, PhoneNumber: null }:
                                            resI = resI.Where(o => o.LoanApplicationId == obj.ApplicationId).ToList();
                                            break;
                                        case { Channel: null, ApplicantName: null, ApplicationId: null, Status: not 0, EmailAddress: null, PhoneNumber: null }:
                                            resI = resI.Where(o => o.Status == obj.Status.ToString()).ToList();
                                            break;
                                        case { Channel: null, ApplicantName: null, ApplicationId: null, Status: 0, EmailAddress: not null, PhoneNumber: null }:
                                            resI = resI.Where(o => o.EmailAddress == obj.EmailAddress).ToList();
                                            break;
                                        default:
                                            break;
                                    };
                                    fneRes = resI.Skip((obj.PageNumber - 1) * obj.PasgeSize).Take(obj.PasgeSize);
                                    r.recordCount = resI.Count();
                                    eget = true;
                                }
                                break;
                            case 2:
                                qy = _repoTP.AsQueryable().Where(o => o.Status == (int)AdroitLoanApplicationStatus.Approved).ToList();
                                if (qy.Any())
                                {
                                    allSav = allSav.Where(o => qy.Select(o => o.CustomerId).Contains(o.Id)).ToList();
                                    resI = GetList(qy, allSav, genders);
                                    fneRes = resI.Skip((obj.PageNumber - 1) * obj.PasgeSize).Take(obj.PasgeSize);
                                    r.recordCount = resI.Count();
                                    eget = true;
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    case 2:
                        switch (obj.Det)
                        {
                            case 1:
                                query = _repoRS.AsQueryable().Where(o => o.Status == (int)AdroitLoanApplicationStatus.Approved && (o.DateCreated > obj.StartDate && o.DateCreated < obj.EndDate.AddDays(1))).ToList();
                                if (query.Any())
                                {
                                    allSav = allSav.Where(o => query.Select(o => o.CustomerId).Contains(o.Id)).ToList();
                                    var res = GetList(query, allSav);

                                    switch (obj)
                                    {
                                        case { Channel: null, ApplicantName: not null, ApplicationId: null, Status: 0, EmailAddress: null, PhoneNumber: null }:
                                            res = res.Where(o => o.FirstName.Contains(obj.ApplicantName) || o.LastName.Contains(obj.ApplicantName)).ToList();
                                            break;
                                        case { Channel: null, ApplicantName: null, ApplicationId: not null, Status: 0, EmailAddress: null, PhoneNumber: null }:
                                            res = res.Where(o => o.LoanApplicationId == obj.ApplicationId).ToList();
                                            break;
                                        case { Channel: null, ApplicantName: null, ApplicationId: null, Status: not 0, EmailAddress: null, PhoneNumber: null }:
                                            res = res.Where(o => o.Status == obj.Status).ToList();
                                            break;
                                        case { Channel: null, ApplicantName: null, ApplicationId: null, Status: 0, EmailAddress: not null, PhoneNumber: null }:
                                            res = res.Where(o => o.EmailAddress == obj.EmailAddress).ToList();
                                            break;
                                        default:
                                            break;
                                    };
                                    fneRes = res.Skip((obj.PageNumber - 1) * obj.PasgeSize).Take(obj.PasgeSize);
                                    r.recordCount = res.Count();
                                    eget = true;
                                }
                                break;
                            case 2:
                                query = _repoRS.AsQueryable().Where(o => o.Status == (int)AdroitLoanApplicationStatus.Approved).ToList();
                                if (query.Count > 0)
                                {
                                    allSav = allSav.Where(o => query.Select(o => o.CustomerId).Contains(o.Id)).ToList();
                                    var res = GetList(query, allSav);
                                    fneRes = res.Skip((obj.PageNumber - 1) * obj.PasgeSize).Take(obj.PasgeSize);
                                    r.recordCount = res.Count();
                                    eget = true;
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = eget ? fneRes : new List<object>();
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


        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("get")]
        public async Task<IActionResult> GetAll([FromQuery] FilFormModel obj)
        {
            var r = new ReturnObject(); bool eget = false;
            try
            {
                IQueryable<RegularLoan>? query = null;
                IQueryable<RegularLoan>? fneRes = null;
                query = obj.Det switch
                {
                    1 => _repoF.GetLoanFilter(new FilFormModelIn
                    {
                        EndDate = obj.EndDate,
                        StartDate = obj.StartDate,
                        Status = obj.Status,
                        ApplicantName = obj.ApplicantName,
                        EmailAddress = obj.EmailAddress,
                        ApplicationId = obj.ApplicationId,
                        Channel = obj.Channel,
                        PhoneNumber = obj.PhoneNumber,
                        FilterDet = "approved"
                    }),
                    2 => _repo.AsQueryable().Where(o => o.Status == (int)AdroitLoanApplicationStatus.Approved),

                    _ => null
                };
                if (query.Any())
                {
                    eget = true;
                    fneRes = query.Skip((obj.PageNumber - 1) * obj.PasgeSize)
                         .Take(obj.PasgeSize);
                }
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = eget ? fneRes.ToList() : new List<RegularLoan>();
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
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getbyCusId/{cusId}")]
        public async Task<IActionResult> GetByCustomerId([FromRoute] int cusId)
        {
            var r = new ReturnObject();
            try
            {
                var res = _repo.AsQueryable().FirstOrDefault(o => o.CustomerId == cusId && o.Status == (int)AdroitLoanApplicationStatus.Approved);
                if (res != null)
                {
                    var resBs = _repoDoc.AsQueryable().FirstOrDefault(o => o.CustomerId == cusId && o.LoanApplicationId == res.LoanApplicationId);
                    if (resBs is not null)
                    {
                        var SavePath = $"{_config["FileFolder:BankStatementPath"]}/{resBs.BankStatementOfAccount}";

                        var client = new HttpClient();
                        var request = new HttpRequestMessage(HttpMethod.Get, SavePath);

                        var response = await client.SendAsync(request);
                        response.EnsureSuccessStatusCode();
                        var image = await response.Content.ReadAsByteArrayAsync();

                        resBs.BankStatementOfAccount = Convert.ToBase64String(image);
                    }
                    string? enumName = Enum.GetName(typeof(AdroitLoanApplicationStatus), res.Status);
                    LoanApplicationVM aa = new LoanApplicationVM();
                    aa.ApplicationId = res.ApplicantNumber;
                    aa.SubmissionDate = res.DateCreated.ToString("dddd, dd MMMM yyyy");
                    aa.ApplicationDate = res.DateCreated.ToString("dddd, dd MMMM yyyy");
                    aa.ProcessingFee = "N/A";
                    aa.Duration = res.LoanDuration.ToString();
                    aa.AssignedLoanOfficer = "N/A";
                    aa.Status = enumName != null ? enumName : "N/A";
                    aa.AmountRequested = res.LoanAmount.ToString();
                    aa.TotalAmount = res.LoanAmount.ToString();
                    var finalres = new { Information = aa, bankStatement = resBs };
                    r.data = finalres;
                }
                r.status = res != null ? true : false;
                r.message = res != null ? "Record Found Successfully" : "Not Found";
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
        [Route("addComment")]
        public async Task<IActionResult> AddComment([FromBody] RegularLoanCommentFormModel obj)
        {
            string actionUrl = $"{this.ControllerContext.RouteData.Values["controller"].ToString()}/{this.ControllerContext.RouteData.Values["action"].ToString()}";
            var response = new ReturnObject();
            var RequestTime = DateTime.Now;
            try
            {
                var com = new RegularLoanComment { LoanApplicationId = obj.LoanApplicationId, Description = obj.Description };
                var repo = await _repoComment.InsertOneAsync(com);
                return StatusCode(StatusCodes.Status200OK, repo);
            }
            catch (Exception ex)
            {
                LogService.LoggerCreateAsync(JsonConvert.SerializeObject(obj), actionUrl, DateTime.Now, ex.ToString(), "", (int)ServiceLogLevel.Exception);
                return StatusCode(StatusCodes.Status500InternalServerError, new ReturnObject { status = false, message = "Error occured while processing request, please try again." });
            }
        }
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("Decline")]
        public async Task<IActionResult> Decline([FromBody] RegularLoanReasonTodeclineFormModel obj)
        {
            string actionUrl = $"{this.ControllerContext.RouteData.Values["controller"].ToString()}/{this.ControllerContext.RouteData.Values["action"].ToString()}";
            var response = new ReturnObject();
            var RequestTime = DateTime.Now;
            try
            {
                JavaScriptSerializer js = new();
                var com = new RegularLoanReasonToDecline { LoanApplicationId = obj.LoanApplicationId, Comment = obj.Comment };
                com.Reasons = js.Serialize(obj.Reasons);
                var repo = await _repoRTD.InsertOneAsync(com);
                if (repo.status = true)
                {
                    var res = _repo.AsQueryable().FirstOrDefault(o => o.ApplicantNumber == obj.LoanApplicationId);
                    if (res != null)
                    {
                        res.Status = (int)AdroitLoanApplicationStatus.Declined;
                        _repo.ReplaceOne(res);
                    }
                }
                return StatusCode(StatusCodes.Status200OK, repo);
            }
            catch (Exception ex)
            {
                LogService.LoggerCreateAsync(JsonConvert.SerializeObject(obj), actionUrl, DateTime.Now, ex.ToString(), "", (int)ServiceLogLevel.Exception);
                return StatusCode(StatusCodes.Status500InternalServerError, new ReturnObject { status = false, message = "Error occured while processing request, please try again." });
            }
        }

        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getComments")]
        public async Task<IActionResult> GetAllComment([FromBody] RegularLoanCommentFormModelToGet obj)
        {
            var r = new ReturnObject();
            try
            {
                var res = _repoComment.AsQueryable().Where(o => o.LoanApplicationId == obj.LoanApplicationId);
                if (res != null)
                {
                    r.data = res;
                }
                r.status = res != null ? true : false;
                r.message = res != null ? "Record Found Successfully" : "Not Found";
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
        [Route("ManualDisbursementDeterminate")]
        public async Task<IActionResult> ManualDisbursementDeterminate([FromBody] DisturbFormModelToGet obj)
        {
            var r = new ReturnObject();
            try
            {
                switch (obj.LoanCategory.ToLower())
                {
                    case "regular loan":
                        var res = _repoLD.AsQueryable().FirstOrDefault(o => o.LoanApplicationId == obj.LoanApplicationId);
                        if (res != null)
                        {
                            if (res.DisbursementStatus)
                            {
                                r.status = res != null ? true : false;
                                r.message = res != null ? "Record Already Disbursed" : "Not Found";
                            }
                            else
                            {
                                res.Description = obj.Description;
                                _repoLD.ReplaceOne(res);
                                r.status = res != null ? true : false;
                                r.message = res != null ? "Record Disbursed Manually" : "Not Found";
                            }
                        }
                        return Ok(r);
                    case "loan topup":
                        var resII = _repoTP.AsQueryable().FirstOrDefault(o => o.ApplicantNumber == obj.LoanApplicationId);
                        if (resII != null)
                        {
                            if (resII.DisbursementStatus)
                            {
                                r.status = resII != null ? true : false;
                                r.message = resII != null ? "Record Already Disbursed" : "Not Found";
                            }
                            else
                            {
                                resII.Comment = obj.Description;
                                _repoTP.ReplaceOne(resII);
                                r.status = resII != null ? true : false;
                                r.message = resII != null ? "Record Disbursed Manually" : "Not Found";
                            }
                        }
                        return Ok(r);
                    case "loan restructure":
                        var resIII = _repoRS.AsQueryable().FirstOrDefault(o => o.LoanApplicationId == obj.LoanApplicationId);
                        if (resIII != null)
                        {
                            if (resIII.DisbursementStatus)
                            {
                                r.status = resIII != null ? true : false;
                                r.message = resIII != null ? "Record Already Disbursed" : "Not Found";
                            }
                            else
                            {
                                resIII.Comment = obj.Description;
                                _repoRS.ReplaceOne(resIII);
                                r.status = resIII != null ? true : false;
                                r.message = resIII != null ? "Record Disbursed Manually" : "Not Found";
                            }
                        }
                        return Ok(r);
                    default:
                        break;
                }
                r.status = false;
                r.message = "Not Found";
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
        [HttpPut]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("Update")]
        public async Task<IActionResult> Update([FromBody] RegularLoanCommentFormModelToGet obj)
        {
            var r = new ReturnObject();
            try
            {
                switch (Regex.Replace(obj.LoanCategory, @"\s", "").ToLower())
                {
                    case "regularloan":
                        var res = _repo.AsQueryable().FirstOrDefault(o => o.ApplicantNumber == obj.LoanApplicationId);
                        if (res != null)
                        {
                            res.Status = (int)AdroitLoanApplicationStatus.Disburse;
                            _repo.ReplaceOne(res);
                            r.data = res;
                        }
                        r.status = res != null ? true : false;
                        r.message = res != null ? "Record Found Successfully" : "Not Found";
                        return Ok(r);
                    case "loantopup":
                        var resII = _repoTP.AsQueryable().FirstOrDefault(o => o.ApplicantNumber == obj.LoanApplicationId);
                        if (resII != null)
                        {
                            resII.Status = (int)AdroitLoanApplicationStatus.Disburse;
                            _repoTP.ReplaceOne(resII);
                            r.data = resII;
                        }
                        r.status = resII != null ? true : false;
                        r.message = resII != null ? "Record Found Successfully" : "Not Found";
                        return Ok(r);
                    case "loanrestructure":
                        var resIII = _repoRS.AsQueryable().FirstOrDefault(o => o.LoanApplicationId == obj.LoanApplicationId);
                        if (resIII != null)
                        {
                            resIII.Status = (int)AdroitLoanApplicationStatus.Disburse;
                            _repoRS.ReplaceOne(resIII);
                            r.data = resIII;
                        }
                        r.status = resIII != null ? true : false;
                        r.message = resIII != null ? "Record Found Successfully" : "Not Found";
                        return Ok(r);
                    default:
                        break;
                }
                r.status = false;
                r.message = "Not Found";
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
        [HttpPut]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("Return")]
        public async Task<IActionResult> Return([FromBody] RegularLoanCommentFormModelToGet obj)
        {
            var r = new ReturnObject();
            try
            {
                switch (Regex.Replace(obj.LoanCategory, @"\s", "").ToLower())
                {
                    case "regularloan":
                        var res = _repo.AsQueryable().FirstOrDefault(o => o.ApplicantNumber == obj.LoanApplicationId);
                        if (res != null)
                        {
                            res.Status = (int)AdroitLoanApplicationStatus.Under_Review;
                            _repo.ReplaceOne(res);
                            r.data = res;
                        }
                        r.status = res != null ? true : false;
                        r.message = res != null ? "Record Found Successfully" : "Not Found";
                        return Ok(r);
                    case "loantopup":
                        var resII = _repoTP.AsQueryable().FirstOrDefault(o => o.ApplicantNumber == obj.LoanApplicationId);
                        if (resII != null)
                        {
                            resII.Status = (int)AdroitLoanApplicationStatus.Under_Review;
                            _repoTP.ReplaceOne(resII);
                            r.data = resII;
                        }
                        r.status = resII != null ? true : false;
                        r.message = resII != null ? "Record Found Successfully" : "Not Found";
                        return Ok(r);
                    case "loanrestructure":
                        var resIII = _repoRS.AsQueryable().FirstOrDefault(o => o.LoanApplicationId == obj.LoanApplicationId);
                        if (resIII != null)
                        {
                            resIII.Status = (int)AdroitLoanApplicationStatus.Under_Review;
                            _repoRS.ReplaceOne(resIII);
                            r.data = resIII;
                        }
                        r.status = resIII != null ? true : false;
                        r.message = resIII != null ? "Record Found Successfully" : "Not Found";
                        return Ok(r);
                    default:
                        break;
                }
                r.status = false;
                r.message = "Not Found";
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
        [HttpPut]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("Disburse")]
        public async Task<IActionResult> Disburse([FromBody] RegularLoanCommentFormModelToGet obj)
        {
            var o = new RegularLoan();
            int statusCode = 0;
            StringContent? requestApi = null;
            HttpResponseMessage? rawResponse = null;
            var _passPhrase = _config.GetSection("PassPhrases:Key2").Value;
            var disburseTo = _config.GetSection("DisburseTo:Key1").Value;
            var disburseUrl = _config.GetSection("DisburseTo:Url").Value;
            var r = new ReturnObject();
            try
            {
                using var httpclient = new HttpClient();
                RegularLoanDisbursement rd = null;
                RegularLoan res = null;
                res = _repo.AsQueryable().FirstOrDefault(o => o.ApplicantNumber == obj.LoanApplicationId);
                Models.CRM.Customer? cus = null;
                List<ClientNextOfKin>? next = null;
                List<ClientEmploymentHistory>? emp = null;
                var listOfString = new List<DisError>();
                var encrpt = new Helper.DataEncryption();
                var listOfNumbers = new List<PhoneNumber>();
                string b = "";
                List<carddetail> lstc = new();
                if (res != null)
                {
                    var rec = _repoRegularLoanRepaymentPlan.AsQueryable().Where(o => o.LoanApplicationId == obj.LoanApplicationId);
                    var filter = Builders<RegularLoanDisbursement>.Filter.And(Builders<RegularLoanDisbursement>.Filter.Eq(o => o.ClientId, auth.ClientId),
    Builders<RegularLoanDisbursement>.Filter.Eq(o => o.CustomerId, res.CustomerId));

                    var update = Builders<RegularLoanDisbursement>.Update.Set(o => o.IsClosed, true);

                    var result = await _customerDet.UpdateManyAsync(filter, update);

                    emp = _reo.AsQueryable().Where(o => o.CustomerId == res.CustomerId.ToString()).ToList();
                    next = _repoNK.AsQueryable().Where(o => o.CustomerId == res.CustomerId.ToString()).ToList();
                    cus = _context.Customers.FirstOrDefault(o => o.Id == res.CustomerId);
                    listOfString.Add(new DisError { Items = "No Error Yet" });
                    listOfNumbers.Add(new PhoneNumber { Numbers = cus.AlternativePhoneNumber });
                    listOfNumbers.Add(new PhoneNumber { Numbers = cus.PhoneNumber });
                    lstc.Add(new carddetail
                    {
                        CardNumber = await encrpt.EncryptAsync(res.CardNumber, _passPhrase),
                        CardPin = await encrpt.EncryptAsync(res.CardPin, _passPhrase),
                        ExpiryDate = res.ExpiryDate,
                        CVV = await encrpt.EncryptAsync(res.CVV, _passPhrase),
                        NameOnCard = res.NameOnCard
                    });
                    b = string.IsNullOrEmpty(cus.Bvn) ? "123456789" : cus.Bvn;
                    RepaymentAPI ra = new();
                    ra.clientId = auth.ClientId;
                    ra.loanCustomerId = cus.Id;
                    ra.loanApplicationId = obj.LoanApplicationId;
                    ra.adroitUserId = auth.UserId;
                    ra.createdBy = auth.CreatedBy;
                    List<LoanRepaymentSchedule> lo = new();
                    foreach (var oo in rec)
                    {
                        lo.Add(new LoanRepaymentSchedule
                        {
                            InterestRate = oo.InterestRate,
                            MonthlyRepaymentLoanAmount = oo.MonthlyRepaymentAmount,
                            LoanApplicationId = oo.LoanApplicationId,
                            LoanRepaymentDate = oo.MonthlyRepaymentDate
                        });
                    }
                    rd = new RegularLoanDisbursement
                    {
                        LoanRepaymentSchedule = lo,
                        UniqueId = Guid.NewGuid().ToString(),
                        DisbursedTo = disburseTo,
                        CustomerId = cus.Id,
                        Bvn = await encrpt.EncryptAsync(b, _passPhrase),
                        GenderId = cus.GenderId.GetValueOrDefault(),
                        LoanApplicationId = res.LoanApplicationId,
                        LoanTenor = res.LoanDuration.ToString(),
                        EmploymentType = res.EmploymentType,
                        CustomerEmail = cus.EmailAddress,
                        IsClosed = false,
                        Treated = false,
                        CustomerNIN = cus.Nin,
                        FacebookId = cus.FacebookId,
                        WhatsappNumber = cus.WhatsappNumber,
                        LinkedinId = cus.LinkedinId,
                        DisbursementStatus = false,
                        DateApproved = DateTime.Now,
                        DateCreated = DateTime.Now,
                        DateDisbursed = DateTime.Now,
                        OfficeAddress = res.EmployerAddress,
                        HomeAddress = "",
                        NearestBusstop = "",
                        ChannelId = cus.RegistrationChannelId.GetValueOrDefault().ToString(),
                        InterestRate = res.Interest,
                        NextOfKinDetail = next,
                        WorkDetail = emp,
                        EmployerInformation = emp,
                        Repaymentcarddetail = lstc,
                        ChannelName = "",
                        DisbursementError = listOfString,
                        CustomerPhoneNumber = listOfNumbers
                    };

                    switch (obj.LoanCategory.ToLower().Trim())
                    {
                        case "regularloan":
                            rd.LoanAmount = res.LoanAmount;
                            rd.LoanType = "Regular Loan";
                            var recVal = Helper.GetTotalLoanAmount(res.LoanAmount, res.InterestRate);
                            rd.LoanAmountWithInterest = recVal.FirstOrDefault().Value;
                            _ = _repoLD.InsertOneAsync(rd);
                            //call for repayment schedule
                            ra.loanType = rd.LoanType;
                            ra.DisbursementId = rd.UniqueId;
                            var objApi = JsonConvert.SerializeObject(ra);
                            requestApi = new StringContent(objApi, Encoding.UTF8, "application/json");
                            rawResponse = await httpclient.PostAsync(disburseUrl, requestApi);
                            var r1 = await rawResponse.Content.ReadAsStringAsync();
                            statusCode = (int)rawResponse.StatusCode;
                            if (statusCode != 200)
                            {
                                r.status = false;
                                r.message = "Error From Repayment API";
                                return Ok(r);
                            }
                            res.LoanCategory = "Regular Loan";
                            res.Status = (int)AdroitLoanApplicationStatus.Disburse;
                            _repo.ReplaceOne(res);
                            r.data = res;
                            r.status = res != null ? true : false;
                            r.message = res != null ? "Record Found Successfully" : "Not Found";
                            return Ok(r);

                        case "loantopup":
                            var resII = _repoTP.AsQueryable().FirstOrDefault(o => o.CurrentLoanApplicationId == obj.LoanApplicationId);
                            if (resII != null)
                            {
                                rd.LoanAmount = Convert.ToDecimal(resII.NewLoanAmount);
                                rd.LoanType = "Loan TopUp";
                                var recValII = Helper.GetTotalLoanAmount(Convert.ToDecimal(resII.NewLoanAmount), Convert.ToDecimal(resII.InterestRate));
                                rd.LoanAmountWithInterest = recValII.FirstOrDefault().Value;
                                _ = _repoLD.InsertOneAsync(rd);
                                ra.loanType = rd.LoanType;
                                ra.DisbursementId = rd.UniqueId;
                                requestApi = new StringContent(JsonConvert.SerializeObject(ra), Encoding.UTF8, "application/json");
                                rawResponse = await httpclient.PostAsync(disburseUrl, requestApi);
                                statusCode = (int)rawResponse.StatusCode;
                                if (statusCode != 200)
                                {
                                    r.status = false;
                                    r.message = "Error From Repayment API";
                                    return Ok(r);
                                }
                                resII.Status = (int)AdroitLoanApplicationStatus.Disburse;
                                _repoTP.ReplaceOne(resII);

                                Random generator = new Random();
                                string rr = generator.Next(7, 1000000).ToString("D6");
                                o.LoanApplicationId = $"APP-{DateTime.Now.ToString("dd.m.hh.mm.ss.fff").Replace(".", "")}-{rr}";
                                o.CVV = res.CVV;
                                o.LoanDurationValue = resII.NewLoanTopUpTenor;
                                o.LoanCategory = "Loan TopUp";
                                o.CardNumber = res.CardNumber;
                                o.ApplicantNumber = $"APP-{DateTime.Now.ToString("dd.m.hh.mm.ss.fff").Replace(".", "")}-{rr}";
                                o.WorkEmail = res.WorkEmail;
                                o.AccountHolderName = res.AccountHolderName;
                                o.ApplicationChannel = res.ApplicationChannel;
                                o.BankAccount = res.BankAccount;
                                o.Bank = res.Bank;
                                o.BankName = res.BankName;
                                o.BankStatementOfAccount = res.BankStatementOfAccount;
                                o.BusinessAddress = res.BusinessAddress;
                                o.BusinessAge = res.BusinessAge;
                                o.BusinessType = res.BusinessType;
                                o.CardNumber = res.CardNumber;
                                o.CardPin = res.CardPin;
                                o.ClientId = res.ClientId;
                                o.CreatedBy = res.CreatedBy;
                                o.CustomerId = res.CustomerId;
                                o.DocumentPassword = res.DocumentPassword;
                                o.EncryptedCardDetails = res.EncryptedCardDetails;
                                o.EmployerAddress = res.EmployerAddress;
                                o.EmployerName = res.EmployerName;
                                o.EmploymentType = res.EmploymentType;
                                o.ExpiryDate = res.ExpiryDate;
                                o.FirstName = res.FirstName;
                                o.GrossSalaryOrIncome = res.GrossSalaryOrIncome;
                                o.LoanAmount = Convert.ToDecimal(resII.NewLoanTopUpAmount);
                                o.StageName = res.StageName;
                                o.ApplicationChannel = res.ApplicationChannel;
                                o.Status = res.Status;
                                o.BusinessAddress = res.BusinessAddress;
                                o.BusinessName = res.BusinessName;
                                o.BusinessType = res.BusinessType;
                                o.Status = (int)AdroitLoanApplicationStatus.Disburse;
                                _repo.InsertOne(o);
                                res.Status = (int)AdroitLoanApplicationStatus.Closed;
                                _repo.ReplaceOne(res);
                                r.data = resII;
                            }
                            r.status = resII != null ? true : false;
                            r.message = resII != null ? "Record Found Successfully" : "Not Found";
                            return Ok(r);
                        case "loanrestructure":
                            var resIII = _repoRS.AsQueryable().FirstOrDefault(o => o.LoanApplicationId == obj.LoanApplicationId);
                            if (resIII != null)
                            {
                                rd.LoanAmount = Convert.ToDecimal(resIII.LoanAmount);
                                rd.LoanType = "Loan Restructure";
                                var recValIII = Helper.GetTotalLoanAmount(Convert.ToDecimal(resIII.LoanAmount), Convert.ToDecimal(res.InterestRate));
                                rd.LoanAmountWithInterest = recValIII.FirstOrDefault().Value;
                                _ = _repoLD.InsertOneAsync(rd);
                                ra.loanType = rd.LoanType;
                                ra.DisbursementId = rd.UniqueId;
                                requestApi = new StringContent(JsonConvert.SerializeObject(ra), Encoding.UTF8, "application/json");
                                rawResponse = await httpclient.PostAsync(disburseUrl, requestApi);
                                statusCode = (int)rawResponse.StatusCode;
                                if (statusCode != 200)
                                {
                                    r.status = false;
                                    r.message = "Error From Repayment API";
                                    return Ok(r);
                                }
                                resIII.Status = (int)AdroitLoanApplicationStatus.Disburse;
                                _repoRS.ReplaceOne(resIII);


                                o.LoanApplicationId = $"{res.LoanApplicationId}-R";
                                o.CVV = res.CVV;
                                o.LoanCategory = "Loanr Restructure";
                                o.LoanDurationValue = res.LoanDurationValue;
                                o.CardNumber = res.CardNumber;
                                o.ApplicantNumber = res.ApplicantNumber;
                                o.WorkEmail = res.WorkEmail;
                                o.AccountHolderName = res.AccountHolderName;
                                o.ApplicationChannel = res.ApplicationChannel;
                                o.BankAccount = res.BankAccount;
                                o.Bank = res.Bank;
                                o.BankName = res.BankName;
                                o.BankStatementOfAccount = res.BankStatementOfAccount;
                                o.BusinessAddress = res.BusinessAddress;
                                o.BusinessAge = res.BusinessAge;
                                o.BusinessType = res.BusinessType;
                                o.CardNumber = res.CardNumber;
                                o.CardPin = res.CardPin;
                                o.ClientId = res.ClientId;
                                o.CreatedBy = res.CreatedBy;
                                o.CustomerId = res.CustomerId;
                                o.DocumentPassword = res.DocumentPassword;
                                o.EncryptedCardDetails = res.EncryptedCardDetails;
                                o.EmployerAddress = res.EmployerAddress;
                                o.EmployerName = res.EmployerName;
                                o.EmploymentType = res.EmploymentType;
                                o.ExpiryDate = res.ExpiryDate;
                                o.FirstName = res.FirstName;
                                o.GrossSalaryOrIncome = res.GrossSalaryOrIncome;
                                o.LoanAmount = Convert.ToDecimal(resIII.LoanAmount);
                                o.StageName = res.StageName;
                                o.ApplicationChannel = res.ApplicationChannel;
                                o.Status = res.Status;
                                o.BusinessAddress = res.BusinessAddress;
                                o.BusinessName = res.BusinessName;
                                o.BusinessType = res.BusinessType;
                                o.Status = (int)AdroitLoanApplicationStatus.Disburse;
                                _repo.InsertOne(o);
                                res.Status = (int)AdroitLoanApplicationStatus.Closed;
                                _repo.ReplaceOne(res);
                            }
                            r.status = resIII != null ? true : false;
                            r.message = resIII != null ? "Record Found Successfully" : "Not Found";
                            return Ok(r);
                    }

                    _ = rec.ExecuteDelete();
                }
                r.status = res != null ? true : false;
                r.message = res != null ? "Record Found Successfully" : "Not Found";
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

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getRepaymentDetails")]
        public async Task<IActionResult> GetAllRepaymentDetails([FromQuery] RegularLoanCommentFormModelToGet obj)
        {
            var r = new ReturnObject();
            try
            {
                List<RegularLoanRepaymentReturnModel> lstRes = new();
                var rec = _repoRegularLoanRepaymentPlan.AsQueryable().Where(o => o.LoanApplicationId == obj.LoanApplicationId);
                foreach (var o in rec)
                {
                    //calculate interest
                    decimal inter = Convert.ToDecimal(o.InterestRate);
                    decimal prin = Convert.ToDecimal(o.LoanAmount);

                    var recVal = Helper.GetTotalLoanAmount(prin, inter);
                    lstRes.Add(new RegularLoanRepaymentReturnModel
                    {
                        repaymentDate = o.MonthlyRepaymentDate,
                        principal = o.LoanAmount,
                        Interest = recVal.FirstOrDefault().Key.ToString(),
                        TotalPayment = recVal.FirstOrDefault().Value.ToString()
                    });
                }

                r.data = lstRes;
                r.status = lstRes != null ? true : false;
                r.message = lstRes != null ? "Record Found Successfully" : "Not Found";
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

    }
}