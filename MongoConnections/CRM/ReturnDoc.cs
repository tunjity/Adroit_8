public class ReturnDoc
{
    public string UniqueId { get; set; }
    public string CustomerId { get; set; }
    public string PassportPhotograph { get; set; }
    public string Bvn { get; set; }
    public ReturnDocFileName PassportPhotographFileName { get; set; }
    public string ESignature { get; set; }
    public ReturnDocFileName ESignatureFileName { get; set; }
    public string ProofOfResidence { get; set; }
    public ReturnDocFileName ProofOfResidenceFileName { get; set; }
    public string ProofOfResidenceType { get; set; }
    public string ProofOfIdentity { get; set; }
    public ReturnDocFileName ProofOfIdentityFileName { get; set; }
    public string ProofOfIdentityType { get; set; }
    public string ProofOfIdentityExpiryDate { get; set; }
    public string ProofOfEmployment { get; set; }
    public ReturnDocFileName ProofOfEmploymentFileName { get; set; }
    public string ProofOfEmploymentType { get; set; }

}

public class ReturnDocFileName{
    public string Name{get; set;}
}