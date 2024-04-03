using Adroit_v8.Model;
using Adroit_v8.Models.FormModel;
using Adroit_v8.MongoConnections.CustomerCentric;
using static Adroit_v8.EnumFile.EnumHelper;
using static Adroit_v8.Config.Helper;
using Microsoft.AspNetCore.Authorization;

namespace Adroit_v8.Controllers.LoanApplication
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LoanTopUpController : ControllerBase
    {
        private readonly AdroitDbContext _context;
        private readonly ICustomerCentricRepository<LoanTopUp> _repo;
        private readonly ICustomerCentricRepository<RegularLoanTopUpRepaymentPlan> _repoD;
        public LoanTopUpController(AdroitDbContext context, ICustomerCentricRepository<RegularLoanTopUpRepaymentPlan> repoD, ICustomerCentricRepository<LoanTopUp> repo)
        {
            _context = context;
            _repoD = repoD;
            _repo = repo;
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("get")]
        public async Task<IActionResult> GetAll([FromQuery] FilFormModel obj)
        {
            var r = new ReturnObject(); bool eget = false;

            List<LoanTopUpResponse> neRes = new();
            List<LoanTopUp> lstOfCusId = new();
            List<Customer> allSav = null;
            List<Model.Gender> genders = null;
            try
            {

                lstOfCusId = _repo.AsQueryable().Where(o => o.Status == (int)AdroitLoanApplicationStatus.Under_Review).ToList();
                genders = _context.Genders.ToList();
                allSav = _context.Customers.ToList();
                switch (obj.Det)
                {
                    case 1:

                        lstOfCusId = lstOfCusId.Where(o => o.DateCreated > obj.StartDate
                                && o.DateCreated < obj.EndDate.AddDays(1)).ToList();
                        genders = _context.Genders.ToList();
                        allSav = allSav.Where(o => lstOfCusId.Select(o => o.CustomerId).Contains(o.Id)).ToList();

                        if (obj.Status != 0)
                            allSav = allSav.Where(o => o.Status.GetValueOrDefault() == obj.Status).ToList();
                        if (!string.IsNullOrEmpty(obj.CustomerReference))
                            allSav = allSav.Where(o => o.CustomerRef == obj.CustomerReference).ToList();
                        if (!string.IsNullOrEmpty(obj.Bvn))
                            allSav = allSav.Where(o => o.Bvn == obj.Bvn).ToList();
                        if (!string.IsNullOrEmpty(obj.EmailAddress))
                            allSav = allSav.Where(o => o.EmailAddress == obj.EmailAddress).ToList();

                        neRes = GetList(lstOfCusId, allSav, genders);
                        break;
                    case 2:
                        allSav = _context.Customers.Where(o => lstOfCusId.Select(o => o.CustomerId).Contains(o.Id)).ToList();
                        neRes = GetList(lstOfCusId, allSav, genders);
                        break;
                    default:
                        break;
                }


                var fneRes = neRes.Skip((obj.PageNumber - 1) * obj.PasgeSize)
                     .Take(obj.PasgeSize);
                if (neRes.Any())
                    eget = true;
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = fneRes.ToList();
                r.recordCount = neRes.Count();
                return (Ok(r));
            }
            catch (Exception ex)
            {
                return (StatusCode(StatusCodes.Status500InternalServerError, new ReturnObject
                {
                    status = false,
                    message = ex.Message
                }));
            }
        }
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("Update")]
        public async Task<IActionResult> Update([FromBody] CommentFormModelToGet obj)
        {
            var r = new ReturnObject();
            try
            {
                var res = _repo.AsQueryable().FirstOrDefault(o => o.LoanApplicationId == obj.LoanApplicationId);
                if (res != null)
                {
                    res.Status = obj.Status;
                    res.Comment = obj.Comment;
                    _repo.ReplaceOne(res);
                    r.data = res;
                }

                r.status = res != null ? true : false;
                r.message = res != null ? "Record Updated Successfully" : "Not Found";
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
        [Route("ViewLoan/{loanId}")]
        public Task<IActionResult> ViewLoan([FromRoute] string loanId)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                var lstret = new List<LoanTopUp>();
                List<Customer> allSav = null;
                var retII = _repo.AsQueryable();
                var ret = retII.FirstOrDefault(o => o.LoanApplicationId.ToLower() == loanId.ToLower().Trim());
                List<Model.Gender> genders = _context.Genders.ToList();
                lstret.Add(ret);
                var rS = _repoD.AsQueryable().Where(o => o.LoanApplicationId.ToLower() == loanId.ToLower().Trim());
                allSav = _context.Customers.Where(o => o.Id == ret.CustomerId).ToList();
                List<LoanTopUpResponse>? res = GetList(lstret, allSav, genders);
                var resII = new
                {
                    CusDetail = res.Count > 0 ? res.FirstOrDefault() : null,
                    repaymentSchedule = rS
                };
                r.data = resII;
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
    }
}