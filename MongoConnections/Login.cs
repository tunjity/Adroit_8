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
    public class newRootobject
    {
        public object id { get; set; }
        public bool status { get; set; }
        public string statusMessage { get; set; }
        public int statusCode { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public string id { get; set; }
        public string clientId { get; set; }
        public string clientName { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string phoneNumber1 { get; set; }
        public string phoneNumber2 { get; set; }
        public string emailAddress { get; set; }
        public string officeAddress { get; set; }
        public string companyWebsite { get; set; }
        public string createdBy { get; set; }
        public string clientLogo { get; set; }
        public string clientGooglePlayUrl { get; set; }
        public string clientAppleStoreUrl { get; set; }
        public int isDeleted { get; set; }
        public DateTime dateCreated { get; set; }
    }

}
