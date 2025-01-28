using System.Collections.Concurrent;
using System.Diagnostics;
using Cache.Caches;

namespace Cache.Tests;

public class LruCacheTests 
{
    #region Basic Operations

    [Fact]
    public void Get_NonExistentKey_ReturnsDefault()
    {
        var cache = new LruCache<string, string>(capacity: 5);
        var result = cache.Get("nonexistent");
        Assert.Null(result);
    }

    [Fact]
    public void Put_NullKey_ThrowsException()
    {
        var cache = new LruCache<string, string>(capacity: 5);

        Assert.Throws<ArgumentNullException>(() => cache.Put(null!, "value"));
    }

    [Fact]
    public void Put_UpdateExistingKey_UpdatesValue()
    {
        var cache = new LruCache<string, string>(capacity: 5);
        cache.Put("key", "value1");
        cache.Put("key", "value2");
        
        Assert.Equal("value2", cache.Get("key"));
    }

    #endregion

    #region Capacity and Eviction

    [Fact]
    public void Cache_AtCapacity_EvictsLeastRecentlyUsed()
    {
        var cache = new LruCache<string, int>(capacity: 3);
        
        // Fill cache
        cache.Put("1", 1);
        cache.Put("2", 2);
        cache.Put("3", 3);
        
        // Access "1" making it most recently used
        cache.Get("1");
        
        // Add new item, should evict "2"
        cache.Put("4", 4);
        
        Assert.Equal(1, cache.Get("1")); // Most recently used
        Assert.Equal(0, cache.Get("2")); // Should be evicted (returns default)
        Assert.Equal(3, cache.Get("3"));
        Assert.Equal(4, cache.Get("4")); // Newly added
    }

    [Fact]
    public void Cache_EvictionOrder_RespectsGetOperations()
    {
        var cache = new LruCache<string, int>(capacity: 3);
        
        // Fill cache
        cache.Put("1", 1);
        cache.Put("2", 2);
        cache.Put("3", 3);
        
        // Access items in reverse order
        cache.Get("1");
        cache.Get("2");
        cache.Get("3");
        
        // Add new item, should evict the least recently accessed
        cache.Put("4", 4);
        
        Assert.Equal(0, cache.Get("1")); // Should be evicted
        Assert.Equal(2, cache.Get("2"));
        Assert.Equal(3, cache.Get("3"));
        Assert.Equal(4, cache.Get("4"));
    }

    #endregion

    #region Concurrency

    [Fact]
    public async Task ConcurrentOperations_MultipleThreads_MaintainsConsistency()
    {
        var cache = new LruCache<string, int>(capacity: 100);
        var tasks = new List<Task>();
        var iterations = 1000;
        var successfulGets = new ConcurrentBag<int>();
        
        // Multiple threads doing puts and gets
        for (int i = 0; i < 10; i++)
        {
            var threadId = i;
            tasks.Add(Task.Run(() =>
            {
                for (int j = 0; j < iterations; j++)
                {
                    var key = $"{threadId}-{j}";
                    cache.Put(key, j);
                    var value = cache.Get(key);
                    if (value != default)
                    {
                        successfulGets.Add(1);
                    }
                }
            }));
        }

        await Task.WhenAll(tasks.ToArray());
        
        // Due to capacity constraints and evictions, not all gets will be successful
        // but we should have some successful operations
        Assert.True(successfulGets.Count > 0);
    }

