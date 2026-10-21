using IndependentWork7;

namespace IndependentWork7.Tests;

public class CacheTests
{

    [Fact]
    public void Constructor_ZeroCapacity_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Cache<string, int>(0));
    }


    [Fact]
    public void Add_NewItem_CountIncreases()
    {
        var cache = new Cache<string, int>(3);

        cache.Add("a", 1);
        cache.Add("b", 2);

        Assert.Equal(2, cache.Count);
    }

    [Fact]
    public void Count_EmptyCache_IsZero()
    {
        var cache = new Cache<string, int>(3);

        Assert.Equal(0, cache.Count);
    }

    [Fact]
    public void Add_ExistingKey_UpdatesValueWithoutIncreasingCount()
    {
        var cache = new Cache<string, int>(3);
        cache.Add("a", 1);

        cache.Add("a", 100);

        Assert.Equal(100, cache.Get("a"));
        Assert.Equal(1, cache.Count);
    }


    [Fact]
    public void Get_ExistingKey_ReturnsValue()
    {
        var cache = new Cache<int, string>(2);
        cache.Add(1, "один");

        Assert.Equal("один", cache.Get(1));
    }

    [Fact]
    public void Get_MissingKey_ThrowsKeyNotFoundException()
    {
        var cache = new Cache<string, int>(2);

        Assert.Throws<KeyNotFoundException>(() => cache.Get("немає"));
    }

    [Fact]
    public void ContainsKey_ExistingKey_ReturnsTrue()
    {
        var cache = new Cache<string, int>(2);
        cache.Add("a", 1);

        Assert.True(cache.ContainsKey("a"));
    }

    [Fact]
    public void ContainsKey_MissingKey_ReturnsFalse()
    {
        var cache = new Cache<string, int>(2);

        Assert.False(cache.ContainsKey("a"));
    }


    [Fact]
    public void Add_WhenFull_RemovesOldestItem()
    {
        var cache = new Cache<string, int>(2);
        cache.Add("a", 1);
        cache.Add("b", 2);

        cache.Add("c", 3); 

        Assert.False(cache.ContainsKey("a"));
        Assert.True(cache.ContainsKey("b"));
        Assert.True(cache.ContainsKey("c"));
        Assert.Equal(2, cache.Count);
    }

    [Fact]
    public void Add_SeveralOverflows_RemovesItemsInInsertionOrder()
    {
        var cache = new Cache<string, int>(2);
        cache.Add("a", 1);
        cache.Add("b", 2);
        cache.Add("c", 3); 
        cache.Add("d", 4); 

        Assert.False(cache.ContainsKey("a"));
        Assert.False(cache.ContainsKey("b"));
        Assert.Equal(3, cache.Get("c"));
        Assert.Equal(4, cache.Get("d"));
    }

    [Fact]
    public void Add_UpdatingKey_DoesNotChangeEvictionOrder()
    {
        var cache = new Cache<string, int>(2);
        cache.Add("a", 1);
        cache.Add("b", 2);
        cache.Add("a", 10); 

        cache.Add("c", 3);

        Assert.False(cache.ContainsKey("a"));
        Assert.True(cache.ContainsKey("b"));
    }

    [Fact]
    public void Get_EvictedKey_ThrowsKeyNotFoundException()
    {
        var cache = new Cache<string, int>(1);
        cache.Add("a", 1);
        cache.Add("b", 2);

        Assert.Throws<KeyNotFoundException>(() => cache.Get("a"));
    }
}