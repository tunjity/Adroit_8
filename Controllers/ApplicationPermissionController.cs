using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Adroit_v8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationPermissionController : AuthController
    {
        private readonly ISSMongoRepository<ApplicationPage> _ApplicationPage;
        private readonly ISSMongoRepository<ApplicationPermissionGetDto1> _ApplicationPermissionActionListGet;
        private readonly ISSMongoRepository<ApplicationGetModuleDTO1> _ApplicationModuleGet1;

        public ApplicationPermissionController(
            ISSMongoRepository<ApplicationPage> ApplicationPage,
            ISSMongoRepository<ApplicationPermissionGetDto1> ApplicationPermissionActionListGet,
            ISSMongoRepository<ApplicationGetModuleDTO1> ApplicationModuleGet1, IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            _ApplicationPermissionActionListGet = ApplicationPermissionActionListGet;
            _ApplicationModuleGet1 = ApplicationModuleGet1;
            _ApplicationPage = ApplicationPage;

        }
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(NewPermissionReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(NewPermissionReturnObject))]
        [Route("GetUserApplicationPagesAndPermission")]
        public async Task<NewPermissionReturnObject> GetUserApplicationPagesAndPermission()
        {
            AuthDto auth = new AuthDto();
            var userDet = GetAuthData();
            if (userDet == null)
                return new NewPermissionReturnObject { status = false, statusCode = 400, message = "Access denied" };
            List<PermissionReturn> objResList = new();
            List<PermissionReturnObject> objRes = new();
            List<ApplicationPageII> allFormII = new();
            ApplicationPageII FormII = new();
            var allMod = _ApplicationModuleGet1.AsQueryable().Where(o => o.ApplicationId == userDet.ApplicationId).ToList();
            List<ApplicationPage>? allForm = _ApplicationPage.AsQueryable().Where(o => o.ApplicationId == userDet.ApplicationId).ToList();
            var allpermission = _ApplicationPermissionActionListGet.AsQueryable().Where(o => o.ApplicationId == userDet.ApplicationId).ToList();

            foreach (var module in allMod)
            {
                PermissionReturnObject res = new();
                PermissionReturn resII = new();
                res.ModuleName = module.ApplicationModuleName;
                List<ApplicationPage>? forms = allForm.Where(f => f.ApplicationModuleId == module.ApplicationModuleId).ToList();
                foreach (var form in forms)
                {

                    ApplicationPageII applicationPageII = new();

                    applicationPageII.ApplicationId = form.ApplicationId;
                    applicationPageII.ApplicationModuleId = form.ApplicationModuleId;
                    applicationPageII.ApplicationPageId = form.ApplicationPageId;
                    applicationPageII.ApplicationPageName = form.ApplicationPageName;
                    applicationPageII.ApplicationPageDescription = form.ApplicationPageDescription;
                    applicationPageII.ApplicationPageCode = form.ApplicationPageCode;
                    form.permissions.AddRange(allpermission.Where(p => p.ApplicationPageId == form.ApplicationPageId));

                    foreach (var permission in form.permissions)
                    {
                        applicationPageII.permissions.Add(new ApplicationPermissionGetDto1II
                        {
                            ApplicationId = permission.ApplicationId,
                            ApplicationRoleId = permission.ApplicationRoleId,
                            ApplicationPageId = permission.ApplicationPageId,
                            ApplicationPermissionId = permission.ApplicationPermissionId,
                            CanView = permission.CanView,
                            CanAdd = permission.CanAdd,
                            CanUpdate = permission.CanUpdate,
                            CanRemove = permission.CanRemove,
                            CanApprove = permission.CanApprove,
                            CanReject = permission.CanReject,
                            CanDecline = permission.CanDecline,
                            CanAssign = permission.CanAssign,
                            CanReAssign = permission.CanReAssign,
                            CanReview = permission.CanReview,
                            CanAdjust = permission.CanAdjust,
                            CanComment = permission.CanComment,
                            CanDownload = permission.CanDownload,
                            CanUpload = permission.CanUpload,
                            CanSearch = permission.CanSearch,
                            CanDisburse = permission.CanDisburse,
                            CanReturn = permission.CanReturn,
                            CanDecide = permission.CanDecide,
                            CanEditRepayment = permission.CanEditRepayment

                        });
                    }
                    allFormII.Add(applicationPageII);
                }
                res.Forms.AddRange(allFormII);

                objRes.Add(res);

                resII.modules.AddRange(objRes);
                objResList.Add(resII);
            }
            return new NewPermissionReturnObject { status = true, statusCode = 200, message = "Successful", data = objResList };
        }

    }
}
