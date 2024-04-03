namespace Adroit_v8.MongoConnections
{
    public class ValidateUser
    {
        public string userName { get; set; }
        public string userPassword { get; set; }
        public string ipAddress { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string applicationId { get; set; }
        public string clientId { get; set; }
    }
    public class LoginValidateOTP
    {
        public string OTP { get; set; }
    }
    public class Login
    {
        public int otp { get; set; }
        public string ipAddress { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string userName { get; set; }
        public string userPassword { get; set; }
        public string applicationId { get; set; }
        public string clientId { get; set; }

    }
    public class LoginRootBase: Login
    {
        public string clientId { get; set; }
        public string applicationId { get; set; }
    }
    public class Rootobject
    {
        public string clientId { get; set; }
        public string userName { get; set; }
        public string userPassword { get; set; }
    }

}
