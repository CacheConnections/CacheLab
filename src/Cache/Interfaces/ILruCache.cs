using Cache.Objects;

namespace Cache.Interfaces;

public interface ILruCache<T>
{
    /// <summary>
    /// Retrieve the value in the cache by the string <paramref name="key"/>
    /// </summary>
    /// <param name="key">The string key to lookup</param>
    /// <returns>The value if found, null if not found</returns>
    T? Get(string key);

    /// <summary>
    /// Attempts to retrieve the value associated with the specified key
    /// </summary>
    /// <param name="key">The string key to lookup</param>
    /// <param name="value">When this method returns, contains the value associated with the specified key,
    /// if the key is found; otherwise, the default value for the type of the value parameter.</param>
    /// <returns>true if the cache contains an element with the specified key, otherwise, false.</returns>
    bool TryGet(string key, out T value);

    /// <summary>
    /// Store the T <paramref name="val"/> with lookup string <paramref name="key"/>
    /// </summary>
    /// <param name="key">The string key to identify the value by in the cache</param>
    /// <param name="val">The value of type T</param>
    void Put(string key, T val);
}
