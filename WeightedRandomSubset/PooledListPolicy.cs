using Microsoft.Extensions.ObjectPool;

namespace WeightedRandomSubset;

/// <remarks>
///     Relies on Remove never shrinking the underlying array of a list
/// </remarks>
internal class PooledListPolicy<T> : PooledObjectPolicy<List<T>> where T : notnull
{
    private readonly int _listCapacity;

    public PooledListPolicy(int listCapacity)
    {
        _listCapacity = listCapacity;
    }

    public override List<T> Create()
    {
        return new List<T>(_listCapacity);
    }

    public override bool Return(List<T> list)
    {
        list.Clear();
        return true;
    }
}
