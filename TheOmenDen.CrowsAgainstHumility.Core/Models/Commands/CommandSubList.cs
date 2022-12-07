using System.Collections;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Commands;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models.Commands;

internal sealed class CommandSubList : IEnumerable<ICrowGameCommand>
{
    private readonly List<ICrowGameCommand> _commands = new();

    public CommandSubList()
    {
    }

    internal Int32 Count => _commands.Count;

    internal void RemoveLast()
    {
        if (_commands.Any())
        {
            _commands.RemoveAt(_commands.Count - 1);
        }
    }

    internal void Add(ICrowGameCommand command)
    {
        _commands.Add(command);
    }

    internal ICrowGameCommand[] ToArray() => _commands.ToArray();

    internal List<ICrowGameCommand> ToList() => new(_commands);
    
    internal ICrowGameCommand? LastOrDefault() => _commands.LastOrDefault();

    #region IEnumerable.GetEnumerator Implementations
    public IEnumerator<ICrowGameCommand> GetEnumerator() => _commands.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    #endregion
}
