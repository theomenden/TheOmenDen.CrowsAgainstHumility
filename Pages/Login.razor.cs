namespace TheOmenDen.CrowsAgainstHumility.Pages;
public partial class Login : ComponentBase
{
    private const string PasswordPattern = @"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$";
}