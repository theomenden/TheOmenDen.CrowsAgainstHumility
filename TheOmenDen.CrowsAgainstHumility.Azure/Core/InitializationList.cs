namespace TheOmenDen.CrowsAgainstHumility.Azure.Core;
internal class InitializationList
{
    private readonly StringComparer _stringComparer = StringComparer.OrdinalIgnoreCase;
    private readonly object _listLock = new ();
    private List<string>? _list;

    public bool IsEmpty
    {
        get
        {
            lock (_listLock)
            {
                return _list?.Count is 0;
            }
        }
    }

    public IList<string>? Values
    {
        get
        {
            lock (_listLock)
            {
                return _list;
            }
        }
    }

    public bool InitializationCanBegin(string value)
    {
        lock (_listLock)
        {
            return _list is null || _list.Contains(value, _stringComparer);
        }
    }

    public bool Setup(IEnumerable<string> values)
    {
        if (values is null)
        {
            throw new ArgumentNullException(nameof(values));
        }

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

    public bool Remove(string key)
    {
        lock (_listLock)
        {
            return _list is not null 
                   && _list.RemoveAll(k => _stringComparer.Equals(k, key)) is not 0;
        }
    }

    public void Clear()
    {
        lock (_listLock)
        {
            _list = new List<string>();
        }
    }
}
