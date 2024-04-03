using Adroit_v8.MongoConnections;
using Newtonsoft.Json;
using System.Text;

namespace Adroit_v8.Service
{
    public class LogService : IDisposable
    {

        /// <summary>
        /// Activity logger service
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static void LoggerCreateAsync(string request, string requestUrl, DateTime requestTime, string response, string exception, int loglevelId)
        {
            try
            {
                Task.Run(async () =>
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

                    //Get logger service URL
                    var loggerServiceUrl = (new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).
                    AddJsonFile("appsettings.json", false).Build()).
                    GetSection("LoggerServiceUrl").Value;
                    
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
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
