namespace TheOmenDen.CrowsAgainstHumility.Components;

public partial class ChatMessageComponent: ComponentBase
{
    [Parameter] public String? Username { get; set; } = String.Empty;
    [Parameter] public String? Message { get; set; } = String.Empty;
}
