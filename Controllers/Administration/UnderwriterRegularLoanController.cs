
using Adroit_v8.MongoConnections;
using Adroit_v8.MongoConnections.UnderWriterModel;
using Microsoft.AspNetCore.Authorization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
namespace Adroit_v8.Controllers.Administration
{
    [Route("api/Administration/[controller]")]
    [ApiController]
    [Authorize]
    public class UnderRegularLoanController : AuthController
    {

        private readonly IMongoRepository<AdministrationRegularLoanCharge> _repoRegularLoanCharge;
        private IMongoRepository<AdministrationRegularLoanInterestRate> _repoRegularLoanInterestRate;
        public UnderRegularLoanController(IMongoRepository<AdministrationRegularLoanCharge> repoRegularLoanCharge,
            IMongoRepository<AdministrationRegularLoanInterestRate> repoRegularLoanInterestRate, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _repoRegularLoanCharge = repoRegularLoanCharge;
            _repoRegularLoanInterestRate = repoRegularLoanInterestRate;
            //  _context = context;
        }
        #region RegularLoanInterestRate
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("addRegularLoanInterestRate")]
        public Task<IActionResult> AddRegularLoanInterestRate([FromBody] AdminRegularLoanInterestRatePost obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Saved Successfully";
            try
            {
                AdministrationRegularLoanInterestRate RegularLoanTenor = new()
                {
                    UniqueId = Guid.NewGuid().ToString(),
                    EmploymentTypeId = obj.EmploymentTypeId,
                    InterestRate = obj.InterestRate,
                    LoanAmountFrom = obj.LoanAmountFrom,
                    LoanAmountTo = obj.LoanAmountTo
                };
                _repoRegularLoanInterestRate.InsertOne(RegularLoanTenor);

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
        [Route("getallRegularLoanInterestRate")]
        public Task<IActionResult> GetAllRegularLoanInterestRate([FromQuery] PaginationWithOutFilterModel obj)
        {
            var r = new ReturnObject();bool eget = false;
            try
            {
                var query = _repoRegularLoanInterestRate.AsQueryable();
                var fneRes = query.Skip((obj.PageNumber - 1) * obj.PasgeSize)
                      .Take(obj.PasgeSize);
                if (query.Any())
                    eget = true;
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = fneRes.ToList();
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
        [Route("getRegularLoanInterestRatebyuniqueid/id")]
        public Task<IActionResult> GetRegularLoanInterestRate(string id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoRegularLoanInterestRate.FindById(id);
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
        [HttpDelete]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("deleteRegularLoanInterestRatebyuniqueid/id")]
        public Task<IActionResult> DeleteRegularLoanInterestRate(string id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Deleted Successfully";
            try
            {
                _repoRegularLoanInterestRate.DeleteById(id);
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
        [Route("updateRegularLoanInterestRate")]
        public Task<IActionResult> UpdateRegularLoanInterestRate([FromBody] AdminRegularLoanInterestRatePut obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record updated Successfully";
            try
            {
                var retChecker = _repoRegularLoanInterestRate.FindById(obj.UniqueId);
                if (retChecker is null)
                {
                    r.status = false;
                    r.message = "Record Not Found";
                }
                else
                {
                    retChecker.EmploymentTypeId = obj.EmploymentTypeId;
                    retChecker.UniqueId = obj.UniqueId;
                    retChecker.InterestRate = obj.InterestRate;
                    retChecker.LoanAmountFrom = obj.LoanAmountFrom;
                    retChecker.LoanAmountTo = obj.LoanAmountTo;
                    _repoRegularLoanInterestRate.ReplaceOne(retChecker);
                }

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
        #endregion

        #region RegularLoanCharge
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("addRegularLoanCharge")]
        public Task<IActionResult> AddRegularLoanCharge([FromBody] AdminRegularLoanChargePost obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Saved Successfully";
            try
            {
                AdministrationRegularLoanCharge RegularLoanTenor = new()
                {
                    UniqueId = Guid.NewGuid().ToString(),
                    EmploymentTypeId = obj.EmploymentTypeId,
                    IsPercentage = obj.IsPercentage,
                    ChargeAmount = obj.ChargeAmount,
                    LoanAmountFrom = obj.LoanAmountFrom,
                    LoanAmountTo = obj.LoanAmountTo,
                    LoanTenorid = obj.LoanTenorid,
                    //Status = obj.Status
                };
                _repoRegularLoanCharge.InsertOneAsync(RegularLoanTenor);

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
        [Route("getallRegularLoanCharge")]
        public Task<IActionResult> GetAllRegularLoanCharge([FromQuery] PaginationWithOutFilterModel obj)
        {
            var r = new ReturnObject();bool eget = false;
            try
            {
                var query= _repoRegularLoanCharge.AsQueryable();  var fneRes = query.Skip((obj.PageNumber - 1) * obj.PasgeSize)
                      .Take(obj.PasgeSize);
                if (query.Any())
                    eget = true;
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = fneRes.ToList();
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
        [Route("getRegularLoanChargebyuniqueid/id")]
        public Task<IActionResult> GetRegularLoanCharge(string id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoRegularLoanCharge.FindById(id);
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
        [Route("GetLoanCharge")]
        public Task<IActionResult> GetLoanCharge([FromBody] GetAdminRegularLoanCharge obj)
        {
            var res = new AdministrationRegularLoanCharge();
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                var resList = _repoRegularLoanCharge.AsQueryable()
                     .Where(o => o.LoanTenorid == obj.LoanTenorid
                              && o.EmploymentTypeId == obj.EmploymentTypeId
                              && o.LoanAmountFrom <= obj.LoanAmount
                              && o.LoanAmountTo >= obj.LoanAmount
                           );
                if (resList.Count() > 0)
                    res = resList.FirstOrDefault();
                r.data = res != null ? res.ChargeAmount : "100";
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
        [Route("GetLoanInterestRate")]
        public Task<IActionResult> GetLoanInterestRate([FromBody] GetAdminRegularLoanCharge obj)
        {
            var res = new AdministrationRegularLoanInterestRate();
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                var resList = _repoRegularLoanInterestRate.AsQueryable()
                     .Where(o => o.EmploymentTypeId == obj.EmploymentTypeId
                              && o.LoanAmountFrom <= obj.LoanAmount
                              && o.LoanAmountTo >= obj.LoanAmount
                           );
                if (resList.Count() > 0)
                    res = resList.FirstOrDefault();
                r.data = res != null ? res.InterestRate : "3.5%";
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
        [HttpDelete]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("deleteRegularLoanChargebyuniqueid/id")]
        public Task<IActionResult> DeleteRegularLoanCharge(string id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Deleted Successfully";
            try
            {
                _repoRegularLoanCharge.DeleteById(id);
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
        [Route("updateRegularLoanCharge")]
        public Task<IActionResult> UpdateRegularLoanCharge([FromBody] AdminRegularLoanChargePut obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record updated Successfully";
            try
            {
                var retChecker = _repoRegularLoanCharge.FindById(obj.UniqueId);
                if (retChecker is null)
                {
                    r.status = false;
                    r.message = "Record Not Found";
                }
                else
                {
                    retChecker.EmploymentTypeId = obj.EmploymentTypeId;
                    retChecker.ChargeAmount = obj.ChargeAmount;
                    retChecker.IsPercentage = obj.IsPercentage;
                    retChecker.LoanAmountFrom = obj.LoanAmountFrom;
                    retChecker.LoanAmountTo = obj.LoanAmountTo;
                    retChecker.LoanTenorid = obj.LoanTenorid;
                    retChecker.UniqueId = obj.UniqueId;
                    _repoRegularLoanCharge.ReplaceOne(retChecker);
                }

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
        #endregion
    }
}
