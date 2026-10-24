namespace lab8v6;

public static class CollectionHelper<T>
{
    public static TAccumulate Aggregate<TAccumulate>(
        IEnumerable<T> collection,
        TAccumulate seed,
        Func<TAccumulate, T, TAccumulate> func)
    {
        if (collection == null)
            throw new ArgumentNullException(nameof(collection));
        if (func == null)
            throw new ArgumentNullException(nameof(func));

        TAccumulate result = seed;

        foreach (T item in collection)
        {
            result = func(result, item);
        }

        return result;
    }
}