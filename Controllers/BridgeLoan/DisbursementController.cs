using System;
using System.IO;
using System.Net.Mail;
using System.Xml.Linq;
using Adroit_v8.Models.FormModel;
using Adroit_v8.MongoConnections.Models;
using Adroit_v8.Service;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Driver;
using Newtonsoft.Json;
using OfficeOpenXml;
using SharpCompress.Common;
using static Adroit_v8.EnumFile.EnumHelper;

namespace Adroit_v8.Controllers.BridgeLoan
{
    [Route("api/BridgeLoan/[controller]")]
    [ApiController]
    [Authorize]
    public class DisbursementController : AuthController
    {
        private readonly IMongoRepository<Disbursement> _repo;
        private readonly IMongoRepository<Documentation> _repoDoc;
        private readonly IMongoRepository<DisbursementNew> _repoNew;
        private readonly IMongoRepository<DisbursementComment> _repoCom;
        string errMsg = "Unable to process request, kindly try again";

        public DisbursementController(
            IMongoRepository<Disbursement> repo,
            IMongoRepository<Documentation> repoDoc,
            IMongoRepository<DisbursementNew> repoNew,
            IMongoRepository<DisbursementComment> _repoCom,
            IHttpContextAccessor httpContextAccessor
        )
            : base(httpContextAccessor)
        {
            _repo = repo;
            _repoDoc = repoDoc;
            _repoNew = repoNew;
            this._repoCom = _repoCom;
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getprocessed")]
        public IActionResult GetAllProcessed([FromQuery] FilFormModel obj)
        {
            var r = new ReturnObject();
            bool eget = false;
            try
            {
                IQueryable<DisbursementNew> query = null;
                IQueryable<Documentation> queryDoc = null;
                List<DisbursementNew> fneRes = null;
                query = _repoNew
                    .AsQueryable()
                    .Where(o => o.Status == DisbursementEnum.Processed.ToString());
                queryDoc = _repoDoc.AsQueryable();
                //  DocumentationStage
                foreach (var ret in query)
                {
                    ret.DocumentationStage = queryDoc
                        .FirstOrDefault(o => o.ObligorDob == ret.DOB && o.PhoneNo == ret.PHONENO)
                        ?.DocumentationStatus;
                }
                if (query.Any())
                {
                    switch (obj.Det)
                    {
                        case 1:
                            query = query.Where(o =>
                                o.Status == DisbursementEnum.Processed.ToString()
                                && o.DateCreated > obj.StartDate.AddDays(-1)
                                && o.DateCreated < obj.StartDate.AddDays(1)
                            );
                            break;
                        default:
                            break;
                    }
                    fneRes = query
                        .Skip((obj.PageNumber - 1) * obj.PasgeSize)
                        .Take(obj.PasgeSize)
                        .ToList();
                    eget = true;
                }

                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = fneRes;
                r.recordCount = eget ? query.Count() : 0;
                r.recordPageNumber = obj.PageNumber;
                return (Ok(r));
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ReturnObject { status = false, message = ex.Message }
                );
            }
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getNew")]
        public IActionResult GetAllNew()
        {
            var r = new ReturnObject();
            bool eget = true;
            try
            {
                var query = _repoNew
                    .AsQueryable()
                    .Where(o =>
                        o.Status == DisbursementEnum.Processed.ToString()
                        && o.DateCreated >= DateTime.Today
                        && o.DateCreated < DateTime.Today.AddDays(1)
                    );

                var queryDoc = _repoDoc.AsQueryable();
                //  DocumentationStage
                foreach (var ret in query)
                {
                    ret.DocumentationStage = queryDoc
                        .FirstOrDefault(o => o.ObligorDob == ret.DOB && o.PhoneNo == ret.PHONENO)
                        ?.DocumentationStatus;
                }
                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = query;
                r.recordCount = eget ? query.Count() : 0;
                r.recordPageNumber = 1;
                return (Ok(r));
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ReturnObject { status = false, message = ex.Message }
                );
            }
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getprocessedForDownload")]
        public IActionResult GetAllProcessedForDownload([FromQuery] ForDownload obj)
        {
            var r = new ReturnObject();
            bool eget = false;
            try
            {
                IQueryable<DisbursementNew> query = null;
                //IQueryable<DisbursementNew> fneRes = null;
                query = _repoNew
                    .AsQueryable()
                    .Where(o =>
                        o.Status == DisbursementEnum.Processed.ToString()
                        && o.DateCreated > obj.StartDate.AddDays(-1)
                        && o.DateCreated < obj.StartDate.AddDays(1) //add 1 day to make sure it includes the end date
                    );
                if (query.Any())
                {
                    ExcelPackage.LicenseContext = LicenseContext.Commercial;
                    DataTable dataTable = ConvertListToDataTable(query);
                    // Create a new Excel package
                    using var package = new ExcelPackage();
                    var excelFilePath =
                        $"BridgeLoan_Adroit_File_{DateTime.Today.ToString("dd-MMM-yyyy")}.xlsx";

                    var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                    // Load the data from DataTable to Excel worksheet
                    worksheet.Cells.LoadFromDataTable(dataTable, true);

                    // Set the content type for Excel files
                    var contentType =
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    // Convert the Excel package to a byte array
                    var fileBytes = package.GetAsByteArray();

                    // Return the file as a content result
                    return File(fileBytes, contentType, excelFilePath);
                }

                r.status = eget ? true : false;
                r.message = eget ? "Record Fetched Successfully" : "No Record Found";
                r.data = query.ToList();
                r.recordCount = query.Count();
                r.recordPageNumber = 1;
                return (Ok(r));
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ReturnObject { status = false, message = ex.Message }
                );
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
                var rec = _repoNew.FindById(id);
                return Ok(rec);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ReturnObject { status = false, message = ex.Message }
                );
            }
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getreturned")]
        public IActionResult GetAllReturned([FromQuery] PaginationWithOutFilterModel obj)
        {
            var r = new ReturnObject();
            bool eget = false;
            try
            {
                var query = _repoNew
                    .AsQueryable()
                    .Where(o => o.Status == DisbursementEnum.Returned.ToString())
                    .ToList();
                var queryDoc = _repoDoc.AsQueryable();
                foreach (var ret in query)
                {
                    ret.comments = _repoCom
                        .AsQueryable()
                        .FirstOrDefault(o => o.DisbursementUniqueId == ret.UniqueId);

                    ret.DocumentationStage = queryDoc
                        .FirstOrDefault(o => o.ObligorDob == ret.DOB && o.PhoneNo == ret.PHONENO)
                        ?.DocumentationStatus;
                }
                var fneRes = query.Skip((obj.PageNumber - 1) * obj.PasgeSize).Take(obj.PasgeSize);
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
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ReturnObject { status = false, message = ex.Message }
                );
            }
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("getdisbursed")]
        public IActionResult GetAllDisburse([FromQuery] FilFormModel obj)
        {
            var r = new ReturnObject();
            bool eget = false;
            try
            {
                IQueryable<Disbursement>? query = null;
                switch (obj.Det)
                {
                    case 1:
                        query = _repo
                            .AsQueryable()
                            .Where(o =>
                                o.Status == DisbursementEnum.Processed.ToString()
                                && o.DateCreated.Date == obj.StartDate
                            );
                        if (!string.IsNullOrEmpty(obj.Bvn))
                            query = query.Where(o => o.BVN == obj.Bvn);
                        break;
                    case 2:
                        query = _repo
                            .AsQueryable()
                            .Where(o => o.Status == DisbursementEnum.Processed.ToString());
                        break;
                    default:
                        break;
                }

                var fneRes = query.Skip((obj.PageNumber - 1) * obj.PasgeSize).Take(obj.PasgeSize);
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
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ReturnObject { status = false, message = ex.Message }
                );
            }
        }

        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("return")]
        public async Task<IActionResult> Return([FromBody] DisbursementFmForReturn obj)
        {
            var res = new ReturnObject { message = "record updated successfully", status = true };
            var RequestTime = DateTime.UtcNow;
            string actionUrl =
                $"{ControllerContext.RouteData.Values["controller"]}/{ControllerContext.RouteData.Values["action"]}";
            try
            {
                DisbursementComment com =
                    new() { DisbursementUniqueId = obj.UniqueId, Comment = obj.Comments };
                _ = await _repoCom.InsertOneAsync(com);
                var rec = _repoNew.FindById(obj.UniqueId);
                if (rec != null)
                {
                    DisbursementNew la =
                        new()
                        {
                            ClientId = rec.ClientId,
                            UniqueId = obj.UniqueId,
                            Surname = obj.Surname,
                            Firstname = obj.Firstname,
                            Middlename = obj.Middlename,
                            EmailAddress = obj.Email,
                            HouseNo = obj.HouseNo,
                            StreetName = obj.StreetName,
                            City = obj.City,
                            State = obj.State,
                            DOB = obj.DOB,
                            BVN = obj.BVN,
                            IdNo = obj.IdNo,
                            Status = DisbursementEnum.Returned.ToString(),
                            IdDateIssued = obj.IdDateIssued,
                            TransferAmount = obj.TransferAmount,
                            PreferredNaration = obj.PreferredNaration,
                            Gender = obj.Gender,
                            RepaymentDate = obj.RepaymentDate,
                            Id = rec.Id,
                            CreatedBy = rec.CreatedBy
                        };
                    _repoNew.ReplaceOne(la);
                }
                _ = LogService_Old.LoggerCreateAsync(
                    JsonConvert.SerializeObject(obj),
                    actionUrl,
                    RequestTime,
                    JsonConvert.SerializeObject(res),
                    "",
                    (int)ServiceLogLevel.Information
                );
                return Ok(res);
            }
            catch (Exception ex)
            {
                _ = LogService_Old.LoggerCreateAsync(
                    JsonConvert.SerializeObject(obj),
                    actionUrl,
                    RequestTime,
                    "",
                    ex.InnerException.ToString(),
                    (int)ServiceLogLevel.Error
                );
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ReturnObject { status = false, message = ex.Message }
                );
            }
        }

        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("returnToProcess")]
        public async Task<IActionResult> ReturnToProcess(
            [FromBody] DisbursementFmFromReturnToProcess obj
        )
        {
            var res = new ReturnObject { message = "record updated successfully", status = true };
            var RequestTime = DateTime.UtcNow;
            string actionUrl =
                $"{ControllerContext.RouteData.Values["controller"]}/{ControllerContext.RouteData.Values["action"]}";
            try
            {
                var rec = _repoNew.FindById(obj.UniqueId);
                if (rec != null)
                {
                    DisbursementNew la =
                        new()
                        {
                            ClientId = rec.ClientId,
                            UniqueId = obj.UniqueId,
                            Surname = obj.Surname,
                            Firstname = obj.Firstname,
                            Middlename = obj.Middlename,
                            EmailAddress = obj.Email,
                            HouseNo = obj.HouseNo,
                            StreetName = obj.StreetName,
                            City = obj.City,
                            State = obj.State,
                            DOB = obj.DOB,
                            BVN = obj.BVN,
                            IdNo = obj.IdNo,
                            Status = DisbursementEnum.Processed.ToString(),
                            IdDateIssued = obj.IdDateIssued,
                            TransferAmount = obj.TransferAmount,
                            PreferredNaration = obj.PreferredNaration,
                            Gender = obj.Gender,
                            RepaymentDate = obj.RepaymentDate,
                            Id = rec.Id,
                            CreatedBy = rec.CreatedBy
                        };
                    _repoNew.ReplaceOne(la);
                }
                _ = LogService_Old.LoggerCreateAsync(
                    JsonConvert.SerializeObject(obj),
                    actionUrl,
                    RequestTime,
                    JsonConvert.SerializeObject(res),
                    "",
                    (int)ServiceLogLevel.Information
                );
                return Ok(res);
            }
            catch (Exception ex)
            {
                _ = LogService_Old.LoggerCreateAsync(
                    JsonConvert.SerializeObject(obj),
                    actionUrl,
                    RequestTime,
                    "",
                    ex.InnerException.ToString(),
                    (int)ServiceLogLevel.Error
                );
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ReturnObject { status = false, message = ex.Message }
                );
            }
        }

        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("add")]
        public async Task<IActionResult> Add([FromBody] DisbursementFm obj)
        {
            var RequestTime = DateTime.UtcNow;
            string actionUrl =
                $"{ControllerContext.RouteData.Values["controller"]}/{ControllerContext.RouteData.Values["action"]}";
            try
            {
                List<DisbursementFm> lstMd = new();
                lstMd.Add(obj);
                var res = await Adds(lstMd, 2);
                _ = LogService_Old.LoggerCreateAsync(
                    JsonConvert.SerializeObject(obj),
                    actionUrl,
                    RequestTime,
                    JsonConvert.SerializeObject(res),
                    "",
                    (int)ServiceLogLevel.Information
                );
                return Ok(res);
            }
            catch (Exception ex)
            {
                _ = LogService_Old.LoggerCreateAsync(
                    JsonConvert.SerializeObject(obj),
                    actionUrl,
                    RequestTime,
                    "",
                    ex.InnerException.ToString(),
                    (int)ServiceLogLevel.Error
                );
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ReturnObject { status = false, message = ex.Message }
                );
            }
        }

        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("addbulk")]
        public async Task<IActionResult> AddBulk([FromForm] DisbursementFmUpload obj)
        {
            List<DisbursementFm> la = new();
            var RequestTime = DateTime.UtcNow;
            string actionUrl =
                $"{ControllerContext.RouteData.Values["controller"]}/{ControllerContext.RouteData.Values["action"]}";
            try
            {
                if (obj.UploadedExcel == null || obj.UploadedExcel.Length == 0)
                    return BadRequest("No file uploaded.");

                using (var stream = new MemoryStream())
                {
                    await obj.UploadedExcel.CopyToAsync(stream);
                    using var package = new ExcelPackage(stream);
                    ExcelPackage.LicenseContext = LicenseContext.Commercial;
                    var worksheet = package.Workbook.Worksheets.First();
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var excelData = new DisbursementFm
                        {
                            Surname = worksheet.Cells[row, 1].Value?.ToString(),
                            Firstname = worksheet.Cells[row, 2].Value?.ToString(),
                            Middlename = worksheet.Cells[row, 3].Value?.ToString(),
                            PHONENO = worksheet.Cells[row, 4].Value?.ToString(),
                            Email = worksheet.Cells[row, 5].Value?.ToString(),
                            Gender = worksheet.Cells[row, 6].Value?.ToString(),
                            HouseNo = worksheet.Cells[row, 7].Value?.ToString(),
                            StreetName = worksheet.Cells[row, 8].Value?.ToString(),
                            City = worksheet.Cells[row, 9].Value?.ToString(),
                            State = worksheet.Cells[row, 10].Value?.ToString(),
                            DOB = worksheet.Cells[row, 11].Value?.ToString(),
                            BVN = worksheet.Cells[row, 12].Value?.ToString(),
                            IdNo = worksheet.Cells[row, 13].Value?.ToString(),
                            IdDateIssued = worksheet.Cells[row, 14].Value?.ToString(),
                            TransferAmount = worksheet.Cells[row, 15].Value?.ToString(),
                            PreferredNaration = worksheet.Cells[row, 16].Value?.ToString(),
                            RepaymentDate = worksheet.Cells[row, 17].Value?.ToString()
                        };

                        la.Add(excelData);
                    }
                }
                if (la.Count <= 0)
                    return Ok("No Record In Excel Uploaded");
                var res = await Adds(la, 1);
                _ = LogService_Old.LoggerCreateAsync(
                    JsonConvert.SerializeObject(obj),
                    actionUrl,
                    RequestTime,
                    JsonConvert.SerializeObject(res),
                    "",
                    (int)ServiceLogLevel.Information
                );
                return Ok(res);
            }
            catch (Exception ex)
            {
                _ = LogService_Old.LoggerCreateAsync(
                    JsonConvert.SerializeObject(obj),
                    actionUrl,
                    RequestTime,
                    "",
                    ex.InnerException.ToString(),
                    (int)ServiceLogLevel.Error
                );
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ReturnObject { status = false, message = ex.Message }
                );
            }
        }

        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("addbulkWithDate")]
        public async Task<IActionResult> AddBulkWithDate([FromForm] DisbursementFmUploadDate obj)
        {
            List<DisbursementFm> la = new();
            var RequestTime = DateTime.UtcNow;
            string actionUrl =
                $"{ControllerContext.RouteData.Values["controller"]}/{ControllerContext.RouteData.Values["action"]}";
            try
            {
                if (obj.UploadedExcel == null || obj.UploadedExcel.Length == 0)
                    return BadRequest("No file uploaded.");

                using (var stream = new MemoryStream())
                {
                    await obj.UploadedExcel.CopyToAsync(stream);
                    using var package = new ExcelPackage(stream);
                    ExcelPackage.LicenseContext = LicenseContext.Commercial;
                    var worksheet = package.Workbook.Worksheets.First();
                    var rowCount = worksheet.Dimension.Rows;
                    for (int row = 2; row <= rowCount; row++)
                    {
                        var excelData = new DisbursementFm
                        {
                            Surname = worksheet.Cells[row, 1].Value?.ToString(),
                            Firstname = worksheet.Cells[row, 2].Value?.ToString(),
                            Middlename = worksheet.Cells[row, 3].Value?.ToString(),
                            PHONENO = worksheet.Cells[row, 4].Value?.ToString(),
                            Email = worksheet.Cells[row, 5].Value?.ToString(),
                            Gender = worksheet.Cells[row, 6].Value?.ToString(),
                            HouseNo = worksheet.Cells[row, 7].Value?.ToString(),
                            StreetName = worksheet.Cells[row, 8].Value?.ToString(),
                            City = worksheet.Cells[row, 9].Value?.ToString(),
                            State = worksheet.Cells[row, 10].Value?.ToString(),
                            DOB = worksheet.Cells[row, 11].Value?.ToString(),
                            BVN = worksheet.Cells[row, 12].Value?.ToString(),
                            IdNo = worksheet.Cells[row, 13].Value?.ToString(),
                            IdDateIssued = worksheet.Cells[row, 14].Value?.ToString(),
                            TransferAmount = worksheet.Cells[row, 15].Value?.ToString(),
                            PreferredNaration = worksheet.Cells[row, 16].Value?.ToString(),
                            StartDate = obj.StartDate,
                            RepaymentDate = worksheet.Cells[row, 17].Value?.ToString()
                        };
                        la.Add(excelData);
                    }
                }

                if (la.Count <= 0)
                    return Ok("No Record In Excel Uploaded");
                var res = await Adds(la, 2);
                _ = LogService_Old.LoggerCreateAsync(
                    JsonConvert.SerializeObject(obj),
                    actionUrl,
                    RequestTime,
                    JsonConvert.SerializeObject(res),
                    "",
                    (int)ServiceLogLevel.Information
                );
                return Ok(res);
            }
            catch (Exception ex)
            {
                _ = LogService_Old.LoggerCreateAsync(
                    JsonConvert.SerializeObject(obj),
                    actionUrl,
                    RequestTime,
                    "",
                    ex.InnerException.ToString(),
                    (int)ServiceLogLevel.Error
                );
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ReturnObject { status = false, message = ex.Message }
                );
            }
        }

        [NonAction]
        static DataTable ConvertListToDataTable(IQueryable<DisbursementNew> personList)
        {
            DataTable dataTable = new DataTable();

            // Assuming all items in the list have the same properties
            foreach (var property in typeof(DisbursementNew).GetProperties())
            {
                dataTable.Columns.Add(property.Name, property.PropertyType);
            }

            foreach (var person in personList)
            {
                DataRow row = dataTable.NewRow();
                foreach (var property in typeof(DisbursementNew).GetProperties())
                {
                    row[property.Name] = property.GetValue(person);
                }
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        [NonAction]
        private async Task<ReturnObject> Adds(List<DisbursementFm> mob, int det)
        {
            var res = new ReturnObject();
            List<Disbursement> lstD = new();
            List<DisbursementNew> lstDa = new();
            foreach (var obj in mob)
            {
                Disbursement la =
                    new()
                    {
                        StartDate = obj.StartDate.GetValueOrDefault(),
                        Surname = obj.Surname,
                        Firstname = obj.Firstname,
                        Middlename = obj.Middlename,
                        EmailAddress = obj.Email,
                        HouseNo = obj.HouseNo,
                        StreetName = obj.StreetName,
                        City = obj.City,
                        State = obj.State,
                        PHONENO = obj.PHONENO,
                        DOB = obj.DOB,
                        Gender = obj.Gender,
                        RepaymentDate = obj.RepaymentDate,
                        BVN = obj.BVN,
                        IdNo = obj.IdNo,
                        Status = DisbursementEnum.Processed.ToString(),
                        IdDateIssued = obj.IdDateIssued,
                        TransferAmount = obj.TransferAmount,
                        PreferredNaration = obj.PreferredNaration,
                        UniqueId = Guid.NewGuid().ToString()
                    };
                //var whatToDelete = Builders<DisbursementNew>.Filter.Eq("StartDate" , obj.StartDate.GetValueOrDefault());
                //await _repoNew.DeleteManyAsync(whatToDelete);
                DisbursementNew lat =
                    new()
                    {
                        StartDate = obj.StartDate.GetValueOrDefault(),
                        Surname = obj.Surname,
                        Firstname = obj.Firstname,
                        Middlename = obj.Middlename,
                        EmailAddress = obj.Email,
                        HouseNo = obj.HouseNo,
                        StreetName = obj.StreetName,
                        City = obj.City,
                        State = obj.State,
                        PHONENO = obj.PHONENO,
                        DOB = obj.DOB,
                        Gender = obj.Gender,
                        RepaymentDate = obj.RepaymentDate,
                        BVN = obj.BVN,
                        IdNo = obj.IdNo,
                        Status = DisbursementEnum.Processed.ToString(),
                        IdDateIssued = obj.IdDateIssued,
                        TransferAmount = obj.TransferAmount,
                        PreferredNaration = obj.PreferredNaration,
                        UniqueId = Guid.NewGuid().ToString()
                    };
                lstD.Add(la);
                lstDa.Add(lat);
            }
            res = det switch
            {
                1 => _repo.InsertMany(lstD),
                2 => _repoNew.InsertMany(lstDa),
                _ => null
            };
            //var whatToDelete = Builders<DisbursementNew>.Filter.Eq("CustomerId", obj.);
            //await _repoNew.DeleteManyAsync(whatToDelete);
            return res;
        }
    }
}
