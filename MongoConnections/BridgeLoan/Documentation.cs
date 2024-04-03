using Amazon.Util.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Adroit_v8.MongoConnections.Models
{
    [BsonCollection("AdroitBridgeLoanDocumentation")]
    public class Documentation : BaseDtoII
    {
        public string Lender { get; set; }
        public string ObligorName { get; set; }
        public string ObligorDob { get; set; }
        public string FacilityType { get; set; }
        public string PhoneNo { get; set; }
        public string InterestRate { get; set; }
        public string DocumentationStatus { get; set; }
        public string ValueDate { get; set; }
        public string MaturityDate { get; set; }
        [NotMapped]
        public string? DocumentSting { get; set; }
        public string Comment { get; set; }
        public string Tenor { get; set; }
        public string Amount { get; set; }

    }
    public class DocumentationFm
    {
        public string Lender { get; set; }
        public string ObligorName { get; set; }
        public string PhoneNo { get; set; }
        public string ObligorDob { get; set; }
        public string FacilityType { get; set; }
        public string InterestRate { get; set; }
        public string DocumentationStatus { get; set; }
        public string ValueDate { get; set; }
        public string MaturityDate { get; set; }
        public string Comment { get; set; }
        public string CreatedBy { get; set; } 
        public string Tenor { get; set; }
        public string Amount { get; set; }
        public IFormFile DocumentationDoc { get; set; }
    }
    public class DocumentationUpdateFm: DocumentationFm
    {
        public string UniqueId { get; set; }
    }
}
