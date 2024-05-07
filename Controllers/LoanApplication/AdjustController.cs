using Adroit_v8.Models.FormModel;
using Adroit_v8.MongoConnections.LoanApplication;
using Adroit_v8.Service;
using MongoDB.Driver;
using Nancy.Json;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Adroit_v8.MongoConnections.CustomerCentric;
using static Adroit_v8.EnumFile.EnumHelper;
using static Adroit_v8.Config.Helper;
using Adroit_v8.Model;
using System.Text.RegularExpressions;
using System.Text;
using System.Net.Http;
using Adroit_v8.Models.Administration;
using Adroit_v8.Config;

namespace Adroit_v8.Controllers.LoanApplication
{
    [Route("api/LoanApplication/[controller]")]
    [ApiController]
    [Authorize]
    public class AdjustController : AuthController
    {
        private readonly AdroitDbContext _context;
        private readonly IAdroitRepository<RegularLoan> _repo;
        private readonly IAdroitRepository<RegularLoanStepSix> _repoDoc;
        private readonly IAdroitRepository<RegularLoanReasonToDecline> _repoRTD;
        private readonly IAdroitRepository<RegularLoanAdjustment> _repoRLA;
        private readonly IMongoCollection<RegularLoan> _customerLoan;
        private readonly IFilterRepository _repoF;
        private readonly ICustomerCentricRepository<RegularLoanRestructure> _repoRS;
        private readonly ICustomerCentricRepository<LoanTopUp> _repoTP;
        string errMsg = "Unable to process request, kindly try again";
        private readonly IConfiguration _config;
        string clientId = "";
        public AdjustController(IAdroitRepository<RegularLoan> repo, AdroitDbContext context, IAdroitRepository<RegularLoanAdjustment> repoRLA, ICustomerCentricRepository<RegularLoanRestructure> repoRS, ICustomerCentricRepository<LoanTopUp> repoTP, IConfiguration config,
            IAdroitRepository<RegularLoanReasonToDecline> repoRTD, IFilterRepository repoF, IAdroitRepository<RegularLoanStepSix> repoDoc, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _repo = repo;
            _context = context;
            _repoRLA = repoRLA;
            _repoRS = repoRS;
            _repoTP = repoTP;
            _config = config;
            _repoRTD = repoRTD;
            _repoF = repoF;
            _repoDoc = repoDoc;
            string? connectionURI = _config.GetSection("MongoDB").GetSection("ConnectionURI").Value;
            string? databaseName = _config.GetSection("MongoDB").GetSection("MobileDatabaseName").Value;
            MongoClient client = new MongoClient(connectionURI);
            IMongoDatabase database = client.GetDatabase(databaseName);
            _customerLoan = database.GetCollection<RegularLoan>("RegularLoan");
            clientId = GetAuthData().ClientId;
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
                        FilterDet = "adjust"
                    }),
                    2 => _repo.AsQueryable().Where(o => o.Status == (int)AdroitLoanApplicationStatus.Adjust),

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
                var res = _repo.AsQueryable().FirstOrDefault(o => o.CustomerId == cusId && o.Status == (int)AdroitLoanApplicationStatus.Adjust);
                if (res != null)
                {
                    var det = _repoRLA.AsQueryable().FirstOrDefault(o => o.LoanApplicationId == res.LoanApplicationId);
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
                    aa.AmountRequested = res.LoanAmount.ToString(); aa.Interest = res.Interest.ToString();
                    aa.TotalAmount = res.LoanAmount.ToString();

                    var newAdjustedDetail = new
                    {
                        adjustAmount = det != null ? det.AdjustedAmount : "0.00",
                        initialLoanAmount = res.LoanAmount,
                        initialLoanTenor = res.LoanDuration,
                        adjustedLoanTenor = det != null ? det.AdjustedTenor : "N/A",
                        comment = det != null ? det.Description : "N/A"
                    };
                    var finalres = new { Information = aa, AdjustedDetail = newAdjustedDetail, bankStatement = resBs };
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
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getbyLoanId/{loanId}")]
        public async Task<IActionResult> GetByLoanId([FromRoute] string loanId)
        {
            var r = new ReturnObject();
            try
            {
                var res = _repo.AsQueryable().FirstOrDefault(o => o.LoanApplicationId == loanId && o.Status == (int)AdroitLoanApplicationStatus.Adjust);
                if (res != null)
                {
                    var det = _repoRLA.AsQueryable().FirstOrDefault(o => o.LoanApplicationId == res.LoanApplicationId);
                    var resBs = _repoDoc.AsQueryable().FirstOrDefault(o => o.CustomerId == res.CustomerId && o.LoanApplicationId == res.LoanApplicationId);
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
                    aa.AmountRequested = res.LoanAmount.ToString(); aa.Interest = res.Interest.ToString();

                    aa.TotalAmount = res.LoanAmount.ToString();

                    var newAdjustedDetail = new
                    {
                        adjustAmount = det != null ? det.AdjustedAmount : "0.00",
                        initialLoanAmount = res.LoanAmount,
                        initialLoanTenor = res.LoanDuration,
                        adjustedLoanTenor = det != null ? det.AdjustedTenor : "N/A",
                        comment = det != null ? det.Description : "N/A"
                    };
                    var finalres = new { Information = aa, AdjustedDetail = newAdjustedDetail, bankStatement = resBs };
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
                                qy = _repoTP.AsQueryable().Where(o => o.Status == (int)AdroitLoanApplicationStatus.Adjust
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
                                qy = _repoTP.AsQueryable().Where(o => o.Status == (int)AdroitLoanApplicationStatus.Adjust).ToList();

                                if (qy.Count > 0)
                                {
                                    allSav = _context.Customers.Where(o => qy.Select(o => o.CustomerId).Contains(o.Id)).ToList();
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
                                query = _repoRS.AsQueryable().Where(o => o.Status == (int)AdroitLoanApplicationStatus.Adjust && (o.DateCreated > obj.StartDate && o.DateCreated < obj.EndDate.AddDays(1))).ToList();
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
                                query = _repoRS.AsQueryable().Where(o => o.Status == (int)AdroitLoanApplicationStatus.Adjust).ToList();
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
        [Route("Update")]
        public async Task<IActionResult> Update([FromBody] RegularLoanCommentFormModelToGet obj)
        {
            var r = new ReturnObject();
            try
            {
                var res = _repo.AsQueryable().FirstOrDefault(o => o.ApplicantNumber == obj.LoanApplicationId);
                if (res != null)
                {
                    res.Status = (int)AdroitLoanApplicationStatus.Review;
                    _repo.ReplaceOne(res); r.data = res;
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
        [Route("UpdateWithBankStatement")]
        public async Task<IActionResult> UpdateWithBankStatement([FromForm] UpdateWithBankStatement obj)
        {
            int statusCode = 0;
            StringContent? requestApi = null;
            HttpResponseMessage? rawResponse = null;
            var r = new ReturnObject();
            long cusId = 0;
            var disburseUrl = _config.GetSection("DisburseTo:RepaymentUrl").Value;
            var bankStatementUrl = _config.GetSection("FileFolder:AddBankStatement").Value;
            using var httpclient = new HttpClient();
            try
            {
                switch (Regex.Replace(obj.LoanCategory, @"\s", "").ToLower())
                {
                    case "regularloan":
                        var res = _repo.AsQueryable().FirstOrDefault(o => o.ApplicantNumber == obj.LoanApplicationId);
                        if (res != null)
                        {
                            cusId = res.CustomerId;
                            res.Status = (int)AdroitLoanApplicationStatus.Review;
                            res.LoanAmount = obj.AdjustedAmount;
                            _repo.ReplaceOne(res);
                        }
                        break;
                    case "loantopup":
                        var resII = _repoTP.AsQueryable().FirstOrDefault(o => o.ApplicantNumber == obj.LoanApplicationId);
                        if (resII != null)
                        {
                            cusId = resII.CustomerId;
                            resII.Status = (int)AdroitLoanApplicationStatus.Under_Review;
                            resII.NewLoanTopUpAmount = obj.AdjustedAmount;
                            resII.NewLoanTopUpTenor = obj.AdjustedTenor;
                            _repoTP.ReplaceOne(resII);
                        }
                        break;
                    case "loanrestructure":
                        var resIII = _repoRS.AsQueryable().FirstOrDefault(o => o.LoanApplicationId == obj.LoanApplicationId);
                        if (resIII != null)
                        {
                            cusId = resIII.CustomerId;
                            var ra = new
                            {
                                clientId = clientId,
                                loanApplicationId = obj.LoanApplicationId,
                                loanTenorId = obj.AdjustedTenor,
                                loanAmount = obj.AdjustedAmount
                            };
                            var objApi = JsonConvert.SerializeObject(ra);
                            requestApi = new StringContent(objApi, Encoding.UTF8, "application/json");
                            rawResponse = await httpclient.PostAsync(disburseUrl, requestApi);
                            var r1 = await rawResponse.Content.ReadAsStringAsync();
                            statusCode = (int)rawResponse.StatusCode;
                            if (statusCode != 200)
                            {
                                r.status = false;
                                r.message = "Error From Restructure Repayment API ";
                            }
                            else
                            {
                                r.status = true;
                                r.message = "Loan Restructured Successfully";
                            }
                        }
                        break;
                    default:
                        break;
                }
                if (obj.BankStatement != null)
                {
                    var bas = Helper.ConvertIFormFilesToBase64(obj.BankStatement);
                    string docName = $"{Guid.NewGuid().ToString()}{"_"}{Path.GetFileName(obj.BankStatement.FileName)}";
                    var objra = new
                    {
                        statementOfAccount = bas,
                        fileName = docName,
                        customerId = cusId
                    };
                    var Api = JsonConvert.SerializeObject(objra);
                    requestApi = new StringContent(Api, Encoding.UTF8, "application/json");
                    rawResponse = await httpclient.PostAsync(bankStatementUrl, requestApi);
                    var r11 = await rawResponse.Content.ReadAsStringAsync();
                    statusCode = (int)rawResponse.StatusCode;
                    if (statusCode != 200)
                    {
                        r.status = false;
                        r.message = "Error From The Document API";
                    }

                }
                else
                {
                    r.status = true;
                    r.message = "Loan Restructured Successfully";
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
    }
}
