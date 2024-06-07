namespace Adroit_v8.Config
{
    public class SSOSettings
    {
        public string Permission { get; set; } = null!;
        public string applicationId { get; set; } = null!;
        public string clientId { get; set; } = null!;
        public string XApiKey { get; set; } = null!;
        public string baseurl { get; set; } = null!;
    }
    public class UploadSettings
    {
        public string mailUrl { get; set; } = null!;
        public string Url { get; set; } = null!;
        public string mailTemplatUrl { get; set; } = null!;
    }
    public class DisburseToSettings
    {
        public string Key1 { get; set; } = null!;
        public string Url { get; set; } = null!;
        public string UrlTwo { get; set; } = null!;
        public string RepaymentUrl { get; set; } = null!;
        public string RepaymentLoanAdjustmentUrl { get; set; } = null!;
    }

    public class PassPhrasesSettings
    {
        public string Key1 { get; set; } = null!;
        public string Key2 { get; set; } = null!;
        public string Key3 { get; set; } = null!;
        public string Key4 { get; set; } = null!;
    }
    public class JwtSettings
    {
        public string Key { get; set; } = null!;
        public string Issuer { get; set; } = null!;
    }
    public class FileFolderSettings
    {
        public string Path { get; set; } = null!;
        public string BankStatementPath { get; set; } = null!;
        public string AddBankStatement { get; set; } = null!;
    }
    public class SSMongoDBSettings
    {
        public string ConnectionURI { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
    }
    public class MongoDBSettings
    {
        public string ConnectionURI { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string MobileDatabaseName { get; set; } = null!;
        public string LoanRecoveryCollectionName { get; set; } = null!;
    }
    public class MongoDB
    {
        public string ConnectionURI { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string MobileDatabaseName { get; set; } = null!;
        public string LoanRecoveryCollectionName { get; set; } = null!;
    }

}
