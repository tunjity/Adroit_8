
namespace Adroit_v8.EnumFile
{
    public static class EnumHelper
    {
        public enum ServiceLogLevel
        {
            Information = 1,
            Warning,
            Error,
            Exception
        }
        public enum LoanDisbursementLoanType
        {
            RegularLoan = 1,
            TopUp,
            LoanRestart
        }
        public enum LoanApplicationReassignmnetStatus
        {
            Pending = 1,
            Treated
        }
        public enum AdroitLoanApplicationStatus
        {
            Under_Review = 1,
            Review,
            Approved,
            Adjust,
            Declined,
            Disburse,
            Closed,
            CustomerDeclineOfferLetter
        }
        public enum DisbursementEnum
        {
            Processed = 1,
            Returned,
            Disbursed
        }
        public enum StaffLoanType
        {
            RegularLoan = 1,
            CarLoan
        }
        public enum StaffLoanStatus
        {
            Pending = 1,
            Approved,
            Declined,
            Disbursed,
            Rejected
        }
        public enum GeneralSetUpEnum
        {
            Active = 1,
            InActive
        }
        public enum GeneralFilterStatusEnum
        {
            Active = 1,
            InActive
        }
        public enum CustomerRegistrationStage
        {
            StageCustomer = 1,
            StageEmploymentInfo,
            StageResidentialInfo,
            StageNextOfKin,
            StageBankDetail,
            StageDocumentUpload,
            Completed
        }
        public enum CustomerRegistrationChannel
        {
            Default = 1,
            MobileApp,
            CustomerPortal,
            USSD,
            ThirdParty
        }
    }
}