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

    public ICollection<String> RoleNames { get; set; } = new HashSet<String>();

    public String ImageUrl { get; set; }

    public ICollection<String> Pronouns { get; set; } = new HashSet<String>();

    public ICollection<LoginViewModel> Logins { get; set; } = new HashSet<LoginViewModel>();
}
