# CacheLab

[![CI](https://github.com/USERNAME/CacheLab/actions/workflows/ci.yml/badge.svg)](https://github.com/USERNAME/CacheLab/actions/workflows/ci.yml)
[![codecov](https://codecov.io/gh/USERNAME/CacheLab/branch/main/graph/badge.svg)](https://codecov.io/gh/USERNAME/CacheLab)

A high-performance .NET caching library providing various cache implementation strategies optimized for different use cases.

## 🚀 Features

- Thread-safe cache implementations
- Multiple caching strategies:
  - LRU (Least Recently Used) Cache
  - More implementations coming soon...
- Comprehensive benchmarking suite
- High performance and low memory footprint
- Fully tested with high code coverage
- Modern .NET support (NET 9.0)

## 📦 Project Structure

```
CacheLab/
├── src/
│   └── Cache/            # Core cache implementations
├── tests/
│   └── Cache.Tests/     # Unit tests
└── benchmarks/
    └── Cache.Benchmarks/ # Performance benchmarks
```

## 🔧 Installation

```bash
dotnet add package CacheLab  # Coming soon to NuGet
```

## 🎯 Quick Start

```csharp
using Cache.Caches;

// Create a new LRU cache with capacity of 1000 items
var cache = new LruCache<string>(capacity: 1000);

// Add items
cache.Put("key1", "value1");

// Retrieve items
string? value = cache.Get("key1");
```

## 🏃 Performance

The library includes comprehensive benchmarks for various scenarios:
- Single-threaded operations
- Concurrent access patterns
- Cache eviction scenarios
- Hit/miss ratio analysis
- High contention workloads

To run the benchmarks:

```bash
cd benchmarks/Cache.Benchmarks
dotnet run -c Release
```

## 🧪 Running Tests

```bash
dotnet test
```

## 🤝 Contributing

Contributions are welcome! Here's how you can help:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Areas for Contribution

- Additional cache implementations (MRU, LFU, etc.)
- Performance optimizations
- Documentation improvements
- Bug fixes
- New features and enhancements

## 📝 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🎯 Roadmap

- [ ] Pluggable Architecture
  - [ ] ICachePolicy
  - [ ] ICacheBenchmarks
- [ ] Additional cache implementations
  - [ ] Most Recently Used (MRU)
  - [ ] Least Frequently Used (LFU)
  - [ ] TinyLFU
  - [ ] Time-Based Expiration
  - [ ] Two-Queue Cache (2Q)
  - [ ] ARC (Adaptive Replacement Cache)
  - [ ] LIRS (Low Inter-Reference Recency Set)
  - [ ] SLRU (Segmented LRU)
  - [ ] Clock/Second Chance Cache
  - [ ] Weighted Policies (Cost Aware)
  - [ ] Multi-tier/Hybrid Cache
- [ ] Better error handling and logging
- [ ] Documentation and examples
- [ ] Cache statistics and monitoring
- [ ] Cache persistence
- [ ] Event notifications for cache operations
- [ ] Distributed cache support
- [ ] Custom eviction policies
- [ ] Cache warm-up strategies