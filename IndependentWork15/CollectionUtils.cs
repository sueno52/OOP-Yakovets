namespace IndependentWork15;
public static class CollectionUtils
{
    public static IEnumerable<TResult> Map<TSource, TResult>(
        IEnumerable<TSource> collection,
        Func<TSource, TResult> mapper)
    {
        foreach (var item in collection)
        {
            yield return mapper(item);
        }
    }

    public static T MaxBy<T, TKey>(
        IEnumerable<T> collection,
        Func<T, TKey> keySelector)
        where TKey : IComparable<TKey>
    {
        T maxItem = default!;
        TKey maxKey = default!;
        bool first = true;

        foreach (var item in collection)
        {
            TKey key = keySelector(item);
            if (first || key.CompareTo(maxKey) > 0)
            {
                maxKey = key;
                maxItem = item;
                first = false;
            }
        }

        if (first)
            throw new InvalidOperationException("Колекція порожня, максимуму немає.");

        return maxItem;
    }

    public static IEnumerable<T> Filter<T>(
        IEnumerable<T> collection,
        Predicate<T> predicate)
    {
        foreach (var item in collection)
        {
            if (predicate(item))
                yield return item;
        }
    }

    public static void ForEach<T>(
        IEnumerable<T> collection,
        Action<T> action)
    {
        foreach (var item in collection)
        {
            action(item);
        }
    }
}