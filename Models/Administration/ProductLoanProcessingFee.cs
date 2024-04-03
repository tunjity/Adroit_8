namespace Adroit_v8.Models.Administration
{
    public class ProductLoanProcessingFee : BaseEntity
    {
        public int ProductId { get; set; }
        public bool IsOptInProcessingFee { get; set; }
        public bool IsFixedPrice { get; set; }
        public decimal FixedPrice { get; set; }
        public decimal Principal { get; set; }
    }
}
