using Adroit_v8.Config;
using Adroit_v8.Models.Administration;
using Adroit_v8.Models.CRM;
using Adroit_v8.Models.FormModel;
using Adroit_v8.MongoConnections;
using Adroit_v8.MongoConnections.CRM;
using Adroit_v8.MongoConnections.Models;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using static Adroit_v8.EnumFile.EnumHelper;

namespace Adroit_v8.Controllers.CRM
{
    [Route("api/CRM/[controller]")]
    [ApiController]
    [Authorize]
    public class ClientController : AuthController
    {
        private readonly CreditWaveContext _context;
        private ICRMGenericRepository<Customer> _repo;
        private readonly IMongoRepository<ClientBank> _repoB;
        private readonly IMongoRepository<ClientDoc> _repoDoc;
        private readonly IMongoRepository<ClientEmploymentHistory> _reo;
        private readonly IMongoRepository<ClientNextOfKin> _repoNK;
        private readonly IMongoRepository<ClientResidentialInfo> _repoRInfo;
        private readonly IMongoRepository<CustomerStageHolder> _repoCustomerStageHolder;
        private readonly IMongoRepository<ClientEmploymentSector> _repoEmploymentSector;
        private string errMsg = "Unable to process request, kindly try again";
        private readonly IConfiguration _config;
        public ClientController(ICRMGenericRepository<Customer> repo, IMongoRepository<ClientDoc> repoDoc, IMongoRepository<ClientBank> repoB, IMongoRepository<ClientNextOfKin> repoNK, IMongoRepository<ClientEmploymentHistory> reo, IMongoRepository<ClientResidentialInfo> repoRInfo, IMongoRepository<ClientEmploymentSector> repoEmploymentSector, IConfiguration config,
            IMongoRepository<CustomerStageHolder> repoCustomerStageHolder, CreditWaveContext context, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _repo = repo;
            _repoDoc = repoDoc;
            _repoB = repoB;
            _repoNK = repoNK;
            _reo = reo;
            _repoRInfo = repoRInfo;
            _repoEmploymentSector = repoEmploymentSector;
            _config = config;
            _repoCustomerStageHolder = repoCustomerStageHolder;
            _context = context;
        }

        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("add")]
        public IActionResult Add([FromBody] CrmFormModel obj)
        {
            //Customer? ret = new();
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Saved Successfully";
            try
            {
                //Bvn Validation
                if (string.IsNullOrWhiteSpace(obj.Bvn))
                {
                    return (StatusCode(StatusCodes.Status500InternalServerError, new ReturnObject
                    {
                        status = false,
                        message = "Please Enter Valid BVN"
                    }));
                }

                Customer cus = new()
                {
                    CustomerRef = Guid.NewGuid().ToString(),
                    TitleId = obj.TitleId,
                    GenderId = obj.GenderId,
                    DateOfBirth = obj.DOB,
                    FirstName = obj.FirstName,
                    LastName = obj.LastName,
                    MiddleName = obj.MiddleName,
                    MaritalStatusId = obj.MaritalStatusId,
                    NumberOfDependent = obj.NoOfDependantId,
                    EducationalLevelId = obj.EducationLevelId,
                    PhoneNumber = obj.PhoneNumber,
                    AlternativePhoneNumber = obj.AltPhoneNumber,
                    EmailAddress = obj.Email,
                    Bvn = obj.Bvn,
                    UniqueId = Guid.NewGuid().ToString(),
                    RegistrationChannelId = (int)CustomerRegistrationChannel.Default
                };
                _repo.Insert(cus);

                //get recently added cus to update escrow
                var res = _context.Customers.FirstOrDefault(o => o.CustomerRef == cus.CustomerRef);
                if (res is not null)
                {
                    var es = $"{res.Id}-{res.PhoneNumber.Substring(res.PhoneNumber.Length - 4)}";
                    res.EscrowAccountNumber = es.Replace("-", "");
                    _context.SaveChanges();
                }

                CustomerStageHolder st = new()
                {
                    CustomerId = cus.Id.ToString(),
                    StageId = CustomerRegistrationStage.StageCustomer.ToString(),
                    UniqueId = Guid.NewGuid().ToString(),
                };
                _repoCustomerStageHolder.InsertOne(st);

                ClientEmploymentSector stb = new()
                {
                    HasBVN = obj.HasBVN,
                    EmploymentSector = obj.EmploymentSector,
                    UniqueId = Guid.NewGuid().ToString(),
                };
                _repoEmploymentSector.InsertOne(stb);
                r.data = cus;
                // }
                return Ok(r);
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
        [Route("edit")]
        public async Task<ActionResult> Edit([FromBody] CrmFormModelForEdit obj)
        {
            Customer? ret = new();
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Saved Successfully";
            try
            {
                //get the present stage
                var cusStage = _repoCustomerStageHolder.AsQueryable().Where(o => o.CustomerId.Equals(obj.CusId.ToString())).OrderBy(o => o.StageId).FirstOrDefault();
                if (cusStage == null)
                {
                    r.message = "No reg stage for this user"; r.status = false;
                    return Ok(r);
                }

                ret = _context.Customers.FirstOrDefault(o => o.Id == obj.CusId);
                if (ret == null)
                {
                    r.status = false;
                    r.message = "Record Doesnt Exist";
                }
                else
                {
                    var res = await _context.Customers.Where(o => o.Id == obj.CusId)
                     .ExecuteUpdateAsync(setters => setters
                     .SetProperty(b => b.TitleId, obj.TitleId)
                     .SetProperty(b => b.GenderId, obj.GenderId)
                     .SetProperty(b => b.DateOfBirth, obj.DOB)
                     .SetProperty(b => b.FirstName, obj.FirstName)
                     .SetProperty(b => b.LastName, obj.LastName)
                     .SetProperty(b => b.MiddleName, obj.MiddleName)
                     .SetProperty(b => b.MaritalStatusId, obj.MaritalStatusId)
                     .SetProperty(b => b.NumberOfDependent, obj.NoOfDependantId)
                     .SetProperty(b => b.EducationalLevelId, obj.EducationLevelId)
                     .SetProperty(b => b.PhoneNumber, obj.PhoneNumber)
                     .SetProperty(b => b.AlternativePhoneNumber, obj.AltPhoneNumber)
                     .SetProperty(b => b.EmailAddress, obj.Email)
                     );
                    var getRes = await _context.Customers.FirstOrDefaultAsync(o => o.Id == obj.CusId);
                    r.data = new
                    {
                        titleId = getRes?.TitleId,
                        firstName = getRes?.FirstName,
                        middleName = getRes?.MiddleName,
                        lastName = getRes?.LastName,
                        genderId = getRes?.GenderId,
                        dateOfBirth = getRes?.DateOfBirth,
                        maritalStatusId = getRes?.MaritalStatusId,
                        numberOfDependent = getRes?.NumberOfDependent,
                        educationalLevelId = getRes?.EducationalLevelId,
                        phoneNumber = getRes?.PhoneNumber,
                        bvn = getRes?.Bvn,
                        alternativePhoneNumber = getRes?.Bvn,
                        emailAddress = getRes?.EmailAddress,
                        nin = getRes?.Nin,
                        customerRef = getRes?.CustomerRef,
                        isDeleted = getRes?.IsDeleted,
                        isActive = getRes?.IsActive,
                        customerCentricStatus = getRes?.CustomerCentricStatus,
                        isBlackListedCustomer = getRes?.IsBlackListedCustomer,
                        dateCreated = getRes?.DateCreated,
                        registrationStageId = getRes?.RegistrationStageId,
                        registrationChannelId = getRes?.RegistrationChannelId,
                        // clientCode = getRes?.ClientCode,
                        hasValidBvn = getRes?.HasValidBvn,
                        id = getRes?.Id,
                        statusName = getRes?.StatusName,
                        status = getRes?.Status,
                        sentTitle = obj.TitleId,
                        sentGender = obj.GenderId,
                        sentMaritalStatus = obj.MaritalStatusId,
                        sentEduLevel = obj.EducationLevelId,
                        sentNoOfDependantId = obj.NoOfDependantId
                    };

                }
                return Ok(r);
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
        [Route("getbycustId/{cusId}")]
        public IActionResult Get([FromRoute] int cusId)
        {
            var doc = new ReturnDoc();
            var r = new ReturnObject();
            r.status = true;
            r.message = "Record Fetched Successfully";
            try
            {
                var jes = from re in _context.Customers
                          join ge in _context.Genders on re.GenderId equals ge.Id
                          join ms in _context.Maritalstatuses on re.MaritalStatusId equals ms.Id
                          join ed in _context.Educationallevels on re.EducationalLevelId equals ed.Id
                          join tt in _context.Titles on re.TitleId equals tt.Id
                          where re.Id == cusId
                          select new { title = tt.Name,uniqueId =re.UniqueId, bvn = re.Bvn, titleId = re.TitleId, firstName = re.FirstName, middleName = re.MiddleName, lastName = re.LastName, phone = re.PhoneNumber, altPhone = re.AlternativePhoneNumber, email = re.EmailAddress, gender = ge.Name, genderId = re.GenderId, dob = re.DateOfBirth, marritalStatus = ms.Name, marritalStatusId = re.MaritalStatusId, noOfde = re.NumberOfDependent, noOfdeId = re.NumberOfDependent, eduLevel = ed.Name, eduLevelId = re.EducationalLevelId };
                var newjes = jes.FirstOrDefault();
                if (newjes != null)
                {
                    var dc = _repoDoc.AsQueryable().FirstOrDefault(re => re.CustomerId == cusId.ToString());
                    if (dc != null)
                    {
                        var signName = new ReturnDocFileName();
                        signName.Name = dc.ESignatureFileName;
                        var poeName = new ReturnDocFileName();
                        poeName.Name = dc.ProofOfEmploymentFileName;
                        var pasName = new ReturnDocFileName();
                        pasName.Name = dc.PassportPhotographFileName;
                        var resName = new ReturnDocFileName();
                        resName.Name = dc.ProofOfResidenceFileName;
                        var poiName = new ReturnDocFileName();
                        poiName.Name = dc.ProofOfIdentityFileName;
                        doc.ProofOfEmploymentFileName = poeName;
                        doc.ProofOfIdentityFileName = poiName;
                        doc.PassportPhotographFileName = pasName;
                        doc.ProofOfResidenceFileName = resName;
                        doc.UniqueId = dc.UniqueId;
                        doc.CustomerId = dc.CustomerId;
                        doc.PassportPhotograph = Helper.ConvertFromPathToBase64( dc.PassportPhotograph);
                        doc.ESignature = Helper.ConvertFromPathToBase64 (dc.ESignature);
                        doc.ESignatureFileName = signName;
                        doc.ProofOfResidence = Helper.ConvertFromPathToBase64(dc.ProofOfResidence);
                        doc.ProofOfResidenceType = dc.ProofOfResidenceType;
                        doc.ProofOfIdentity = Helper.ConvertFromPathToBase64(dc.ProofOfIdentity);
                        doc.ProofOfIdentityType = dc.ProofOfIdentityType;
                        doc.ProofOfIdentityExpiryDate = dc.ProofOfIdentityExpiryDate;
                        doc.ProofOfEmployment = Helper.ConvertFromPathToBase64(dc.ProofOfEmployment);
                        doc.ProofOfEmploymentType = dc.ProofOfEmploymentType;
                    }
                    var nk = _repoNK.AsQueryable().FirstOrDefault(re => re.CustomerId == cusId.ToString());
                    var bk = _repoB.AsQueryable().FirstOrDefault(re => re.CustomerId == cusId.ToString());
                    var emp = _reo.AsQueryable().FirstOrDefault(re => re.CustomerId == cusId.ToString());
                    var rI = _repoRInfo.AsQueryable().FirstOrDefault(re => re.CustomerId == cusId.ToString());
                    var st = _repoCustomerStageHolder.AsQueryable().FirstOrDefault(re => re.CustomerId == cusId.ToString());
                    r.data = new
                    {
                        stage = st,
                        personalandcontactInformation = newjes,
                        employerInformation = emp,
                        nextOfKin = nk,
                        bankDetail = bk,
                        residentialInformation = rI,
                        documentUpload = doc
                    };
                }
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
        [Route("getall")]
        public IActionResult GetAll([FromQuery] PaginationWithOutFilterModel obj)
        {
            var r = new ReturnObject(); bool eget = false;
            try
            {
                var query = _repo.GetAll().OrderByDescending(o=>o.Id);
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
                return (StatusCode(StatusCodes.Status500InternalServerError, new ReturnObject
                {
                    status = false,
                    message = ex.Message
                }));
            }
        }
    }
}
