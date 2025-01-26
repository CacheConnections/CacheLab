# LruCache Benchmark Run 1-26/2025

### Environment 

```
BenchmarkDotNet v0.14.0, macOS Sequoia 15.1.1 (24B2091) [Darwin 24.1.0]
Apple M4 Pro, 1 CPU, 12 logical and 12 physical cores
.NET SDK 9.0.101
  [Host]     : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD
  DefaultJob : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD
```

### Results

```
| Method                                 | ThreadCount | Mean       | Error    | StdDev   | Gen0    | Gen1    | Gen2   | Allocated |
|--------------------------------------- |------------ |-----------:|---------:|---------:|--------:|--------:|-------:|----------:|
| SingleThreaded_Insertions_WithEviction | 1           |   719.8 us |  2.35 us |  2.08 us | 86.9141 | 43.9453 | 0.9766 | 703.13 KB |
| HighContention_SameKeysAccess          | 1           |   728.9 us |  9.42 us |  8.35 us | 58.5938 |  2.9297 | 0.9766 | 473.86 KB |
| SingleThreaded_Insertions_WithEviction | 4           |   731.3 us |  1.42 us |  1.26 us | 86.9141 | 43.9453 | 0.9766 | 703.13 KB |
| Concurrent_Insertions_WithEviction     | 1           |   731.6 us |  4.05 us |  3.79 us | 86.9141 | 43.9453 | 0.9766 | 703.44 KB |
| SingleThreaded_Insertions_WithEviction | 8           |   733.9 us |  1.65 us |  1.37 us | 86.9141 | 43.9453 | 0.9766 | 703.13 KB |
| SingleThreaded_Insertions_WithEviction | 2           |   740.7 us |  2.93 us |  2.74 us | 86.9141 | 43.9453 | 0.9766 | 703.13 KB |
| Concurrent_Insertions_WithEviction     | 2           |   920.4 us |  3.66 us |  3.06 us | 86.9141 | 43.9453 | 0.9766 | 703.62 KB |
| Concurrent_Insertions_WithEviction     | 4           |   981.8 us |  3.20 us |  2.99 us | 87.8906 | 44.9219 | 1.9531 | 703.98 KB |
| Concurrent_Insertions_WithEviction     | 8           | 1,034.2 us |  6.77 us |  6.33 us | 87.8906 | 44.9219 | 1.9531 | 704.69 KB |
| Concurrent_HitMissPattern              | 1           | 1,047.9 us |  3.39 us |  2.83 us | 46.8750 | 23.4375 | 1.9531 | 375.32 KB |
| SingleThreaded_HitMissPattern          | 8           | 1,050.4 us |  2.48 us |  2.32 us | 46.8750 | 23.4375 | 1.9531 | 375.02 KB |
| SingleThreaded_HitMissPattern          | 4           | 1,051.7 us |  2.85 us |  2.53 us | 46.8750 | 23.4375 | 1.9531 | 375.01 KB |
| SingleThreaded_HitMissPattern          | 1           | 1,057.8 us |  2.93 us |  2.60 us | 46.8750 | 23.4375 | 1.9531 | 375.01 KB |
| SingleThreaded_HitMissPattern          | 2           | 1,058.6 us |  3.15 us |  2.79 us | 46.8750 | 23.4375 | 1.9531 | 375.01 KB |
| HighContention_SameKeysAccess          | 4           | 1,426.3 us | 11.15 us | 10.43 us | 58.5938 | 19.5313 | 1.9531 | 474.08 KB |
| HighContention_SameKeysAccess          | 8           | 1,431.9 us |  6.77 us |  6.34 us | 58.5938 | 19.5313 | 1.9531 | 475.17 KB |
| HighContention_SameKeysAccess          | 2           | 1,440.3 us | 18.77 us | 17.56 us | 58.5938 |  5.8594 | 1.9531 | 474.15 KB |
| Concurrent_HitMissPattern              | 2           | 1,468.5 us |  6.89 us |  6.11 us | 46.8750 | 23.4375 | 1.9531 | 375.41 KB |
| Concurrent_HitMissPattern              | 4           | 1,688.3 us | 14.08 us | 13.17 us | 46.8750 | 23.4375 | 1.9531 | 375.56 KB |
| Concurrent_HitMissPattern              | 8           | 1,726.4 us |  8.40 us |  7.86 us | 46.8750 | 25.3906 | 1.9531 | 375.88 KB |
```
