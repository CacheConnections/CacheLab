using Cache.Caches;

namespace Cache.Tests;

public class LruCacheTests 
{
    [Fact]
    public void TestCache()
    {
        LruCache<int> cache = new(10);
        for(int i = 1; i <= 10; i++)
        {
            cache.Put(i.ToString(), i);
        }

        cache.Put(11.ToString(), 11);
        cache.Put(12.ToString(), 12);
        
        Assert.Equal(0, cache.Get(1.ToString()));
        Assert.Equal(0, cache.Get(2.ToString()));
        Assert.Equal(3, cache.Get(3.ToString()));
        Assert.Equal(4, cache.Get(4.ToString()));
    }
}
