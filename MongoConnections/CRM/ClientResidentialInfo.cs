namespace Adroit_v8.MongoConnections.CRM
{
    [BsonCollection("AdroitCRMClientResidentialInfo")]
    public class ClientResidentialInfo : BaseDtoII
    {
        public string StateId { get; set; }
        public string CustomerId { get; set; }
        public string LgaId { get; set; }
        public string PermanentAddress { get; set; }
        public string NearestLandmark { get; set; }
        public string ResidentialStatus { get; set; }
        public string NoOfYearsAtResidence { get; set; }
      
    } 
    public class ClientResidentialInfoFm 
    {
        public string StateId { get; set; }
        public string CustomerId { get; set; }
        public string LgaId { get; set; }
        public string PermanentAddress { get; set; }
        public string NearestLandmark { get; set; }
        public string ResidentialStatus { get; set; }
        public string NoOfYearsAtResidence { get; set; }
    } 
    public class ClientResidentialInfoUpdateFm : ClientResidentialInfoFm
    {
        public string UniqueId { get; set; }
    }
}

