using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using TheOmenDen.CrowsAgainstHumility.Core.Enumerations;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models;
public class ApplicationUser: IdentityUser<Guid>
{
    [Required]
    public DateTime CreatedDate { get; set; }
    
    [Required]
    [MaxLength(256)]
    [Display(Name = "First Name")]
    public String FirstName { get; set; }

    [Required]
    [MaxLength(256)]
    [Display(Name ="Last Name")]
    public String LastName { get; set; }

    [Required]
    [MaxLength(256)]
    [Display(Name = "User Name")]
    public override String UserName { get; set; }

    [Required]
    [MaxLength(256)]
    [EmailAddress]
    public override String Email { get; set; }

    public NotificationType NotificationType { get; set; }

    public String? ImageUrl { get; set; }

    public String GetUIImageUrl => String.IsNullOrWhiteSpace(ImageUrl)
        ? @"img\avatars\avatar-00.png"
        : ImageUrl;
    
    public virtual ICollection<CrowGamePlayer> CrowGamePlayers { get; set; } = new HashSet<CrowGamePlayer>();

    public virtual ICollection<CawChatMessage> ChatMessagesToUsers { get; set; } = new HashSet<CawChatMessage>();

    public virtual ICollection<CawChatMessage> ChatMessagesFromUsers { get; set; } = new HashSet<CawChatMessage>();
}
