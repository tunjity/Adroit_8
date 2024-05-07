using Adroit_v8.Models.FormModel;
using Adroit_v8.MongoConnections.LoanApplication;
using Microsoft.AspNetCore.Authorization;
using static Adroit_v8.EnumFile.EnumHelper;
using static Adroit_v8.Config.Helper;
using Adroit_v8.MongoConnections.CustomerCentric;
using Adroit_v8.Model;

namespace Adroit_v8.Controllers.LoanApplication
{
    [Route("api/LoanApplication/[controller]")]
    [ApiController]
    [Authorize]
    public class DeclinedController : AuthController
    {
        private readonly AdroitDbContext _context;
        private readonly IAdroitRepository<RegularLoan> _repo;
        private readonly IAdroitRepository<RegularLoanStepSix> _repoDoc;
        private readonly IFilterRepository _repoF;
        private readonly ICustomerCentricRepository<RegularLoanRestructure> _repoRS;
        private readonly ICustomerCentricRepository<LoanTopUp> _repoTP;
        private string errMsg = "Unable to process request, kindly try again";

        public DeclinedController(IAdroitRepository<RegularLoan> repo, ICustomerCentricRepository<LoanTopUp> repoTP, ICustomerCentricRepository<RegularLoanRestructure> repoRS, AdroitDbContext context, IFilterRepository repoF, IAdroitRepository<RegularLoanStepSix> repoDoc, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _repo = repo;
            _repoTP = repoTP;
            _repoRS = repoRS;
            _context = context;
            _repoF = repoF;
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
                        FilterDet = "declined"
                    }),
                    2 => _repo.AsQueryable().Where(o => o.Status == (int)AdroitLoanApplicationStatus.Declined),
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
                var res = _repo.AsQueryable().FirstOrDefault(o => o.LoanApplicationId == loanId);
                if (res != null)
                {
                    var resBs = _repoDoc.AsQueryable().FirstOrDefault(o => o.CustomerId == res.CustomerId && o.LoanApplicationId == res.LoanApplicationId);
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
                                qy = _repoTP.AsQueryable().Where(o => o.Status == (int)AdroitLoanApplicationStatus.Declined
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
                                qy = _repoTP.AsQueryable().Where(o => o.Status == (int)AdroitLoanApplicationStatus.Declined).ToList();
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
                                query = _repoRS.AsQueryable().Where(o => o.Status == (int)AdroitLoanApplicationStatus.Declined && (o.DateCreated > obj.StartDate && o.DateCreated < obj.EndDate.AddDays(1))).ToList();
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
                                query = _repoRS.AsQueryable().Where(o => o.Status == (int)AdroitLoanApplicationStatus.Declined).ToList();
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


    }
}