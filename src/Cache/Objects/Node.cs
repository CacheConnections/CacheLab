namespace Cache.Objects;

/// <summary>
/// A Node represents a node in the doubly linked list of the LruCache
/// </summary>
public class Node<TKey, TValue> 
{
    /// The next node in the list
    public Node<TKey, TValue>? Next { get; set; }

    /// The previous node in the list
    public Node<TKey, TValue>? Prev { get; set; }
    
    /// The value stored by the consumer in the LruCache
    public TValue? Val { get; set; }
    
    /// The string key the consumer uses for their stored value
    public TKey? Key { get; set; }

    /// <summary>
    /// Creates a new node
    /// </summary>
    /// <param name="key">The <see langword="string"/> key</param>
    /// <param name="val">The value of type T</param>
    /// <param name="next">The node to link after this new node</param>
    /// <param name="prev">The node to link previous to this node</param>
    public Node(TValue? val = default, Node<TKey, TValue>? next = null, Node<TKey, TValue>? prev = null, TKey? key = default) 
    {
        Next = next;
        Prev = prev;
        Val = val;
        Key = key;
    }
}
