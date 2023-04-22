namespace TheOmenDen.CrowsAgainstHumility.Azure.ServiceBus.Structures;
public sealed class InitializationList
{
    private readonly StringComparer _comparer = StringComparer.OrdinalIgnoreCase;
    private readonly object _listLock = new();
    private List<String>? _list;

    public bool IsEmpty()
    {
        lock (_listLock)
        {
            return _list?.Count == 0;
        }
    }

    public IEnumerable<string> GetValues()
    {
        lock (_listLock)
        {
            return _list ?? Enumerable.Empty<string>();
        }
    }

    public bool ContainsOrNotInitialized(string value)
    {
        lock (_listLock)
        {
            return _list is null || _list.Contains(value, _comparer);
        }
    }

    public bool InitializeWithValues(IEnumerable<string> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        lock (_listLock)
        {
            if (_list is not null)
            {
                return false;
            }

            _list = values.ToList();
            return true;
        }
    }

    public bool Remove(string value)
    {
        lock (_listLock)
        {
            return _list is not null && _list.RemoveAll(v => _comparer.Equals(v, value)) != 0;
        }
    }

    public void Clear()
    {
        lock (_listLock)
        {
            if (_list is not null)
            {
                _list.Clear();
                return;
            }

            _list = new List<string>();
        }
    }
}
