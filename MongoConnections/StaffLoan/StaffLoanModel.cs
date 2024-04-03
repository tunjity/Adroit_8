using Adroit_v8.MongoConnections;

[BsonCollection("StaffLoanInterestRate")]
public class StaffLoanInterestRateModel : BaseDtoII
{
    public decimal InterestRate { get; set; }
}
public class StaffLoanInterestRatenCreateModel 
{
    public decimal InterestRate { get; set; }
}

public class MainResponse 
{
    //StaffLoanModel, List<ScheduleResponse>
    public List<ScheduleResponse> lSR { get; set; }
    public StaffLoanModel SLM { get; set; }
}

public class ScheduleResponse
 {
     public string RepaymentSchedule { get; set; }
     public decimal Amount { get; set; }
 }
[BsonCollection("StaffLoan")]
public class StaffLoanModel : BaseDtoII
{
    public string StaffId  { get; set; }
    public string FirstName   { get; set; }
    public string LastName   { get; set; }
    public string PersonalEmail { get; set; }
    public string OfficialEmail { get; set; }
    public string PhoneNumber { get; set; }
    public decimal InterestRate { get; set; }
    public int LoanType { get; set; }
    public decimal LoanAmount { get; set; }
    public int LoanStatusId { get; set; }
    public int LoanTenorid { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public string Purpose { get; set; }
}

public class StaffLoanCreateModel
{
    public string StaffId  { get; set; }
    public string PersonalEmail { get; set; }
    public string OfficialEmail { get; set; }
    public string FirstName   { get; set; }
    public string LastName   { get; set; }
    public string PhoneNumber   { get; set; }
    public decimal InterestRate { get; set; }
    public int LoanType { get; set; }
    public decimal LoanAmount { get; set; }
    public int LoanTenorid { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public string Purpose { get; set; }
}
public class StaffLoanUpdateStatus
{
    public string UniqueId { get; set; }
    public int LoanStatusId { get; set; }
}