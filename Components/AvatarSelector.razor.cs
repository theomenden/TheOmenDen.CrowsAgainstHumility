namespace TheOmenDen.CrowsAgainstHumility.Components;

public partial class AvatarSelector: ComponentBase
{
    [Parameter] public string SelectedImageUrl { get; set; } = String.Empty;

    [Parameter] public EventCallback<String> SelectedImageUrlChanged { get; set; }
    
    private Task SelectImage(string imageUrl)
    {
        SelectedImageUrl = imageUrl;
        return SelectedImageUrlChanged.InvokeAsync(SelectedImageUrl);
    }
}
