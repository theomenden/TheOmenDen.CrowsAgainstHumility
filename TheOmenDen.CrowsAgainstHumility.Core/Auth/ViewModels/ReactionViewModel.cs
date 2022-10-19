namespace TheOmenDen.CrowsAgainstHumility.Core.Auth.ViewModels;
public sealed class ReactionViewModel
{
    public Int32 Id { get; set; }
    
    public Guid UserId { get; set; }

    public String Usernname { get; set; }

    public String Emoji { get; set; }

    public Int32? CommentId { get; set; }
}
