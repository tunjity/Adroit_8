
namespace Adroit_v8.Controllers
{
    public class AuthController : ControllerBase
    {
        public IHttpContextAccessor _httpContextAccessor;
        public AuthController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        [NonAction]
        public AuthDto GetAuthData()
        {
            AuthDto auth = new AuthDto();
            try
            {
                auth.ClientId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ClientId").Value;
                auth.FirstName = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "FirstName").Value;
                auth.LastName = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "LastName").Value;
                auth.ApplicationId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ApplicationId").Value;
                auth.email = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "email") != null ? _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "email").Value : "";
                auth.UserName = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserName").Value;
                auth.UserId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
                auth.CreatedBy = $"{auth.UserName}, {auth.FirstName} {auth.LastName}| {auth.UserId}";
            }
            catch (Exception ex)
            {
                //Log exception and claims to db as Unautorized Access
                //var difference = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault();

                return null;
            }
            return auth;
        }

        //[NonAction]
        //private ClaimsPrincipal ValidateJwtToken(string token)
        //{
        //    try
        //    {
        //        var tokenHandler = new JwtSecurityTokenHandler();
        //        var validationparameters = new TokenValidationParameters
        //        {
        //            ValidateIssuerSigningKey = true,
        //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])),
        //            ValidateIssuer = true,
        //            ValidIssuer = _config["Jwt:Issuer"],
        //            ValidAudience = _config["Jwt:Audience"],
        //            ValidateAudience = true,
        //            ValidateLifetime = true,
        //            ClockSkew = TimeSpan.Zero
        //        };

        //        SecurityToken validatedToken;
        //        ClaimsPrincipal claimsPrincipal = tokenHandler.ValidateToken(token, validationparameters, out validatedToken);
        //        return claimsPrincipal;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}
    }
}
