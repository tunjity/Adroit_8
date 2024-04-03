using Adroit_v8.Models.Administration;
using Adroit_v8.Models.FormModel;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;

namespace Adroit_v8.Controllers.Administration
{
    [Route("api/Administration/[controller]")]
    [ApiController]
    [Authorize]
    public class AdminProductController : AuthController
    {
        private IGenericRepository<Adminproduct> _repoAdminProduct;
        private IGenericRepository<ProductLoanProcessingFee> _repoAdminProductLoanProcessingFee;
        private readonly CreditWaveContext _context;
        private IGenericRepository<ProductLateFee> _repoAdminProductLateFee;
        private string errMsg = "Unable to process request, kindly try again";

        public AdminProductController(IGenericRepository<Adminproduct> repoAdminProduct, CreditWaveContext context, IGenericRepository<ProductLoanProcessingFee> repoAdminProductLoanProcessingFee,
            IGenericRepository<ProductLateFee> repoAdminProductLateFee, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _repoAdminProduct = repoAdminProduct;
            _context = context;
            _repoAdminProductLateFee = repoAdminProductLateFee;
            _repoAdminProductLoanProcessingFee = repoAdminProductLoanProcessingFee;
        }


        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("add")]
        public Task<IActionResult> Add([FromBody] AdminproductFormMode obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Saved Successfully";
            DateTime utcDateTime = DateTime.Now.ToUniversalTime();
            try
            {
                if (obj.Enddate.ToString() == "01/01/2000" || obj.Enddate.ToString() == "2000/01/01")
                    obj.Enddate = null;
                Adminproduct Adminproduct = new Adminproduct
                {
                    UniqueId = Guid.NewGuid().ToString(),
                    Status = 1,
                    Minimuimamount = obj.Minimuimamount,
                    Maximuimamount = obj.Maximuimamount,
                    InterestRate = obj.InterestRate,
                    Tenor = obj.Tenor,
                    Startdate = obj.Startdate,
                    AsEndDate = obj.AsEndDate,
                    Enddate = obj.Enddate,
                    Datecreated = utcDateTime,
                    Name = obj.Name
                };
                _repoAdminProduct.Insert(Adminproduct);
                ProductLateFee productLateFee = new()
                {
                    UniqueId = Guid.NewGuid().ToString(),
                    ProductId = Adminproduct.Id,
                    LateFeeType = obj.LateFeeType,
                    FixedPrice = obj.FixedPrice,
                    LateFeePrincipal = obj.LateFeePrincipal,
                    GracePeriod = obj.GracePeriod,
                    FeeFrequency = obj.FeeFrequency
                };
                _repoAdminProductLateFee.Insert(productLateFee);
                ProductLoanProcessingFee ProductLoanProcessingFee = new()
                {
                    UniqueId = Guid.NewGuid().ToString(),
                    ProductId = Adminproduct.Id,
                    IsOptInProcessingFee = obj.IsOptInProcessingFee,
                    IsFixedPrice = obj.IsFixedPrice,
                    FixedPrice = obj.FixedPrice,
                    Principal = obj.Principal
                };
                _repoAdminProductLoanProcessingFee.Insert(ProductLoanProcessingFee);
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
        [Route("getall")]
        public Task<IActionResult> GetAll([FromQuery] PaginationWithOutFilterModel obj)
        {
            var r = new ReturnObject(); bool eget = false;
            try
            {
                var ad = _repoAdminProductLateFee.GetAll().Where(o => o.Isdeleted == 0).ToList();
                var ls = _repoAdminProductLoanProcessingFee.GetAll().ToList();
                var pro = _repoAdminProduct.GetAll().ToList();
                foreach (var be in pro)
                {
                    if(be.AsEndDate == false)
                    {
                        be.Enddate = null;
                    }
                }
                    
                var query = from parent in pro
                            join child in ad on parent.Id equals child.ProductId into childGroup
                            join childII in ls on parent.Id equals childII.ProductId into childGroupII
                            select new
                            {
                                AdminProduct = parent,
                                AdminProductLateFee = childGroup.ToList(),
                                AdminProductLoanProcessingFee = childGroupII.ToList(),
                            };
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

        [HttpDelete]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("deletebyid/id")]
        public Task<IActionResult> Delete(int id)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Deleted Successfully";
            try
            {
                var rec = _repoAdminProduct.Get(id);
                _repoAdminProduct.SoftDelete(rec);
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
        [Route("update")]
        public Task<IActionResult> Update([FromBody] AdminProductModificationFormModel obj)
        {
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record updated Successfully";
            try
            {
                var retChecker = _repoAdminProduct.Get(obj.Id);
                if (retChecker != null)
                {
                    retChecker.Status = obj.Status;
                    retChecker.Minimuimamount = obj.Minimuimamount;
                    retChecker.Maximuimamount = obj.Maximuimamount;
                    retChecker.InterestRate = obj.InterestRate;
                    retChecker.Tenor = obj.Tenor;
                    retChecker.Startdate = obj.Startdate;
                    retChecker.AsEndDate = obj.AsEndDate;
                    retChecker.Enddate = obj.Enddate;
                    _repoAdminProduct.Update(retChecker);

                    var res = _context.ProductLateFees.FirstOrDefault(o => o.ProductId == obj.Id);
                    var resP = _context.ProductLoanProcessingFees.FirstOrDefault(o => o.ProductId == obj.Id);
                    if (res != null)
                    {
                        res.FeeFrequency = obj.FeeFrequency;
                        res.LateFeeType = obj.LateFeeType;
                        res.FixedPrice = obj.FixedPrice;
                        res.LateFeePrincipal = obj.LateFeePrincipal;
                        res.GracePeriod = obj.GracePeriod;
                        _repoAdminProductLateFee.Update(res);
                    }
                    if (resP != null)
                    {
                        resP.IsOptInProcessingFee = obj.IsOptInProcessingFee;
                        resP.IsFixedPrice = obj.IsFixedPrice;
                        resP.FixedPrice = obj.FixedPrice;
                        resP.Principal = obj.Principal;
                        _repoAdminProductLoanProcessingFee.Update(resP);
                    }
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
        //[HttpGet]
        //[SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        //[SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        //[Route("getallvald")]
        //public Task<IActionResult> GetAllValidBanks()
        //{
        //    var r = new ReturnObject();
        //    r.status = true;
        //    r.message = "Record Fetched Successfully";
        //    try
        //    {
        //        r.data = _repoBank.GetAllIsValid();
        //        return Task.FromResult<IActionResult>(Ok(r));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Task.FromResult<IActionResult>(StatusCode(StatusCodes.Status500InternalServerError, new ReturnObject
        //        {
        //            status = false,
        //            message =ex.Message
        //        }));
        //    }
        //}
        //[HttpGet]
        //[SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        //[SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        //[Route("getbankbyid/id")]
        //public Task<IActionResult> GetBanks(int id)
        //{
        //    var r = new ReturnObject();
        //    r.status = true;
        //    r.message = "Record Fetched Successfully";
        //    try
        //    {
        //        r.data = _repoBank.Get(id);
        //        return Task.FromResult<IActionResult>(Ok(r));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Task.FromResult<IActionResult>(StatusCode(StatusCodes.Status500InternalServerError, new ReturnObject
        //        {
        //            status = false,
        //            message =ex.Message
        //        }));
        //    }
        //}
        //[HttpDelete]
        //[SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        //[SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        //[Route("deletebankbyid/id")]
        //public Task<IActionResult> DeleteBanks(int id)
        //{
        //    var r = new ReturnObject();
        //    r.status = true;
        //    r.message = "Record Deleted Successfully";
        //    try
        //    {
        //        var rec = _repoBank.Get(id);
        //        _repoBank.SoftDelete(rec);
        //        return Task.FromResult<IActionResult>(Ok(r));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Task.FromResult<IActionResult>(StatusCode(StatusCodes.Status500InternalServerError, new ReturnObject
        //        {
        //            status = false,
        //            message =ex.Message
        //        }));
        //    }
        //}
        //[HttpPut]
        //[SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        //[SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        //[Route("updatebank")]
        //public Task<IActionResult> UpdateBank([FromBody] UtilityModificationFormModel obj)
        //{
        //    var r = new ReturnObject();
        //    r.status = true;
        //    r.message = "Record updated Successfully";
        //    try
        //    {
        //        var retChecker = _context.Banks.FirstOrDefault(o => o.Name.ToLower() == obj.Name.ToLower() && o.Isdeleted == 0);
        //        if (retChecker is null)
        //        {
        //            var ret = _context.Banks.FirstOrDefault(o => o.Name.ToLower() == obj.Name.ToLower() && o.Isdeleted == 0);
        //            if (ret != null)
        //            {
        //                r.status = false;
        //                r.message = "Record Already Exist";
        //            }
        //            else
        //            {
        //                r.status = false;
        //                r.message = "Record Not Found";
        //            }
        //        }
        //        else
        //        {
        //            retChecker.Name = obj.Name;
        //            _repoBank.Update(retChecker);
        //        }

        //        return Task.FromResult<IActionResult>(Ok(r));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Task.FromResult<IActionResult>(StatusCode(StatusCodes.Status500InternalServerError, new ReturnObject
        //        {
        //            status = false,
        //            message =ex.Message
        //        }));
        //    }
        //}
    }
}
