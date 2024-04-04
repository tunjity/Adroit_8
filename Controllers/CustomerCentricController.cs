using System.Linq;
using Adroit_v8.Model;
using Adroit_v8.Models.FormModel;
using Adroit_v8.MongoConnections;
using Adroit_v8.MongoConnections.CustomerCentric;
using Adroit_v8.MongoConnections.LoanApplication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Adroit_v8.Config.Helper;

namespace Adroit_v8.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerCentricController : AuthController
    {
        private readonly AdroitDbContext _context;
        private ICustomerCentricRepository<CustomerCentricSavings> _repo;
        private ICustomerCentricRepository<LoanBidding> _repoLoanBidding;
        private readonly IMongoRepository<RegularLoanDisbursement> _repoLD;
        private ICustomerCentricRepository<ConsumerCentricEscrow> _repoEs;
        private ICustomerCentricRepository<CustomerCentricPayment> _repoPayment;
        private ICustomerCentricRepository<CustomerCentricWalleToBankTransfer> _repoTransfer;
        private readonly IAdroitRepository<RegularLoanRepaymentPlan> _repoRegularLoanRepaymentPlan;
        private ICustomerCentricRepository<CustomerCentricWalleToBankTransferStatus> _repoTransferStatus;
        private ICustomerCentricRepository<CustomerCentricAirtime> _repoAirtime;
        private ICustomerCentricRepository<CustomerCentricP2p> _repoP2p;
        private ICustomerCentricRepository<MobileAppP2PLoanRequestMonthlyRepaymentCollection> _repoP2pRepay;
        private ICustomerCentricRepository<CustomerCentricData> _repoData;
        AuthDto auth = new AuthDto();
        public CustomerCentricController(AdroitDbContext context,
            ICustomerCentricRepository<CustomerCentricData> repoData,
            ICustomerCentricRepository<LoanBidding> repoLoanBidding,
            ICustomerCentricRepository<ConsumerCentricEscrow> repoEs,
            ICustomerCentricRepository<CustomerCentricAirtime> repoAirtime,
            ICustomerCentricRepository<CustomerCentricP2p> repoP2p, IMongoRepository<RegularLoanDisbursement> repoLD,
             IAdroitRepository<RegularLoanRepaymentPlan> repoRegularLoanRepaymentPlan,
             ICustomerCentricRepository<MobileAppP2PLoanRequestMonthlyRepaymentCollection> repoP2pRepay,
            ICustomerCentricRepository<CustomerCentricWalleToBankTransferStatus> repoTransferStatus,
            ICustomerCentricRepository<CustomerCentricWalleToBankTransfer> repoTransfer,
            ICustomerCentricRepository<CustomerCentricPayment> repoPayment,
            ICustomerCentricRepository<CustomerCentricSavings> repo, IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
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
            _repoP2pRepay = repoP2pRepay;
            _repoEs = repoEs;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _repoData = repoData;
            _repoLoanBidding = repoLoanBidding;
            _repoP2p = repoP2p;
            _repoLD = repoLD;
            _repoRegularLoanRepaymentPlan = repoRegularLoanRepaymentPlan;
            _repoAirtime = repoAirtime;
            _repoTransferStatus = repoTransferStatus;
            _repoTransfer = repoTransfer;
            _repoPayment = repoPayment;
            _repo = repo;
        }

        #region  fixeddeposit
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getallfixeddeposits")]
        public IActionResult GetAllfixeddeposits([FromQuery] CustomerCentricFilter obj)
        {
            IEnumerable<CustomerCentricResponse> neRes = null;
            IEnumerable<CustomerCentricResponse> allSav = null;
            List<Customer> query = new();
            var r = new ReturnObject();
            var eget = false;
            try
            {
                switch (obj.Det)
                {
                    case 1:
                        neRes = getAllFixedDeposit().Where(o =>
                        o.DateCreated.Day > obj.StartDate.Day
                        && o.DateCreated.Day < obj.EndDate.Day + 1);
                        if (obj.Status != 0)
                            neRes = neRes.Where(o => o.Status == obj.Status);
                        break;
                    case 2:
                        if (string.IsNullOrWhiteSpace(obj.SearchName))
                        {
                            neRes = getAllFixedDeposit();
                        }
                        else
                        {
                            switch (obj.SearchType.ToLower())
                            {
                                case "email":
                                    neRes = getAllFixedDeposit().Where(o => o.EmailAddress.ToLower().Contains(obj.SearchName.ToLower()));

                                    break;
                                case "phone":
                                    allSav = (from c in _context.FixedDepositSetups
                                              join d in _context.Customers.Where(o => o.PhoneNumber.ToLower().Contains(obj.SearchName.ToLower()))
                                              on c.CustomerId equals d.Id
                                              select new CustomerCentricResponse
                                              {
                                                  CustomerRef = d.CustomerRef,
                                                  EmailAddress = d.EmailAddress,
                                                  FirstName = d.FirstName,
                                                  LastName = d.LastName,
                                                  MiddleName = d.MiddleName,
                                                  DateOfBirth = d.DateOfBirth,
                                                  Bvn = d.Bvn,
                                                  CustomerId = c.CustomerId.GetValueOrDefault(),
                                                  StatusName = c.Status.ToString()
                                              }).AsEnumerable();

                                    neRes = allSav.Distinct(new EntityComparer<CustomerCentricResponse>(user => user.CustomerId));
                                    break;
                                case "name":
                                    allSav = getAllFixedDeposit().Where(o => o.FirstName.ToLower().Contains(obj.SearchName.ToLower())
                                              || o.LastName.ToLower().Contains(obj.SearchName.ToLower())
                                              || o.MiddleName.ToLower().Contains(obj.SearchName.ToLower())
                                              );
                                    break;
                                default:

                                    break;
                            }
                        };
                        break;
                    default:
                        break;
                };
                var fneRes = neRes.Skip((obj.PageNumber - 1) * obj.PasgeSize)
                    .Take(obj.PasgeSize);
                if (neRes.Any())
                    eget = true;
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = fneRes.ToList();
                r.recordCount = neRes.Count();
                r.recordPageNumber = obj.PageNumber;
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

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("GetAllfixeddepositsStattus")]
        public IActionResult GetAllfixeddepositsStattus()
        {

            var r = new ReturnObject();
            var eget = true;
            try
            {
                var fneRes = _context.FixDepositStatus.Select(fd => new DropDownDetail { Id = fd.Id, Name = fd.StatusName }).ToList();
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = fneRes.ToList();
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
        [Route("getallfixeddepositsForFilter")]
        public IActionResult GetAllfixeddepositForFilter([FromBody] FilterModel obj)
        {
            var r = new ReturnObject();
            var eget = false;
            r.message = "Record Fetched Successfully";
            try
            {
                string searchString = obj.SearchBy.ToLower().Trim();
                var allSav = (from c in _context.FixedDepositSetups
                              join d in _context.Customers on c.CustomerId equals d.Id
                              where d.EmailAddress.ToLower().Trim() == searchString
                              || d.PhoneNumber.ToLower().Trim() == searchString
                              || d.FirstName.ToLower().Trim() == searchString
                              select new CustomerCentricResponse
                              {
                                  CustomerRef = d.CustomerRef,
                                  EmailAddress = d.EmailAddress,
                                  FirstName = d.FirstName,
                                  LastName = d.LastName,
                                  MiddleName = d.MiddleName,
                                  DateOfBirth = d.DateOfBirth,
                                  Bvn = d.Bvn,
                                  CustomerId = c.CustomerId.GetValueOrDefault(),
                                  StatusName = c.Status.ToString()
                              }).AsEnumerable();

                var neRes = allSav.Distinct(new EntityComparer<CustomerCentricResponse>(user => user.CustomerId));
                if (neRes.Any())
                    eget = true;
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = eget ? neRes.ToList() : "";
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
        [HttpPut]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("ModifyfixeddepositsWithReferenceId")]
        public IActionResult Modifyfixeddepositspaymentsstatus([FromBody] FilterUpdateModelWithDesciption obj)
        {
            var r = new ReturnObject();
            var eget = false;
            try
            {
                var allSav = _context.FixedDepositSetups.FirstOrDefault(o => o.ReferenceId.Trim().ToLower() == obj.EntityId.Trim().ToLower());

                if (allSav is not null)
                {
                    eget = true;
                    _context.FixedDepositSetups
                        .Where(o => o.ReferenceId.Trim().ToLower() == obj.EntityId.Trim().ToLower())
                        .ExecuteUpdate(o => o.SetProperty(s => s.Status, Convert.ToInt32(obj.Status))
                        .SetProperty(s => s.Reason, obj.Description));

                }
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = eget ? allSav : "";
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
        // Method for getting all savings by customer ID
        // Returns a list of savings or an error message with status code
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("GetfixeddepositByCusId/{cusId}")]
        public IActionResult GetfixeddepositByCustomerId(int cusId)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";

            try
            {
                var allSav = (from c in _context.FixedDepositSetups
                              join d in _context.Customers on c.CustomerId equals d.Id
                              join e in _context.FixDepositStatus on c.Status equals e.Id
                              where c.CustomerId == cusId
                              select new
                              {
                                  c,
                                  d,
                                  e
                              }).ToList();
                var res = new CustomerCentricResponseForView();
                if (allSav is not null && allSav.Count > 0)
                {
                    res.PhoneNumber = allSav.FirstOrDefault().d.PhoneNumber;
                    res.EmailAddress = allSav.FirstOrDefault().d.EmailAddress;
                    res.FullName = $"{allSav.FirstOrDefault().d.FirstName} {allSav.FirstOrDefault().d.MiddleName} {allSav.FirstOrDefault().d.LastName}";
                    res.DateOfBirth = allSav.FirstOrDefault().d.DateOfBirth;
                    res.Bvn = allSav.FirstOrDefault().d.Bvn;
                    foreach (var rr in allSav.ToList().Select(o => o.c))
                    {
                        rr.StatusName = allSav.ToList().Select(o => o.e).FirstOrDefault(o => o.Id == rr.Status).StatusName;
                    }
                    res.ListItem = allSav.ToList().Select(o => o.c);
                    r.data = res;
                }
                return Ok(r);
            }
            catch (Exception ex)
            {
                // If an error occurs, return a 500 Internal Server Error status code
                // along with the error message
                return (StatusCode(StatusCodes.Status500InternalServerError, new ReturnObject
                {
                    status = false,
                    message = ex.Message
                }));
            }
        }
        #endregion

        #region  Savings
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("GetAllSavingStattus")]
        public IActionResult GetAllSavingStattus()
        {

            var r = new ReturnObject();
            var eget = true;
            try
            {
                var fneRes = _context.SavingsStatus.Select(fd => new DropDownDetail { Id = fd.Id, Name = fd.StatusName }).ToList();
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = fneRes.ToList();
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

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getallSavings")]
        public IActionResult GetAllSavings([FromQuery] CustomerCentricFilter obj)
        {
            IEnumerable<CustomerCentricResponse> neRes = null;
            List<Customer> allSav = null;
            var r = new ReturnObject();
            var eget = false;
            try
            {
                var lstOfCusId = _repo.AsQueryable().Select(o => new { o.CustomerId, o.IsActive }).ToList();
                switch (obj.Det)
                {
                    case 1:
                        allSav = _context.Customers.Where(o => lstOfCusId.Select(o => o.CustomerId).Contains(o.Id)
                        //allSav = _context.Customers.Where(o => lstOfCusId.Select(o => o.CustomerId).Contains(o.Id)
                        && o.DateCreated > obj.StartDate
                        && o.DateCreated < obj.EndDate.AddDays(1)).ToList();
                        foreach (var item in allSav)
                        {
                            item.CustomerCentricStatus = lstOfCusId?.FirstOrDefault(o => o.CustomerId == item.Id)?.IsActive == true ? "Active" : "InActive";
                        }
                        if (obj.Status != 0)
                            allSav = allSav.Where(o => o.Status.GetValueOrDefault() == obj.Status).ToList();
                        break;
                    case 2:
                        if (string.IsNullOrWhiteSpace(obj.SearchName))
                        {
                            allSav = _context.Customers.Where(o => lstOfCusId.Select(o => o.CustomerId).Contains(o.Id)).ToList();
                            foreach (var item in allSav)
                            {
                                item.CustomerCentricStatus = lstOfCusId?.FirstOrDefault(o => o.CustomerId == item.Id)?.IsActive == true ? "Active" : "InActive";
                            }
                        }
                        else
                        {
                            switch (obj.SearchType.ToLower())
                            {
                                case "email":
                                    allSav = _context.Customers.Where(o => lstOfCusId.Select(o => o.CustomerId).Contains(o.Id) &&
                                    o.EmailAddress.ToLower().Contains(obj.SearchName.ToLower())
                                    ).ToList();
                                    foreach (var item in allSav)
                                    {
                                        item.CustomerCentricStatus = lstOfCusId?.FirstOrDefault(o => o.CustomerId == item.Id)?.IsActive == true ? "Active" : "InActive";
                                    }
                                    break;
                                case "phone":
                                    allSav = _context.Customers.Where(o => lstOfCusId.Select(o => o.CustomerId).Contains(o.Id) &&
                                    o.PhoneNumber.ToLower().Contains(obj.SearchName.ToLower())
                                    ).ToList();
                                    foreach (var item in allSav)
                                    {
                                        item.CustomerCentricStatus = lstOfCusId?.FirstOrDefault(o => o.CustomerId == item.Id)?.IsActive == true ? "Active" : "InActive";
                                    }
                                    break;
                                case "name":
                                    allSav = _context.Customers.Where(o => lstOfCusId.Select(o => o.CustomerId).Contains(o.Id) &&
                                                 o.FirstName.ToLower().Contains(obj.SearchName.ToLower())
                                              || o.LastName.ToLower().Contains(obj.SearchName.ToLower())
                                              || o.MiddleName.ToLower().Contains(obj.SearchName.ToLower())).ToList();
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    default:
                        break;
                };


                var fneRes = allSav.Skip((obj.PageNumber - 1) * obj.PasgeSize)
                     .Take(obj.PasgeSize);
                if (allSav.Any())
                    eget = true;
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = fneRes.ToList();
                r.recordCount = allSav.Count();
                r.recordPageNumber = obj.PageNumber;
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
        [Route("getallSavingsForFilter")]
        public IActionResult GetAllSavingsForFilter([FromBody] FilterModel obj)
        {
            var r = new ReturnObject();
            var eget = false;
            try
            {
                string searchString = obj.SearchBy.ToLower().Trim();
                //different db
                var allSav = _context.Customers.Where(p => p.EmailAddress.ToLower().Trim() == searchString || p.PhoneNumber.ToLower().Trim() == searchString || p.FirstName.ToLower().Trim() == searchString).ToList();
                foreach (var item in allSav)
                {
                    var lstOfCusId = _repo.AsQueryable().Where(o => o.CustomerId == item.Id).ToList();
                    foreach (var i in lstOfCusId)
                        item.CustomerCentricStatus = lstOfCusId?.FirstOrDefault(o => o.CustomerId == item.Id)?.IsActive == true ? "Active" : "InActive";
                }
                if (allSav.Any())
                    eget = true;
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = eget ? allSav : "";
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

        // Method for getting all savings by customer ID
        // Returns a list of savings or an error message with status code
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("GetSavingsByCusId/{cusId}")]
        public IActionResult GetSavingsByCustomerId(int cusId)
        {
            var r = new ReturnObject();
            var eget = false;

            try
            {
                var lstOfCusId = _repo.AsQueryable()
                       .Where(o => o.CustomerId == cusId)
                       .ToList();
                var allSav = _context.Customers.FirstOrDefault(p => p.Id == cusId);
                var res = new CustomerCentricResponseForView();
                if (lstOfCusId.Any())
                {
                    eget = true;
                    res.PhoneNumber = allSav.PhoneNumber;
                    res.EmailAddress = allSav.EmailAddress;
                    res.FullName = $"{allSav.FirstName} {allSav.MiddleName} {allSav.LastName}";
                    res.DateOfBirth = allSav.DateOfBirth;
                    res.Bvn = allSav.Bvn;
                    res.ListItem = lstOfCusId;
                }
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = eget ? res : "";
                return (Ok(r));

            }
            catch (Exception ex)
            {
                // If an error occurs, return a 500 Internal Server Error status code
                // along with the error message
                return (StatusCode(StatusCodes.Status500InternalServerError, new ReturnObject
                {
                    status = false,
                    message = ex.Message
                }));
            }
        }
        #endregion

        #region  billspayment
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("GetAllbillspaymentsStattus")]
        public IActionResult GetAllbillspaymentsStattus()
        {

            var r = new ReturnObject();
            var eget = true;
            try
            {
                var fneRes = _context.BillsPaymentStatus.Select(fd => new DropDownDetail { Id = fd.Id, Name = fd.StatusName }).ToList();
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = fneRes.ToList();
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


        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getallbillspayments")]
        public IActionResult GetAllbillspayments([FromQuery] CustomerCentricFilter obj)
        {
            IEnumerable<CustomerCentricResponse> neRes = null;
            List<Customer> allSav = null;
            var r = new ReturnObject();
            var eget = false;
            try
            {
                var lstOfCusId = _repoPayment.AsQueryable().Select(o => new { o.CustomerId, o.Status }).ToList();
                switch (obj.Det)
                {

                    case 1:
                        allSav = _context.Customers.Where(o => lstOfCusId.Select(o => o.CustomerId).Contains(o.Id)
                        && o.DateCreated > obj.StartDate
                        && o.DateCreated < obj.EndDate.AddDays(1)).ToList();

                        foreach (var item in allSav)
                        {

                            item.CustomerCentricStatus = lstOfCusId.FirstOrDefault(o => o.CustomerId == item.Id).Status;
                        }
                        if (obj.Status != 0)
                            allSav = allSav.Where(o => o.Status.GetValueOrDefault() == obj.Status).ToList();
                        break;
                    case 2:
                        if (string.IsNullOrWhiteSpace(obj.SearchName))
                        {
                            allSav = _context.Customers.Where(o => lstOfCusId.Select(o => o.CustomerId).Contains(o.Id)).ToList();
                            foreach (var item in allSav)
                            {
                                item.CustomerCentricStatus = lstOfCusId.FirstOrDefault(o => o.CustomerId == item.Id).Status;
                            }
                        }
                        else
                        {
                            switch (obj.SearchType.ToLower())
                            {
                                case "email":
                                    allSav = _context.Customers.Where(o => lstOfCusId.Select(o => o.CustomerId).Contains(o.Id) &&
                                    o.EmailAddress.ToLower().Contains(obj.SearchName.ToLower())
                                    ).ToList();
                                    foreach (var item in allSav)
                                    {
                                        item.CustomerCentricStatus = lstOfCusId?.FirstOrDefault(o => o.CustomerId == item.Id)?.Status;
                                    }
                                    break;
                                case "phone":
                                    allSav = _context.Customers.Where(o => lstOfCusId.Select(o => o.CustomerId).Contains(o.Id) &&
                                    o.PhoneNumber.ToLower().Contains(obj.SearchName.ToLower())
                                    ).ToList();
                                    foreach (var item in allSav)
                                    {
                                        item.CustomerCentricStatus = lstOfCusId?.FirstOrDefault(o => o.CustomerId == item.Id)?.Status;
                                    }
                                    break;
                                case "name":
                                    allSav = _context.Customers.Where(o => lstOfCusId.Select(o => o.CustomerId).Contains(o.Id) &&
                                                 o.FirstName.ToLower().Contains(obj.SearchName.ToLower())
                                              || o.LastName.ToLower().Contains(obj.SearchName.ToLower())
                                              || o.MiddleName.ToLower().Contains(obj.SearchName.ToLower())).ToList();
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    default:
                        break;
                }
                var fneRes = allSav.Skip((obj.PageNumber - 1) * obj.PasgeSize)
                         .Take(obj.PasgeSize);
                if (allSav.Any())
                    eget = true;
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = fneRes.ToList();
                r.recordCount = allSav.Count();
                r.recordPageNumber = obj.PageNumber;
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
        [Route("getallbillspaymentsForFilter")]
        public IActionResult GetAllbillspaymentForFilter([FromBody] FilterModel obj)
        {
            var r = new ReturnObject();
            var eget = false;
            try
            {
                string searchString = obj.SearchBy.ToLower().Trim();
                var lstOfCusId = _repoPayment.AsQueryable().Select(o => new { o.CustomerId, o.Status }).ToList();
                var allSav = _context.Customers.Where(p => p.EmailAddress.ToLower().Trim() == searchString ||
                                                           p.PhoneNumber.ToLower().Trim() == searchString ||
                                                           p.FirstName.ToLower().Trim() == searchString &&
                                                           lstOfCusId.Select(o => o.CustomerId).Contains(p.Id)
                                                           ).ToList();
                foreach (var item in allSav)
                {
                    item.CustomerCentricStatus = lstOfCusId.FirstOrDefault(o => o.CustomerId == item.Id).Status;
                }
                if (allSav.Any())
                    eget = true;
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = eget ? allSav : "";
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
        [HttpPut]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("ModifybillsPaymentsStatusWithTransactionReference")]
        public IActionResult Modifybillspaymentsstatus([FromBody] FilterUpdateModel obj)
        {
            var r = new ReturnObject();
            var eget = false;
            try
            {
                var allSav = _repoPayment.AsQueryable().FirstOrDefault(o => o.TransactionReference.Trim().ToLower() == obj.EntityId.Trim().ToLower());

                if (allSav is not null)
                {
                    eget = true;
                    allSav.Status = obj.Status;
                    _repoPayment.ReplaceOne(allSav);
                }
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = eget ? allSav : "";
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

        // Method for getting all savings by customer ID
        // Returns a list of savings or an error message with status code
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("GetbillspaymentByCusId/{cusId}")]
        public IActionResult GetbillspaymentByCustomerId(int cusId)
        {
            var r = new ReturnObject();
            var eget = false;
            try
            {
                var lstOfCusId = _repoPayment.AsQueryable().Where(o => o.CustomerId == cusId).ToList();
                var allSav = _context.Customers.FirstOrDefault(p => p.Id == cusId);
                var res = new CustomerCentricResponseForView();
                if (lstOfCusId.Any())
                {
                    eget = true;
                    res.PhoneNumber = allSav.PhoneNumber;
                    res.EmailAddress = allSav.EmailAddress;
                    res.FullName = $"{allSav.FirstName} {allSav.MiddleName} {allSav.LastName}";
                    res.DateOfBirth = allSav.DateOfBirth;
                    res.Bvn = allSav.Bvn;
                    res.ListItem = lstOfCusId;
                }
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = eget ? res : "";
                return (Ok(r));

            }
            catch (Exception ex)
            {
                // If an error occurs, return a 500 Internal Server Error status code
                // along with the error message
                return (StatusCode(StatusCodes.Status500InternalServerError, new ReturnObject
                {
                    status = false,
                    message = ex.Message
                }));
            }
        }
        #endregion

        #region  loanRepayment
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getallloanRepayment")]
        public IActionResult GetAllLoanRepayment([FromQuery] CustomerCentricFilter obj)
        {
            IEnumerable<CustomerCentricResponse> neRes = null;
            List<Customer> allSav = null;
            var r = new ReturnObject();
            var eget = false;
            try
            {
                var lstOfCusId = _repoRegularLoanRepaymentPlan.AsQueryable().Select(o => new { o.CustomerId,o.StatusName}).ToList();

                switch (obj.Det)
                {
                    case 1:
                        allSav = _context.Customers.Where(o => lstOfCusId.Select(o => o.CustomerId).Contains(o.Id)
                             && o.DateCreated > obj.StartDate
                        && o.DateCreated < obj.EndDate.AddDays(1)).ToList();
                        foreach (var item in allSav)
                        {
                            item.CustomerCentricStatus = lstOfCusId.FirstOrDefault(o => o.CustomerId == item.Id).StatusName;
                        }
                        if (obj.Status != 0)
                            allSav = allSav.Where(o => o.Status.GetValueOrDefault() == obj.Status).ToList();
                        break;
                    case 2:
                        if (string.IsNullOrWhiteSpace(obj.SearchName))
                        {
                            allSav = _context.Customers.Where(o => lstOfCusId.Select(o => o.CustomerId).Contains(o.Id)).ToList();
                            foreach (var item in allSav)
                            {
                                item.CustomerCentricStatus = lstOfCusId.FirstOrDefault(o => o.CustomerId == item.Id).StatusName;
                            }
                        }
                        else
                        {
                            switch (obj.SearchType.ToLower())
                            {
                                case "email":
                                    allSav = _context.Customers.Where(o => lstOfCusId.Select(o => o.CustomerId).Contains(o.Id) &&
                                    o.EmailAddress.ToLower().Contains(obj.SearchName.ToLower())
                                    ).ToList();
                                    foreach (var item in allSav)
                                    {
                                        item.CustomerCentricStatus = lstOfCusId?.FirstOrDefault(o => o.CustomerId == item.Id)?.StatusName;
                                    }
                                    break;
                                case "phone":
                                    allSav = _context.Customers.Where(o => lstOfCusId.Select(o => o.CustomerId).Contains(o.Id) &&
                                    o.PhoneNumber.ToLower().Contains(obj.SearchName.ToLower())
                                    ).ToList();
                                    foreach (var item in allSav)
                                    {
                                        item.CustomerCentricStatus = lstOfCusId?.FirstOrDefault(o => o.CustomerId == item.Id)?.StatusName;
                                    }
                                    break;
                                case "name":
                                    allSav = _context.Customers.Where(o => lstOfCusId.Select(o => o.CustomerId).Contains(o.Id) &&
                                                 o.FirstName.ToLower().Contains(obj.SearchName.ToLower())
                                              || o.LastName.ToLower().Contains(obj.SearchName.ToLower())
                                              || o.MiddleName.ToLower().Contains(obj.SearchName.ToLower())).ToList();
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    default:
                        break;
                }

                var fneRes = allSav.Skip((obj.PageNumber - 1) * obj.PasgeSize)
                    .Take(obj.PasgeSize);
                if (allSav.Any())
                    eget = true;
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = fneRes.ToList();
                r.recordCount = allSav.Count();
                r.recordPageNumber = obj.PageNumber;
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

        // Method for getting all savings by customer ID
        // Returns a list of savings or an error message with status code
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("GetloanRepaymentByCusId/{cusId}")]
        public IActionResult GetloanRepaymentByCustomerId(int cusId)
        {
            var r = new ReturnObject();
            var eget = false;
            try
            {
                var lstOfCusId = _repoLD.AsQueryable().Where(o => o.CustomerId == cusId).ToList();
                var allSav = _context.Customers.FirstOrDefault(p => p.Id == cusId);
                var res = new CustomerCentricResponseForView();
                if (lstOfCusId.Any())
                {
                    eget = true;
                    res.PhoneNumber = allSav.PhoneNumber;
                    res.EmailAddress = allSav.EmailAddress;
                    res.FullName = $"{allSav.FirstName} {allSav.MiddleName} {allSav.LastName}";
                    res.DateOfBirth = allSav.DateOfBirth;
                    res.Bvn = allSav.Bvn;
                    res.ListItem = lstOfCusId;
                }
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = eget ? res : "";
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
        #endregion

        #region  transfer
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getalltransfers")]
        public IActionResult GetAlltransfers([FromQuery] CustomerCentricFilter obj)
        {
            IEnumerable<CustomerCentricResponse> neRes = null;
            List<Customer> allSav = null;
            var r = new ReturnObject();
            var eget = false;
            try
            {
                var lstOfCusId = _repoTransfer.AsQueryable().Select(o => new { o.PayerCustomerId, o.StatusName }).ToList();

                switch (obj.Det)
                {

                    case 1:

                        allSav = _context.Customers.Where(o => lstOfCusId.Select(o => o.PayerCustomerId).Contains(o.Id)
                             && o.DateCreated > obj.StartDate
                        && o.DateCreated < obj.EndDate.AddDays(1)).ToList();
                        foreach (var item in allSav)
                        {

                            item.CustomerCentricStatus = lstOfCusId.FirstOrDefault(o => o.PayerCustomerId == item.Id).StatusName;
                        }
                        if (obj.Status != 0)
                            allSav = allSav.Where(o => o.Status.GetValueOrDefault() == obj.Status).ToList();
                        break;
                    case 2:
                        if (string.IsNullOrWhiteSpace(obj.SearchName))
                        {
                            allSav = _context.Customers.Where(o => lstOfCusId.Select(o => o.PayerCustomerId).Contains(o.Id)).ToList();
                            foreach (var item in allSav)
                            {
                                item.CustomerCentricStatus = lstOfCusId.FirstOrDefault(o => o.PayerCustomerId == item.Id).StatusName;
                            }
                        }
                        else
                        {
                            switch (obj.SearchType.ToLower())
                            {
                                case "email":
                                    allSav = _context.Customers.Where(o => lstOfCusId.Select(o => o.PayerCustomerId).Contains(o.Id) &&
                                    o.EmailAddress.ToLower().Contains(obj.SearchName.ToLower())
                                    ).ToList();
                                    foreach (var item in allSav)
                                    {
                                        item.CustomerCentricStatus = lstOfCusId?.FirstOrDefault(o => o.PayerCustomerId == item.Id)?.StatusName;
                                    }
                                    break;
                                case "phone":
                                    allSav = _context.Customers.Where(o => lstOfCusId.Select(o => o.PayerCustomerId).Contains(o.Id) &&
                                    o.PhoneNumber.ToLower().Contains(obj.SearchName.ToLower())
                                    ).ToList();
                                    foreach (var item in allSav)
                                    {
                                        item.CustomerCentricStatus = lstOfCusId?.FirstOrDefault(o => o.PayerCustomerId == item.Id)?.StatusName;
                                    }
                                    break;
                                case "name":
                                    allSav = _context.Customers.Where(o => lstOfCusId.Select(o => o.PayerCustomerId).Contains(o.Id) &&
                                                 o.FirstName.ToLower().Contains(obj.SearchName.ToLower())
                                              || o.LastName.ToLower().Contains(obj.SearchName.ToLower())
                                              || o.MiddleName.ToLower().Contains(obj.SearchName.ToLower())).ToList();
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    default:
                        break;
                }

                var fneRes = allSav.Skip((obj.PageNumber - 1) * obj.PasgeSize)
                    .Take(obj.PasgeSize);
                if (allSav.Any())
                    eget = true;
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = fneRes.ToList();
                r.recordCount = allSav.Count();
                r.recordPageNumber = obj.PageNumber;
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

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("GetAlltransfersStattus")]
        public IActionResult GetAlltransfersStattus()
        {

            var r = new ReturnObject();
            var eget = true;
            try
            {
                var fneRes = _repoTransferStatus.AsQueryable().Select(fd => new DropDownDetail { TransactionId = fd.TransactionId, Name = fd.Status }).ToList();
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = fneRes.ToList();
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
        [Route("getalltransfersForFilter")]
        public IActionResult GetAlltransferForFilter([FromBody] FilterModel obj)
        {
            var r = new ReturnObject();
            var eget = false;
            try
            {
                string searchString = obj.SearchBy.ToLower().Trim();
                var lstOfCusId = _repoTransfer.AsQueryable().Select(o => new { o.PayerCustomerId, o.StatusName }).ToList();
                var allSav = _context.Customers.Where(p => p.EmailAddress.ToLower().Trim() == searchString ||
                                                           p.PhoneNumber.ToLower().Trim() == searchString ||
                                                           p.FirstName.ToLower().Trim() == searchString &&
                                                           lstOfCusId.Select(o => o.PayerCustomerId).Contains(p.Id)
                                                           ).ToList();
                foreach (var item in allSav)
                {
                    item.CustomerCentricStatus = lstOfCusId.FirstOrDefault(o => o.PayerCustomerId == item.Id)?.StatusName;
                }
                if (allSav.Any())
                    eget = true;
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = eget ? allSav : "";
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
        [HttpPut]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("ModifytransferStatusWithTransactionReference")]
        public IActionResult Modifytransfersstatus([FromBody] FilterUpdateModelWithDesciption obj)
        {
            var r = new ReturnObject();
            var eget = false;
            try
            {
                var allSav = _repoTransfer.AsQueryable().FirstOrDefault(o => o.TransactionReference.Trim().ToLower() == obj.EntityId.Trim().ToLower());

                if (allSav is not null)
                {
                    var c = new CustomerCentricWalleToBankTransferStatus();
                    eget = true;
                    allSav.StatusName = obj.Status;
                    _repoTransfer.ReplaceOne(allSav);

                    // c.Status = obj.Status;
                    // c.TransactionId = allSav.TransactionId;
                    // c.Description = obj.Description;

                    // _repoTransferStatus.InsertOneAsync(c);

                }
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = eget ? allSav : "";
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

        // Method for getting all savings by customer ID
        // Returns a list of savings or an error message with status code
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("GettransferByCusId/{cusId}")]
        public IActionResult GettransferByCustomerId(int cusId)
        {
            var r = new ReturnObject();
            var eget = false;
            try
            {
                var lstOfCusId = _repoTransfer.AsQueryable().Where(o => o.PayerCustomerId == cusId).ToList();
                var allSav = _context.Customers.FirstOrDefault(p => p.Id == cusId);
                var res = new CustomerCentricResponseForView();
                if (lstOfCusId.Any())
                {
                    eget = true;
                    res.PhoneNumber = allSav.PhoneNumber;
                    res.EmailAddress = allSav.EmailAddress;
                    res.FullName = $"{allSav.FirstName} {allSav.MiddleName} {allSav.LastName}";
                    res.DateOfBirth = allSav.DateOfBirth;
                    res.Bvn = allSav.Bvn;
                    res.ListItem = lstOfCusId;
                }
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = eget ? res : "";
                return (Ok(r));
            }
            catch (Exception ex)
            {
                // If an error occurs, return a 500 Internal Server Error status code
                // along with the error message
                return (StatusCode(StatusCodes.Status500InternalServerError, new ReturnObject
                {
                    status = false,
                    message = ex.Message
                }));
            }
        }
        #endregion

        #region  airtime
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getallairtimes")]
        public IActionResult GetAllairtimes([FromQuery] CustomerCentricFilter obj)
        {
            IEnumerable<CustomerCentricResponse> neRes = null;
            List<Customer> allSav = null;
            var r = new ReturnObject();
            var eget = false;
            try
            {
                var lstOfCusId = _repoAirtime.AsQueryable().Select(o => new { o.CustomerId, o.Status }).ToList();

                switch (obj.Det)
                {
                    case 1:
                        allSav = _context.Customers.Where(o => lstOfCusId.Select(o => o.CustomerId).Contains(o.Id)
                             && o.DateCreated > obj.StartDate
                        && o.DateCreated < obj.EndDate.AddDays(1)).ToList();
                        foreach (var item in allSav)
                        {

                            item.CustomerCentricStatus = lstOfCusId.FirstOrDefault(o => o.CustomerId == item.Id).Status;
                        }
                        if (obj.Status != 0)
                            allSav = allSav.Where(o => o.Status.GetValueOrDefault() == obj.Status).ToList();
                        break;
                    case 2:
                        if (string.IsNullOrWhiteSpace(obj.SearchName))
                        {
                            allSav = _context.Customers.Where(o => lstOfCusId.Select(o => Convert.ToInt64(o.CustomerId)).Contains(o.Id)).ToList();
                            foreach (var item in allSav)
                            {
                                item.CustomerCentricStatus = lstOfCusId.FirstOrDefault(o => o.CustomerId == item.Id).Status;
                            }
                        }
                        else
                        {
                            switch (obj.SearchType.ToLower())
                            {
                                case "email":
                                    allSav = _context.Customers.Where(o => lstOfCusId.Select(o => Convert.ToInt64(o.CustomerId)).Contains(o.Id) &&
                                    o.EmailAddress.ToLower().Contains(obj.SearchName.ToLower())
                                    ).ToList();
                                    foreach (var item in allSav)
                                    {
                                        item.CustomerCentricStatus = lstOfCusId?.FirstOrDefault(o => Convert.ToInt64(o.CustomerId) == item.Id)?.Status;
                                    }
                                    break;
                                case "phone":
                                    allSav = _context.Customers.Where(o => lstOfCusId.Select(o => Convert.ToInt64(o.CustomerId)).Contains(o.Id) &&
                                    o.PhoneNumber.ToLower().Contains(obj.SearchName.ToLower())
                                    ).ToList();
                                    foreach (var item in allSav)
                                    {
                                        item.CustomerCentricStatus = lstOfCusId?.FirstOrDefault(o => Convert.ToInt64(o.CustomerId) == item.Id)?.Status;
                                    }
                                    break;
                                case "name":
                                    allSav = _context.Customers.Where(o => lstOfCusId.Select(o => Convert.ToInt64(o.CustomerId)).Contains(o.Id) &&
                                                 o.FirstName.ToLower().Contains(obj.SearchName.ToLower())
                                              || o.LastName.ToLower().Contains(obj.SearchName.ToLower())
                                              || o.MiddleName.ToLower().Contains(obj.SearchName.ToLower())).ToList();
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    default:
                        break;
                }

                var fneRes = allSav.Skip((obj.PageNumber - 1) * obj.PasgeSize)
                    .Take(obj.PasgeSize);
                if (allSav.Any())
                    eget = true;
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = fneRes.ToList();
                r.recordCount = allSav.Count();
                r.recordPageNumber = obj.PageNumber;
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
        [Route("getallairtimesForFilter")]
        public IActionResult GetAllairtimeForFilter([FromBody] FilterModel obj)
        {
            var r = new ReturnObject();
            var eget = false;
            try
            {
                string searchString = obj.SearchBy.ToLower().Trim();
                var lstOfCusId = _repoAirtime.AsQueryable().Select(o => new { o.CustomerId, o.Status }).ToList();
                var allSav = _context.Customers.Where(p => p.EmailAddress.ToLower().Trim() == searchString ||
                                                           p.PhoneNumber.ToLower().Trim() == searchString ||
                                                           p.FirstName.ToLower().Trim() == searchString &&
                                                           lstOfCusId.Select(o => Convert.ToInt64(o.CustomerId)).Contains(p.Id)
                                                           ).ToList();
                foreach (var item in allSav)
                {
                    item.CustomerCentricStatus = lstOfCusId.FirstOrDefault(o => o.CustomerId == item.Id).Status;
                }
                if (allSav.Any())
                    eget = true;
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = eget ? allSav : "";
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
        [HttpPut]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("ModifyairtimePaymentsStatusWithTransactionReference")]
        public IActionResult Modifyairtimesstatus([FromBody] FilterUpdateModel obj)
        {
            var r = new ReturnObject();
            var eget = false;
            try
            {
                var allSav = _repoAirtime.AsQueryable().FirstOrDefault(o => o.TransactionReference.Trim().ToLower() == obj.EntityId.Trim().ToLower());

                if (allSav is not null)
                {
                    allSav.Status = obj.Status;
                    _repoAirtime.ReplaceOne(allSav);
                    eget = true;
                }
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = eget ? allSav : "";
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

        // Method for getting all savings by customer ID
        // Returns a list of savings or an error message with status code
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("GetairtimeByCusId/{cusId}")]
        public IActionResult GetairtimeByCustomerId(int cusId)
        {
            var r = new ReturnObject();
            var eget = false;
            try
            {
                var lstOfCusId = _repoAirtime.AsQueryable().Where(o => o.CustomerId == cusId).ToList();
                var allSav = _context.Customers.FirstOrDefault(p => p.Id == cusId);
                var res = new CustomerCentricResponseForView();
                if (lstOfCusId.Any())
                {
                    eget = true;
                    res.PhoneNumber = allSav.PhoneNumber;
                    res.EmailAddress = allSav.EmailAddress;
                    res.FullName = $"{allSav.FirstName} {allSav.MiddleName} {allSav.LastName}";
                    res.DateOfBirth = allSav.DateOfBirth;
                    res.Bvn = allSav.Bvn;
                    res.ListItem = lstOfCusId;
                }
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = eget ? res : "";
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
        #endregion

        #region  P2p
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getallp2ps")]
        public IActionResult GetAllp2ps([FromQuery] CustomerCentricFilter obj)
        {
            IEnumerable<CustomerCentricResponse> neRes = null;
            List<Customer> allSav = null;
            Customer forLender = null;
            Customer forBorrower = null;
            List<CustomerCentricP2pResponse> allLender = new();
            var r = new ReturnObject();
            var eget = false;
            try
            {
                allSav = _context.Customers.ToList();
                var lstOfCusId = _repoP2p.AsQueryable();
                switch (obj.Det)
                {
                    case 1:
                        lstOfCusId = lstOfCusId.Where(o => o.RequestDate > Convert.ToDateTime(obj.StartDate)
                        && o.RequestDate < Convert.ToDateTime(obj.EndDate.AddDays(1)));
                        foreach (var item in lstOfCusId)
                        {
                            forLender = allSav.FirstOrDefault(o => o.Id == item.LenderCustomerId);
                            forBorrower = allSav.FirstOrDefault(o => o.Id == item.BorrowerCustomerId);
                            var k = new CustomerCentricP2pResponse()
                            {
                                P2PLoanRequestId = item.P2PLoanRequestId,
                                Amount = item.Amount,
                                BorrowerName = forBorrower != null ? $"{forBorrower.FirstName} {forBorrower.LastName}" : null,
                                LenderName = forLender != null ? $"{forLender.FirstName} {forLender.LastName}" : null,
                                BorrowerEmailAddress = forBorrower != null ? forBorrower.EmailAddress : null,
                                LenderEmailAddress = forLender != null ? forLender.EmailAddress : null,
                                LenderPhoneNumber = forLender != null ? forLender.PhoneNumber : null,
                                BorrowerPhoneNumber = forBorrower != null ? forBorrower.PhoneNumber : null,
                                Tenor = item.LoanTenor.ToString(),
                                StartDate = item.StartDate.ToString(),
                                EndDate = item.EndDate.ToString(),
                                Status = item.ApprovalStatus.ToString()
                            };
                            allLender.Add(k);
                        }
                        if (obj.Status != 0)
                            allLender = allLender.Where(o => o.Status == obj.Status.ToString()).ToList();
                        break;
                    case 2:
                        if (string.IsNullOrWhiteSpace(obj.SearchName))
                        {
                            allSav = _context.Customers.ToList();
                            foreach (var item in lstOfCusId)
                            {
                                forLender = allSav.FirstOrDefault(o => o.Id == item.LenderCustomerId);
                                forBorrower = allSav.FirstOrDefault(o => o.Id == item.BorrowerCustomerId);
                                var k = new CustomerCentricP2pResponse()
                                {
                                    P2PLoanRequestId = item.P2PLoanRequestId,
                                    Amount = item.Amount,
                                    BorrowerName = forBorrower != null ? $"{forBorrower.FirstName} {forBorrower.LastName}" : null,
                                    LenderName = forLender != null ? $"{forLender.FirstName} {forLender.LastName}" : null,
                                    BorrowerEmailAddress = forBorrower != null ? forBorrower.EmailAddress : null,
                                    LenderEmailAddress = forLender != null ? forLender.EmailAddress : null,
                                    LenderPhoneNumber = forLender != null ? forLender.PhoneNumber : null,
                                    BorrowerPhoneNumber = forBorrower != null ? forBorrower.PhoneNumber : null,
                                    Tenor = item.LoanTenor.ToString(),
                                    StartDate = item.StartDate.ToString(),
                                    EndDate = item.EndDate.ToString(),
                                    Status = item.ApprovalStatus.ToString()
                                };
                                allLender.Add(k);
                            }
                        }
                        else
                        {
                            switch (obj.SearchType.ToLower())
                            {
                                case "email":
                                    allSav = _context.Customers.Where(o => o.EmailAddress.ToLower().Contains(obj.SearchName.ToLower())).ToList();
                                    foreach (var item in lstOfCusId)
                                    {
                                        forLender = allSav.FirstOrDefault(o => o.Id == item.LenderCustomerId);
                                        forBorrower = allSav.FirstOrDefault(o => o.Id == item.BorrowerCustomerId);
                                        var k = new CustomerCentricP2pResponse()
                                        {
                                            P2PLoanRequestId = item.P2PLoanRequestId,
                                            Amount = item.Amount,
                                            BorrowerName = forBorrower != null ? $"{forBorrower.FirstName} {forBorrower.LastName}" : null,
                                            LenderName = forLender != null ? $"{forLender.FirstName} {forLender.LastName}" : null,
                                            BorrowerEmailAddress = forBorrower != null ? forBorrower.EmailAddress : null,
                                            LenderEmailAddress = forLender != null ? forLender.EmailAddress : null,
                                            LenderPhoneNumber = forLender != null ? forLender.PhoneNumber : null,
                                            BorrowerPhoneNumber = forBorrower != null ? forBorrower.PhoneNumber : null,
                                            Tenor = item.LoanTenor.ToString(),
                                            StartDate = item.StartDate.ToString(),
                                            EndDate = item.EndDate.ToString(),
                                            Status = item.ApprovalStatus.ToString()
                                        };
                                        allLender.Add(k);
                                    }
                                    break;
                                case "phone":
                                    allSav = _context.Customers.Where(o => lstOfCusId.Select(o => Convert.ToInt64(o.LenderCustomerId)).Contains(o.Id) &&
                                    o.PhoneNumber.ToLower().Contains(obj.SearchName.ToLower())
                                    ).ToList();
                                    foreach (var item in lstOfCusId)
                                    {
                                        forLender = allSav.FirstOrDefault(o => o.Id == item.LenderCustomerId);
                                        forBorrower = allSav.FirstOrDefault(o => o.Id == item.BorrowerCustomerId);
                                        var k = new CustomerCentricP2pResponse()
                                        {
                                            P2PLoanRequestId = item.P2PLoanRequestId,
                                            Amount = item.Amount,
                                            BorrowerName = forBorrower != null ? $"{forBorrower.FirstName} {forBorrower.LastName}" : null,
                                            LenderName = forLender != null ? $"{forLender.FirstName} {forLender.LastName}" : null,
                                            BorrowerEmailAddress = forBorrower != null ? forBorrower.EmailAddress : null,
                                            LenderEmailAddress = forLender != null ? forLender.EmailAddress : null,
                                            LenderPhoneNumber = forLender != null ? forLender.PhoneNumber : null,
                                            BorrowerPhoneNumber = forBorrower != null ? forBorrower.PhoneNumber : null,
                                            Tenor = item.LoanTenor.ToString(),
                                            StartDate = item.StartDate.ToString(),
                                            EndDate = item.EndDate.ToString(),
                                            Status = item.ApprovalStatus.ToString()
                                        };
                                        allLender.Add(k);
                                    }
                                    break;
                                case "name":
                                    allSav = _context.Customers.Where(o => lstOfCusId.Select(o => Convert.ToInt64(o.LenderCustomerId)).Contains(o.Id)
                                              && o.FirstName.ToLower().Contains(obj.SearchName.ToLower())
                                              || o.LastName.ToLower().Contains(obj.SearchName.ToLower())
                                              || o.MiddleName.ToLower().Contains(obj.SearchName.ToLower())).ToList();
                                    foreach (var item in lstOfCusId)
                                    {
                                        forLender = allSav.FirstOrDefault(o => o.Id == item.LenderCustomerId);
                                        forBorrower = allSav.FirstOrDefault(o => o.Id == item.BorrowerCustomerId);
                                        var k = new CustomerCentricP2pResponse()
                                        {
                                            P2PLoanRequestId = item.P2PLoanRequestId,
                                            Amount = item.Amount,
                                            BorrowerName = forBorrower != null ? $"{forBorrower.FirstName} {forBorrower.LastName}" : null,
                                            LenderName = forLender != null ? $"{forLender.FirstName} {forLender.LastName}" : null,
                                            BorrowerEmailAddress = forBorrower != null ? forBorrower.EmailAddress : null,
                                            LenderEmailAddress = forLender != null ? forLender.EmailAddress : null,
                                            LenderPhoneNumber = forLender != null ? forLender.PhoneNumber : null,
                                            BorrowerPhoneNumber = forBorrower != null ? forBorrower.PhoneNumber : null,
                                            Tenor = item.LoanTenor.ToString(),
                                            StartDate = item.StartDate.ToString(),
                                            EndDate = item.EndDate.ToString(),
                                            Status = item.ApprovalStatus.ToString()
                                        };
                                        allLender.Add(k);
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    default:
                        break;

                }
                var fneRes = allLender.Skip((obj.PageNumber - 1) * obj.PasgeSize)
                      .Take(obj.PasgeSize);
                if (allLender.Any())
                    eget = true;
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = fneRes.ToList();
                r.recordCount = allLender.Count();
                r.recordPageNumber = obj.PageNumber;
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

        // Method for getting all savings by customer ID
        // Returns a list of savings or an error message with status code
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("Getp2pByP2PLoanRequestId/{P2PLoanRequestId}")]
        public IActionResult Getp2pByCustomerId(string P2PLoanRequestId)
        {
            Customer? forLender = null;
            Customer? forBorrower = null;
            var r = new ReturnObject();
            var eget = false;
            try
            {
                var lstRe = _repoP2pRepay.AsQueryable().Where(o => o.P2PLoanRequestId == P2PLoanRequestId);
                var lstOfCusId = _repoP2p.AsQueryable().FirstOrDefault(o => o.P2PLoanRequestId == P2PLoanRequestId);
                var res = new CustomerCentricResponseForView();
                if (lstOfCusId is not null)
                {
                    forBorrower = _context.Customers.FirstOrDefault(p => p.Id == lstOfCusId.BorrowerCustomerId);
                    forLender = _context.Customers.FirstOrDefault(p => p.Id == lstOfCusId.LenderCustomerId);
                    eget = true;
                }
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = eget ? new
                {
                    LenderDetails = forLender,
                    BorrowerDetails = forBorrower,
                    Tenor = lstOfCusId.LoanTenor,
                    StartDate = lstOfCusId.StartDate,
                    EndDate = lstOfCusId.EndDate,
                    p2pDateCreated = lstOfCusId.RequestDate,
                    RepaymentDetail = lstRe
                } : "";
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
        [HttpPut]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("Updatep2pById/{MonthlyId}")]
        public IActionResult Updatep2pById(string MonthlyId)
        {
            var r = new ReturnObject();
            var eget = true;
            try
            {
                var lstRe = _repoP2pRepay.AsQueryable().Where(o => o.MonthlyRepaymentId == MonthlyId).FirstOrDefault();
                lstRe.RepaymentStatus = "Paid";
                _repoP2pRepay.ReplaceOne(lstRe);
                r.status = eget ? true : false;
                r.message = eget ? "Record Updated Successfully" : "No Record Found";

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
        #endregion

        #region  data
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getalldatas")]
        public IActionResult GetAlldatas([FromQuery] CustomerCentricFilter obj)
        {
            IEnumerable<CustomerCentricResponse> neRes = null;
            List<Customer> allSav = null;
            var r = new ReturnObject();
            var eget = false;
            try
            {
                var lstOfCusId = _repoData.AsQueryable().Select(o => new { o.CustomerId, o.Status }).ToList();

                switch (obj.Det)
                {
                    case 1:
                        allSav = _context.Customers.Where(o => lstOfCusId.Select(o => o.CustomerId).Contains(o.Id)
                             && o.DateCreated > obj.StartDate
                        && o.DateCreated < obj.EndDate.AddDays(1)).ToList();
                        foreach (var item in allSav)
                        {
                            item.CustomerCentricStatus = lstOfCusId.FirstOrDefault(o => o.CustomerId == item.Id).Status;
                        }
                        if (obj.Status != 0)
                            allSav = allSav.Where(o => o.Status.GetValueOrDefault() == obj.Status).ToList();
                        break;
                    case 2:
                        if (string.IsNullOrWhiteSpace(obj.SearchName))
                        {
                            allSav = _context.Customers.Where(o => lstOfCusId.Select(o => Convert.ToInt64(o.CustomerId)).Contains(o.Id)).ToList();
                            foreach (var item in allSav)
                            {
                                item.CustomerCentricStatus = lstOfCusId.FirstOrDefault(o => o.CustomerId == item.Id).Status;
                            }
                        }
                        else
                        {
                            switch (obj.SearchType.ToLower())
                            {
                                case "email":
                                    allSav = _context.Customers.Where(o => lstOfCusId.Select(o => Convert.ToInt64(o.CustomerId)).Contains(o.Id) &&
                                    o.EmailAddress.ToLower().Contains(obj.SearchName.ToLower())
                                    ).ToList();
                                    foreach (var item in allSav)
                                    {
                                        item.CustomerCentricStatus = lstOfCusId?.FirstOrDefault(o => Convert.ToInt64(o.CustomerId) == item.Id)?.Status;
                                    }
                                    break;
                                case "phone":
                                    allSav = _context.Customers.Where(o => lstOfCusId.Select(o => Convert.ToInt64(o.CustomerId)).Contains(o.Id) &&
                                    o.PhoneNumber.ToLower().Contains(obj.SearchName.ToLower())
                                    ).ToList();
                                    foreach (var item in allSav)
                                    {
                                        item.CustomerCentricStatus = lstOfCusId?.FirstOrDefault(o => Convert.ToInt64(o.CustomerId) == item.Id)?.Status;
                                    }
                                    break;
                                case "name":
                                    allSav = _context.Customers.Where(o => lstOfCusId.Select(o => Convert.ToInt64(o.CustomerId)).Contains(o.Id) &&
                                                 o.FirstName.ToLower().Contains(obj.SearchName.ToLower())
                                              || o.LastName.ToLower().Contains(obj.SearchName.ToLower())
                                              || o.MiddleName.ToLower().Contains(obj.SearchName.ToLower())).ToList();
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    default:
                        break;
                }

                var fneRes = allSav.Skip((obj.PageNumber - 1) * obj.PasgeSize)
                    .Take(obj.PasgeSize);
                if (allSav.Any())
                    eget = true;
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = fneRes.ToList();
                r.recordCount = allSav.Count();
                r.recordPageNumber = obj.PageNumber;
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
        [Route("getalldatasForFilter")]
        public IActionResult GetAlldataForFilter([FromBody] FilterModel obj)
        {
            var r = new ReturnObject();
            var eget = false;
            try
            {
                string searchString = obj.SearchBy.ToLower().Trim();
                var lstOfCusId = _repoData.AsQueryable().Select(o => new { o.CustomerId, o.Status }).ToList();
                var allSav = _context.Customers.Where(p => p.EmailAddress.ToLower().Trim() == searchString ||
                                                           p.PhoneNumber.ToLower().Trim() == searchString ||
                                                           p.FirstName.ToLower().Trim() == searchString &&
                                                           lstOfCusId.Select(o => Convert.ToInt64(o.CustomerId)).Contains(p.Id)
                                                           ).ToList();
                foreach (var item in allSav)
                {
                    item.CustomerCentricStatus = lstOfCusId.FirstOrDefault(o => o.CustomerId == item.Id).Status;
                }
                if (allSav.Any())
                    eget = true;
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = eget ? allSav : "";
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
        [HttpPut]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("ModifydataPaymentsStatusWithTransactionReference")]
        public IActionResult Modifydatasstatus([FromBody] FilterUpdateModel obj)
        {
            var r = new ReturnObject();
            var eget = false;
            try
            {
                var allSav = _repoData.AsQueryable().FirstOrDefault(o => o.TransactionReference.Trim().ToLower() == obj.EntityId.Trim().ToLower());

                if (allSav is not null)
                {
                    allSav.Status = obj.Status;
                    _repoData.ReplaceOne(allSav);
                    eget = true;
                }
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = eget ? allSav : "";
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

        // Method for getting all savings by customer ID
        // Returns a list of savings or an error message with status code
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("GetdataByCusId/{cusId}")]
        public IActionResult GetdataByCustomerId(int cusId)
        {
            var r = new ReturnObject();
            var eget = false;
            try
            {
                var lstOfCusId = _repoData.AsQueryable().Where(o => o.CustomerId == cusId).ToList();
                var allSav = _context.Customers.FirstOrDefault(p => p.Id == cusId);
                var res = new CustomerCentricResponseForView();
                if (lstOfCusId.Any())
                {
                    eget = true;
                    res.PhoneNumber = allSav.PhoneNumber;
                    res.EmailAddress = allSav.EmailAddress;
                    res.FullName = $"{allSav.FirstName} {allSav.MiddleName} {allSav.LastName}";
                    res.DateOfBirth = allSav.DateOfBirth;
                    res.Bvn = allSav.Bvn;
                    res.ListItem = lstOfCusId;
                }
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = eget ? res : "";
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
        #endregion

        #region  loanbidding
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getallloanbiddings")]
        public IActionResult GetAllloanbiddings([FromQuery] CustomerCentricFilter obj)
        {
            var r = new ReturnObject();
            var eget = false;
            var allLender = new List<LoanBidding>();
            try
            {
                var lstOfCusId = _repoLoanBidding.AsQueryable();
                if (obj.Det == 1)
                {
                    allLender = lstOfCusId.Where(o => o.DateCreated > Convert.ToDateTime(obj.StartDate)
                        && o.DateCreated < Convert.ToDateTime(obj.EndDate).AddDays(1)).ToList();
                }
                else if (obj.Det == 2)
                {
                    if (string.IsNullOrEmpty(obj.SearchName))
                    {
                        allLender = lstOfCusId.ToList();
                    }
                    else
                    {
                        switch (obj.SearchName.ToLower())
                        {
                            case "email":
                                allLender = lstOfCusId.Where(o => o.BiddersEmailAddress.ToLower().Contains(obj.SearchType.ToLower()) || o.LenderName.ToLower().Contains(obj.SearchType.ToLower())).ToList();
                                break;
                            case "phone":
                                allLender = lstOfCusId.Where(o => o.BiddersPhoneNumber.Contains(obj.SearchType.ToLower()) || o.LenderPhoneNumber.Contains(obj.SearchType.ToLower())).ToList();
                                break;
                            case "name":
                                allLender = lstOfCusId.Where(o => o.BiddersName.Contains(obj.SearchName) || o.LenderName.Contains(obj.SearchName)).ToList();
                                break;
                            default:
                                break;
                        }
                    }
                }


                var fneRes = allLender.Skip((obj.PageNumber - 1) * obj.PasgeSize)
                      .Take(obj.PasgeSize);
                if (allLender.Any())
                    eget = true;
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = fneRes.ToList();
                r.recordCount = allLender.Count();
                r.recordPageNumber = obj.PageNumber;
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

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("GetloanbiddingByLoanOfferId/{loanOfferId}")]
        public IActionResult GetloanbiddingByLoanOfferId(string loanOfferId)
        {
            var r = new ReturnObject();
            var eget = false;
            try
            {
                var lstOfCusId = _repoLoanBidding.AsQueryable().FirstOrDefault(o => o.LoanOfferId == loanOfferId);

                if (lstOfCusId is not null)
                    eget = true;
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = eget ? lstOfCusId : "";
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
        #endregion

        #region  escrow
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("GetAllEscrowStatus")]
        public IActionResult GetAllEscrowStatus()
        {

            var r = new ReturnObject();
            var eget = true;
            try
            {
                var fneRes = _context.EscrowStatus.Select(fd => new DropDownDetail { Id = fd.Id, Name = fd.StatusName }).ToList();
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = fneRes.ToList();
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

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getallescrows")]
        public IActionResult GetAllescrows([FromQuery] CustomerCentricFilter obj)
        {
            List<CustomerCentricResponseEscrow> neRes = new();
            List<Customer> allSav = _context.Customers.ToList();
            var r = new ReturnObject();
            var eget = false;
            try
            {
                var lstOfCusId = _repoEs.AsQueryable().ToList();
                switch (obj.Det)
                {
                    case 1:
                        allSav = allSav.Where(o => lstOfCusId.Select(o => Convert.ToInt64(o.SellerId)).Contains(o.Id)
                              && o.DateCreated > obj.StartDate
                        && o.DateCreated < obj.EndDate.AddDays(1)).ToList();

                        if (obj.Status != 0)
                            allSav = allSav.Where(o => o.Status.GetValueOrDefault() == obj.Status).ToList();
                        break;
                    case 2:
                        if (string.IsNullOrWhiteSpace(obj.SearchName))
                        {
                            // allSav = allSav.Where(o => lstOfCusId.Select(o => Convert.ToInt64(o.SellerId)).Contains(o.Id)).ToList();
                            foreach (var item in lstOfCusId)
                            {
                                var by = allSav.FirstOrDefault(x => x.Id == Convert.ToInt64(item.BuyerId));
                                var sy = allSav.FirstOrDefault(x => x.Id == Convert.ToInt64(item.SellerId));
                                CustomerCentricResponseEscrow c = new();
                                c.BuyerEmailAddress = by != null ? by.EmailAddress : "Not Found";
                                c.SellerEmailAddress = sy != null ? sy.EmailAddress : "Not Found";
                                c.BuyerName = item.BuyerFullName;
                                c.SellerName = item.SellerFullName;
                                c.SellerPhoneNumber = sy != null ? sy.PhoneNumber : "Not Found";
                                c.BuyerPhoneNumber = by != null ? by.PhoneNumber : "Not Found";
                                c.TransactionDate = item.TransactionDate.ToString();
                                c.Status = item.TransactionStatusName.ToString();
                                c.TransactionReference = item.TransactionReference.ToString();
                                neRes.Add(c);
                            }
                        }
                        else
                        {
                            switch (obj.SearchType.ToLower())
                            {
                                case "email":
                                    allSav = allSav.Where(o => o.EmailAddress.ToLower().Contains(obj.SearchName.ToLower())
                                    ).ToList();

                                    lstOfCusId = lstOfCusId.Where(o => allSav.Select(o => o.Id).Contains(o.SellerId)).ToList();
                                    foreach (var item in lstOfCusId)
                                    {
                                        var by = allSav.FirstOrDefault(x => x.Id == Convert.ToInt64(item.BuyerId));
                                        var sy = allSav.FirstOrDefault(x => x.Id == Convert.ToInt64(item.SellerId));
                                        CustomerCentricResponseEscrow c = new();
                                        c.BuyerEmailAddress = by != null ? by.EmailAddress : "Not Found";
                                        c.SellerEmailAddress = sy != null ? sy.EmailAddress : "Not Found";
                                        c.BuyerName = item.BuyerFullName;
                                        c.SellerName = item.SellerFullName;
                                        c.SellerPhoneNumber = sy != null ? sy.PhoneNumber : "Not Found";
                                        c.BuyerPhoneNumber = by != null ? by.PhoneNumber : "Not Found";
                                        c.TransactionDate = item.TransactionDate.ToString();
                                        c.Status = item.TransactionStatusName.ToString();
                                        c.TransactionReference = item.TransactionReference.ToString();
                                        neRes.Add(c);
                                    }
                                    break;
                                case "phone":
                                    allSav = allSav.Where(o => o.PhoneNumber.ToLower().Contains(obj.SearchName.ToLower())
                                   ).ToList();

                                    lstOfCusId = lstOfCusId.Where(o => allSav.Select(o => o.Id).Contains(o.SellerId)).ToList();
                                    foreach (var item in lstOfCusId)
                                    {
                                        var by = allSav.FirstOrDefault(x => x.Id == Convert.ToInt64(item.BuyerId));
                                        var sy = allSav.FirstOrDefault(x => x.Id == Convert.ToInt64(item.SellerId));
                                        CustomerCentricResponseEscrow c = new();
                                        c.BuyerEmailAddress = by != null ? by.EmailAddress : "Not Found";
                                        c.SellerEmailAddress = sy != null ? sy.EmailAddress : "Not Found";
                                        c.BuyerName = item.BuyerFullName;
                                        c.SellerName = item.SellerFullName;
                                        c.SellerPhoneNumber = sy != null ? sy.PhoneNumber : "Not Found";
                                        c.BuyerPhoneNumber = by != null ? by.PhoneNumber : "Not Found";
                                        c.TransactionDate = item.TransactionDate.ToString();
                                        c.Status = item.TransactionStatusName.ToString();
                                        c.TransactionReference = item.TransactionReference.ToString();
                                        neRes.Add(c);
                                    }
                                    break;
                                case "name":

                                    allSav = allSav.Where(o => o.FirstName.ToLower().Contains(obj.SearchName.ToLower())
                                              || o.LastName.ToLower().Contains(obj.SearchName.ToLower())
                                              || o.MiddleName.ToLower().Contains(obj.SearchName.ToLower())).ToList();

                                    lstOfCusId = lstOfCusId.Where(o => allSav.Select(o => o.Id).Contains(o.SellerId)).ToList();
                                    foreach (var item in lstOfCusId)
                                    {
                                        var by = allSav.FirstOrDefault(x => x.Id == Convert.ToInt64(item.BuyerId));
                                        var sy = allSav.FirstOrDefault(x => x.Id == Convert.ToInt64(item.SellerId));
                                        CustomerCentricResponseEscrow c = new();
                                        c.BuyerEmailAddress = by != null ? by.EmailAddress : "Not Found";
                                        c.SellerEmailAddress = sy != null ? sy.EmailAddress : "Not Found";
                                        c.BuyerName = item.BuyerFullName;
                                        c.SellerName = item.SellerFullName;
                                        c.SellerPhoneNumber = sy != null ? sy.PhoneNumber : "Not Found";
                                        c.BuyerPhoneNumber = by != null ? by.PhoneNumber : "Not Found";
                                        c.TransactionDate = item.TransactionDate.ToString();
                                        c.Status = item.TransactionStatusName.ToString();
                                        c.TransactionReference = item.TransactionReference.ToString();
                                        neRes.Add(c);
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }

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


        // Method for getting all savings by customer ID
        // Returns a list of savings or an error message with status code
        [HttpPut]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("ModifyescrowPaymentsStatusWithTransactionReference")]
        public IActionResult Modifyescrowsstatus([FromBody] FilterUpdateModel obj)
        {
            var r = new ReturnObject();
            var eget = false;
            try
            {
                var allSav = _repoEs.AsQueryable().FirstOrDefault(o => o.TransactionReference.Trim().ToLower() == obj.EntityId.Trim().ToLower());

                if (allSav is not null)
                {
                    allSav.TransactionStatusName = obj.Status;
                    _repoEs.ReplaceOne(allSav);
                    eget = true;
                }
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = eget ? allSav : "";
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
        #endregion

        [NonAction]
        private IEnumerable<CustomerCentricResponse> getAllFixedDeposit()
        {
            var allSav = (from c in _context.FixedDepositSetups
                          where c.ClientId == auth.ClientId
                          join e in _context.FixDepositStatus on c.Status equals e.Id
                          join d in _context.Customers on c.CustomerId equals d.Id
                          select new CustomerCentricResponse
                          {
                              CustomerRef = d.CustomerRef,
                              EmailAddress = d.EmailAddress,
                              FirstName = d.FirstName,
                              LastName = d.LastName,
                              MiddleName = d.MiddleName,
                              DateOfBirth = d.DateOfBirth,
                              Bvn = d.Bvn,
                              CustomerId = c.CustomerId.GetValueOrDefault(),
                              // DateCreated =c.DateCreated,
                              Status = c.Status.Value,
                              StatusName = e.StatusName.ToString()
                          }).AsEnumerable();
            return allSav.Distinct(new EntityComparer<CustomerCentricResponse>(user => user.CustomerId));
        }
    }
}