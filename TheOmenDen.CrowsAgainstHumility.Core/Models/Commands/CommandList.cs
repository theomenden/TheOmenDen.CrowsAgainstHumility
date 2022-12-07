using System.Collections;
using TheOmenDen.CrowsAgainstHumility.Core.Interfaces.Commands;

namespace TheOmenDen.CrowsAgainstHumility.Core.Models.Commands;
public sealed class CommandList : IEnumerable<ICrowGameCommand>
{
    private readonly List<CommandSubList> _subCommands = new(5);

    public CommandList()
    {
        _subCommands.Add(new CommandSubList());
    }

    public void Add(ICrowGameCommand command)
    {
        lock (_subCommands)
        {
            var currentList = _subCommands.LastOrDefault();

            currentList?.Add(command);
        }
    }



    public void Clear()
    {
        _subCommands.Clear();
        _subCommands.Add(new CommandSubList());
    }

    public IEnumerator<ICrowGameCommand> GetEnumerator() => _subCommands.SelectMany(subList => subList).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
