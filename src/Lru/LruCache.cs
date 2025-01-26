namespace Lru;

/// <summary>
/// LruCache - string key -> T value
/// </summary>
public class LruCache<T> 
{
    /// The dictionary we use for O(1) lookups
    private Dictionary<string, Node<T>> _cache;

    /// The head of the linked list
    private Node<T> _head;

    /// The tail of the linked list
    private Node<T> _tail;

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
        _cache = new Dictionary<string, Node<T>>(); 
        _head = new Node<T>();
        _tail = new Node<T>();
        _head.Next = _tail;
        _tail.Prev = _head;
        _capacity = capacity;
    }

    /// <summary>
    /// Adds the new node to the doubly linked list at the front 
    /// </summary>
    /// <param name="node">The new node to add to the list</param>
    private void AddNodeToList(Node<T> node)
    {
        Node<T> next  = _head.Next ?? _tail;
        _head.Next = node;
        node.Prev = _head;
        node.Next = next;
        next.Prev = node;
    }

    /// <summary>
    /// Removes the last node from the list when the cache reaches capacity
    /// </summary>
    /// <param name="node">The node to remove from the list</param>
    private void RemoveNodeFromList(Node<T> node)
    {
        Node<T> prev = node.Prev ?? _head; // if the previous node is null, then the current node is the head
        Node<T> next = node.Next ?? _tail; // if the next node is null, then the current node is the tail
        prev.Next = next;
        next.Prev = prev;
    }

    /// <summary>
    /// Retrieve the value in the cache by the string <paramref name="key"/>
    /// </summary>
    /// <param name="key">The string key to lookup</param>
    public T? Get(string key) 
    {
        lock (_lock) 
        {
            if (!_cache.ContainsKey(key)) {
                return default;
            }

            Node<T> node = _cache[key];
            RemoveNodeFromList(node);
            AddNodeToList(node);
            return _cache[key].Val;
        }
    }

    /// <summary>
    /// Store the T <paramref name="val"/> with lookup string <paramref name="key"/>
    /// </summary>
    /// <param name="key">The string key to identify the value by in the cache</param>
    /// <param name="val">The value of type T</param>
    public void Put(string key, T val)
    {
        lock(_lock)
        {
            Node<T> node = new Node<T>(val, key: key);

            if (string.IsNullOrWhiteSpace(node.Key)) {
                return;
            }

            // The node already exists in the cache, move it to the front as it is the most recently used
            if (!_cache.TryAdd(node.Key, node)) {
                var existingNode = _cache[node.Key];
                RemoveNodeFromList(existingNode);
                _cache[node.Key] = node;
            }

            AddNodeToList(node);

            if (_cache.Count() > _capacity) {
                Node<T> lruNode = _tail.Prev ?? _head;
                RemoveNodeFromList(lruNode);
                _cache.Remove(lruNode.Key!);
            }
        }
    }
}

