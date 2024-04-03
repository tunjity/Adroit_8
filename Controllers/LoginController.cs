using Adroit_v8.MongoConnections;
using Adroit_v8.Service;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using System.Text;
using static Adroit_v8.Config.Helper;
using static Adroit_v8.EnumFile.EnumHelper;

namespace Adroit_v8.Controllers
{
    [Route("api/Adroit/[controller]")]
    [ApiController]
    public class LoginController :ControllerBase
    {
        private Microsoft.Extensions.Caching.Memory.IMemoryCache cache;
        private readonly IConfiguration _config;
        string errMsg = "Unable to process request, kindly try again";
        string clientId = "";
        private readonly IMapper _mapper;
        public LoginController(IConfiguration config, IMapper mapper)
        {
            _config = config;
            clientId = _config.GetSection("MongoDB").GetSection("ConnectionURI").Value;
            cache = cache;
            _mapper = mapper;
        }

        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("ValidateUser")]
        public async Task<IActionResult> ValidateUser([FromBody] ValidateUser obj)
        {
            var RequestTime = DateTime.UtcNow;
            string actionUrl = $"{ControllerContext.RouteData.Values["controller"]}/{ControllerContext.RouteData.Values["action"]}";


            try
            {
                string? clientId = _config.GetSection("SSO").GetSection("clientId").Value;
                string? xApiKey = _config.GetSection("SSO").GetSection("XApiKey").Value;
                string? loggerServiceUrl = _config.GetSection("SSO").GetSection("baseurl").Value;

                loggerServiceUrl += "api/Login/validate_login_details";
                var requestApi = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
                using var httpclient = new HttpClient();
                httpclient.DefaultRequestHeaders.Add("XApiKey", xApiKey);
                using var rawResponse = await httpclient.PostAsync(loggerServiceUrl, requestApi);
                int statusCode = (int)rawResponse.StatusCode;
                if (statusCode != 200)
                {
                    _ = LogService_Old.LoggerCreateAsync(JsonConvert.SerializeObject(obj), actionUrl, RequestTime, JsonConvert.SerializeObject(obj), rawResponse.Content.ReadAsStringAsync().ToString(), (int)ServiceLogLevel.Error);
                    return Ok("An Error Occured");
                }
                var res = rawResponse.Content.ReadAsStringAsync();
                var newRes = JsonConvert.DeserializeObject<APIResponseforAll>(res.Result);
                return Ok(newRes);
            }
            catch (Exception ex)
            {
                return Ok("An Error Occured");
            }
        }

        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("UserLogin")]
        public async Task<ReturnObject> UserLogin([FromBody] Login obj)
        {
            var resp = new ReturnObject();
            var RequestTime = DateTime.UtcNow;
            string actionUrl = $"{ControllerContext.RouteData.Values["controller"]}/{ControllerContext.RouteData.Values["action"]}";

            resp.message = "Incorrect Username Or Password";
            try
            {
                string? clientId = _config.GetSection("SSO").GetSection("clientId").Value;
                string? xApiKey = _config.GetSection("SSO").GetSection("XApiKey").Value;
                string? loggerServiceUrl = _config.GetSection("SSO").GetSection("baseurl").Value;

                var mda = _mapper.Map<LoginRootBase>(obj);
                mda.clientId = clientId;
                loggerServiceUrl += "api/Login/user_login";
                var requestApi = new StringContent(JsonConvert.SerializeObject(mda), Encoding.UTF8, "application/json");
                using var httpclient = new HttpClient();
                httpclient.DefaultRequestHeaders.Add("XApiKey", xApiKey);
                using var rawResponse = await httpclient.PostAsync(loggerServiceUrl, requestApi);
                int statusCode = (int)rawResponse.StatusCode;
                if (statusCode != 200)
                {
                    _ = LogService_Old.LoggerCreateAsync(JsonConvert.SerializeObject(obj), actionUrl, RequestTime, JsonConvert.SerializeObject(resp), rawResponse.Content.ReadAsStringAsync().ToString(), (int)ServiceLogLevel.Error);
                    return resp;
                }
                resp.status = true;
                resp.message = "User Exist";
                var res = rawResponse.Content.ReadAsStringAsync();

                resp.data = JsonConvert.DeserializeObject<APIResponseWithToken>(res.Result);
                return resp;
            }
            catch (Exception ex)
            {
                return resp;
            }
        }
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("GetByUserId")]
        public async Task<ReturnObject> GetByUserId(string userId)
        {
            var resp = new ReturnObject();

            resp.message = "Incorrect Username Or Password";
            try
            {
                string? clientId = _config.GetSection("SSO").GetSection("clientId").Value;
                string? xApiKey = _config.GetSection("SSO").GetSection("XApiKey").Value;
                string? loggerServiceUrl = _config.GetSection("SSO").GetSection("baseurl").Value;
                var mda = new { clientId = clientId, userId = userId };
                loggerServiceUrl += "api/Login/user_login";
                var requestApi = new StringContent(JsonConvert.SerializeObject(mda), Encoding.UTF8, "application/json");
                using var httpclient = new HttpClient();
                httpclient.DefaultRequestHeaders.Add("XApiKey", xApiKey);
                using var rawResponse = await httpclient.PostAsync(loggerServiceUrl, requestApi);
                int statusCode = (int)rawResponse.StatusCode;
                if (statusCode != 200)
                {
                    //  _ = LogService_Old.LoggerCreateAsync(JsonConvert.SerializeObject(obj), actionUrl, RequestTime, JsonConvert.SerializeObject(resp), rawResponse.Content.ReadAsStringAsync().ToString(), (int)ServiceLogLevel.Error);
                    return resp;
                }
                resp.status = true;
                resp.message = "User Exist";
                var res = rawResponse.Content.ReadAsStringAsync();

                resp.data = JsonConvert.DeserializeObject<APIResponseforAll>(res.Result);
                return resp;
            }
            catch (Exception ex)
            {
                return resp;
            }
        }
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ReturnObject))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Type = typeof(ReturnObject))]
        [Route("ResendOTP")]
        public async Task<ReturnObject> ResendOTP([FromBody] ValidateUser obj)
        {
            var resp = new ReturnObject();
            var RequestTime = DateTime.UtcNow;
            string actionUrl = $"{ControllerContext.RouteData.Values["controller"]}/{ControllerContext.RouteData.Values["action"]}";

            resp.message = "Incorrect Username Or Password";
            try
            {
                string? clientId = _config.GetSection("SSO").GetSection("clientId").Value;
                string? xApiKey = _config.GetSection("SSO").GetSection("XApiKey").Value;
                string? loggerServiceUrl = _config.GetSection("SSO").GetSection("baseurl").Value;

                var mda = _mapper.Map<Rootobject>(obj);
                mda.clientId = clientId;
                loggerServiceUrl += "api/Login/resend_user_login_otp";
                var requestApi = new StringContent(JsonConvert.SerializeObject(mda), Encoding.UTF8, "application/json");
                using var httpclient = new HttpClient();
                httpclient.DefaultRequestHeaders.Add("XApiKey", xApiKey);
                using var rawResponse = await httpclient.PostAsync(loggerServiceUrl, requestApi);
                int statusCode = (int)rawResponse.StatusCode;
                if (statusCode != 200)
                {
                    _ = LogService_Old.LoggerCreateAsync(JsonConvert.SerializeObject(obj), actionUrl, RequestTime, JsonConvert.SerializeObject(resp), rawResponse.Content.ReadAsStringAsync().ToString(), (int)ServiceLogLevel.Error);
                    return resp;
                }
                resp.status = true;
                resp.message = "User Exist";
                var res = rawResponse.Content.ReadAsStringAsync();
                resp.data = JsonConvert.DeserializeObject<APIResponseforAll>(res.Result);
                _ = LogService_Old.LoggerCreateAsync(JsonConvert.SerializeObject(obj), actionUrl, RequestTime, JsonConvert.SerializeObject(resp), rawResponse.Content.ReadAsStringAsync().ToString(), (int)ServiceLogLevel.Information);

                return resp;
            }
            catch (Exception ex)
            {
                _ = LogService_Old.LoggerCreateAsync(JsonConvert.SerializeObject(obj), actionUrl, RequestTime, JsonConvert.SerializeObject(resp), ex.Message.ToString(), (int)ServiceLogLevel.Error);

                return resp;
            }
        }
    }
}
