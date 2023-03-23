namespace TheOmenDen.CrowsAgainstHumility.Core.Models;
public sealed class ContactFormInputModel
{
    public string FirstName { get; set; } = String.Empty;
    public string LastName { get; set; } = String.Empty;
    public string Email { get; set; } = String.Empty;
    public string Subject { get; set; } = String.Empty;
    public string Body { get; set; } = String.Empty;
}
