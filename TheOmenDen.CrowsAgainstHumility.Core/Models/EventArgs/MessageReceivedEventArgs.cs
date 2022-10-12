namespace TheOmenDen.CrowsAgainstHumility.Core.Models.EventArgs;
public sealed class MessageReceivedEventArgs : System.EventArgs
{
    public MessageReceivedEventArgs(String username, String message)
    {
        Username = username;
        Message = message;
    }

    public String Username { get; set; }

    public String Message { get; set; }
}
