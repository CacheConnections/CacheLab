using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Cache.Caches;

namespace Cache.Benchmarks.CacheBenchmarks;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class LruBenchmarks 
{
    private LruCache<string> _cache;
    private readonly string[] _keys;
    private readonly string[] _values;
    private readonly string[] _overflowKeys;
    private readonly string[] _overflowValues;
    
    private const int CacheSize = 10000;
    private const int OperationCount = 20000; // Increased to ensure we test eviction
    private const int OverflowSize = 5000; // Additional items beyond cache capacity

    [Params(1, 2, 4, 8)]
    public int ThreadCount { get; set; }

    public LruBenchmarks()
    {
        _cache = new LruCache<string>(capacity: CacheSize);
        
        // Keys/values that fit within cache capacity
        _keys = Enumerable.Range(0, CacheSize).Select(i => $"key_{i}").ToArray();
        _values = Enumerable.Range(0, CacheSize).Select(i => $"value_{i}").ToArray();

        // Additional keys/values that will cause eviction
        _overflowKeys = Enumerable.Range(CacheSize, OverflowSize).Select(i => $"key_{i}").ToArray();
        _overflowValues = Enumerable.Range(CacheSize, OverflowSize).Select(i => $"value_{i}").ToArray();
    }   

    [GlobalSetup]
    public void Setup()
    {
        _cache = new LruCache<string>(capacity: CacheSize);
    }

    [Benchmark]
    public void SingleThreaded_Insertions_WithEviction()
    {
        // First fill the cache to capacity
        for (int i = 0; i < CacheSize; i++)
        {
            _cache.Put(_keys[i], _values[i]);
        }

        // Then insert additional items to force eviction
        for (int i = 0; i < OverflowSize; i++)
        {
            _cache.Put(_overflowKeys[i], _overflowValues[i]);
        }
    }

    [Benchmark]
    public void Concurrent_Insertions_WithEviction()
    {
        // First fill the cache to capacity
        for (int i = 0; i < CacheSize; i++)
        {
            _cache.Put(_keys[i], _values[i]);
        }

        var tasks = new Task[ThreadCount];
        for (int t = 0; t < ThreadCount; t++)
        {
            int threadId = t;
            tasks[t] = Task.Run(() =>
            {
                int itemsPerThread = OverflowSize / ThreadCount;
                int start = threadId * itemsPerThread;
                int end = start + itemsPerThread;
                
                for (int i = start; i < end; i++)
                {
                    _cache.Put(_overflowKeys[i], _overflowValues[i]);
                }
            });
        }
        Task.WaitAll(tasks);
    }

    [Benchmark]
    public void SingleThreaded_HitMissPattern()
    {
        // Fill cache to 80% capacity with initial data
        int initialSize = (int)(CacheSize * 0.8);
        for (int i = 0; i < initialSize; i++)
        {
            _cache.Put(_keys[i], _values[i]);
        }

        // Perform operations that mix hits and misses
        for (int i = 0; i < OperationCount; i++)
        {
            if (i % 4 == 0) // 25% miss ratio
            {
                // Try to get or insert items that aren't in cache
                string key = _overflowKeys[i % OverflowSize];
                if (i % 2 == 0)
                {
                    _cache.Get(key); // Miss
                }
                else
                {
                    _cache.Put(key, _overflowValues[i % OverflowSize]); // Insert causing eviction
                }
            }
            else
            {
                // Access items that should be in cache (hits)
                _cache.Get(_keys[i % initialSize]);
            }
        }
    }

    [Benchmark]
    public void Concurrent_HitMissPattern()
    {
        // Fill cache to 80% capacity with initial data
        int initialSize = (int)(CacheSize * 0.8);
        for (int i = 0; i < initialSize; i++)
        {
            _cache.Put(_keys[i], _values[i]);
        }

        var tasks = new Task[ThreadCount];
        for (int t = 0; t < ThreadCount; t++)
        {
            tasks[t] = Task.Run(() =>
            {
                int operationsPerThread = OperationCount / ThreadCount;
                for (int i = 0; i < operationsPerThread; i++)
                {
                    if (i % 4 == 0) // 25% miss ratio
                    {
                        string key = _overflowKeys[i % OverflowSize];
                        if (i % 2 == 0)
                        {
                            _cache.Get(key); // Miss
                        }
                        else
                        {
                            _cache.Put(key, _overflowValues[i % OverflowSize]); // Insert causing eviction
                        }
                    }
                    else
                    {
                        _cache.Get(_keys[i % initialSize]); // Hit
                    }
                }
            });
        }
        Task.WaitAll(tasks);
    }

    [Benchmark]
    public void HighContention_SameKeysAccess()
    {
        // Fill cache with a small set of items that will be heavily contended
        int contentionSetSize = 100;
        for (int i = 0; i < contentionSetSize; i++)
        {
            _cache.Put(_keys[i], _values[i]);
        }

        var tasks = new Task[ThreadCount];
        for (int t = 0; t < ThreadCount; t++)
        {
            tasks[t] = Task.Run(() =>
            {
                var random = new Random();
                for (int i = 0; i < OperationCount / ThreadCount; i++)
                {
                    int keyIndex = random.Next(contentionSetSize);
                    if (random.Next(2) == 0)
                    {
                        _cache.Put(_keys[keyIndex], _values[keyIndex]);
                    }
                    else
                    {
                        _cache.Get(_keys[keyIndex]);
                    }
                }
            });
        }
        Task.WaitAll(tasks);
    }
}
