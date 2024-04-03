namespace Adroit_v8.MongoConnections.CRM
{
    [BsonCollection("AdroitCRMClientDoc")]
    public class ClientDoc : BaseDtoII
    {
        public string CustomerId { get; set; }
        public string PassportPhotograph { get; set; }
        public string PassportPhotographFileName { get; set; }
        public string ESignature { get; set; }
        public string ESignatureFileName { get; set; }
        public string ProofOfResidence { get; set; }
        public string ProofOfResidenceFileName { get; set; }
        public string ProofOfResidenceType { get; set; }
        public string ProofOfIdentity { get; set; }
        public string ProofOfIdentityFileName { get; set; }
        public string ProofOfIdentityType { get; set; }
        public string ProofOfIdentityExpiryDate { get; set; }
        public string ProofOfEmployment { get; set; }
        public string ProofOfEmploymentFileName { get; set; }
        public string ProofOfEmploymentType { get; set; }

    }
    public class ClientDocUpdateFm:ClientDocFM{
        public string UniqueId { get; set; }
    }
    public class ClientDocFM
    {
        public string CustomerId { get; set; }
        public IFormFile PassportPhotograph { get; set; }
        public IFormFile ESignature { get; set; }
        public IFormFile ProofOfResidence { get; set; }
        public string ProofOfResidenceType { get; set; }
        public IFormFile ProofOfIdentity { get; set; }
        public string ProofOfIdentityType { get; set; }
        public string ProofOfIdentityExpiryDate { get; set; }
        public IFormFile ProofOfEmployment { get; set; }
        public string ProofOfEmploymentType { get; set; }

    }
}
