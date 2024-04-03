using Adroit_v8.MongoConnections.CRM;
using Adroit_v8.MongoConnections;
using static Adroit_v8.Config.Helper;
using static Adroit_v8.EnumFile.EnumHelper;
using Newtonsoft.Json;
using Adroit_v8.Service;
using Microsoft.AspNetCore.Authorization;
using Adroit_v8.Config;
using Adroit_v8.MongoConnections.Models;

namespace Adroit_v8.Controllers.CRM
{
    [Route("api/CRM/[controller]")]
    [ApiController]
    [Authorize]
    public class DocumentController : AuthController
    {
        private readonly IMongoRepository<ClientDoc> _repo;
        private readonly IMongoRepository<CustomerStageHolder> _repoCustomerStageHolder;
        string errMsg = "Unable to process request, kindly try again";
        private readonly IConfiguration _config;
        public DocumentController(IMongoRepository<ClientDoc> repo,
            IMongoRepository<CustomerStageHolder> repoCustomerStageHolder, IConfiguration config, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _repo = repo;
            _repoCustomerStageHolder = repoCustomerStageHolder;
            _config = config;
        }

        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("add")]
        public async Task<IActionResult> Add([FromForm] ClientDocFM obj)
        {
            var RequestTime = DateTime.UtcNow;
            string actionUrl = $"{ControllerContext.RouteData.Values["controller"]}/{ControllerContext.RouteData.Values["action"]}";
            try
            {
                ClientDoc la = new()
                {
                    ProofOfResidenceType = obj.ProofOfResidenceType,
                    CustomerId = obj.CustomerId,
                    ProofOfIdentityType = obj.ProofOfIdentityType,
                    ProofOfIdentityExpiryDate = obj.ProofOfIdentityExpiryDate,
                    ProofOfEmploymentType = obj.ProofOfEmploymentType,
                    UniqueId = Guid.NewGuid().ToString()
                };
                if (obj.ESignature != null)
                {
                    var SavePath = $"{_config["FileFolder:Path"]}{"ESignature"}";
                    string fileName = $"{la.UniqueId}{"_"}{Path.GetFileName(obj.ESignature.FileName)}";
                    _ = Task.Run(() => { Helper.ProcessFileUpload(obj.ESignature, la.UniqueId, fileName, SavePath); });
                    la.ESignature = Path.Combine(SavePath, fileName);
                    la.ESignatureFileName = Path.GetFileName(obj.ESignature.FileName);
                }
                if (obj.PassportPhotograph != null)
                {
                    var SavePath = $"{_config["FileFolder:Path"]}{"PassportPhotograph"}";
                    string fileName = $"{la.UniqueId}{"_"}{Path.GetFileName(obj.PassportPhotograph.FileName)}";
                    _ = Task.Run(() => { Helper.ProcessFileUpload(obj.PassportPhotograph, la.UniqueId, fileName, SavePath); });
                    la.PassportPhotograph = Path.Combine(SavePath, fileName); 
                    la.PassportPhotographFileName = Path.GetFileName(obj.PassportPhotograph.FileName);
                }
                if (obj.ProofOfResidence != null)
                {
                    var SavePath = $"{_config["FileFolder:Path"]}{"ProofOfResidence"}";
                    string fileName = $"{la.UniqueId}{"_"}{Path.GetFileName(obj.ProofOfResidence.FileName)}";
                    _ = Task.Run(() => { Helper.ProcessFileUpload(obj.ProofOfResidence, la.UniqueId, fileName, SavePath); });
                    la.ProofOfResidence = Path.Combine(SavePath, fileName);
                    la.ProofOfResidenceFileName = Path.GetFileName(obj.ProofOfResidence.FileName);
                }
                if (obj.ProofOfIdentity != null)
                {
                    var SavePath = $"{_config["FileFolder:Path"]}{"ProofOfResidence"}";
                    string fileName = $"{la.UniqueId}{"_"}{Path.GetFileName(obj.ProofOfIdentity.FileName)}";
                    _ = Task.Run(() => { Helper.ProcessFileUpload(obj.ProofOfIdentity, la.UniqueId, fileName, SavePath); });
                    la.ProofOfIdentity = Path.Combine(SavePath, fileName);
                    la.ProofOfIdentityFileName = Path.GetFileName(obj.ProofOfIdentity.FileName);
                }
                if (obj.ProofOfEmployment != null)
                {
                    var SavePath = $"{_config["FileFolder:Path"]}{"ProofOfEmployment"}";
                    string fileName = $"{la.UniqueId}{"_"}{Path.GetFileName(obj.ProofOfEmployment.FileName)}";
                    _ = Task.Run(() => { Helper.ProcessFileUpload(obj.ProofOfEmployment, la.UniqueId, fileName, SavePath); });
                    la.ProofOfEmployment = Path.Combine(SavePath, fileName);
                    la.ProofOfEmploymentFileName = Path.GetFileName(obj.ProofOfEmployment.FileName);
                }

                var res = await _repo.InsertOneAsync(la);
                CustomerStageHolder st = new()
                {
                    CustomerId = obj.CustomerId,
                    StageId = CustomerRegistrationStage.StageDocumentUpload.ToString()
                };
                _repoCustomerStageHolder.InsertOne(st);
                _ = LogService_Old.LoggerCreateAsync(JsonConvert.SerializeObject(obj), actionUrl, RequestTime, JsonConvert.SerializeObject(res), "", (int)ServiceLogLevel.Information);
                res.data = obj;
                return Ok(res);
            }
            catch (Exception ex)
            {
                _ = LogService_Old.LoggerCreateAsync(JsonConvert.SerializeObject(obj), actionUrl, RequestTime, "", ex.InnerException.ToString(), (int)ServiceLogLevel.Error);
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
        public async Task<IActionResult> Update([FromForm] ClientDocUpdateFm obj)
        {

            string fileNameESignature, fileNamePassportPhotograph, fileNameProofOfResidence, fileNameProofOfIdentity, fileNameProofOfEmployment = "";
            var RequestTime = DateTime.UtcNow;
            string actionUrl = $"{ControllerContext.RouteData.Values["controller"]}/{ControllerContext.RouteData.Values["action"]}";
            try
            {
                var la = _repo.FindById(obj.UniqueId);
                if (la != null)
                {
                    la.UniqueId = obj.UniqueId;
                    la.Id = la.Id;
                    la.ClientId = la.ClientId;
                    la.ProofOfResidenceType = obj.ProofOfResidenceType;
                    la.CustomerId = obj.CustomerId;
                    la.ProofOfIdentityType = obj.ProofOfIdentityType;
                    la.ProofOfIdentityExpiryDate = obj.ProofOfIdentityExpiryDate;
                    la.ProofOfEmploymentType = obj.ProofOfEmploymentType;

                    if (obj.ESignature != null)
                    {
                        var SavePath = $"{_config["FileFolder:Path"]}{"ESignature"}";
                        fileNameESignature = $"{la.UniqueId}{"_Updated_"}{Path.GetFileName(obj.ESignature.FileName)}";
                        _ = Task.Run(() => { Helper.ProcessFileUpload(obj.ESignature, la.UniqueId, fileNameESignature, SavePath); });
                        la.ESignature = Path.Combine(SavePath, fileNameESignature);
                    }
                    else
                        la.ESignature = la.ESignature;

                    if (obj.ProofOfIdentity != null)
                    {
                        var SavePath = $"{_config["FileFolder:Path"]}{"ESignature"}";
                        fileNameProofOfIdentity = $"{la.UniqueId}{"_Updated_"}{Path.GetFileName(obj.ProofOfIdentity.FileName)}";
                        _ = Task.Run(() => { Helper.ProcessFileUpload(obj.ProofOfIdentity, la.UniqueId, fileNameProofOfIdentity, SavePath); });
                        la.ProofOfIdentity = Path.Combine(SavePath, fileNameProofOfIdentity);
                    }
                    else
                        la.ProofOfIdentity = la.ProofOfIdentity;
                    if (obj.ProofOfEmployment != null)
                    {
                        var SavePath = $"{_config["FileFolder:Path"]}{"ProofOfEmployment"}";
                        fileNameProofOfEmployment = $"{la.UniqueId}{"_Updated_"}{Path.GetFileName(obj.ProofOfEmployment.FileName)}";
                        _ = Task.Run(() => { Helper.ProcessFileUpload(obj.ProofOfEmployment, la.UniqueId, fileNameProofOfEmployment, SavePath); });
                        la.ProofOfEmployment = Path.Combine(SavePath, fileNameProofOfEmployment);
                    }
                    else
                        la.ProofOfEmployment = la.ProofOfEmployment;

                    if (obj.ProofOfResidence != null)
                    {
                        var SavePath = $"{_config["FileFolder:Path"]}{"ProofOfResidence"}";
                        fileNameProofOfResidence = $"{la.UniqueId}{"_Updated_"}{Path.GetFileName(obj.ProofOfResidence.FileName)}";
                        _ = Task.Run(() => { Helper.ProcessFileUpload(obj.ProofOfResidence, la.UniqueId, fileNameProofOfResidence, SavePath); });
                        la.ProofOfResidence = Path.Combine(SavePath, fileNameProofOfResidence);
                    }
                    else
                        la.ProofOfResidence = la.ProofOfResidence;

                    if (obj.PassportPhotograph != null)
                    {
                        var SavePath = $"{_config["FileFolder:Path"]}{"PassportPhotograph"}";
                        fileNamePassportPhotograph = $"{la.UniqueId}{"_Updated_"}{Path.GetFileName(obj.PassportPhotograph.FileName)}";
                        _ = Task.Run(() => { Helper.ProcessFileUpload(obj.ESignature, la.UniqueId, fileNamePassportPhotograph, SavePath); });
                        la.PassportPhotograph =  Path.Combine(SavePath, fileNamePassportPhotograph);
                    }
                    else
                        la.PassportPhotograph = la.PassportPhotograph;

                    la.ProofOfEmploymentFileName = Path.GetFileName(obj.ProofOfEmployment.FileName);
                    la.ESignatureFileName = Path.GetFileName(obj.ESignature.FileName);
                    la.ProofOfIdentityFileName = Path.GetFileName(obj.ProofOfIdentity.FileName);
                    la.ProofOfResidenceFileName = Path.GetFileName(obj.ProofOfResidence.FileName);
                    la.PassportPhotographFileName = Path.GetFileName(obj.PassportPhotograph.FileName);
                    _repo.ReplaceOne(la);
                }
                else
                {
                    _ = LogService_Old.LoggerCreateAsync(JsonConvert.SerializeObject(obj), actionUrl, RequestTime, JsonConvert.SerializeObject(la), "", (int)ServiceLogLevel.Warning);
                    return StatusCode(StatusCodes.Status500InternalServerError, new ReturnObject
                    {
                        status = false,
                        message = "Record not Valid"
                    });
                }
                _ = LogService_Old.LoggerCreateAsync(JsonConvert.SerializeObject(obj), actionUrl, RequestTime, JsonConvert.SerializeObject(la), "", (int)ServiceLogLevel.Information);
                return Ok(new ReturnObject
                {
                    status = true,
                    message = "Record Update Successfully"
                });
            }
            catch (Exception ex)
            {
                _ = LogService_Old.LoggerCreateAsync(JsonConvert.SerializeObject(obj), actionUrl, RequestTime, "", ex.InnerException.ToString(), (int)ServiceLogLevel.Warning);
                return StatusCode(StatusCodes.Status500InternalServerError, new ReturnObject
                {
                    status = false,
                    message = ex.Message
                });
            }
        }

    }
}
