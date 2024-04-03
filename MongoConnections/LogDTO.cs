using System.ComponentModel.DataAnnotations;

namespace Adroit_v8.MongoConnections
{
    public class LogDTO
    {
        //Service name
        [Required]
        public string ServiceName { get; set; }
        [Required]
        public string RequestUrl { get; set; }

        //Request
        [Required]
        public string Request { get; set; }

        [Required]
        public DateTime RequestTime { get; set; }

        //Response
        public string Response { get; set; }
        public DateTime ResponseTime { get; set; }

        //Exception
        public string Exception { get; set; }

        [Required]
        public int LogLevelId { get; set; }

    }
}
