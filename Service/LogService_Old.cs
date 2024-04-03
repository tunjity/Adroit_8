using Adroit_v8.MongoConnections;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Text;
namespace Adroit_v8.Service
{
    public static class LogService_Old
    {
        /// <summary>
        /// Activity logger service
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// 
        public async static Task<T> Logger<T>(string request, string requestUrl, DateTime requestTime, string response, string exception, int loglevelId)
        {
            try
            {
                LogDTO obj = new()
                {
                    ServiceName = (new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", false).Build()).GetSection("ServiceName").Value,
                    Request = request,
                    RequestTime = requestTime,
                    RequestUrl = requestUrl,
                    Response = response,
                    ResponseTime = DateTime.Now,
                    LogLevelId = loglevelId,
                    Exception = exception
                };
                var client = new HttpClient();
                var xApiKey = (new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", false).Build()).GetSection("XApiKey").Value;
                var loggerServiceUrl = (new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", false).Build()).GetSection("LoggerServiceUrl").Value;
                var req = new HttpRequestMessage(HttpMethod.Post, loggerServiceUrl);
                req.Headers.Add("XApiKey", xApiKey);
                var setRep = JsonConvert.SerializeObject(obj);
                var content = new StringContent(setRep, null, "application/json");
                req.Content = content;
                ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                var webResponse = await client.SendAsync(req);
                var j = webResponse.Content.ReadAsStringAsync().Result;
                var r = JsonConvert.DeserializeObject<T>(j);

                if (!webResponse.IsSuccessStatusCode)
                {
                    string apiResponse = webResponse.Content.ToString();

                    return default(T);
                }
                return r;
            }
            catch (Exception ex)
            { //log to text 
                return default(T);
            }
        }

        public static async Task LoggerCreateAsync(string request, string requestUrl, DateTime requestTime, string response, string exception, int loglevelId)
        {
            LogDTO obj = new()
            {
                ServiceName = (new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).
                AddJsonFile("appsettings.json", false).Build()).
                GetSection("ServiceName").Value,

                Request = request,
                RequestTime = requestTime,
                RequestUrl = requestUrl,
                Response = response,
                ResponseTime = DateTime.Now,
                LogLevelId = loglevelId,
                Exception = exception
            };

            //Get XApiKey from app settings
            var xApiKey = (new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).
            AddJsonFile("appsettings.json", false).Build()).
            GetSection("XApiKey").Value;

            //   await Task.Run(async () =>
            //{
            //Get logger service URL
            var loggerServiceUrl = (new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).
            AddJsonFile("appsettings.json", false).Build()).
            GetSection("LoggerServiceUrl").Value;
            // JsonConvert.SerializeObject(obj)


            var requestApi = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
            using var httpclient = new HttpClient();
            httpclient.DefaultRequestHeaders.Add("XApiKey", xApiKey);
            using var rawResponse = await httpclient.PostAsync(loggerServiceUrl, requestApi);

            int statusCode = (int)rawResponse.StatusCode;
            if (statusCode != 200)
            {
                //log to text     httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + secretKey);


                //Sendmail that logger service is down

            }
            // });
        }
    }
}
