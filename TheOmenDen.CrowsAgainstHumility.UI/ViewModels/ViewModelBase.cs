using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TheOmenDen.CrowsAgainstHumility.ViewModels;

public abstract partial class ViewModelBase: ObservableObject
{
    public virtual async Task OnInitializedAsync() => await Loaded().ConfigureAwait(true);

    protected virtual void NotifyStateChanged() => OnPropertyChanged(String.Empty);

    [RelayCommand]
    public virtual Task Loaded() => Task.CompletedTask;
}
