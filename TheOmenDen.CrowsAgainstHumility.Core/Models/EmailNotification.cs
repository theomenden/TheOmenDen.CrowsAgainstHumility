namespace TheOmenDen.CrowsAgainstHumility.Core.Models;
public sealed class EmailNotification
{
    public Int32 Id { get; set; }

    public String FromName { get; set; } = "The Omen Den";

    public String FromEmail { get; set; } = "support@theomenden";

    public String ToName { get; set; }

    public String ToEmail { get; set; }

    public String Subject { get; set; }

    public String Body { get; set; }

    public Boolean IsHtml { get; set; }

    public Int32 TryCount { get; set; }

    public Boolean HasBeenSent { get; set; }
}
