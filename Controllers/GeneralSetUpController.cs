using Adroit_v8.Models.FormModel;
using Adroit_v8.Service;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static Adroit_v8.Config.Helper;

namespace Adroit_v8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GeneralSetUpController : AuthController
    {
        private readonly IGenericRepository<FixedDepositAmountRange> _repoFixedDepositAmountRange;
        private readonly IGenericRepository<FixedDepositPreliquidationCharges> _repoFixedDepositPreliquidationCharges;
        private readonly IGenericRepository<FixedDepositTenor> _repoFixedDepositTenor;
        private readonly IGenericRepository<LateFeeType> _repoLateFeeType;
        private readonly IGenericRepository<DeclineReason> _repoDeclineReason;
        private readonly IGenericRepository<FeeFrequency> _repoFeeFrequency;
        private readonly IGenericRepository<FixedDepositStatus> _repoFixedDepositStatus;
        private readonly IGenericRepository<LateFeePrincipal> _repoLateFeePrincipal;
        private readonly IGenericRepository<RegularLoanCharge> _repoRegularLoanCharge;
        private readonly IGenericRepository<RegularLoanInterestRate> _repoRegularLoanInterestRate;
        private readonly IGenericRepository<FixedDepositInterestRate> _repoFixedDepositInterestRate;
        private readonly IMapper _mapper;
        private readonly CreditWaveContext _context;
        private IGenericRepository<Bank> _repoBank;
        private IGenericRepository<RegularLoanTenor> _repoRegularLoanTenor;
        private IGenericRepository<GovernmentIDCardType> _repoGovernmentIDCardType;
        private IGenericRepository<Applicationchannel> _repoApplicationchannel;
        private IGenericRepository<Educationallevel> _repoEducationallevel;
        private IGenericRepository<Employmentstatus> _repoEmploymentstatus;
        private IGenericRepository<EmploymentSector> _repoEmploymentSector;
        private IGenericRepository<Employmenttype> _repoEmploymenttype;
        private IGenericRepository<Gender> _repoGender;
        private IGenericRepository<Lga> _repoLga;
        private IGenericRepository<Maritalstatus> _repoMaritalstatus;
        private IGenericRepository<Nationality> _repoNationality;
        private IGenericRepository<Noofdependant> _repoNoofdependant;
        private IGenericRepository<Noofyearofresidence> _repoNoofyearofresidence;
        private IGenericRepository<Organization> _repoOrganization;
        private IGenericRepository<Residentialstatus> _repoResidentialstatus;
        private IGenericRepository<Salarypaymentdate> _repoSalarypaymentdate;
        private IGenericRepository<Salaryrange> _repoSalaryrange;
        private IGenericRepository<State> _repoState;
        private IGenericRepository<Title> _repoTitle;
        private IGenericRepository<UtilityBillType> _repoUtilityBillType;
        private string errMsg = "Unable to process request, kindly try again";
        string clientid = "";
        private const string bankListCacheKey = "bankList";
        private IMemoryCache _cache;
        public GeneralSetUpController(
            IGenericRepository<FixedDepositPreliquidationCharges> repoFixedDepositPreliquidationCharges,
        IGenericRepository<FixedDepositTenor> repoFixedDepositTenor,
       IGenericRepository<FixedDepositStatus> repoFixedDepositStatus,
       IGenericRepository<LateFeePrincipal> repoLateFeePrincipal,
       IGenericRepository<LateFeeType> repoLateFeeType,
       IGenericRepository<FeeFrequency> repoFeeFrequency,
       IGenericRepository<RegularLoanCharge> repoRegularLoanCharge,
       IGenericRepository<RegularLoanInterestRate> repoRegularLoanInterestRate,
       IGenericRepository<FixedDepositInterestRate> repoFixedDepositInterestRate,
        IGenericRepository<Bank> repoBank, IGenericRepository<GovernmentIDCardType> repoGovernmentIDCardType, IMemoryCache cache, IGenericRepository<RegularLoanTenor> repoRegularLoanTenor, IGenericRepository<EmploymentSector> repoEmploymentSector, IGenericRepository<Applicationchannel> repoApplicationchannel, IGenericRepository<Educationallevel> repoEducationallevel,
            IGenericRepository<Employmentstatus> repoEmploymentstatus, IGenericRepository<Employmenttype> repoEmploymenttype, IGenericRepository<Gender> repoGender, IGenericRepository<Lga> repoLga,
           IMapper mapper, IGenericRepository<DeclineReason> repoDeclineReason, IGenericRepository<Maritalstatus> repoMaritalstatus, IGenericRepository<Nationality> repoNationality, IGenericRepository<Noofdependant> repoNoofdependant,
            IGenericRepository<Noofyearofresidence> repoNoofyearofresidence, IGenericRepository<Organization> repoOrganization, IGenericRepository<Residentialstatus> repoResidentialstatus,
            IGenericRepository<Salarypaymentdate> repoSalarypaymentdate, IGenericRepository<Salaryrange> repoSalaryrange, IGenericRepository<State> repoState, IGenericRepository<Title> repoTitle,
            IGenericRepository<UtilityBillType> repoUtilityBillType, CreditWaveContext context, IGenericRepository<FixedDepositAmountRange> repoFixedDepositAmountRange
            , IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _mapper = mapper;
            _repoDeclineReason = repoDeclineReason;
            _repoFixedDepositPreliquidationCharges = repoFixedDepositPreliquidationCharges;
            _repoFixedDepositTenor = repoFixedDepositTenor;
            _repoFixedDepositStatus = repoFixedDepositStatus;
            _repoRegularLoanCharge = repoRegularLoanCharge;
            _repoRegularLoanInterestRate = repoRegularLoanInterestRate;
            _repoFixedDepositInterestRate = repoFixedDepositInterestRate;
            _repoBank = repoBank;
            _repoLateFeeType = repoLateFeeType;
            _repoFeeFrequency = repoFeeFrequency;
            _repoLateFeePrincipal = repoLateFeePrincipal;
            _repoGovernmentIDCardType = repoGovernmentIDCardType;
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _repoRegularLoanTenor = repoRegularLoanTenor;
            _repoEmploymentSector = repoEmploymentSector;
            _repoApplicationchannel = repoApplicationchannel;
            _repoEducationallevel = repoEducationallevel;
            _repoEmploymentstatus = repoEmploymentstatus;
            _repoEmploymenttype = repoEmploymenttype;
            _repoGender = repoGender;
            _repoLga = repoLga;
            _repoMaritalstatus = repoMaritalstatus;
            _repoNationality = repoNationality;
            _repoNoofdependant = repoNoofdependant;
            _repoNoofyearofresidence = repoNoofyearofresidence;
            _repoOrganization = repoOrganization;
            _repoResidentialstatus = repoResidentialstatus;
            _repoSalarypaymentdate = repoSalarypaymentdate;
            _repoSalaryrange = repoSalaryrange;
            _repoState = repoState;
            _repoTitle = repoTitle;
            _repoUtilityBillType = repoUtilityBillType;
            _context = context;
            _repoFixedDepositAmountRange = repoFixedDepositAmountRange;
            clientid = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ClientId").Value;

        }

        #region LateFeePrincipal
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("addLateFeePrincipal")]
        public Task<IActionResult> AddLateFeePrincipal([FromBody] UtilityFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Saved Successfully";
            try
            {
                var ret = _context.LateFeePrincipals.FirstOrDefault(
                    o => o.Name.ToLower() == obj.Name.ToLower()
                    && o.clientid == clientid
                    && o.Isdeleted == 0
                    );
                if (ret != null)
                {
                    r.status = false;
                    r.message = "Record Already Exist";
                }
                else
                {
                    LateFeePrincipal GovernmentIDCardType = new LateFeePrincipal { UniqueId = Guid.NewGuid().ToString(), Name = obj.Name, Status = obj.StatusID };
                    _repoLateFeePrincipal.Insert(GovernmentIDCardType);
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
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getallLateFeePrincipals")]
        public Task<IActionResult> GetAllLateFeePrincipals()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoLateFeePrincipal.GetAll();
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
        [Route("getallvalidLateFeePrincipals")]
        public Task<IActionResult> GetAllValidLateFeePrincipals()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoLateFeePrincipal.GetAllIsValid();
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
        [Route("getLateFeePrincipalbyid/id")]
        public Task<IActionResult> GetLateFeePrincipals(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoLateFeePrincipal.Get(id);
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
        [Route("deleteLateFeePrincipalbyid/id")]
        public Task<IActionResult> DeleteLateFeePrincipals(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Deleted Successfully";
            try
            {
                var rec = _repoLateFeePrincipal.Get(id);
                _repoLateFeePrincipal.SoftDelete(rec);
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
        [Route("updateLateFeePrincipal")]
        public Task<IActionResult> UpdateLateFeePrincipal([FromBody] UtilityModificationFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record updated Successfully";
            try
            {
                var retChecker = _repoLateFeePrincipal.Get(obj.Id);
                if (retChecker is null)
                {
                    r.status = false;
                    r.message = "Record Not Found";
                }
                else
                {
                    retChecker.Name = obj.Name;
                    retChecker.Status = obj.StatusID;
                    _repoLateFeePrincipal.Update(retChecker);
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

        #region LateFeeType
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("addLateFeeTypes")]
        public Task<IActionResult> AddLateFeeType([FromBody] UtilityFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Saved Successfully";
            try
            {
                var ret = _context.LateFeeTypes.FirstOrDefault(o => o.Name.ToLower() == obj.Name.ToLower()
                    && o.clientid == clientid && o.Isdeleted == 0);
                if (ret != null)
                {
                    r.status = false;
                    r.message = "Record Already Exist";
                }
                else
                {
                    LateFeeType LateFeeType = new LateFeeType { UniqueId = Guid.NewGuid().ToString(), Name = obj.Name, Status = obj.StatusID };
                    _repoLateFeeType.Insert(LateFeeType);
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
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getallLateFeeTypes")]
        public Task<IActionResult> GetAllLateFeeTypes()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoLateFeeType.GetAll();
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
        [Route("getallvalidLateFeeTypes")]
        public Task<IActionResult> GetAllValidLateFeeTypes()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoLateFeeType.GetAllIsValid();
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
        [Route("getLateFeeTypebyid/id")]
        public Task<IActionResult> GetLateFeeTypes(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoLateFeeType.Get(id);
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
        [Route("deleteLateFeeTypebyid/id")]
        public Task<IActionResult> DeleteLateFeeTypes(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Deleted Successfully";
            try
            {
                var rec = _repoLateFeeType.Get(id);
                _repoLateFeeType.SoftDelete(rec);
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
        [Route("updateLateFeeType")]
        public Task<IActionResult> UpdateLateFeeType([FromBody] UtilityModificationFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record updated Successfully";
            try
            {
                var retChecker = _repoLateFeeType.Get(obj.Id);
                if (retChecker is null)
                {
                    r.status = false;
                    r.message = "Record Not Found";
                }
                else
                {
                    retChecker.Name = obj.Name;
                    retChecker.Status = obj.StatusID;
                    _repoLateFeeType.Update(retChecker);
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
        #region DeclineReason
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("addDeclineReasons")]
        public Task<IActionResult> AddDeclineReason([FromBody] UtilityFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Saved Successfully";
            try
            {
                var ret = _context.DeclineReasons.FirstOrDefault(o => o.Name.ToLower() == obj.Name.ToLower()
                    && o.clientid == clientid && o.Isdeleted == 0);
                if (ret != null)
                {
                    r.status = false;
                    r.message = "Record Already Exist";
                }
                else
                {
                    DeclineReason LateFeeType = new DeclineReason { UniqueId = Guid.NewGuid().ToString(), Name = obj.Name, Status = obj.StatusID };
                    _repoDeclineReason.Insert(LateFeeType);
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
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getallDeclineReason")]
        public Task<IActionResult> GetAllDeclineReasons()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoDeclineReason.GetAll().ToList();
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
        [Route("getallvalidDeclineReasons")]
        public Task<IActionResult> GetAllValidDeclineReasons()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoDeclineReason.GetAllIsValid();
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
        [Route("getDeclineReasonbyid/id")]
        public Task<IActionResult> GetDeclineReasons(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoDeclineReason.Get(id);
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
        [Route("deleteDeclineReasonbyid/id")]
        public Task<IActionResult> DeleteDeclineReasons(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Deleted Successfully";
            try
            {
                var rec = _repoDeclineReason.Get(id);
                _repoDeclineReason.SoftDelete(rec);
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
        [Route("updateDeclineReason")]
        public Task<IActionResult> UpdateDeclineReason([FromBody] UtilityModificationFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record updated Successfully";
            try
            {
                var retChecker = _repoLateFeeType.Get(obj.Id);
                if (retChecker is null)
                {
                    r.status = false;
                    r.message = "Record Not Found";
                }
                else
                {
                    retChecker.Name = obj.Name;
                    retChecker.Status = obj.StatusID;
                    _repoLateFeeType.Update(retChecker);
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
        #region FeeFrequency
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("addFeeFrequencys")]
        public Task<IActionResult> AddFeeFrequency([FromBody] UtilityFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Saved Successfully";
            try
            {
                var ret = _context.FeeFrequencys.FirstOrDefault(o => o.Name.ToLower() == obj.Name.ToLower()
                    && o.clientid == clientid && o.Isdeleted == 0);
                if (ret != null)
                {
                    r.status = false;
                    r.message = "Record Already Exist";
                }
                else
                {
                    FeeFrequency FeeFrequency = new FeeFrequency { UniqueId = Guid.NewGuid().ToString(), Name = obj.Name, Status = obj.StatusID };
                    _repoFeeFrequency.Insert(FeeFrequency);
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
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getallFeeFrequencys")]
        public Task<IActionResult> GetAllFeeFrequencys()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoFeeFrequency.GetAll();
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
        [Route("getallvalidFeeFrequencys")]
        public Task<IActionResult> GetAllValidFeeFrequencys()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoFeeFrequency.GetAllIsValid();
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
        [Route("getFeeFrequencybyid/id")]
        public Task<IActionResult> GetFeeFrequencys(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoFeeFrequency.Get(id);
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
        [Route("deleteFeeFrequencybyid/id")]
        public Task<IActionResult> DeleteFeeFrequencys(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Deleted Successfully";
            try
            {
                var rec = _repoFeeFrequency.Get(id);
                _repoFeeFrequency.SoftDelete(rec);
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
        [Route("updateFeeFrequency")]
        public Task<IActionResult> UpdateFeeFrequency([FromBody] UtilityModificationFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record updated Successfully";
            try
            {
                var retChecker = _repoFeeFrequency.Get(obj.Id);
                if (retChecker is null)
                {
                    r.status = false;
                    r.message = "Record Not Found";
                }
                else
                {
                    retChecker.Name = obj.Name;
                    retChecker.Status = obj.StatusID;
                    _repoFeeFrequency.Update(retChecker);
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
        #region GovernmentIDCardType
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("addGovernmentIDCardTypes")]
        public Task<IActionResult> AddGovernmentIDCardType([FromBody] UtilityFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Saved Successfully";
            try
            {
                var ret = _context.GovernmentIDCardTypes.FirstOrDefault(o => o.Name.ToLower() == obj.Name.ToLower()
                    && o.clientid == clientid
                    && o.Isdeleted == 0);
                if (ret != null)
                {
                    r.status = false;
                    r.message = "Record Already Exist";
                }
                else
                {
                    GovernmentIDCardType GovernmentIDCardType = new GovernmentIDCardType { UniqueId = Guid.NewGuid().ToString(), Name = obj.Name, Status = obj.StatusID };
                    _repoGovernmentIDCardType.Insert(GovernmentIDCardType);
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
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getallGovernmentIDCardTypes")]
        public Task<IActionResult> GetAllGovernmentIDCardTypes()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoGovernmentIDCardType.GetAll();
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
        [Route("getallvalidGovernmentIDCardTypes")]
        public Task<IActionResult> GetAllValidGovernmentIDCardTypes()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoGovernmentIDCardType.GetAllIsValid();
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
        [Route("getGovernmentIDCardTypebyid/id")]
        public Task<IActionResult> GetGovernmentIDCardTypes(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoGovernmentIDCardType.Get(id);
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
        [Route("deleteGovernmentIDCardTypebyid/id")]
        public Task<IActionResult> DeleteGovernmentIDCardTypes(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Deleted Successfully";
            try
            {
                var rec = _repoGovernmentIDCardType.Get(id);
                _repoGovernmentIDCardType.SoftDelete(rec);
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
        [Route("updateGovernmentIDCardType")]
        public Task<IActionResult> UpdateGovernmentIDCardType([FromBody] UtilityModificationFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record updated Successfully";
            try
            {
                var retChecker = _repoGovernmentIDCardType.Get(obj.Id);
                if (retChecker is null)
                {
                    r.status = false;
                    r.message = "Record Not Found";
                }
                else
                {
                    retChecker.Name = obj.Name;
                    retChecker.Status = obj.StatusID;
                    _repoGovernmentIDCardType.Update(retChecker);
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
        #region FixedDepositTenor
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("addFixedDepositTenor")]
        public Task<IActionResult> AddFixedDepositTenor([FromBody] FixedDepositTenorFM obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Saved Successfully";
            try
            {
                var ret = _context.FixedDepositTenors.FirstOrDefault(o => o.Name.ToLower() == obj.Name.ToLower()
                    && o.clientid == clientid && o.Isdeleted == 0);
                if (ret != null)
                {
                    r.status = false;
                    r.message = "Record Already Exist";
                }
                else
                {
                    FixedDepositTenor RegularLoanTenor = new FixedDepositTenor { UniqueId = Guid.NewGuid().ToString(), Code = obj.Code, Name = obj.Name, Status = 1, Description = obj.Description, Days = obj.Days };
                    _repoFixedDepositTenor.Insert(RegularLoanTenor);
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
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getallFixedDepositTenor")]
        public Task<IActionResult> GetAllFixedDepositTenors()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoFixedDepositTenor.GetAll();
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
        [Route("getallvalidFixedDepositTenors")]
        public Task<IActionResult> GetAllValidFixedDepositTenors()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoFixedDepositTenor.GetAllIsValid();
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
        [Route("getFixedDepositTenorbyid/id")]
        public Task<IActionResult> GetFixedDepositTenors(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoFixedDepositTenor.Get(id);
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
        [Route("deleteFixedDepositTenorbyid/id")]
        public Task<IActionResult> DeleteFixedDepositTenors(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Deleted Successfully";
            try
            {
                var rec = _repoFixedDepositTenor.Get(id);
                _repoFixedDepositTenor.SoftDelete(rec);
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
        [Route("updateFixedDepositTenor")]
        public Task<IActionResult> UpdateFixedDepositTenor([FromBody] FixedDepositTenorUpdateFM obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record updated Successfully";
            try
            {
                var retChecker = _repoFixedDepositTenor.Get(obj.Id);
                if (retChecker is null)
                {
                    r.status = false;
                    r.message = "Record Not Found";
                }
                else
                {
                    retChecker.Name = obj.Name;
                    retChecker.Status = obj.Status;
                    retChecker.Description = obj.Description;
                    retChecker.Code = obj.Code;
                    retChecker.Days = obj.Days;
                    _repoFixedDepositTenor.Update(retChecker);
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
        #region FixedDepositPreliquidationCharges
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("addFixedDepositPreliquidationCharges")]
        public Task<IActionResult> AddFixedDepositPreliquidationCharges([FromBody] FixedDepositPreliquidationChargesFm obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Saved Successfully";
            try
            {
                FixedDepositPreliquidationCharges RegularLoanTenor = new FixedDepositPreliquidationCharges
                {
                    UniqueId = Guid.NewGuid().ToString(),
                    FromAmount = obj.FromAmount,
                    IsPercentage = obj.IsPercentage,
                    AmountCharge = obj.AmountCharge,
                    ToAmount = obj.ToAmount,
                    Status = obj.Status
                };
                _repoFixedDepositPreliquidationCharges.Insert(RegularLoanTenor);

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
        [Route("getallFixedDepositPreliquidationCharges")]
        public Task<IActionResult> GetAllFixedDepositAmountRange()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoFixedDepositAmountRange.GetAll();
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
        [Route("getallvalidFixedDepositAmountRange")]
        public Task<IActionResult> GetAllValidFixedDepositAmountRange()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoFixedDepositAmountRange.GetAllIsValid();
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
        [Route("getFixedDepositAmountRangebyid/id")]
        public Task<IActionResult> GetFixedDepositAmountRange(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoFixedDepositAmountRange.Get(id);
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
        [Route("deleteFixedDepositAmountRangebyid/id")]
        public Task<IActionResult> DeleteFixedDepositAmountRange(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Deleted Successfully";
            try
            {
                var rec = _repoFixedDepositAmountRange.Get(id);
                _repoFixedDepositAmountRange.SoftDelete(rec);
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
        [Route("updateFixedDepositAmountRange")]
        public Task<IActionResult> UpdateFixedDepositAmountRange([FromBody] FixedDepositAmountRangeUpdateFm obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record updated Successfully";
            try
            {
                var retChecker = _repoFixedDepositAmountRange.Get(obj.Id);
                if (retChecker is null)
                {
                    r.status = false;
                    r.message = "Record Not Found";
                }
                else
                {
                    retChecker.FromAmount = obj.FromAmount;
                    retChecker.Status = obj.Status;
                    retChecker.ToAmount = obj.ToAmount;
                    _repoFixedDepositAmountRange.Update(retChecker);
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
        #region FixedDepositAmountRange
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("addFixedDepositAmountRange")]
        public Task<IActionResult> AddFixedDepositAmountRange([FromBody] FixedDepositAmountRangeFm obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Saved Successfully";
            try
            {
                FixedDepositAmountRange RegularLoanTenor = new FixedDepositAmountRange
                {
                    UniqueId = Guid.NewGuid().ToString(),
                    FromAmount = obj.FromAmount,
                    ToAmount = obj.ToAmount,
                    Status = obj.Status
                };
                _repoFixedDepositAmountRange.Insert(RegularLoanTenor);

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
        [Route("getallFixedDepositAmountRange")]
        public Task<IActionResult> GetAllFixedDepositPreliquidationCharges()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoFixedDepositPreliquidationCharges.GetAll();
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
        [Route("getallvalidFixedDepositPreliquidationCharges")]
        public Task<IActionResult> GetAllValidFixedDepositPreliquidationCharges()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoFixedDepositPreliquidationCharges.GetAllIsValid();
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
        [Route("getFixedDepositPreliquidationChargesbyid/id")]
        public Task<IActionResult> GetFixedDepositPreliquidationCharges(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoFixedDepositPreliquidationCharges.Get(id);
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
        [Route("deleteFixedDepositPreliquidationChargesbyid/id")]
        public Task<IActionResult> DeleteFixedDepositPreliquidationCharges(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Deleted Successfully";
            try
            {
                var rec = _repoFixedDepositPreliquidationCharges.Get(id);
                _repoFixedDepositPreliquidationCharges.SoftDelete(rec);
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
        [Route("updateFixedDepositPreliquidationCharges")]
        public Task<IActionResult> UpdateFixedDepositPreliquidationCharges([FromBody] FixedDepositPreliquidationChargesUpdateFm obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record updated Successfully";
            try
            {
                var retChecker = _repoFixedDepositPreliquidationCharges.Get(obj.Id);
                if (retChecker is null)
                {
                    r.status = false;
                    r.message = "Record Not Found";
                }
                else
                {
                    retChecker.FromAmount = obj.FromAmount;
                    retChecker.Status = obj.Status;
                    retChecker.ToAmount = obj.ToAmount;
                    retChecker.IsPercentage = obj.IsPercentage;
                    retChecker.AmountCharge = obj.AmountCharge;
                    _repoFixedDepositPreliquidationCharges.Update(retChecker);
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
        #region RegularLoanInterestRate
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("addRegularLoanInterestRate")]
        public Task<IActionResult> AddRegularLoanInterestRate([FromBody] RegularLoanInterestRatePost obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Saved Successfully";
            try
            {
                RegularLoanInterestRate RegularLoanTenor = new RegularLoanInterestRate
                {
                    UniqueId = Guid.NewGuid().ToString(),
                    EmploymentTypeId = obj.EmploymentTypeId,
                    InterestRate = obj.InterestRate,
                    LoanAmountFrom = obj.LoanAmountFrom,
                    LoanAmountTo = obj.LoanAmountTo,
                    Status = obj.Status
                };
                _repoRegularLoanInterestRate.Insert(RegularLoanTenor);

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
        public Task<IActionResult> GetAllRegularLoanInterestRate()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoRegularLoanInterestRate.GetAll();
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
        [Route("getallvalidRegularLoanInterestRate")]
        public Task<IActionResult> GetAllValidRegularLoanInterestRate()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoRegularLoanInterestRate.GetAllIsValid();
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
        [Route("getRegularLoanInterestRatebyid/id")]
        public Task<IActionResult> GetRegularLoanInterestRate(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoRegularLoanInterestRate.Get(id);
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
        [Route("deleteRegularLoanInterestRatebyid/id")]
        public Task<IActionResult> DeleteRegularLoanInterestRate(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Deleted Successfully";
            try
            {
                var rec = _repoRegularLoanInterestRate.Get(id);
                _repoRegularLoanInterestRate.SoftDelete(rec);
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
        public Task<IActionResult> UpdateRegularLoanInterestRate([FromBody] RegularLoanInterestRateUpdate obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record updated Successfully";
            try
            {
                var retChecker = _repoRegularLoanInterestRate.Get(obj.Id);
                if (retChecker is null)
                {
                    r.status = false;
                    r.message = "Record Not Found";
                }
                else
                {
                    retChecker.EmploymentTypeId = obj.EmploymentTypeId;
                    retChecker.Status = obj.Status;
                    retChecker.InterestRate = obj.InterestRate;
                    retChecker.LoanAmountFrom = obj.LoanAmountFrom;
                    retChecker.LoanAmountTo = obj.LoanAmountTo;
                    _repoRegularLoanInterestRate.Update(retChecker);
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
        #region FixedDepositInterestRate
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("addFixedDepositInterestRate")]
        public Task<IActionResult> AddFixedDepositInterestRate([FromBody] FixedDepositInterestRatePost obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Saved Successfully";
            try
            {
                FixedDepositInterestRate FixedDepositTenor = new FixedDepositInterestRate
                {
                    UniqueId = Guid.NewGuid().ToString(),
                    //EmploymentTypeId = obj.EmploymentTypeId,
                    InterestRate = obj.InterestRate,
                    LoanAmountFrom = obj.LoanAmountFrom,
                    LoanAmountTo = obj.LoanAmountTo,
                    FixedDepositTenor = obj.FixedDepositTenor,
                    Status = obj.Status
                };
                _repoFixedDepositInterestRate.Insert(FixedDepositTenor);

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
        [Route("getallFixedDepositInterestRate")]
        public Task<IActionResult> GetAllFixedDepositInterestRate()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoFixedDepositInterestRate.GetAll();
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
        [Route("getallvalidFixedDepositInterestRate")]
        public Task<IActionResult> GetAllValidFixedDepositInterestRate()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoFixedDepositInterestRate.GetAllIsValid();
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
        [Route("getFixedDepositInterestRatebyid/id")]
        public Task<IActionResult> GetFixedDepositInterestRate(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoFixedDepositInterestRate.Get(id);
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
        [Route("deleteFixedDepositInterestRatebyid/id")]
        public Task<IActionResult> DeleteFixedDepositInterestRate(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Deleted Successfully";
            try
            {
                var rec = _repoFixedDepositInterestRate.Get(id);
                _repoFixedDepositInterestRate.SoftDelete(rec);
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
        [Route("updateFixedDepositInterestRate")]
        public Task<IActionResult> UpdateFixedDepositInterestRate([FromBody] FixedDepositInterestRateUpdate obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record updated Successfully";
            try
            {
                var retChecker = _repoFixedDepositInterestRate.Get(obj.Id);
                if (retChecker is null)
                {
                    r.status = false;
                    r.message = "Record Not Found";
                }
                else
                {
                    retChecker.FixedDepositTenor = obj.FixedDepositTenor;
                    retChecker.Status = obj.Status;
                    retChecker.InterestRate = obj.InterestRate;
                    retChecker.LoanAmountFrom = obj.LoanAmountFrom;
                    retChecker.LoanAmountTo = obj.LoanAmountTo;
                    _repoFixedDepositInterestRate.Update(retChecker);
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
        public Task<IActionResult> AddRegularLoanCharge([FromBody] RegularLoanChargePost obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Saved Successfully";
            try
            {
                RegularLoanCharge RegularLoanTenor = new RegularLoanCharge
                {
                    UniqueId = Guid.NewGuid().ToString(),
                    EmploymentTypeId = obj.EmploymentTypeId,
                    IsPercentage = obj.IsPercentage,
                    ChargeAmount = obj.ChargeAmount,
                    LoanAmountFrom = obj.LoanAmountFrom,
                    LoanAmountTo = obj.LoanAmountTo,
                    LoanTenorid = obj.LoanTenorid,
                    Status = obj.Status
                };
                _repoRegularLoanCharge.Insert(RegularLoanTenor);

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
        public Task<IActionResult> GetAllRegularLoanCharge()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoRegularLoanCharge.GetAll();
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
        [Route("getallvalidRegularLoanCharge")]
        public Task<IActionResult> GetAllValidRegularLoanCharge()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoRegularLoanCharge.GetAllIsValid();
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
        [Route("getRegularLoanChargebyid/id")]
        public Task<IActionResult> GetRegularLoanCharge(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoRegularLoanCharge.Get(id);
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
        [Route("deleteRegularLoanChargebyid/id")]
        public Task<IActionResult> DeleteRegularLoanCharge(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Deleted Successfully";
            try
            {
                var rec = _repoRegularLoanCharge.Get(id);
                _repoRegularLoanCharge.SoftDelete(rec);
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
        public Task<IActionResult> UpdateRegularLoanCharge([FromBody] RegularLoanChargeUpdate obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record updated Successfully";
            try
            {
                var retChecker = _repoRegularLoanCharge.Get(obj.Id);
                if (retChecker is null)
                {
                    r.status = false;
                    r.message = "Record Not Found";
                }
                else
                {
                    retChecker.EmploymentTypeId = obj.EmploymentTypeId;
                    retChecker.Status = obj.Status;
                    retChecker.ChargeAmount = obj.ChargeAmount;
                    retChecker.IsPercentage = obj.IsPercentage;
                    retChecker.LoanAmountFrom = obj.LoanAmountFrom;
                    retChecker.LoanAmountTo = obj.LoanAmountTo;
                    retChecker.LoanTenorid = obj.LoanTenorid;
                    _repoRegularLoanCharge.Update(retChecker);
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
        #region FixedDepositStatus
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("addFixedDepositStatus")]
        public Task<IActionResult> AddFixedDepositStatus([FromBody] FixedDepositStatusFm obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Saved Successfully";
            try
            {
                var ret = _context.FixedDepositStatuses.FirstOrDefault(o => o.Name.ToLower() == obj.Name.ToLower() && o.clientid == clientid && o.Isdeleted == 0);
                if (ret != null)
                {
                    r.status = false;
                    r.message = "Record Already Exist";
                }
                else
                {
                    FixedDepositStatus RegularLoanTenor = new FixedDepositStatus { UniqueId = Guid.NewGuid().ToString(), Name = obj.Name, Status = obj.Status };
                    _repoFixedDepositStatus.Insert(RegularLoanTenor);
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
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getallFixedDepositStatus")]
        public Task<IActionResult> GetAllFixedDepositStatus()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoFixedDepositStatus.GetAll();
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
        [Route("getallvalidFixedDepositStatus")]
        public Task<IActionResult> GetAllValidFixedDepositStatus()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoFixedDepositStatus.GetAllIsValid();
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
        [Route("getFixedDepositStatusbyid/id")]
        public Task<IActionResult> GetFixedDepositStatus(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoFixedDepositStatus.Get(id);
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
        [Route("deleteFixedDepositStatusbyid/id")]
        public Task<IActionResult> DeleteFixedDepositStatus(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Deleted Successfully";
            try
            {
                var rec = _repoFixedDepositStatus.Get(id);
                _repoFixedDepositStatus.SoftDelete(rec);
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
        [Route("updateFixedDepositStatus")]
        public Task<IActionResult> UpdateFixedDepositStatus([FromBody] UtilityModificationFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record updated Successfully";
            try
            {
                var retChecker = _repoFixedDepositStatus.Get(obj.Id);
                if (retChecker is null)
                {
                    r.status = false;
                    r.message = "Record Not Found";
                }
                else
                {
                    retChecker.Name = obj.Name;
                    retChecker.Status = obj.StatusID;
                    _repoFixedDepositStatus.Update(retChecker);
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
        #region banks
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("addbanks")]
        public Task<IActionResult> AddBank([FromBody] UtilityBankFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Saved Successfully";
            try
            {
                var ret = _context.Banks.FirstOrDefault(o => o.Name.ToLower() == obj.Name.ToLower() && o.clientid == clientid && o.Isdeleted == 0);
                if (ret != null)
                {
                    r.status = false;
                    r.message = "Record Already Exist";
                }
                else
                {
                    Bank RegularLoanTenor = new Bank { UniqueId = Guid.NewGuid().ToString(), BankCode = obj.BankCode, Name = obj.Name, Status = obj.StatusID };
                    _repoBank.Insert(RegularLoanTenor);
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
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getallbanks")]
        public Task<IActionResult> GetAllBanks()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoBank.GetAll();
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
        [Route("getallvalidbanks")]
        public Task<IActionResult> GetAllValidBanks()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoBank.GetAllIsValid();
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
        [Route("getbankbyid/id")]
        public Task<IActionResult> GetBanks(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoBank.Get(id);
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
        [Route("deletebankbyid/id")]
        public Task<IActionResult> DeleteBanks(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Deleted Successfully";
            try
            {
                var rec = _repoBank.Get(id);
                _repoBank.SoftDelete(rec);
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
        [Route("updateBank")]
        public Task<IActionResult> UpdateBank([FromBody] UtilityBankUpdateFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record updated Successfully";
            try
            {
                var retChecker = _repoBank.Get(obj.Id);
                if (retChecker is null)
                {
                    r.status = false;
                    r.message = "Record Not Found";
                }
                else
                {
                    retChecker.Name = obj.Name;
                    retChecker.Status = obj.StatusID;
                    retChecker.BankCode = obj.BankCode;
                    _repoBank.Update(retChecker);
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
        #region RegularLoanTenor
        /// RegularLoanTenors region
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("addRegularLoanTenors")]
        public Task<IActionResult> AddRegularLoanTenor([FromBody] UtilityFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Saved Successfully";
            try
            {
                var ret = _context.RegularLoanTenors.FirstOrDefault(o => o.Name.ToLower() == obj.Name.ToLower() && o.clientid == clientid && o.Isdeleted == 0);
                if (ret != null)
                {
                    r.status = false;
                    r.message = "Record Already Exist";
                }
                else
                {
                    RegularLoanTenor RegularLoanTenor = new RegularLoanTenor { UniqueId = Guid.NewGuid().ToString(), Name = obj.Name, Status = obj.StatusID };
                    _repoRegularLoanTenor.Insert(RegularLoanTenor);
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
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getallRegularLoanTenors")]
        public Task<IActionResult> GetAllRegularLoanTenors()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoRegularLoanTenor.GetAll();
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
        [Route("getallvalidRegularLoanTenors")]
        public Task<IActionResult> GetAllValidRegularLoanTenors()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoRegularLoanTenor.GetAllIsValid();
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
        [Route("getRegularLoanTenorbyid/id")]
        public Task<IActionResult> GetRegularLoanTenors(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoRegularLoanTenor.Get(id);
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
        [Route("deleteRegularLoanTenorbyid/id")]
        public Task<IActionResult> DeleteRegularLoanTenors(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Deleted Successfully";
            try
            {
                var rec = _repoRegularLoanTenor.Get(id);
                _repoRegularLoanTenor.SoftDelete(rec);
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
        [Route("updateRegularLoanTenor")]
        public Task<IActionResult> UpdateRegularLoanTenor([FromBody] UtilityModificationFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record updated Successfully";
            try
            {
                var retChecker = _repoRegularLoanTenor.Get(obj.Id);
                if (retChecker is null)
                {
                    r.status = false;
                    r.message = "Record Not Found";
                }
                else
                {
                    retChecker.Name = obj.Name;
                    retChecker.Status = obj.StatusID;
                    _repoRegularLoanTenor.Update(retChecker);
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
        #region Applicationchannels
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("addApplicationchannels")]
        public Task<IActionResult> AddApplicationchannel([FromBody] UtilityFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Saved Successfully";
            try
            {
                Applicationchannel Applicationchannel = new Applicationchannel { UniqueId = Guid.NewGuid().ToString(), Name = obj.Name, Status = obj.StatusID };
                var ret = _context.Applicationchannels.FirstOrDefault(o => o.Name.ToLower() == obj.Name.ToLower() && o.clientid == clientid && o.Isdeleted == 0);
                if (ret != null)
                {
                    r.status = false;
                    r.message = "Record Already Exist";
                }
                else
                {
                    _repoApplicationchannel.Insert(Applicationchannel);
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
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getallApplicationchannels")]
        public Task<IActionResult> GetAllApplicationchannels()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoApplicationchannel.GetAll();
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
        [Route("getallvalidApplicationchannels")]
        public Task<IActionResult> GetAllValidApplicationchannels()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoApplicationchannel.GetAllIsValid();
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
        [Route("getApplicationchannelbyid/id")]
        public Task<IActionResult> GetApplicationchannels(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoApplicationchannel.Get(id);
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
        [Route("deleteApplicationchannelbyid/id")]
        public Task<IActionResult> DeleteApplicationchannels(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record deleted Successfully";
            try
            {
                var rec = _repoApplicationchannel.Get(id);
                _repoApplicationchannel.SoftDelete(rec);
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
        [Route("updateApplicationchannel")]
        public Task<IActionResult> UpdateApplicationchannel([FromBody] UtilityModificationFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record updated Successfully";
            try
            {
                var Applicationchannel = _repoApplicationchannel.Get(obj.Id);
                if (Applicationchannel is not null)
                {
                    Applicationchannel.Name = obj.Name;
                    Applicationchannel.Status = obj.StatusID;
                    _repoApplicationchannel.Update(Applicationchannel);
                }
                else
                {
                    r.status = false;
                    r.message = "Record Not Found";
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
        #region EducationalLevel
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("addeducationallevels")]
        public Task<IActionResult> AddEducationallevel([FromBody] UtilityFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Saved Successfully";
            try
            {
                Educationallevel educationallevel = new Educationallevel { UniqueId = Guid.NewGuid().ToString(), Name = obj.Name, Status = obj.StatusID };
                var ret = _context.Educationallevels.FirstOrDefault(o => o.Name.ToLower() == obj.Name.ToLower() && o.clientid == clientid && o.Isdeleted == 0);
                if (ret != null)
                {
                    r.status = false;
                    r.message = "Record Already Exist";
                }
                else
                {
                    _repoEducationallevel.Insert(educationallevel);
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
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getallEducationalLevels")]
        public Task<IActionResult> GetAllEducationalLevels()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoEducationallevel.GetAll();
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
        [Route("getallvalidEducationalLevels")]
        public Task<IActionResult> GetAllValidEducationalLevels()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoEducationallevel.GetAllIsValid();
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
        [Route("getEducationalLevelbyid/id")]
        public Task<IActionResult> GetEducationalLevels(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoEducationallevel.Get(id);
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
        [Route("deleteEducationalLevelbyid/id")]
        public Task<IActionResult> DeleteEducationalLevels(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record deleted Successfully";
            try
            {
                var rec = _repoEducationallevel.Get(id);
                _repoEducationallevel.SoftDelete(rec);
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
        [Route("updateEducationalLevel")]
        public Task<IActionResult> UpdateEducationalLevel([FromBody] UtilityModificationFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record updated Successfully";
            try
            {
                var educationallevel = _repoEducationallevel.Get(obj.Id);
                if (educationallevel is not null)
                {
                    educationallevel.Name = obj.Name;
                    educationallevel.Status = obj.StatusID;
                    _repoEducationallevel.Update(educationallevel);
                }
                else
                {
                    r.status = false;
                    r.message = "Record Not Found";
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
        #region Employmentstatuss
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("addEmploymentstatuss")]
        public Task<IActionResult> AddEmploymentstatus([FromBody] UtilityFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Saved Successfully";
            try
            {
                Employmentstatus Employmentstatus = new Employmentstatus { UniqueId = Guid.NewGuid().ToString(), Name = obj.Name, Status = obj.StatusID };
                var ret = _context.Employmentstatuses.FirstOrDefault(o => o.Name.ToLower() == obj.Name.ToLower() && o.clientid == clientid && o.Isdeleted == 0);
                if (ret != null)
                {
                    r.status = false;
                    r.message = "Record Already Exist";
                }
                else
                {
                    _repoEmploymentstatus.Insert(Employmentstatus);
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
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getallEmploymentstatuss")]
        public Task<IActionResult> GetAllEmploymentstatuss()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoEmploymentstatus.GetAll();
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
        [Route("getallvalidEmploymentstatuss")]
        public Task<IActionResult> GetAllValidEmploymentstatuss()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoEmploymentstatus.GetAllIsValid();
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
        [Route("getEmploymentstatusbyid/id")]
        public Task<IActionResult> GetEmploymentstatuss(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoEmploymentstatus.Get(id);
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
        [Route("deleteEmploymentstatusbyid/id")]
        public Task<IActionResult> DeleteEmploymentstatuss(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record deleted Successfully";
            try
            {
                var rec = _repoEmploymentstatus.Get(id);
                _repoEmploymentstatus.SoftDelete(rec);
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
        [Route("updateEmploymentstatus")]
        public Task<IActionResult> UpdateEmploymentstatus([FromBody] UtilityModificationFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record updated Successfully";
            try
            {
                var Employmentstatus = _repoEmploymentstatus.Get(obj.Id);
                if (Employmentstatus is not null)
                {
                    Employmentstatus.Name = obj.Name;
                    Employmentstatus.Status = obj.StatusID;
                    _repoEmploymentstatus.Update(Employmentstatus);
                }
                else
                {
                    r.status = false;
                    r.message = "Record Not Found";
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
        #region EmploymentSector
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("addEmploymentSector")]
        public Task<IActionResult> AddEmploymentSector([FromBody] UtilityFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Saved Successfully";
            try
            {
                EmploymentSector Employmentstatus = new EmploymentSector { UniqueId = Guid.NewGuid().ToString(), Name = obj.Name, Status = obj.StatusID };
                var ret = _context.EmploymentSectors.FirstOrDefault(o => o.Name.ToLower() == obj.Name.ToLower()
                    && o.clientid == clientid && o.Isdeleted == 0);
                if (ret != null)
                {
                    r.status = false;
                    r.message = "Record Already Exist";
                }
                else
                {
                    _repoEmploymentSector.Insert(Employmentstatus);
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
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getallEmploymentSector")]
        public Task<IActionResult> GetAllEmploymentSector()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoEmploymentSector.GetAll();
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
        [Route("getallvalidEmploymentSector")]
        public Task<IActionResult> GetAllValidEmploymentSector()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoEmploymentSector.GetAllIsValid();
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
        [Route("getEmploymentSectorbyid/id")]
        public Task<IActionResult> GetEmploymentSector(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoEmploymentstatus.Get(id);
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
        [Route("deleteEmploymentSectorbyid/id")]
        public Task<IActionResult> DeleteEmploymentSector(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record deleted Successfully";
            try
            {
                var rec = _repoEmploymentSector.Get(id);
                _repoEmploymentSector.SoftDelete(rec);
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
        [Route("updateEmploymentSector")]
        public Task<IActionResult> UpdateEmploymentSector([FromBody] UtilityModificationFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record updated Successfully";
            try
            {
                var Employmentstatus = _repoEmploymentSector.Get(obj.Id);
                if (Employmentstatus is not null)
                {
                    Employmentstatus.Name = obj.Name;
                    Employmentstatus.Status = obj.StatusID;
                    _repoEmploymentSector.Update(Employmentstatus);

                }
                else
                {
                    r.status = false;
                    r.message = "Record Not Found";
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
        #region Employmenttypes
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("addEmploymenttypes")]
        public Task<IActionResult> AddEmploymenttype([FromBody] UtilityFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Saved Successfully";
            try
            {
                Employmenttype Employmenttype = new Employmenttype { UniqueId = Guid.NewGuid().ToString(), Name = obj.Name, Status = obj.StatusID };
                var ret = _context.Employmenttypes.FirstOrDefault(o => o.Name.ToLower() == obj.Name.ToLower()
                   && o.clientid == clientid && o.Isdeleted == 0);
                if (ret != null)
                {
                    r.status = false;
                    r.message = "Record Already Exist";
                }
                else
                {
                    _repoEmploymenttype.Insert(Employmenttype);
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
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getallEmploymenttypes")]
        public Task<IActionResult> GetAllEmploymenttypes()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoEmploymenttype.GetAll();
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
        [Route("getallvalidEmploymenttypes")]
        public Task<IActionResult> GetAllValidEmploymenttypes()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoEmploymenttype.GetAllIsValid();
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
        [Route("getEmploymenttypebyid/id")]
        public Task<IActionResult> GetEmploymenttypes(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoEmploymenttype.Get(id);
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
        [Route("deleteEmploymenttypebyid/id")]
        public Task<IActionResult> DeleteEmploymenttypes(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record deleted Successfully";
            try
            {
                var rec = _repoEmploymenttype.Get(id);
                _repoEmploymenttype.SoftDelete(rec);
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
        [Route("updateEmploymenttype")]
        public Task<IActionResult> UpdateEmploymenttype([FromBody] UtilityModificationFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record updated Successfully";
            try
            {
                var Employmenttype = _repoEmploymenttype.Get(obj.Id);
                if (Employmenttype is not null)
                {
                    Employmenttype.Name = obj.Name;
                    Employmenttype.Status = obj.StatusID;
                    _repoEmploymenttype.Update(Employmenttype);
                }
                else
                {
                    r.status = false;
                    r.message = "Record Not Found";
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
        #region Genders
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("addGenders")]
        public Task<IActionResult> AddGender([FromBody] UtilityFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Saved Successfully";
            try
            {
                Gender Gender = new Gender { UniqueId = Guid.NewGuid().ToString(), Name = obj.Name, Status = obj.StatusID };
                var ret = _context.Genders.FirstOrDefault(o => o.Name.ToLower() == obj.Name.ToLower()
                    && o.clientid == clientid && o.Isdeleted == 0);
                if (ret != null)
                {
                    r.status = false;
                    r.message = "Record Already Exist";
                }
                else
                {
                    _repoGender.Insert(Gender);
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
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getallGenders")]
        public Task<IActionResult> GetAllGenders()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoGender.GetAll();
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
        [Route("getallvalidGenders")]
        public Task<IActionResult> GetAllValidGenders()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoGender.GetAllIsValid();
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
        [Route("getGenderbyid/id")]
        public Task<IActionResult> GetGenders(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoGender.Get(id);
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
        [Route("deleteGenderbyid/id")]
        public Task<IActionResult> DeleteGenders(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record deleted Successfully";
            try
            {
                var ret = _repoGender.Get(id);
                _repoGender.Delete(ret);
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
        [Route("updateGender")]
        public Task<IActionResult> UpdateGender([FromBody] UtilityModificationFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record updated Successfully";
            try
            {
                var Gender = _repoGender.Get(obj.Id);
                if (Gender is not null)
                {
                    Gender.Name = obj.Name;
                    Gender.Status = obj.StatusID;
                    _repoGender.Update(Gender);

                }
                else
                {
                    r.status = false;
                    r.message = "Record Not Found";
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
        #region Lgas
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("addLgas")]
        public Task<IActionResult> AddLga([FromBody] UtilityFormModelForStateAndLga obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Saved Successfully";
            try
            {
                Lga Lga = new Lga { UniqueId = Guid.NewGuid().ToString(), Name = obj.Name, stateid = obj.DetId, Status = obj.StatusID };
                var ret = _context.Lgas.FirstOrDefault(o => o.Name.ToLower() == obj.Name.ToLower()
                    && o.clientid == clientid && o.Isdeleted == 0);
                if (ret != null)
                {
                    r.status = false;
                    r.message = "Record Already Exist";
                }
                else
                {
                    _repoLga.Insert(Lga);
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

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getallLgasByStateId")]
        public Task<IActionResult> GetAllLgasByStateId(string cotId)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoLga.GetAllIsValid().Where(o => o.stateid == cotId);
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
        [Route("getallLgas")]
        public Task<IActionResult> GetAllLgas()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoLga.GetAll();
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
        [Route("getallvalidLgas")]
        public Task<IActionResult> GetAllValidLgas()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoLga.GetAllIsValid();
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
        [Route("getLgabyid/id")]
        public Task<IActionResult> GetLgas(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoLga.Get(id);
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
        [Route("deleteLgabyid/id")]
        public Task<IActionResult> DeleteLgas(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record deleted Successfully";
            try
            {
                var rec = _repoLga.Get(id);
                _repoLga.SoftDelete(rec);
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
        [Route("updateLga")]
        public Task<IActionResult> UpdateLga([FromBody] UtilityModificationFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record updated Successfully";
            try
            {
                var Lga = _repoLga.Get(obj.Id);
                if (Lga is not null)
                {
                    Lga.Name = obj.Name;
                    Lga.Status = obj.StatusID;
                    _repoLga.Update(Lga);

                }
                else
                {
                    r.status = false;
                    r.message = "Record Not Found";
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
        #region Maritalstatuss
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("addMaritalstatuss")]
        public Task<IActionResult> AddMaritalstatus([FromBody] UtilityFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Saved Successfully";
            try
            {
                Maritalstatus Maritalstatus = new Maritalstatus { UniqueId = Guid.NewGuid().ToString(), Name = obj.Name, Status = obj.StatusID };
                var ret = _context.Maritalstatuses.FirstOrDefault(o => o.Name.ToLower() == obj.Name.ToLower()
                    && o.clientid == clientid && o.Isdeleted == 0);
                if (ret != null)
                {
                    r.status = false;
                    r.message = "Record Already Exist";
                }
                else
                {
                    _repoMaritalstatus.Insert(Maritalstatus);
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
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getallMaritalstatuss")]
        public Task<IActionResult> GetAllMaritalstatuss()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoMaritalstatus.GetAll();
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
        [Route("getallvalidMaritalstatuss")]
        public Task<IActionResult> GetAllValidMaritalstatuss()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoMaritalstatus.GetAllIsValid();
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
        [Route("getMaritalstatusbyid/id")]
        public Task<IActionResult> GetMaritalstatuss(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoMaritalstatus.Get(id);
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
        [Route("deleteMaritalstatusbyid/id")]
        public Task<IActionResult> DeleteMaritalstatuss(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record delete Successfully";
            try
            {
                var rec = _repoMaritalstatus.Get(id);
                _repoMaritalstatus.SoftDelete(rec);
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
        [Route("updateMaritalstatus")]
        public Task<IActionResult> UpdateMaritalstatus([FromBody] UtilityModificationFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record updated Successfully";
            try
            {
                var Maritalstatus = _repoMaritalstatus.Get(obj.Id);
                if (Maritalstatus is not null)
                {
                    Maritalstatus.Name = obj.Name;
                    Maritalstatus.Status = obj.StatusID;
                    _repoMaritalstatus.Update(Maritalstatus);

                }
                else
                {
                    r.status = false;
                    r.message = "Record Not Found";
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
        #region Country
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("addCountry")]
        public Task<IActionResult> AddCountry([FromBody] UtilityFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Saved Successfully";
            try
            {
                Nationality Nationality = new Nationality { UniqueId = Guid.NewGuid().ToString(), Name = obj.Name, Status = obj.StatusID };
                var ret = _context.Nationalities.FirstOrDefault(o => o.Name.ToLower() == obj.Name.ToLower()
                    && o.clientid == clientid && o.Isdeleted == 0);
                if (ret != null)
                {
                    r.status = false;
                    r.message = "Record Already Exist";
                }
                else
                {
                    _repoNationality.Insert(Nationality);
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
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getallCountry")]
        public Task<IActionResult> GetAllCountrys()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoNationality.GetAll();
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
        [Route("getallvalidCountry")]
        public Task<IActionResult> GetAllValidCountry()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoNationality.GetAllIsValid();
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
        [Route("getCountrybyid/id")]
        public Task<IActionResult> GetCountry(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoNationality.Get(id);
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
        [Route("deleteCountrybyid/id")]
        public Task<IActionResult> DeleteCountrys(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record delete Successfully";
            try
            {
                var rec = _repoNationality.Get(id);
                _repoNationality.SoftDelete(rec);
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
        [Route("updateCountry")]
        public Task<IActionResult> UpdateCountry([FromBody] UtilityModificationFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record updated Successfully";
            try
            {
                var Nationality = _repoNationality.Get(obj.Id);
                if (Nationality is not null)
                {
                    Nationality.Name = obj.Name;
                    Nationality.Status = obj.StatusID;
                    _repoNationality.Update(Nationality);

                }
                else
                {
                    r.status = false;
                    r.message = "Record Not Found";
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
        #region Nationalitys
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("addNationalitys")]
        public Task<IActionResult> AddNationality([FromBody] UtilityFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Saved Successfully";
            try
            {
                Nationality Nationality = new Nationality { UniqueId = Guid.NewGuid().ToString(), Name = obj.Name, Status = obj.StatusID };
                var ret = _context.Nationalities.FirstOrDefault(o => o.Name.ToLower() == obj.Name.ToLower()
                    && o.clientid == clientid && o.Isdeleted == 0);
                if (ret != null)
                {
                    r.status = false;
                    r.message = "Record Already Exist";
                }
                else
                {
                    _repoNationality.Insert(Nationality);
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
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getallNationalitys")]
        public Task<IActionResult> GetAllNationalitys()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoNationality.GetAll();
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
        [Route("getallvalidNationalitys")]
        public Task<IActionResult> GetAllValidNationalitys()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoNationality.GetAllIsValid();
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
        [Route("getNationalitybyid/id")]
        public Task<IActionResult> GetNationalitys(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoNationality.Get(id);
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
        [Route("deleteNationalitybyid/id")]
        public Task<IActionResult> DeleteNationalitys(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record delete Successfully";
            try
            {
                var rec = _repoNationality.Get(id);
                _repoNationality.SoftDelete(rec);
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
        [Route("updateNationality")]
        public Task<IActionResult> UpdateNationality([FromBody] UtilityModificationFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record updated Successfully";
            try
            {
                var Nationality = _repoNationality.Get(obj.Id);
                if (Nationality is not null)
                {
                    Nationality.Name = obj.Name;
                    Nationality.Status = obj.StatusID;
                    _repoNationality.Update(Nationality);

                }
                else
                {
                    r.status = false;
                    r.message = "Record Not Found";
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
        #region Noofdependants
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("addNoofdependants")]
        public Task<IActionResult> AddNoofdependant([FromBody] UtilityFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Saved Successfully";
            try
            {
                Noofdependant Noofdependant = new Noofdependant { UniqueId = Guid.NewGuid().ToString(), Name = obj.Name, Status = obj.StatusID };
                var ret = _context.Noofdependants.FirstOrDefault(o => o.Name.ToLower() == obj.Name.ToLower()
                    && o.clientid == clientid && o.Isdeleted == 0);
                if (ret != null)
                {
                    r.status = false;
                    r.message = "Record Already Exist";
                }
                else
                {
                    _repoNoofdependant.Insert(Noofdependant);
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
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getallNoofdependants")]
        public Task<IActionResult> GetAllNoofdependants()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoNoofdependant.GetAll();
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
        [Route("getallvalidNoofdependants")]
        public Task<IActionResult> GetAllValidNoofdependants()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoNoofdependant.GetAllIsValid();
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
        [Route("getNoofdependantbyid/id")]
        public Task<IActionResult> GetNoofdependants(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoNoofdependant.Get(id);
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
        [Route("deleteNoofdependantbyid/id")]
        public Task<IActionResult> DeleteNoofdependants(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record deleted Successfully";
            try
            {
                var rec = _repoNoofdependant.Get(id);
                _repoNoofdependant.SoftDelete(rec);
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
        [Route("updateNoofdependant")]
        public Task<IActionResult> UpdateNoofdependant([FromBody] UtilityModificationFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record updated Successfully";
            try
            {
                var Noofdependant = _repoNoofdependant.Get(obj.Id);
                if (Noofdependant is not null)
                {
                    Noofdependant.Name = obj.Name;
                    Noofdependant.Status = obj.StatusID;
                    _repoNoofdependant.Update(Noofdependant);

                }
                else
                {
                    r.status = false;
                    r.message = "Record Not Found";
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
        #region Noofyearofresidences
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("addNoofyearofresidences")]
        public Task<IActionResult> AddNoofyearofresidence([FromBody] UtilityFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Saved Successfully";
            try
            {
                Noofyearofresidence Noofyearofresidence = new Noofyearofresidence { UniqueId = Guid.NewGuid().ToString(), Name = obj.Name, Status = obj.StatusID };
                var ret = _context.Noofyearofresidences.FirstOrDefault(o => o.Name.ToLower() == obj.Name.ToLower()
                    && o.clientid == clientid && o.Isdeleted == 0);
                if (ret != null)
                {
                    r.status = false;
                    r.message = "Record Already Exist";
                }
                else
                {
                    _repoNoofyearofresidence.Insert(Noofyearofresidence);
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
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getallNoofyearofresidences")]
        public Task<IActionResult> GetAllNoofyearofresidences()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoNoofyearofresidence.GetAll();
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
        [Route("getallvalidNoofyearofresidences")]
        public Task<IActionResult> GetAllValidNoofyearofresidences()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoNoofyearofresidence.GetAllIsValid();
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
        [Route("getNoofyearofresidencebyid/id")]
        public Task<IActionResult> GetNoofyearofresidences(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoNoofyearofresidence.Get(id);
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
        [Route("deleteNoofyearofresidencebyid/id")]
        public Task<IActionResult> DeleteNoofyearofresidences(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record delete Successfully";
            try
            {
                var rec = _repoNoofyearofresidence.Get(id);
                _repoNoofyearofresidence.SoftDelete(rec);
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
        [Route("updateNoofyearofresidence")]
        public Task<IActionResult> UpdateNoofyearofresidence([FromBody] UtilityModificationFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record updated Successfully";
            try
            {
                var Noofyearofresidence = _repoNoofyearofresidence.Get(obj.Id);
                if (Noofyearofresidence is not null)
                {
                    Noofyearofresidence.Name = obj.Name;
                    Noofyearofresidence.Status = obj.StatusID;
                    _repoNoofyearofresidence.Update(Noofyearofresidence);

                }
                else
                {
                    r.status = false;
                    r.message = "Record Not Found";
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
        #region Organizations
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("addOrganizations")]
        public Task<IActionResult> AddOrganization([FromBody] UtilityFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Saved Successfully";
            try
            {
                Organization Organization = new Organization { UniqueId = Guid.NewGuid().ToString(), Name = obj.Name, Status = obj.StatusID };
                var ret = _context.Organizations.FirstOrDefault(o => o.Name.ToLower() == obj.Name.ToLower()
                    && o.clientid == clientid && o.Isdeleted == 0);
                if (ret != null)
                {
                    r.status = false;
                    r.message = "Record Already Exist";
                }
                else
                {
                    _repoOrganization.Insert(Organization);
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
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getallOrganizations")]
        public Task<IActionResult> GetAllOrganizations()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoOrganization.GetAll();
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
        [Route("getallvalidOrganizations")]
        public Task<IActionResult> GetAllValidOrganizations()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoOrganization.GetAllIsValid();
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
        [Route("getOrganizationbyid/id")]
        public Task<IActionResult> GetOrganizations(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoOrganization.Get(id);
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
        [Route("deleteOrganizationbyid/id")]
        public Task<IActionResult> DeleteOrganizations(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record delete Successfully";
            try
            {
                var rec = _repoOrganization.Get(id);
                _repoOrganization.SoftDelete(rec);
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
        [Route("updateOrganization")]
        public Task<IActionResult> UpdateOrganization([FromBody] UtilityModificationFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record updated Successfully";
            try
            {
                var Organization = _repoOrganization.Get(obj.Id);
                if (Organization is not null)
                {
                    Organization.Name = obj.Name;
                    Organization.Status = obj.StatusID;
                    _repoOrganization.Update(Organization);
                }
                else
                {
                    r.status = false;
                    r.message = "Record Not Found";
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
        #region Residentialstatuss
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("addResidentialstatuss")]
        public Task<IActionResult> AddResidentialstatus([FromBody] UtilityFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Saved Successfully";
            try
            {
                Residentialstatus Residentialstatus = new Residentialstatus { UniqueId = Guid.NewGuid().ToString(), Name = obj.Name, Status = obj.StatusID };
                var ret = _context.Residentialstatuses.FirstOrDefault(o => o.Name.ToLower() == obj.Name.ToLower()
                    && o.clientid == clientid && o.Isdeleted == 0);
                if (ret != null)
                {
                    r.status = false;
                    r.message = "Record Already Exist";
                }
                else
                {
                    _repoResidentialstatus.Insert(Residentialstatus);
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
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getallResidentialstatuss")]
        public Task<IActionResult> GetAllResidentialstatuss()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoResidentialstatus.GetAll();
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
        [Route("getallvalidResidentialstatuss")]
        public Task<IActionResult> GetAllValidResidentialstatuss()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoResidentialstatus.GetAllIsValid();
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
        [Route("getResidentialstatusbyid/id")]
        public Task<IActionResult> GetResidentialstatuss(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoResidentialstatus.Get(id);
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
        [Route("deleteResidentialstatusbyid/id")]
        public Task<IActionResult> DeleteResidentialstatuss(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record delete Successfully";
            try
            {
                var rec = _repoResidentialstatus.Get(id);
                _repoResidentialstatus.SoftDelete(rec);
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
        [Route("updateResidentialstatus")]
        public Task<IActionResult> UpdateResidentialstatus([FromBody] UtilityModificationFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record updated Successfully";
            try
            {
                var Residentialstatus = _repoResidentialstatus.Get(obj.Id);
                if (Residentialstatus is not null)
                {
                    Residentialstatus.Name = obj.Name;
                    Residentialstatus.Status = obj.StatusID;
                    _repoResidentialstatus.Update(Residentialstatus);

                }
                else
                {
                    r.status = false;
                    r.message = "Record Not Found";
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
        #region Salarypaymentdates
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("addSalarypaymentdates")]
        public Task<IActionResult> AddSalarypaymentdate([FromBody] UtilityFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Saved Successfully";
            try
            {
                Salarypaymentdate Salarypaymentdate = new Salarypaymentdate { UniqueId = Guid.NewGuid().ToString(), Name = obj.Name, Status = obj.StatusID };
                var ret = _context.Salarypaymentdates.FirstOrDefault(o => o.Name.ToLower() == obj.Name.ToLower()
                    && o.clientid == clientid && o.Isdeleted == 0);
                if (ret != null)
                {
                    r.status = false;
                    r.message = "Record Already Exist";
                }
                else
                {
                    _repoSalarypaymentdate.Insert(Salarypaymentdate);
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
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getallSalarypaymentdates")]
        public Task<IActionResult> GetAllSalarypaymentdates()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoSalarypaymentdate.GetAll();
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
        [Route("getallvalidSalarypaymentdates")]
        public Task<IActionResult> GetAllValidSalarypaymentdates()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoSalarypaymentdate.GetAllIsValid();
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
        [Route("getSalarypaymentdatebyid/id")]
        public Task<IActionResult> GetSalarypaymentdates(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoSalarypaymentdate.Get(id);
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
        [Route("deleteSalarypaymentdatebyid/id")]
        public Task<IActionResult> DeleteSalarypaymentdates(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Saved Successfully";
            try
            {
                var rec = _repoSalarypaymentdate.Get(id);
                _repoSalarypaymentdate.SoftDelete(rec);
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
        [Route("updateSalarypaymentdate")]
        public Task<IActionResult> UpdateSalarypaymentdate([FromBody] UtilityModificationFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record updated Successfully";
            try
            {
                var Salarypaymentdate = _repoSalarypaymentdate.Get(obj.Id);
                if (Salarypaymentdate is not null)
                {
                    Salarypaymentdate.Name = obj.Name;
                    Salarypaymentdate.Status = obj.StatusID;
                    _repoSalarypaymentdate.Update(Salarypaymentdate);

                }
                else
                {
                    r.status = false;
                    r.message = "Record Not Found";
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
        #region Salaryranges
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("addSalaryranges")]
        public Task<IActionResult> AddSalaryrange([FromBody] UtilityFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Saved Successfully";
            try
            {
                Salaryrange Salaryrange = new Salaryrange { UniqueId = Guid.NewGuid().ToString(), Name = obj.Name, Status = obj.StatusID };
                var ret = _context.Salaryranges.FirstOrDefault(o => o.Name.ToLower() == obj.Name.ToLower()
                    && o.clientid == clientid && o.Isdeleted == 0);
                if (ret != null)
                {
                    r.status = false;
                    r.message = "Record Already Exist";
                }
                else
                {
                    _repoSalaryrange.Insert(Salaryrange);
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
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getallSalaryranges")]
        public Task<IActionResult> GetAllSalaryranges()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoSalaryrange.GetAll();
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
        [Route("getallvalidSalaryranges")]
        public Task<IActionResult> GetAllValidSalaryranges()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoSalaryrange.GetAllIsValid();
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
        [Route("getSalaryrangebyid/id")]
        public Task<IActionResult> GetSalaryranges(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoSalaryrange.Get(id);
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
        [Route("deleteSalaryrangebyid/id")]
        public Task<IActionResult> DeleteSalaryranges(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record deleted Successfully";
            try
            {
                var rec = _repoSalaryrange.Get(id);
                _repoSalaryrange.SoftDelete(rec);
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
        [Route("updateSalaryrange")]
        public Task<IActionResult> UpdateSalaryrange([FromBody] UtilityModificationFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record updated Successfully";
            try
            {
                var Salaryrange = _repoSalaryrange.Get(obj.Id);
                if (Salaryrange is not null)
                {
                    Salaryrange.Name = obj.Name;
                    Salaryrange.Status = obj.StatusID;
                    _repoSalaryrange.Update(Salaryrange);

                }
                else
                {
                    r.status = false;
                    r.message = "Record Not Found";
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
        #region States
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("addStates")]
        public Task<IActionResult> AddState([FromBody] UtilityFormModelForStateAndLga obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Saved Successfully";
            try
            {
                State State = new State { UniqueId = Guid.NewGuid().ToString(), Name = obj.Name, Countryid = obj.DetId, Status = obj.StatusID };
                var ret = _context.States.FirstOrDefault(o => o.Name.ToLower() == obj.Name.ToLower()
                    && o.clientid == clientid && o.Isdeleted == 0);
                if (ret != null)
                {
                    r.status = false;
                    r.message = "Record Already Exist";
                }
                else
                {
                    _repoState.Insert(State);
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
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getallStates")]
        public Task<IActionResult> GetAllStates()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoState.GetAll();
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
        [Route("getallvalidStates")]
        public Task<IActionResult> GetAllValidStates()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoState.GetAllIsValid();
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
        [Route("getallStatesByCountryId")]
        public Task<IActionResult> GetAllStatesByCountryId(string cotId)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoState.GetAllIsValid().Where(o => o.Countryid == cotId);
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
        [Route("getStatebyid/id")]
        public Task<IActionResult> GetStates(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoState.Get(id);
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
        [Route("deleteStatebyid/id")]
        public Task<IActionResult> DeleteStates(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record delete Successfully";
            try
            {
                var rec = _repoState.Get(id);
                _repoState.SoftDelete(rec);
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
        [Route("updateState")]
        public Task<IActionResult> UpdateState([FromBody] UtilityModificationFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record updated Successfully";
            try
            {
                var State = _repoState.Get(obj.Id);
                if (State is not null)
                {
                    State.Name = obj.Name;
                    State.Status = obj.StatusID;
                    _repoState.Update(State);

                }
                else
                {
                    r.status = false;
                    r.message = "Record Not Found";
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
        #region Titles
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("addTitles")]
        public Task<IActionResult> AddTitle([FromBody] UtilityFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Saved Successfully";
            try
            {
                Title Title = new Title { UniqueId = Guid.NewGuid().ToString(), Name = obj.Name, Status = obj.StatusID };
                var ret = _context.Titles.FirstOrDefault(o => o.Name.ToLower() == obj.Name.ToLower()
                    && o.clientid == clientid && o.Isdeleted == 0);
                if (ret != null)
                {
                    r.status = false;
                    r.message = "Record Already Exist";
                }
                else
                {
                    _repoTitle.Insert(Title);
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
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getallTitles")]
        public Task<IActionResult> GetAllTitles()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoTitle.GetAll();
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
        [Route("getallvalidTitles")]
        public Task<IActionResult> GetAllValidTitles()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoTitle.GetAllIsValid();
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
        [Route("getTitlebyid/id")]
        public Task<IActionResult> GetTitles(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoTitle.Get(id);
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
        [Route("deleteTitlebyid/id")]
        public Task<IActionResult> DeleteTitles(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record delete Successfully";
            try
            {
                var rec = _repoTitle.Get(id);
                _repoTitle.SoftDelete(rec);
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
        [Route("updateTitle")]
        public Task<IActionResult> UpdateTitle([FromBody] UtilityModificationFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record updated Successfully";
            try
            {
                var Title = _repoTitle.Get(obj.Id);
                if (Title is not null)
                {
                    Title.Name = obj.Name;
                    Title.Status = obj.StatusID;
                    _repoTitle.Update(Title);

                }
                else
                {
                    r.status = false;
                    r.message = "Record Not Found";
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
        #region UtilityBillTypes
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("addUtilityBillTypes")]
        public Task<IActionResult> AddUtilityBillType([FromBody] UtilityFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Saved Successfully";
            try
            {
                UtilityBillType UtilityBillType = new UtilityBillType { UniqueId = Guid.NewGuid().ToString(), Name = obj.Name, Status = obj.StatusID };
                var ret = _context.UtilityBillTypes.FirstOrDefault(o => o.Name.ToLower() == obj.Name.ToLower()
                    && o.clientid == clientid && o.Isdeleted == 0);
                if (ret != null)
                {
                    r.status = false;
                    r.message = "Record Already Exist";
                }
                else
                {
                    _repoUtilityBillType.Insert(UtilityBillType);
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
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getallUtilityBillTypes")]
        public Task<IActionResult> GetAllUtilityBillTypes()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoUtilityBillType.GetAll();
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
        [Route("getallvalidUtilityBillTypes")]
        public Task<IActionResult> GetAllValidUtilityBillTypes()
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoUtilityBillType.GetAllIsValid();
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
        [Route("getUtilityBillTypebyid/id")]
        public Task<IActionResult> GetUtilityBillTypes(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                r.data = _repoUtilityBillType.Get(id);
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
        [Route("deleteUtilityBillTypebyid/id")]
        public Task<IActionResult> DeleteUtilityBillTypes(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record deleted Successfully";
            try
            {
                var rec = _repoUtilityBillType.Get(id);
                _repoUtilityBillType.SoftDelete(rec);
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
        [Route("updateUtilityBillType")]
        public Task<IActionResult> UpdateUtilityBillType([FromBody] UtilityModificationFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record updated Successfully";
            try
            {
                var UtilityBillType = _repoUtilityBillType.Get(obj.Id);
                if (UtilityBillType is not null)
                {
                    UtilityBillType.Name = obj.Name;
                    UtilityBillType.Status = obj.StatusID;
                    _repoUtilityBillType.Update(UtilityBillType);

                }
                else
                {
                    r.status = false;
                    r.message = "Record Not Found";
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