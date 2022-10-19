#nullable disable
namespace TheOmenDen.CrowsAgainstHumility.Core.Auth.ViewModels;
public sealed class UserViewModel
{
    public Guid Id { get; set; }

    public String FirstName { get; set; }

    public String LastName { get; set; }

    public String Username { get; set; }

    public String Email { get; set; }

    public Boolean IsEmailConfirmed { get; set; }

    public String[] RoleNames { get; set; }

    public String ImageUrl { get; set; }

    public String[] Pronouns { get; set; }
}
