namespace Adroit_v8.Model
{

    public partial class LoanRepaymentDetail
    {
        public long Id { get; set; }

        public DateOnly? RepaymentDate { get; set; }

        public decimal? RepaymentAmount { get; set; }

        public bool? IsPartialRepayment { get; set; }

        public int? RepaymentStage { get; set; }

        public bool? HasPaid { get; set; }

        public DateTime? DateCreated { get; set; }

        public string? LoanType { get; set; }

        public long? CustomerId { get; set; }

        public string? LoanApplicationId { get; set; }

        public string? ClientId { get; set; }

        public decimal? PartialRepaymentAmount { get; set; }

        public decimal? RepaymentAmountToBalance { get; set; }
    }
}
