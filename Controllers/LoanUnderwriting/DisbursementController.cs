
using Adroit_v8.Models.FormModel;
using Adroit_v8.MongoConnections.LoanApplication;
using Adroit_v8.Service;
using static Adroit_v8.EnumFile.EnumHelper;
using static Adroit_v8.Config.Helper;
using MongoDB.Driver;
using Nancy.Json;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Adroit_v8.MongoConnections.CustomerCentric;
using Adroit_v8.Model;
using System.Text.RegularExpressions;

namespace Adroit_v8.Controllers.LoanUnderwriting
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DisbursementController : AuthController
    {
        private readonly AdroitDbContext _context;
        private readonly IAdroitRepository<RegularLoan> _repo;
        private readonly IAdroitRepository<RegularLoanStepSix> _repoDoc;
        private readonly IAdroitRepository<RegularLoanComment> _repoComment;
        private readonly IAdroitRepository<RegularLoanReasonToDecline> _repoRTD;
        private readonly ICustomerCentricRepository<RegularLoanRestructure> _repoRS;
        private readonly ICustomerCentricRepository<LoanTopUp> _repoTP;
        private readonly IMongoRepository<RegularLoanDisbursement> _repoLD;
        private readonly IConfiguration _config;
        private readonly IFilterRepository _repoF;
        string errMsg = "Unable to process request, kindly try again";
        public DisbursementController(IAdroitRepository<RegularLoan> repo, AdroitDbContext context, ICustomerCentricRepository<LoanTopUp> repoTP, ICustomerCentricRepository<RegularLoanRestructure> repoRS, IFilterRepository repoF, IConfiguration config, IMongoRepository<RegularLoanDisbursement> repoLD, IAdroitRepository<RegularLoanReasonToDecline> repoRTD,
            IAdroitRepository<RegularLoanComment> repoComment, IAdroitRepository<RegularLoanStepSix> repoDoc, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _repo = repo;
            _context = context;
            _repoTP = repoTP;
            _repoRS = repoRS;
            _repoF = repoF;
            _config = config;
            _repoLD = repoLD;
            _repoRTD = repoRTD;
            _repoComment = repoComment;
            string? connectionURI = _config.GetSection("MongoDB").GetSection("ConnectionURI").Value;
            string? databaseName = _config.GetSection("MongoDB").GetSection("MobileDatabaseName").Value;
            MongoClient client = new MongoClient(connectionURI);
            IMongoDatabase database = client.GetDatabase(databaseName);
            _repoDoc = repoDoc;
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
                        FilterDet = "disbursement"
                    }),
                    2 => _repo.AsQueryable().Where(o => o.Status == (int)AdroitLoanApplicationStatus.Disburse),
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
                r.data =  eget ? fneRes.ToList(): new List<RegularLoan>();
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
                var res = _repo.AsQueryable().FirstOrDefault(o => o.CustomerId == cusId&&o.Status == (int)AdroitLoanApplicationStatus.Disburse);
                if (res != null)
                {
                    var ld = _repoLD.AsQueryable().FirstOrDefault(o => o.CustomerId == cusId && o.LoanApplicationId == res.LoanApplicationId);
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
                    aa.DisbursementStatus = ld.DisbursementStatus;
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
                var genders = _context.Genders.ToList();
                var allSav = _context.Customers.ToList();

                switch (obj.Category)
                {
                    case 1:
                        switch (obj.Det)
                        {
                            case 1:
                                qy = _repoTP.AsQueryable().Where(o => o.Status == (int)AdroitLoanApplicationStatus.Disburse
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
                                qy = _repoTP.AsQueryable().Where(o => o.Status == (int)AdroitLoanApplicationStatus.Disburse).ToList();
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
                                query = _repoRS.AsQueryable().Where(o => o.Status == (int)AdroitLoanApplicationStatus.Disburse && (o.DateCreated > obj.StartDate && o.DateCreated < obj.EndDate.AddDays(1))).ToList();
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
                                query = _repoRS.AsQueryable().Where(o => o.Status == (int)AdroitLoanApplicationStatus.Disburse).ToList();
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
        [HttpPut]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("StopDisbursement")]
        public async Task<IActionResult> StopDisbursement([FromBody] RegularLoanCommentFormModelToGet obj)
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

                switch (obj.LoanCategory.ToLower())
                {
                    case "regular loan":
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
                    case "loan topup":
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
                    case "loan restructure":
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

            var r = new ReturnObject();
            try
            {
                switch (obj.LoanCategory.ToLower())
                {
                    case "regular loan":
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
                    case "loan topup":
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
                    case "loan restructure":
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
            //var r = new ReturnObject();
            //try
            //{
            //    var res = _repo.AsQueryable().FirstOrDefault(o => o.ApplicantNumber == obj.LoanApplicationId);
            //    if (res != null)
            //    {
            //        res.Status = (int)AdroitLoanApplicationStatus.Disburse;
            //        _repo.ReplaceOne(res);
            //        r.data = res;
            //    }
            //    r.status = res != null ? true : false;
            //    r.message = res != null ? "Record Found Successfully" : "Not Found";
            //    return Ok(r);
            //}
            //catch (Exception ex)
            //{
            //    return StatusCode(StatusCodes.Status500InternalServerError, new ReturnObject
            //    {
            //        status = false,
            //        message = ex.Message
            //    });
            //}
        }
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getRepaymentDetails")]
        public async Task<IActionResult> GetAllRepaymentDetails([FromBody] RegularLoanCommentFormModelToGet obj)
        {
            var r = new ReturnObject();
            try
            {
                var res = new
                {
                    repaymentDate = DateTime.Now,
                    principal = "1,000,000",
                    Interest = "600,000",
                    TotalPayment = "1,600,00"
                };
                r.data = res;
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
    }
}