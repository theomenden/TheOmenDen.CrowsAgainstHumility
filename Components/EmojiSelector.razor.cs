using Blazorise;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Services;
using TheOmenDen.CrowsAgainstHumility.Core.Models.ChatModels;

namespace TheOmenDen.CrowsAgainstHumility.Components;

public partial class EmojiSelector: ComponentBase
{
    [Inject] private IBetterTTVEmoteService BetterTtvEmoteService { get; init; }
    [Inject] private IConfiguration Configuration { get; init; }
    private DropdownToggle _dropdownToggle;
    private readonly List<BttvBaseEmote> _bttvEmotes = new (100);

    public EventCallback<BttvBaseEmote> OnBttvEmoteSelect { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await foreach (var emote in BetterTtvEmoteService.GetBetterTtvGlobalEmotesAsync())
        {
            _bttvEmotes.Add(emote);
        }
    }

    private async Task SelectGlobalBttvEmoteAsync(BttvBaseEmote emote)
    {
        await _dropdownToggle.Close(CloseReason.None);
        await OnBttvEmoteSelect.InvokeAsync(emote);
    }

    private String GetEmoteUri(String emoteId, String emoteType) => $"{Configuration["BetterTTV:CdnConnection"]}/emote/{emoteId}/1x.{emoteType}";
}
