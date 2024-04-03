namespace Adroit_v8.Models.UtilityModel;

public partial class Applicationchannel : BaseEntity
{
    public string? Name { get; set; }

}
public partial class DeclineReason : BaseEntity
{
    public string? Name { get; set; }

}

public class CustomerInformatinoDto
{

    public long Id { get; set; }
    public string FirstName { get; set; }
    public string? CustomerRef { get; set; }

    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public string DateOfBirth { get; set; }
    public string PhoneNumber { get; set; }
    public string ClientCode { get; set; }
    public int HasValidBVN { get; set; }
    public int RegistrationChannelId { get; set; }
    public int RegistrationStageId { get; set; }
}