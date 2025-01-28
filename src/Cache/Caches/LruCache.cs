using Cache.Objects;
using Cache.Interfaces;

namespace Cache.Caches;

/// <summary>
/// <inheritdoc/>
/// A least recently used cache implementation using a string as a key
///  pointing to a generic type value. 
///  Provides O(1) lookups, inserts and deletes
/// /// </summary>
public class LruCache<TKey, TValue> : ILruCache<TKey, TValue> where TKey: notnull
{
    /// The dictionary we use for O(1) lookups
    private Dictionary<TKey, Node<TKey, TValue>> _cache;

    /// The head of the linked list
    private Node<TKey, TValue> _head;

    /// The tail of the linked list
    private Node<TKey, TValue> _tail;

    /// The maximum capacity of the cache
    private int _capacity;

    /// Lock for concurrent operations
    private readonly System.Threading.Lock _lock = new();
    
    /// <summary>
    /// LruCache constructor -> creates an empty cache of size <paramref name="capacity">
    /// </summary>
    /// <param name="capacity>The maximum size of the cache</param>
    public LruCache(int capacity)
    {
        if (capacity <= 0)
        {
            throw new ArgumentException("Capacity must be greater than 0", nameof(capacity));
        }

        _cache = new Dictionary<TKey, Node<TKey, TValue>>(); 
        _head = new Node<TKey, TValue>();
        _tail = new Node<TKey, TValue>();
        _head.Next = _tail;
        _tail.Prev = _head;
        _capacity = capacity;
    }

    /// <summary>
    /// Adds the new node to the doubly linked list at the front 
    /// </summary>
    /// <param name="node">The new node to add to the list</param>
    private void AddNodeToList(Node<TKey, TValue> node)
    {
        Node<TKey, TValue> next  = _head.Next ?? _tail;
        _head.Next = node;
        node.Prev = _head;
        node.Next = next;
        next.Prev = node;
    }

    /// <summary>
    /// Removes the last node from the list when the cache reaches capacity
    /// </summary>
    /// <param name="node">The node to remove from the list</param>
    private void RemoveNodeFromList(Node<TKey, TValue> node)
    {
        Node<TKey, TValue> prev = node.Prev ?? _head; // if the previous node is null, then the current node is the head
        Node<TKey, TValue> next = node.Next ?? _tail; // if the next node is null, then the current node is the tail
        prev.Next = next;
        next.Prev = prev;
    }

    /// <inheritdoc/>
    public TValue? Get(TKey key) 
    {
        if (TryGet(key, out TValue value))
        {
            return value;
        }

        return default;
    }

    /// <inheritdoc/>
    public bool TryGet(TKey key, out TValue value)
    {
        lock (_lock) 
        {
            if (!_cache.ContainsKey(key)) 
            {
                value = default!;
                return false;
            }

            Node<TKey, TValue> node = _cache[key];
            RemoveNodeFromList(node);
            AddNodeToList(node);
            value = node.Val!;
            return true;
        }
    }

    /// <inheritdoc/>
    public void Put(TKey key, TValue val)
    {
        lock(_lock)
        {
            Node<TKey, TValue> node = new Node<TKey, TValue>(val, key: key);

            // The node already exists in the cache, move it to the front as it is the most recently used
            if (!_cache.TryAdd(node.Key!, node)) 
            {
                var existingNode = _cache[node.Key!];
                RemoveNodeFromList(existingNode);
                _cache[node.Key!] = node;
            }

            AddNodeToList(node);

            if (_cache.Count > _capacity) 
            {
                Node<TKey, TValue> lruNode = _tail.Prev ?? _head;
                RemoveNodeFromList(lruNode);
                _cache.Remove(lruNode.Key!);
            }
        }
    }
}

