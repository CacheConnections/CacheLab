using Cache.Objects;

namespace Cache.Interfaces;

public interface ILruCache<T>
{
    /// <summary>
    /// Retrieve the value in the cache by the string <paramref name="key"/>
    /// </summary>
    /// <param name="key">The string key to lookup</param>
    T? Get(string key);

    /// <summary>
    /// Store the T <paramref name="val"/> with lookup string <paramref name="key"/>
    /// </summary>
    /// <param name="key">The string key to identify the value by in the cache</param>
    /// <param name="val">The value of type T</param>
    void Put(string key, T val);
}
