using Adroit_v8.MongoConnections.Models;
using Adroit_v8.MongoConnections;
using Adroit_v8.Service;
using Newtonsoft.Json;
using MongoDB.Driver;
using Microsoft.AspNetCore.Authorization;
using Adroit_v8.Config;
using static Adroit_v8.EnumFile.EnumHelper;

namespace Adroit_v8.Controllers.BridgeLoan
{
    [Route("api/BridgeLoan/[controller]")]
    [ApiController]
    [Authorize]
    public class DocumentationController : AuthController
    {
        private readonly IMongoRepository<Documentation> _repo;
        private readonly IMongoRepository<DocumentationDoc> _repoDoc;
        private string errMsg = "Unable to process request, kindly try again";

        // string _CreatedBy = "0";
        private readonly IConfiguration _config;

        private readonly IMongoCollection<DocumentationDoc> _ApplicationGet;

        public DocumentationController(IMongoRepository<Documentation> repo, IConfiguration config,
            IMongoRepository<DocumentationDoc> repoDoc, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _repo = repo;
            _config = config;
            _repoDoc = repoDoc; string? connectionURI = _config.GetSection("MongoDB").GetSection("ConnectionURI").Value;
            string? databaseName = _config.GetSection("MongoDB").GetSection("DatabaseName").Value;
            MongoClient client = new MongoClient(connectionURI);
            IMongoDatabase database = client.GetDatabase(databaseName);
            _ApplicationGet = database.GetCollection<DocumentationDoc>("AdroitBridgeLoanDocumentationDoc");
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("get")]
        public IActionResult GetAll([FromQuery] PaginationWithOutFilterModel obj)
        {
            var r = new ReturnObject(); bool eget = false;
            try
            {
                var query = _repo.AsQueryable().OrderByDescending(o => o.DateCreated);
                var fneRes = query.Skip((obj.PageNumber - 1) * obj.PasgeSize)
                     .Take(obj.PasgeSize);

                if (query.Any())
                    eget = true;
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = fneRes.ToList();
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

        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("add")]
        public async Task<IActionResult> Add([FromForm] DocumentationFm obj)
        {
            var RequestTime = DateTime.UtcNow;
            string actionUrl = $"{ControllerContext.RouteData.Values["controller"]}/{ControllerContext.RouteData.Values["action"]}";
            try
            {
                Documentation la = new()
                {
                    Lender = obj.Lender,
                    ObligorDob = obj.ObligorDob,
                    ObligorName = obj.ObligorName,
                    Comment = obj.Comment,
                    DocumentationStatus = obj.DocumentationStatus,
                    MaturityDate = obj.MaturityDate,
                    FacilityType = obj.FacilityType,
                    InterestRate = obj.InterestRate,
                    ValueDate = obj.ValueDate,
                    PhoneNo = obj.PhoneNo,
                    Tenor = obj.Tenor,
                    Amount = obj.Amount,
                    UniqueId = Guid.NewGuid().ToString(),
                    CreatedBy = obj.CreatedBy
                };
                var res = await _repo.InsertOneAsync(la);
                if (obj.DocumentationDoc != null)
                {
                    var SavePath = $"{_config["FileFolder:Path"]}{"DocumentationDoc"}";
                    var doc = new DocumentationDoc()
                    {
                        UniqueId = Guid.NewGuid().ToString(),
                        DocumentationUniqueId = la.UniqueId
                    };
                    string fileName = $"{doc.UniqueId}{"_"}{Path.GetFileName(obj.DocumentationDoc.FileName)}";
                    _ = Task.Run(() => { Helper.ProcessFileUpload(obj.DocumentationDoc, doc.UniqueId, fileName, SavePath); });
                    doc.DocumentationDocumentString = Path.Combine(SavePath, fileName);
                    await _repoDoc.InsertOneAsync(doc);
                }
                _ = LogService_Old.LoggerCreateAsync(JsonConvert.SerializeObject(obj), actionUrl, RequestTime, JsonConvert.SerializeObject(res), "", (int)ServiceLogLevel.Information);
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

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("byuniqueid/{id}")]
        public IActionResult GetById([FromRoute] string id)
        {
            try
            {
                var rec = _repo.FindById(id);
                var doc = _ApplicationGet.Find(o => o.DocumentationUniqueId == rec.UniqueId).FirstOrDefault();
                rec.DocumentSting = doc != null ? Helper.ConvertFromPathToBase64(doc.DocumentationDocumentString) : null;
                return Ok(rec);
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
        [Route("DownloadbyuniqueidForPdf/{uniqueId}")]
        public IActionResult DownloadPdf([FromRoute] string uniqueId)
        {
            try
            {
                var recII = _repo.FindById(uniqueId);
                if (recII is not null)
                {
                    var rec = _ApplicationGet.Find(o => o.DocumentationUniqueId == uniqueId).FirstOrDefault();
                    if (rec is not null)
                    {
                        var file = Helper.ConvertFromPathToBase64(rec.DocumentationDocumentString);
                        byte[] pdfBytes = Convert.FromBase64String(file);
                        Response.Headers.Add("Content-Disposition", $"inline; filename=Bridge_Loan_{recII.ObligorName.Replace(" ", "")}.pdf");
                        return File(pdfBytes, "application/pdf");
                    }
                }
                return Ok();

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
        public async Task<IActionResult> Update([FromForm] DocumentationUpdateFm obj)
        {
            string fileName = "";
            var RequestTime = DateTime.UtcNow;
            string actionUrl = $"{ControllerContext.RouteData.Values["controller"]}/{ControllerContext.RouteData.Values["action"]}";
            try
            {
                var pcu = _repo.FindById(obj.UniqueId);
                var pcuDoc = _repoDoc.FindById(obj.UniqueId);
                if (pcu != null)
                {

                    // var base64String = Helper.ConvertIFormFilesToBase64(obj.DocumentationDoc);
                    pcu.DocumentationStatus = obj.DocumentationStatus;
                    pcu.ObligorName = obj.ObligorName;
                    pcu.Lender = obj.Lender;
                    pcu.ObligorDob = obj.ObligorDob;
                    pcu.FacilityType = obj.FacilityType;
                    pcu.InterestRate = obj.InterestRate;
                    pcu.ValueDate = obj.ValueDate;
                    pcu.Tenor = obj.Tenor;
                    pcu.PhoneNo = obj.PhoneNo;
                    pcu.Amount = obj.Amount;
                    pcu.MaturityDate = obj.MaturityDate;
                    pcu.Comment = obj.Comment;
                    _repo.ReplaceOne(pcu);

                    if (pcuDoc != null)
                    {
                        string? SavePath = "";
                        if (obj.DocumentationDoc != null)
                        {
                            SavePath = $"{_config["FileFolder:Path"]}{"DocumentationDoc"}";
                            fileName = $"{pcuDoc.UniqueId}{"_Updated_"}{Path.GetFileName(obj.DocumentationDoc.FileName)}";
                            _ = Task.Run(() => { Helper.ProcessFileUpload(obj.DocumentationDoc, pcuDoc.UniqueId, fileName, SavePath); });

                        }
                        pcuDoc.DocumentationDocumentString = Path.Combine(SavePath, fileName);
                        pcuDoc.DocumentationUniqueId = obj.UniqueId;

                        _repoDoc.ReplaceOne(pcuDoc);
                    }
                }
                else
                {
                    _ = LogService_Old.LoggerCreateAsync(JsonConvert.SerializeObject(obj), actionUrl, RequestTime, JsonConvert.SerializeObject(pcu), "", (int)ServiceLogLevel.Warning);
                    return StatusCode(StatusCodes.Status500InternalServerError, new ReturnObject
                    {
                        status = false,
                        message = "Record not Valid"
                    });
                }
                _ = LogService_Old.LoggerCreateAsync(JsonConvert.SerializeObject(obj), actionUrl, RequestTime, JsonConvert.SerializeObject(pcu), "", (int)ServiceLogLevel.Information);
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