using Adroit_v8.Config;
using Adroit_v8.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using static Adroit_v8.EnumFile.EnumHelper;
using static MongoDB.Libmongocrypt.CryptContext;

namespace Adroit_v8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationPermissionController : AuthController
    {
        private readonly SSOSettings _FileFolderSettings;

        public ApplicationPermissionController(
            IOptions<SSOSettings> fileFolderSettings,
            IHttpContextAccessor httpContextAccessor)

            : base(httpContextAccessor)
        {

            _httpContextAccessor = httpContextAccessor;
            _FileFolderSettings = fileFolderSettings.Value;

        }


        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(NewPermissionReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(NewPermissionReturnObject))]
        [Route("get_user_application_pages_permission/{UserId}")]
        public async Task<IActionResult> GetUserApplicationPagesAndPermission([FromRoute] string UserId)
        {
            using var httpclient = new HttpClient();
            StringContent? requestApi = null;
            HttpResponseMessage? rawResponse = null;
            string actionUrl = $"{this.ControllerContext.RouteData.Values["controller"].ToString()}/{this.ControllerContext.RouteData.Values["action"].ToString()}";
            var response = new NewPermissionReturnObject();
            var RequestTime = DateTime.Now;
            try
            {
                //get the user applicationid
                var ApplicationId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ApplicationId").Value.Trim();
                var ClientId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ClientId").Value.Trim();
                var url = _FileFolderSettings.Permission;
                string xApiKey = _FileFolderSettings.XApiKey;
                url = url.Replace("@@UserId", UserId).Replace("@@ApplicationId", ApplicationId).Replace("@@ClientId", ClientId);

                httpclient.DefaultRequestHeaders.Add("XApiKey", xApiKey);
                rawResponse = await httpclient.GetAsync(url);
                var r1 = await rawResponse.Content.ReadAsStringAsync();
                int statusCode = (int)rawResponse.StatusCode;
                var r = JsonConvert.DeserializeObject<ApiResponse>(r1);
                return Ok(r);
            }
            catch (Exception ex)
            {
                LogService.LoggerCreateAsync("", actionUrl, RequestTime, "", ex.ToString(), (int)ServiceLogLevel.Exception);
                return StatusCode(StatusCodes.Status500InternalServerError, new NewPermissionReturnObject
                {
                    status = false,
                    message = "Unable to process request, kindly try again"
                });
            }
        }

    }
}
