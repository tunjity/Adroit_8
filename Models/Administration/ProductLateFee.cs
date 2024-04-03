namespace Adroit_v8.Models.Administration
{
    public class ProductLateFee : BaseEntity
    {
        public int ProductId { get; set; }
        public string LateFeeType { get; set; }
        public decimal FixedPrice  { get; set; } 
        public string LateFeePrincipal { get; set; }
        public string FeeFrequency { get; set; }
        public string GracePeriod  { get; set; }
    }
}
