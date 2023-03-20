using System.Diagnostics.CodeAnalysis;
using TheOmenDen.CrowsAgainstHumility.ViewModels;

namespace TheOmenDen.CrowsAgainstHumility.Components;

public abstract class GameModelComponentBase<TViewModel>: ComponentBase
where TViewModel : ViewModelBase
{
    [Inject]
    [NotNull]
    protected TViewModel ViewModel { get; init; }

    protected override void OnInitialized()
    {
        ViewModel.PropertyChanged += (_, _) => StateHasChanged();
        base.OnInitialized();
    }
    
    protected override Task OnInitializedAsync()
        => ViewModel.OnInitializedAsync();
}
