﻿namespace TheOmenDen.CrowsAgainstHumility.Components;

public partial class FilterCardsForCrowGameComponent: ComponentBase
{
    [Parameter] public Boolean IsCustomFilteringEnabled { get; set; }

    [Parameter] public IEnumerable<Pack> PacksToFilter { get; set; }

    [Inject] private ILogger<FilterCardsForCrowGameComponent> Logger { get; init; }

    private string _filterText;

    private readonly List<String> _filterConditions = new List<String>();
    
    private Task OnTextChangedAsync(String changedText)
    {
        if (String.IsNullOrWhiteSpace(changedText))
        {
            return Task.CompletedTask;
        }

        if (!changedText.AsSpan().Contains(','))
        {
            _filterConditions.Add(changedText);

            return Task.CompletedTask;
        }

        _filterConditions.AddRange(changedText.Split(','));

        _filterText = changedText;

        return Task.CompletedTask;
    }

    private Task OnSubmitFilters()
    {
        return Task.CompletedTask;
    }
}
