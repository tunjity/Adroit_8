
using Adroit_v8.Models.FormModel;
using Adroit_v8.MongoConnections.LoanApplication;
using Adroit_v8.Service;
using Microsoft.AspNetCore.Authorization;
using Adroit_v8.MongoConnections.CustomerCentric;
using static Adroit_v8.EnumFile.EnumHelper;
using static Adroit_v8.Config.Helper;
using Adroit_v8.Model;
using System.Text.RegularExpressions;
using MongoDB.Driver;
using Nancy.Json;
using Newtonsoft.Json;
using Adroit_v8.Models.Administration;
using Adroit_v8.Config;

namespace Adroit_v8.Controllers.LoanUnderwriting
{
    [Route("api/LoanUnderwriting/[controller]")]
    [ApiController]
    [Authorize]
    public class ReviewController : AuthController
    {
        private readonly AdroitDbContext _context;
        private readonly IAdroitRepository<RegularLoan> _repo;
        private readonly IAdroitRepository<MobileAppCustomerAddressCollection> _repoCAC;
        private readonly IAdroitRepository<RegularLoanRepaymentPlan> _repoRegularLoanRepaymentPlan;
        private readonly IAdroitRepository<RegularLoanStepSix> _repoDoc;
        private readonly IAdroitRepository<RegularLoanComment> _repoComment;
        private readonly IAdroitRepository<RegularLoanReasonToDecline> _repoRTD;
        private readonly IAdroitRepository<RegularLoanAdjustment> _repoRLA;
        private readonly ICustomerCentricRepository<RegularLoanRestructure> _repoRS;
        private readonly ICustomerCentricRepository<LoanTopUp> _repoTP;
        private readonly IMongoCollection<RegularLoan> _customerLoan;
        private readonly IFilterRepository _repoF;
        private readonly IConfiguration _config;
        string errMsg = "Unable to process request, kindly try again";
        public ReviewController(IAdroitRepository<RegularLoan> repo, IAdroitRepository<RegularLoanRepaymentPlan> repoRegularLoanRepaymentPlan, AdroitDbContext context, ICustomerCentricRepository<RegularLoanRestructure> repoRS, ICustomerCentricRepository<LoanTopUp> repoTP, IConfiguration config, IFilterRepository repoF,
            IAdroitRepository<RegularLoanAdjustment> repoRLA, IAdroitRepository<MobileAppCustomerAddressCollection> repoCAC, IAdroitRepository<RegularLoanReasonToDecline> repoRTD,
            IAdroitRepository<RegularLoanComment> repoComment, IAdroitRepository<RegularLoanStepSix> repoDoc, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _repo = repo;
            _repoRegularLoanRepaymentPlan = repoRegularLoanRepaymentPlan;
            _context = context;
            _repoRS = repoRS;
            _repoTP = repoTP;
            _config = config;
            _repoF = repoF;
            _repoRLA = repoRLA;
            _repoCAC = repoCAC;
            _repoRTD = repoRTD;
            _repoComment = repoComment;
            string? connectionURI = _config.GetSection("MongoDB").GetSection("ConnectionURI").Value;
            string? databaseName = _config.GetSection("MongoDB").GetSection("MobileDatabaseName").Value;
            MongoClient client = new MongoClient(connectionURI);
            IMongoDatabase database = client.GetDatabase(databaseName);
            _customerLoan = database.GetCollection<RegularLoan>("RegularLoan");
            _repoDoc = repoDoc;
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("offerLetterDetail")]
        public async Task<IActionResult> GetOfferLetterDetail([FromQuery] OfferLetterModel obj)
        {
            var r = new ReturnObject();
            try
            {
                var res = _repo.AsQueryable().FirstOrDefault(o => o.LoanApplicationId == obj.LoanApplicationId);
                if (res == null)
                {
                    r.status = false;
                    r.message = "No record found";
                    return StatusCode(StatusCodes.Status404NotFound, r);
                }
                var allSav = _context.Customers.FirstOrDefault(o => o.Id == res.CustomerId);
                if (allSav == null)
                {
                    r.status = false;
                    r.message = "No customer record found";
                    return StatusCode(StatusCodes.Status404NotFound, r);
                }
                var ad = _repoCAC.AsQueryable().FirstOrDefault(o => o.CustomerId == allSav.Id);
                var lga =( from l in _context.Lgas
                          where l.Id == Convert.ToInt32(ad.LGA)
                          join s in _context.States
                          on l.Stateid equals Convert.ToString(s.Id)
                          join c in _context.Nationalities
                          on s.Countryid equals Convert.ToString(c.Id)
                          select new
                          { state = s.Name, lg = l.Name, coun = c.Name}).FirstOrDefault();
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

                var fneRes = new
                {
                    date = res.DateCreated,
                    loanId = res.LoanApplicationId,
                    customerName = allSav.FirstName + " " + allSav.MiddleName + " " + allSav.LastName,
                    customerAddress = $"{ad.HouseNumber}, {ad.StreetName} of {lga.lg},{lga.state} ,{lga.coun}",
                    loanAmount = res.LoanAmount,
                    loanTenor = res.LoanDuration,
                    firstrepaymentDate = DateTime.Now.AddMonths(1),
                    finalrepaymentDate = DateTime.Now.AddMonths(1 + res.LoanDuration),
                    interestRate = res.InterestRate,
                    creditwaveaza = "i no know yet ooo",
                    repayDet = lstRes
                };

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
                                qy = _repoTP.AsQueryable().Where(o => o.Status == (int)AdroitLoanApplicationStatus.Review
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
                                qy = _repoTP.AsQueryable().Where(o => o.Status == (int)AdroitLoanApplicationStatus.Review).ToList();
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
                                query = _repoRS.AsQueryable().Where(o => o.Status == (int)AdroitLoanApplicationStatus.Review && (o.DateCreated > obj.StartDate && o.DateCreated < obj.EndDate.AddDays(1))).ToList();
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
                                query = _repoRS.AsQueryable().Where(o => o.Status == (int)AdroitLoanApplicationStatus.Review).ToList();
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
            var r = new ReturnObject();
            bool eget = false;
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
                        FilterDet = "review"
                    }),
                    2 => _repo.AsQueryable().Where(o => o.Status == (int)AdroitLoanApplicationStatus.Review),
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
                var res = _repo.AsQueryable().FirstOrDefault(o => o.CustomerId == cusId);
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
       
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getbyLoanId/{loanId}")]
        public async Task<IActionResult> GetByLoanId([FromRoute] string loanId)
        {
            var r = new ReturnObject();
            try
            {
                var res = _repo.AsQueryable().FirstOrDefault(o => o.LoanApplicationId == loanId);
                if (res != null)
                {
                    var resBs = _repoDoc.AsQueryable().FirstOrDefault(o => o.LoanApplicationId == res.LoanApplicationId);
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
                            res.Status = (int)AdroitLoanApplicationStatus.Approved;
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
                            resII.Status = (int)AdroitLoanApplicationStatus.Approved;
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
                            resIII.Status = (int)AdroitLoanApplicationStatus.Approved;
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
                //var res = _repo.AsQueryable().FirstOrDefault(o => o.ApplicantNumber == obj.LoanApplicationId);
                //if (res != null)
                //{
                //    res.Status = (int)AdroitLoanApplicationStatus.Approved;
                //    _repo.ReplaceOne(res);
                //    r.data = res;
                //}
                //r.status = res != null ? true : false;
                //r.message = res != null ? "Record Found Successfully" : "Not Found";
                //return Ok(r);
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
        [Route("Adjust")]
        public async Task<IActionResult> Adjust([FromBody] RegularLoanCommentFormModelAdjust obj)
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
                            res.Status = (int)AdroitLoanApplicationStatus.Adjust;
                            _repo.ReplaceOne(res);
                            r.data = res;
                        }
                        break;
                    case "loantopup":
                        var resII = _repoTP.AsQueryable().FirstOrDefault(o => o.ApplicantNumber == obj.LoanApplicationId);
                        if (resII != null)
                        {
                            resII.Status = (int)AdroitLoanApplicationStatus.Adjust;
                            _repoTP.ReplaceOne(resII);
                            r.data = resII;
                        }
                        break;
                    case "loanrestructure":
                        var resIII = _repoRS.AsQueryable().FirstOrDefault(o => o.LoanApplicationId == obj.LoanApplicationId);
                        if (resIII != null)
                        {
                            resIII.Status = (int)AdroitLoanApplicationStatus.Adjust;
                            _repoRS.ReplaceOne(resIII);
                            r.data = resIII;
                        }
                        break;
                    default:
                        break;
                }
                var com = new RegularLoanAdjustment
                {
                    LoanApplicationId = obj.LoanApplicationId,
                    Description = obj.Description,
                    AdjustedAmount = obj.AdjustedAmount,
                    LoanCategory = obj.LoanCategory,
                    AdjustedTenor = obj.AdjustedTenor
                };
                var repo = await _repoRLA.InsertOneAsync(com);

                return Ok(repo);
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
        [Route("getAdjustbyCusId/{cusId}")]
        public async Task<IActionResult> GetAdjustByCustomerId([FromRoute] int cusId)
        {
            var r = new ReturnObject();
            try
            {
                var res = _repo.AsQueryable().FirstOrDefault(o => o.CustomerId == cusId);
                if (res != null)
                {
                    var resBs = _repoRLA.AsQueryable().Where(o => o.LoanApplicationId == res.ApplicantNumber).OrderByDescending(o => o.DateCreated);
                    r.data = resBs;
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