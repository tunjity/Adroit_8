using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Adroit_v8.Controllers
{

    public class CustomAuthorizeAttribute : TypeFilterAttribute
    {
        public CustomAuthorizeAttribute(string claimType, string claimValue) : base(typeof(ClaimRequirementFilter))
        {
            Arguments = new object[] { new Claim(claimType, claimValue) };
        }
    }

    public class ClaimRequirementFilter : IAuthorizationFilter
    {
        readonly Claim _claim;

        public ClaimRequirementFilter(Claim claim)
        {
            _claim = claim;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //Validate Authentication
            var user = context.HttpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new StatusCodeResult((int)System.Net.HttpStatusCode.Unauthorized);
                return;
            }

            #region Validate Authorization
            AuthDto auth = new AuthDto();
            auth.IsOtpVerified = Convert.ToBoolean(context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "IsOtpVerified").Value);
            auth.ClientId = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ClientId").Value;
            auth.FirstName = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "FirstName").Value;
            auth.LastName = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "LastName").Value;
            auth.ApplicationId = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ApplicationId").Value;
            auth.email = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "email") != null ? context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "email").Value : "";
            auth.UserName = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserName").Value;
            auth.UserId = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            auth.CreatedBy = $"{auth.UserName}, {auth.FirstName} {auth.LastName}| {auth.UserId}";

            string formName = _claim.Type;
            string operation = _claim.Value;


            //Validate claims/Authorization
            var hasClaim = true;

            //using (IDbConnection cn = new SqlConnection(defaultConnectionString))
            //{
            //    string storedProcName = "uspGetAllRoles";
            //    DynamicParameters param = new DynamicParameters();
            //    param.Add("Id", employeeId);
            //    param.Add("FormName", _claim.Type);
            //    param.Add("Operation", operation);
            //    hasClaim = cn.QueryFirstOrDefaultAsync<bool>(storedProcName, param, commandType: CommandType.StoredProcedure).Result;
            //}

            //var hasClaim = context.HttpContext.User.Claims.Any(c => c.Type == _claim.Type && c.Value == _claim.Value);
            if (!hasClaim)
            {
                context.Result = new ForbidResult();
            }

            #endregion
        }
    }

}
