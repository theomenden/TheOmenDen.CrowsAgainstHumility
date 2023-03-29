namespace TheOmenDen.CrowsAgainstHumility.Utilities;

public class DropOutStack<T> : LinkedList<T>
{
    private readonly int _stackSize;

    public DropOutStack(int stackSize)
    {
        _stackSize = stackSize;
    }

    public void Push(T item)
    {
        if (Count >= _stackSize)
        {
            RemoveLast();
        }

        AddFirst(item);
    }
}
