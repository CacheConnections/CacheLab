namespace Cache.Objects;

/// <summary>
/// A Node represents a node in the doubly linked list of the LruCache
/// </summary>
public class Node<T> 
{
    /// The next node in the list
    public Node<T>? Next { get; set; }

    /// The previous node in the list
    public Node<T>? Prev { get; set; }
    
    /// The value stored by the consumer in the LruCache
    public T? Val { get; set; }
    
    /// The string key the consumer uses for their stored value
    public string? Key { get; set; }

    /// <summary>
    /// Creates a new node
    /// </summary>
    /// <param name="key">The <see langword="string"/> key</param>
    /// <param name="val">The value of type T</param>
    /// <param name="next">The node to link after this new node</param>
    /// <param name="prev">The node to link previous to this node</param>
    public Node(T? val = default, Node<T>? next = null, Node<T>? prev = null, string? key = null) 
    {
        Next = next;
        Prev = prev;
        Val = val;
        Key = key;
    }
}
