using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Adroit_v8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationPermissionController : AuthController
    {
        private readonly ISSMongoRepository<ApplicationGetPageDTO> _ApplicationPageGet;
        private readonly ISSMongoRepository<ApplicationGetDTO> _ApplicationGet;
        private readonly ISSMongoRepository<UserApplicationRoleGetDTO> _UserApplicationRoleGet;
        private readonly ISSMongoRepository<ApplicationRoleGetDTO> _ApplicationRoleGet;
        private readonly ISSMongoRepository<ApplicationPermissionGetDto1> _ApplicationPermissionActionListGet;
        private readonly ISSMongoRepository<ApplicationGetModuleDTO1> _ApplicationModuleGet1;

        public ApplicationPermissionController(
            ISSMongoRepository<UserApplicationRoleGetDTO> UserApplicationRoleGet,
            ISSMongoRepository<ApplicationRoleGetDTO> ApplicationRoleGet,
            ISSMongoRepository<ApplicationPermissionGetDto1> ApplicationPermissionActionListGet,
            ISSMongoRepository<ApplicationGetModuleDTO1> ApplicationModuleGet1, ISSMongoRepository<ApplicationGetDTO> ApplicationGet,
            ISSMongoRepository<ApplicationGetPageDTO> ApplicationPageGet, IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            _ApplicationGet = ApplicationGet;
            _UserApplicationRoleGet = UserApplicationRoleGet;
            _ApplicationRoleGet = ApplicationRoleGet;
            _ApplicationPermissionActionListGet = ApplicationPermissionActionListGet;
            _ApplicationModuleGet1 = ApplicationModuleGet1;
            _ApplicationPageGet = ApplicationPageGet;

        }
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("GetUserApplicationPagesAndPermission")]
        public async Task<ReturnObject> GetUserApplicationPagesAndPermission()
        {
            AuthDto auth = new AuthDto();
            var userDet = GetAuthData();
            if (userDet == null)
                return new ReturnObject { status = false, statusCode = 400, message = "Access denied" };
            var userApplicationRoleII = _UserApplicationRoleGet.AsQueryable().ToList();
            var userApplicationRole = userApplicationRoleII.FirstOrDefault(o => o.ApplicationId == userDet.ApplicationId
             && o.UserId == userDet.UserId);

            if (userApplicationRole == null)
                return new ReturnObject { status = false, statusCode = 400, message = "Access denied" };

            var userApplicationRoleId = userApplicationRole.ApplicationRoleId;

            //get the user application pages and permission using the UserApplicationRoleId and the ApplicationId and the ClientId
            var applicationPages = _ApplicationPageGet.AsQueryable().ToList();
            applicationPages = applicationPages.Where(o => o.IsDeleted == 0 && o.ApplicationId == userDet.ApplicationId).ToList();


            //get the application pages and permision
            var applicationPermission = _ApplicationPermissionActionListGet.AsQueryable().Where(o => o.IsDeleted == 0 && o.ApplicationId == userDet.ApplicationId && o.ApplicationRoleId == userApplicationRoleId).ToList();

            //Get All the Applications
            var applicationList = _ApplicationGet.AsQueryable().Where(o => o.IsDeleted == 0).ToList();

            //get the application role 
            var applicationRoleList = _ApplicationRoleGet.AsQueryable().Where(o => o.IsDeleted == 0).ToList();
            //get the application page
            var applicationPageList = _ApplicationPageGet.AsQueryable().Where(o => o.IsDeleted == 0).ToList();

            var applicationModules = _ApplicationModuleGet1.AsQueryable().Where(o => o.IsDeleted == 0).ToList();


            var applicationpagefinalQuery = from a in applicationPages

                                            join c in applicationModules on a.ApplicationModuleId equals c.ApplicationModuleId
                                            join d in applicationList on a.ApplicationId equals d.ApplicationId
                                            select new ApplicationGetPageDTO
                                            {
                                                Id = a.Id,
                                                ApplicationPageId = a.ApplicationPageId,
                                                ApplicationId = a.ApplicationId,
                                                ApplicationName = d.ApplicationName,
                                                ApplicationModuleId = a.ApplicationModuleId,
                                                ApplicationModuleName = c.ApplicationModuleName,
                                                ApplicationPageName = a.ApplicationPageName,
                                                ApplicationPageCode = a.ApplicationPageCode,
                                                ApplicationPageDescription = a.ApplicationPageDescription,
                                                DateCreated = a.DateCreated,
                                                IsDeleted = a.IsDeleted,
                                                ClientId = a.ClientId
                                            };
            var finalQuery = from a in applicationPermission

                             join c in applicationList on a.ApplicationId equals c.ApplicationId
                             join d in applicationRoleList on a.ApplicationRoleId equals d.ApplicationRoleId
                             join e in applicationPageList on a.ApplicationPageId equals e.ApplicationPageId
                             select new ApplicationPermissionGetDTo1
                             {
                                 Id = a.Id,
                                 ApplicationPermissionId = a.ApplicationPermissionId,
                                 ApplicationId = a.ApplicationId,
                                 ApplicationRoleId = d.ApplicationRoleId,
                                 ApplicationPageId = e.ApplicationPageId,
                                 ApplicationPageName = e.ApplicationPageName,
                                 ApplicationRoleName = d.ApplicationRoleName,
                                 ApplicationName = c.ApplicationName,
                                 DateCreated = a.DateCreated,
                                 IsDeleted = a.IsDeleted,
                                 CanView = a.CanView,
                                 CanUpdate = a.CanUpdate,
                                 CanAdd = a.CanAdd,
                                 CanApprove = a.CanApprove,
                                 CanReject = a.CanReject,
                                 CanRemove = a.CanRemove,
                                 CanDecline = a.CanDecline,
                                 CanAssign = a.CanAssign,
                                 CanReAssign = a.CanReAssign,
                                 CanReview = a.CanReview,
                                 CanAdjust = a.CanAdjust,
                                 CanComment = a.CanComment,
                                 CanDownload = a.CanDownload,
                                 CanUpload = a.CanUpload,
                                 CanSearch = a.CanSearch,
                                 CanDisburse = a.CanDisburse,
                                 CanReturn = a.CanReturn,
                                 CanDecide = a.CanDecide,
                                 CanEditRepayment = a.CanEditRepayment,
                             };

            UserApplicationPagesAndPermission p = new()
            {

                Modules = applicationModules,
                Pages = applicationpagefinalQuery,
                Permission = finalQuery,
            };


            return new ReturnObject { status = true, statusCode = 200, message = "Successful", data = p };
        }

    }
}
