using IndependentWork13;

namespace IndependentWork13.Tests;
public class LruCacheTests
{
    [Fact]
    public void Add_WhenFull_RemovesLeastRecentlyUsed()
    {
        var cache = new LruCache<string, int>(2);
        cache.Add("a", 1);
        cache.Add("b", 2);

        cache.Get("a");    
        cache.Add("c", 3);

        Assert.True(cache.ContainsKey("a"));
        Assert.False(cache.ContainsKey("b"));
        Assert.True(cache.ContainsKey("c"));
    }

    [Fact]
    public void Add_WithoutGet_WorksLikeFifo()
    {
        var cache = new LruCache<string, int>(2);
        cache.Add("a", 1);
        cache.Add("b", 2);

        cache.Add("c", 3);

        Assert.False(cache.ContainsKey("a"));
    }

    [Fact]
    public void Add_ExistingKey_MarksKeyAsRecentlyUsed()
    {
        var cache = new LruCache<string, int>(2);
        cache.Add("a", 1);
        cache.Add("b", 2);
        cache.Add("a", 10);

        cache.Add("c", 3);

        Assert.Equal(10, cache.Get("a"));
        Assert.False(cache.ContainsKey("b"));
    }

    [Fact]
    public void Get_MissingKey_ThrowsKeyNotFoundException()
    {
        var cache = new LruCache<string, int>(2);

        Assert.Throws<KeyNotFoundException>(() => cache.Get("немає"));
    }
}