    [Fact]
    public async Task ConcurrentOperations_SameKey_MaintainsConsistency()
    {
        var cache = new LruCache<string, int>(capacity: 10);
        var tasks = new List<Task>();
        var iterations = 100;
        
        // Multiple threads updating and reading same key
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                for (int j = 0; j < iterations; j++)
                {
                    cache.Put("shared-key", j);
                    var value = cache.Get("shared-key");
                    Assert.True(value >= 0); // Value should always be valid
                }
            }));
        }
        
        await Task.WhenAll(tasks.ToArray());
        
        // Final value should be a valid number
        var finalValue = cache.Get("shared-key");
        Assert.True(finalValue >= 0);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Cache_UnderStress_HandlesRapidPutGet()
    {
        var cache = new LruCache<string, int>(capacity: 2);
        
        // Rapidly alternate between put and get
        for (int i = 0; i < 1000; i++)
        {
            cache.Put("key", i);
            var value = cache.Get("key");
            Assert.Equal(i, value);
        }
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(10000)]
    public void Cache_DifferentCapacities_WorksCorrectly(int capacity)
    {
        var cache = new LruCache<string, int>(capacity: capacity);
        
        // Fill to capacity
        for (int i = 0; i < capacity; i++)
        {
            cache.Put(i.ToString(), i);
        }
        
        // Verify all items are present
        for (int i = 0; i < capacity; i++)
        {
            Assert.Equal(i, cache.Get(i.ToString()));
        }
        
        // Add one more item
        cache.Put(capacity.ToString(), capacity);
        
        // First item should be evicted
        Assert.Equal(0, cache.Get("0"));
    }

    [Fact]
    public void Cache_LargeValues_HandlesCorrectly()
    {
        var cache = new LruCache<string, byte[]>(capacity: 3);
        var largeValue = new byte[1024 * 1024]; // 1MB
        
        // Fill with large values
        for (int i = 0; i < 5; i++)
        {
            cache.Put(i.ToString(), largeValue);
        }
        
        // Verify some values are present and some are evicted
        Assert.Null(cache.Get("0")); // Should be evicted
        Assert.Null(cache.Get("1")); // Should be evicted
        Assert.NotNull(cache.Get("2")); // Should be present
        Assert.NotNull(cache.Get("3")); // Should be present
        Assert.NotNull(cache.Get("4")); // Should be present
    }

    #endregion

    #region Performance Characteristics

    [Fact]
    public void Performance_RepeatedAccess_MaintainsOrder()
    {
        var cache = new LruCache<string, int>(capacity: 3);
        
        // Add initial items
        cache.Put("1", 1);
        cache.Put("2", 2);
        cache.Put("3", 3);
        
        // Repeatedly access the first item
        for (int i = 0; i < 1000; i++)
        {
            cache.Get("1");
        }
        
        // Add new item
        cache.Put("4", 4);
        
        // "1" should still be present due to frequent access
        Assert.Equal(1, cache.Get("1"));
        // "2" should be evicted as it was least recently used
        Assert.Equal(0, cache.Get("2"));
    }

    #endregion

    #region Memory Management

    [Fact]
    public void Memory_LargeObjectsAreProperlyCollected()
    {
        var cache = new LruCache<string, byte[]>(capacity: 3);
        var weakRefs = new List<WeakReference>();

        // Create and cache large objects
        for (int i = 0; i < 10; i++)
        {
            var largeObject = new byte[1024 * 1024 * 10]; // 10MB
            cache.Put($"key_{i}", largeObject);
            weakRefs.Add(new WeakReference(largeObject));
        }

        // Force garbage collection
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        // Check that evicted objects are collected
        int collectedCount = weakRefs.Count(wr => !wr.IsAlive);
        Assert.True(collectedCount >= 7); // At least 7 objects should be collected (10 - cache capacity)
    }

    [Fact]
    public void Memory_CacheUnderPressure()
    {
        var cache = new LruCache<string, string>(capacity: 1000);
        var longString = new string('x', 1000000); // 1MB string

        // Fill cache with large strings under memory pressure
        for (int i = 0; i < 1500; i++)
        {
            cache.Put($"key_{i}", longString);

            if (i % 100 == 0)
            {
                GC.Collect();
            }
        }

        // Verify cache is still functional
        Assert.NotNull(cache.Get("key_1499")); // Latest item
        Assert.Null(cache.Get("key_0")); // Should be evicted
    }

    #endregion

    #region Type Safety

    [Fact]
    public void TypeSafety_ValueTypes()
    {
        var cache = new LruCache<string, int>(capacity: 3);
        cache.Put("key", 42);
        Assert.Equal(42, cache.Get("key"));

        var structCache = new LruCache<string, DateTime>(capacity: 3);
        var now = DateTime.UtcNow;
        structCache.Put("time", now);
        Assert.Equal(now, structCache.Get("time"));
    }

    [Fact]
    public void TypeSafety_ReferenceTypes()
    {
        var cache = new LruCache<string, object>(capacity: 3);
        var obj = new object();
        cache.Put("key", obj);
        Assert.Same(obj, cache.Get("key")); // Reference equality

        var listCache = new LruCache<string, List<int>>(capacity: 3);
        var list = new List<int> { 1, 2, 3 };
        listCache.Put("list", list);
        Assert.Same(list, listCache.Get("list")); // Reference equality
    }

    [Fact]
    public void TypeSafety_NullableTypes()
    {
        var cache = new LruCache<string, int?>(capacity: 3);
        cache.Put("null", null);
        cache.Put("value", 42);

        Assert.Null(cache.Get("null"));
        Assert.Equal(42, cache.Get("value"));
    }

    [Fact]
    public void TypeSafety_CustomTypes()
    {
        var cache = new LruCache<string, CustomType>(capacity: 3);
        var custom = new CustomType { Id = 1, Name = "Test" };
        cache.Put("custom", custom);

        var retrieved = cache.Get("custom");
        Assert.NotNull(retrieved);
        Assert.Equal(1, retrieved.Id);
        Assert.Equal("Test", retrieved.Name);
    }

    private class CustomType
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }

    #endregion

    #region Error Handling

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void ErrorHandling_NegativeCapacity_ThrowsArgumentException(int capacity)
    {
        Assert.Throws<ArgumentException>(() => new LruCache<string, int>(capacity));
    }

    #endregion

    #region Load Testing

    [Fact]
    public void LoadTest_SustainedHighThroughput()
    {
        var cache = new LruCache<string, int>(capacity: 10000);
        var sw = Stopwatch.StartNew();
        var operationCount = 1000000;
        var successfulOps = 0;

        Parallel.For(0, operationCount, i =>
        {
            try
            {
                if (i % 2 == 0)
                {
                    cache.Put(i.ToString(), i);
                }
                else
                {
                    cache.Get((i / 2).ToString());
                }
                Interlocked.Increment(ref successfulOps);
            }
            catch
            {
                // Count failed operations
            }
        });

        sw.Stop();
        
        // Verify performance
        Assert.True(sw.ElapsedMilliseconds < 30000); // Should complete within 30 seconds
        Assert.True(successfulOps > operationCount * 0.99); // 99% success rate
    }

    [Fact]
    public async Task LoadTest_MixedWorkload()
    {
        var cache = new LruCache<string, string>(capacity: 1000);
        var operations = new ConcurrentDictionary<int, int>();
        var random = new Random();

        // Run different types of workloads concurrently
        var tasks = new List<Task>();

        // Heavy write workload
        tasks.Add(Task.Run(() =>
        {
            for (int i = 0; i < 10000; i++)
            {
                cache.Put($"write_{i}", $"value_{i}");
                operations.AddOrUpdate(0, 1, (k, v) => v + 1);
            }
        }));

        // Read-heavy workload
        tasks.Add(Task.Run(() =>
        {
            for (int i = 0; i < 50000; i++)
            {
                cache.Get($"write_{random.Next(10000)}");
                operations.AddOrUpdate(1, 1, (k, v) => v + 1);
            }
        }));

        // Mixed workload
        tasks.Add(Task.Run(() =>
        {
            for (int i = 0; i < 20000; i++)
            {
                if (random.Next(2) == 0)
                {
                    cache.Put($"mixed_{i}", $"value_{i}");
                }
                else
                {
                    cache.Get($"mixed_{random.Next(i + 1)}");
                }
                operations.AddOrUpdate(2, 1, (k, v) => v + 1);
            }
        }));

        await Task.WhenAll(tasks.ToArray());

        // Verify all workloads completed
        Assert.True(operations[0] == 10000); // Write workload
        Assert.True(operations[1] == 50000); // Read workload
        Assert.True(operations[2] == 20000); // Mixed workload
    }

    [Fact]
    public async Task LoadTest_RecoveryAfterHeavyLoad()
    {
        var cache = new LruCache<string, int>(capacity: 1000);
        var heavyLoadTask = Task.Run(() =>
        {
            Parallel.For(0, 1000000, i =>
            {
                cache.Put(i.ToString(), i);
                cache.Get((i / 2).ToString());
            });
        });

        // Wait for heavy load to complete
        await heavyLoadTask.WaitAsync(new CancellationToken());

        // Verify cache is still functional
        for (int i = 0; i < 100; i++)
        {
            cache.Put($"test_{i}", i);
            Assert.Equal(i, cache.Get($"test_{i}"));
        }
    }

    #endregion

    #region Integration Scenarios

    [Fact]
    public void Integration_MultipleCacheInstances()
    {
        var cache1 = new LruCache<string, int>(capacity: 100);
        var cache2 = new LruCache<string, int>(capacity: 100);

        // Parallel operations on both caches
        Parallel.For(0, 1000, i =>
        {
            cache1.Put($"key_{i}", i);
            cache2.Put($"key_{i}", i * 2);
        });

        // Cross-reference values between caches
        for (int i = 0; i < 1000; i++)
        {
            var val1 = cache1.Get($"key_{i}");
            var val2 = cache2.Get($"key_{i}");
            if (val1 != default && val2 != default)
            {
                Assert.Equal(val1 * 2, val2);
            }
        }
    }

    [Fact]
    public async Task Integration_CrossThreadCommunication()
    {
        var cache = new LruCache<string, string>(capacity: 100);
        var signal = new ManualResetEventSlim();
        var success = false;

        // Producer thread
        var producer = Task.Run(() =>
        {
            for (int i = 0; i < 100; i++)
            {
                cache.Put($"key_{i}", $"value_{i}");
            }
            signal.Set();
        });

        // Consumer thread
        var consumer = Task.Run(() =>
        {
            signal.Wait();
            success = true;
            for (int i = 0; i < 100; i++)
            {
                var value = cache.Get($"key_{i}");
                if (value != $"value_{i}")
                {
                    success = false;
                    break;
                }
            }
        });

        await Task.WhenAll(producer, consumer);
        Assert.True(success);
    }

    [Fact]
    public void Integration_SystemResourceConstraints()
    {
        var cache = new LruCache<string, byte[]>(capacity: 100);
        var random = new Random();
        var allocated = new List<byte[]>();

        try
        {
            // Try to allocate large amounts of memory while using cache
            for (int i = 0; i < 100; i++)
            {
                var data = new byte[1024 * 1024 * 10]; // 10MB
                random.NextBytes(data);
                cache.Put($"key_{i}", data);
                allocated.Add(data);

                // Verify cache still works
                var retrieved = cache.Get($"key_{i}");
                Assert.NotNull(retrieved);
                Assert.Equal(data.Length, retrieved.Length);
            }
        }
        catch (OutOfMemoryException)
        {
            // Test is successful even if we run out of memory
            // The important thing is that the cache doesn't corrupt
        }

        // Verify cache is still functional
        cache.Put("test", new byte[100]);
        Assert.NotNull(cache.Get("test"));
    }

    #endregion

    #region Value Type Handling

    [Fact]
    public void ValueType_CanStoreAndRetrieveZero()
    {
        var cache = new LruCache<string, int>(capacity: 3);
        cache.Put("zero", 0);

        // Using Get
        var result = cache.Get("zero");
        Assert.Equal(0, result);

        // Using TryGet
        bool found = cache.TryGet("zero", out int value);
        Assert.True(found);
        Assert.Equal(0, value);

        // Non-existent key
        bool notFound = cache.TryGet("nonexistent", out int defaultValue);
        Assert.False(notFound);
        Assert.Equal(0, defaultValue); // Default value for int
    }

    [Fact]
    public void ValueType_DistinguishBetweenDefaultAndNonExistent()
    {
        var cache = new LruCache<string, int>(capacity: 3);
        
        // Store default value (0)
        cache.Put("zero", 0);
        
        // Test existing key with default value
        bool foundZero = cache.TryGet("zero", out int zero);
        Assert.True(foundZero); // Key exists
        Assert.Equal(0, zero);
        
        // Test non-existent key
        bool foundNonExistent = cache.TryGet("nonexistent", out int nonExistent);
        Assert.False(foundNonExistent); // Key doesn't exist
        Assert.Equal(0, nonExistent); // Out parameter gets default value
    }

    [Fact]
    public void ValueType_HandlesAllDefaultValues()
    {
        // Test various value types with their default values
        var intCache = new LruCache<string, int>(capacity: 3);
        var dateCache = new LruCache<string, DateTime>(capacity: 3);
        var boolCache = new LruCache<string, bool>(capacity: 3);
        
        // Store default values
        intCache.Put("default", 0);
        dateCache.Put("default", default);
        boolCache.Put("default", false);
        
        // Verify TryGet can distinguish between stored defaults and non-existent keys
        bool intFound = intCache.TryGet("default", out int intValue);
        bool dateFound = dateCache.TryGet("default", out DateTime dateValue);
        bool boolFound = boolCache.TryGet("default", out bool boolValue);
        
        Assert.True(intFound);
        Assert.True(dateFound);
        Assert.True(boolFound);
        
        // Verify non-existent keys
        Assert.False(intCache.TryGet("nonexistent", out _));
        Assert.False(dateCache.TryGet("nonexistent", out _));
        Assert.False(boolCache.TryGet("nonexistent", out _));
    }

    #endregion
} 
