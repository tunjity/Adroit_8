using Npgsql;
using System.Data;

namespace Adroit_v8.Config
{
    public class DapperConfig
    {
        private readonly IConfiguration _config;
        public DapperConfig(IConfiguration config)
        {
            _config = config;
        }

        public IDbConnection PostgreDbConnection => new NpgsqlConnection(_config.GetConnectionString("ConnectionURI"));
    }


    public static class CustomerRegistrationQuery
    {
        public const string RegistrationStageOne = @"INSERT INTO public.""Customer""(""EmailAddress"", ""PhoneNumber"", ""FirstName"", ""LastName"", ""RegistrationStageId"",""RegistrationChannelId"",""ClientCode"",""CustomerRef"")
	    VALUES (@EmailAddress, @PhoneNumber, @FirstName, @LastName, @RegistrationStageId,@RegistrationChannelId,@ClientCode,@customerRef) RETURNING public.""Customer"".""Id""";

        public const string GetUserInformation = @"SELECT * FROM public.""Customer"" where ""EmailAddress"" = @EmailAddress AND ""ClientCode"" = @ClientCode LIMIT 1 ";
        public const string GetUserInformationByCustomerId = @"SELECT * FROM public.""Customer"" where ""Id"" = @CustomerId AND ""ClientCode"" = @ClientCode LIMIT 1 ";
        public const string GetCustomerByCustomerId = @"SELECT * FROM public.""Customer"" where ""Id"" = @CustomerId ";
        public const string GetCustomerByEmail= @"SELECT * FROM public.""Customer"" where ""EmailAddress"" = @EmailAddress ";
        public const string UpdateCustomerDateOfBirth = @"UPDATE public.""Customer""
	    SET  ""DateOfBirth"" =@DateOfBirth
	    WHERE ""Id"" = @CustomerId AND ""ClientCode"" = @ClientCode";
        public const string GetCustomerDateOfBirth = @"SELECT ""FirstName"", ""LastName"", ""DateOfBirth"" FROM public.""Customer"" where ""Id"" = @CustomerId And ""ClientCode"" = @ClientCode  LIMIT 1 ";
        public const string UpdateCustomerEscrowAccountNumber = @"UPDATE public.""Customer""
	    SET  ""EscrowAccountNumber"" =@EscrowAccountNumber
	    WHERE ""Id"" = @CustomerId AND ""ClientCode"" = @ClientCode";
    }
}
