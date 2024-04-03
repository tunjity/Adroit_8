namespace Adroit_v8.MongoConnections.LoanApplication
{
    public class LoanApplicationVM
    {
        public string ApplicationId { get; set; }
        public string AmountRequested { get; set; }
        public string Interest { get; set; }
        public string TotalAmount { get; set; }
        public string ProcessingFee { get; set; }
        public string Status { get; set; }
        public bool DisbursementStatus { get; set; }
        public string Duration { get; set; }
        public string AssignedLoanOfficer { get; set; }
        public string ApplicationDate { get; set; }
        public string SubmissionDate { get; set; }
    }
}
