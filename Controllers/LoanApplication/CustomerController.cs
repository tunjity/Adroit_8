
using Adroit_v8.Config;
using Adroit_v8.Models.FormModel;
using Adroit_v8.MongoConnections;
using Adroit_v8.MongoConnections.LoanApplication;
using Adroit_v8.Service;
using Microsoft.VisualBasic;
using MongoDB.Driver;
using Nancy.Json;
using Newtonsoft.Json;
using System;
using Dapper;
using static Adroit_v8.EnumFile.EnumHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Adroit_v8.MongoConnections.CustomerCentric;
using SharpCompress.Common;
using System.Text.RegularExpressions;

namespace Adroit_v8.Controllers.LoanApplication
{
    [Route("api/LoanApplication/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerController : AuthController
    {
        //  private readonly IGenericRepository<CustomerInformatinoDto> _repoCustomer;
        private readonly IAdroitRepository<RegularLoan> _repo;
        private readonly IFilterRepository _repoF;
        private readonly IAdroitRepository<LoanTopUpStepOne> _repoTopUp;
        private readonly IAdroitRepository<RegularLoanRepaymentPlan> _repoRegularLoanRepaymentPlan;
        private readonly ICustomerCentricRepository<RegularLoanRestructure> _repoRS;
        private readonly ICustomerCentricRepository<LoanTopUp> _repoTP;
        private readonly IAdroitRepository<RegularLoanStepSix> _repoDoc;
        private readonly IAdroitRepository<RegularLoanReAssignment> _repoReA;
        private readonly IAdroitRepository<RegularLoanComment> _repoComment;
        private readonly IAdroitRepository<RegularLoanSupportingDocumentsOtherForms> _repoSD;
        private readonly IAdroitRepository<RegularLoanReasonToDecline> _repoRTD;
        private readonly IAdroitRepository<RegularLoanSupportingDocumentsGuarantorForm> _repoG;
        private readonly IAdroitRepository<RegularLoanRequestedDocument> _repoRD;
        private readonly IConfiguration _config;
        string errMsg = "Unable to process request, kindly try again";
        public CustomerController(IAdroitRepository<RegularLoan> repo, ICustomerCentricRepository<RegularLoanRestructure> repoRS, ICustomerCentricRepository<LoanTopUp> repoTP, IConfiguration config, IAdroitRepository<LoanTopUpStepOne> repoTopUp, IAdroitRepository<RegularLoanReAssignment> repoReA, IAdroitRepository<RegularLoanSupportingDocumentsGuarantorForm> repoG, IAdroitRepository<RegularLoanRequestedDocument> repoRD,
            IAdroitRepository<RegularLoanReasonToDecline> repoRTD, IAdroitRepository<RegularLoanSupportingDocumentsOtherForms> repoSD,
            IAdroitRepository<RegularLoanComment> repoComment, IAdroitRepository<RegularLoanRepaymentPlan> repoRegularLoanRepaymentPlan, IFilterRepository repoF, IAdroitRepository<RegularLoanStepSix> repoDoc, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _repo = repo;
            _repoRS = repoRS;
            _repoTP = repoTP;
            _config = config;
            _repoTopUp = repoTopUp;
            _repoReA = repoReA;
            _repoG = repoG;
            _repoRD = repoRD;
            _repoRTD = repoRTD;
            _repoSD = repoSD;
            _repoComment = repoComment;
            _repoRegularLoanRepaymentPlan = repoRegularLoanRepaymentPlan;
            _repoF = repoF;
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
                        FilterDet = "customer"
                    }),
                    2 => _repo.AsQueryable().Where(o => o.Status == (int)AdroitLoanApplicationStatus.Under_Review),

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
                RegularLoan? res = _repo.AsQueryable().FirstOrDefault(o => o.CustomerId == cusId && o.Status == (int)AdroitLoanApplicationStatus.Under_Review);
                if (res != null)
                {
                    RegularLoanStepSix? resBs = _repoDoc.AsQueryable().FirstOrDefault(o => o.CustomerId == cusId && o.LoanApplicationId == res.ApplicantNumber);
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
                    aa.AmountRequested = res.LoanAmount.ToString();aa.Interest = res.Interest.ToString();
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
                RegularLoan? res = _repo.AsQueryable().FirstOrDefault(o => o.LoanApplicationId == loanId && o.Status == (int)AdroitLoanApplicationStatus.Under_Review);
                if (res != null)
                {
                    RegularLoanStepSix? resBs = _repoDoc.AsQueryable().FirstOrDefault(o => o.CustomerId == res.CustomerId && o.LoanApplicationId == res.ApplicantNumber);
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
                    aa.AmountRequested = res.LoanAmount.ToString();aa.Interest = res.Interest.ToString();
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
        [Route("getCustomerLoanDecision/{cusId}")]
        public async Task<IActionResult> GetCustomerLoanDecision([FromRoute] int cusId)
        {
            var r = new ReturnObject();
            try
            {
                var res = _repo.AsQueryable().FirstOrDefault(o => o.CustomerId == cusId);
                if (res != null)
                {
                    var resBs = _repoDoc.AsQueryable().FirstOrDefault(o => o.CustomerId == cusId && o.LoanApplicationId == res.ApplicantNumber);
                    //if (resBs.BankStatementOfAccount is not null)
                    //{
                    //    var SavePath = $"{_config["FileFolder:BankStatementPath"]}/{resBs.BankStatementOfAccount}";
                    //    resBs.BankStatementOfAccount = Helper.ConvertFromPathToBase64(SavePath);
                    //}
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
                    aa.AmountRequested = res.LoanAmount.ToString();aa.Interest = res.Interest.ToString();
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
                var res = _repo.AsQueryable().FirstOrDefault(o => o.ApplicantNumber == obj.LoanApplicationId);
                string stringValue = Enum.GetName(typeof(AdroitLoanApplicationStatus), res.Status);
                var com = new RegularLoanComment { LoanApplicationId = obj.LoanApplicationId, CustomerId = res.CustomerId, Status = stringValue, Description = obj.Description };
                var repo = await _repoComment.InsertOneAsync(com);
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
        [Route("getComments/{loanapplicationId}")]
        public async Task<IActionResult> GetAllComment([FromRoute] string loanapplicationId)
        {
            var r = new ReturnObject();
            try
            {
                var res = _repoComment.AsQueryable().Where(o => o.LoanApplicationId == loanapplicationId).OrderByDescending(o => o.DateCreated);
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
        [Route("Decline")]
        public async Task<IActionResult> Decline([FromBody] RegularLoanReasonTodeclineFormModel obj)
        {
            var r = new ReturnObject();
            string actionUrl = $"{this.ControllerContext.RouteData.Values["controller"].ToString()}/{this.ControllerContext.RouteData.Values["action"].ToString()}";
            var response = new ReturnObject();
            var RequestTime = DateTime.Now;
            try
            {
                JavaScriptSerializer js = new();
                var com = new RegularLoanReasonToDecline { LoanApplicationId = obj.LoanApplicationId, Comment = obj.Comment, LoanCategory = obj.LoanCategory };
                com.Reasons = js.Serialize(obj.Reasons);
                var repo = await _repoRTD.InsertOneAsync(com);
                if (repo.status = true)
                {

                    switch (obj.LoanCategory.ToLower())
                    {
                        case "regular loan":
                            var res = _repo.AsQueryable().FirstOrDefault(o => o.ApplicantNumber == obj.LoanApplicationId);
                            if (res != null)
                            {
                                res.Status = (int)AdroitLoanApplicationStatus.Declined;
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
                                resII.Status = (int)AdroitLoanApplicationStatus.Declined;
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
                                resIII.Status = (int)AdroitLoanApplicationStatus.Declined;
                                _repoRS.ReplaceOne(resIII);
                                r.data = resIII;
                            }
                            r.status = resIII != null ? true : false;
                            r.message = resIII != null ? "Record Found Successfully" : "Not Found";
                            return Ok(r);
                        default:
                            break;
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
        [Route("addSupportingDocumentGuarantorForm")]
        public async Task<IActionResult> AddSupportingDocumentGuarantorForm([FromForm] RegularLoanSupportingDocGuarantorFormFormModel obj)
        {
            string actionUrl = $"{this.ControllerContext.RouteData.Values["controller"].ToString()}/{this.ControllerContext.RouteData.Values["action"].ToString()}";
            var response = new ReturnObject();
            var RequestTime = DateTime.Now;
            try
            {
                var com = new RegularLoanSupportingDocumentsGuarantorForm { LoanApplicationId = obj.LoanApplicationId, UniqueId = Guid.NewGuid().ToString() };

                var SavePath = $"{_config["FileFolder:Path"]}{"DocumentGuarantorForm"}";
                string fileName = $"{com.UniqueId}{"_"}{Path.GetFileName(obj.GuarantorForm.FileName)}";
                _ = Task.Run(() => { Helper.ProcessFileUpload(obj.GuarantorForm, com.UniqueId, fileName, SavePath); });
                com.GuarantorForm = Path.Combine(SavePath, fileName);
                var repo = await _repoG.InsertOneAsync(com);
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
        [Route("addSupportingDocumentOtherForms")]
        public async Task<IActionResult> AddSupportingDocumentOtherForms([FromForm] RegularLoanSupportingDocFormModel obj)
        {
            string actionUrl = $"{this.ControllerContext.RouteData.Values["controller"].ToString()}/{this.ControllerContext.RouteData.Values["action"].ToString()}";
            var response = new ReturnObject();
            var RequestTime = DateTime.Now;
            try
            {
                var stOtherDoc = Helper.ConvertIFormFilesToZip(obj.OtherForms);
                var com = new RegularLoanSupportingDocumentsOtherForms { LoanApplicationId = obj.LoanApplicationId, UniqueId = Guid.NewGuid().ToString(), OtherForms = stOtherDoc };
                var repo = await _repoSD.InsertOneAsync(com);
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
        [Route("addRequestedDocument")]
        public async Task<IActionResult> AddRequestDocument([FromBody] RegularLoanRequestedSupportingDocFormModel obj)
        {
            string actionUrl = $"{this.ControllerContext.RouteData.Values["controller"].ToString()}/{this.ControllerContext.RouteData.Values["action"].ToString()}";
            var response = new ReturnObject();
            var RequestTime = DateTime.Now;
            try
            {
                var com = new RegularLoanRequestedDocument { LoanApplicationId = obj.LoanApplicationId, UniqueId = Guid.NewGuid().ToString(), DocName = obj.DocName, CustomerId = obj.CustomerId };
                var repo = await _repoRD.InsertOneAsync(com);
                return StatusCode(StatusCodes.Status200OK, repo);
            }
            catch (Exception ex)
            {
                LogService.LoggerCreateAsync(JsonConvert.SerializeObject(obj), actionUrl, DateTime.Now, ex.ToString(), "", (int)ServiceLogLevel.Exception);
                return StatusCode(StatusCodes.Status500InternalServerError, new ReturnObject { status = false, message = "Error occured while processing request, please try again." });
            }
        }

        [HttpPut]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("Update")]
        public async Task<IActionResult> Update([FromBody] UpdateWithBankStatement obj)
        {
            var r = new ReturnObject();
            //
            try
            {
                switch (Regex.Replace(obj.LoanCategory, @"\s", "").ToLower())
                {
                    case "regularloan":
                        var res = _repo.AsQueryable().FirstOrDefault(o => o.ApplicantNumber == obj.LoanApplicationId);
                        if (res != null)
                        {
                            res.Status = (int)AdroitLoanApplicationStatus.Review;
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
                            resII.Comment = obj.Comment;
                            resII.Status = (int)AdroitLoanApplicationStatus.Review;
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
                            resIII.Comment = obj.Comment;
                            resIII.Status = (int)AdroitLoanApplicationStatus.Review;
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
                    lstRes.Add(new RegularLoanRepaymentReturnModel
                    {
                        repaymentDate = o.MonthlyRepaymentDate,
                        principal = o.PrincipalAmount.ToString(),
                        Interest = o.Interest.ToString(),
                        TotalPayment = Math.Round(o.PrincipalAmount + o.Interest).ToString()
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
       
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("Reassignment")]
        public async Task<IActionResult> ReAssignment([FromBody] Reassignment obj)
        {
            try
            {
                var res = new RegularLoanReAssignment
                {
                    AssigneeUserId = obj.AssigneeUserId,
                    AssigneruserId = obj.AssigneruserId,
                    LoanApplicationId = obj.LoanApplicationId,
                    LoanReAssignStatus = LoanApplicationReassignmnetStatus.Pending.ToString()

                };
                var repo = await _repoReA.InsertOneAsync(res);
                return StatusCode(StatusCodes.Status200OK, repo);
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
        [Route("getReassignmentByUserId/{UserId}")]
        public async Task<IActionResult> GetReassignment([FromRoute] string userId)
        {
            var r = new ReturnObject();
            try
            {
                var res = _repoReA.AsQueryable().Where(o => o.AssigneeUserId == userId).OrderByDescending(o => o.DateCreated);
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
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getCustomerDecisionByCusIdByApplicannumber")]
        public async Task<IActionResult> getCustomerDecision([FromBody] RegularLoanCommentFormModelDecision obj)
        {
            var r = new ReturnObject();
            try
            {
                var res = await CustomerCheckByCustomerId(obj.cusId);
                if (res != null)
                {
                    var resII = _repo.AsQueryable().Where(o => o.ApplicantNumber == obj.LoanApplicationId).OrderByDescending(o => o.DateCreated).ToList();
                    r.data = new
                    {
                        fullName = $"{res.FirstName} {res.LastName}",
                        email = res.EmailAddress,
                        phoneNumber = res.PhoneNumber,
                        dob = res.DateOfBirth,
                        tenor = resII.FirstOrDefault()?.LoanDuration,
                        loanAmount = resII.FirstOrDefault()?.LoanAmount
                    };
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

        [NonAction]
        public async Task<CustomerInformatinoDto> CustomerCheckByCustomerId(long CustomerId)
        {
            try
            {
                using IDbConnection cn = new DapperConfig(_config).PostgreDbConnection;
                return (await cn.QueryFirstOrDefaultAsync<CustomerInformatinoDto>(CustomerRegistrationQuery.GetCustomerByCustomerId, new
                {
                    CustomerId
                }));
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
