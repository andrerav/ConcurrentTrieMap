using System.Collections.Generic;

namespace ConcurrentTrieMap
{
    public interface ICtrieMap<T>
    {
        /// <summary>
        /// Returns the number of nodes in the trie map that contains a value
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Returns the root node. Note that operations on the root node or child nodes thereof
        /// are not guaranteed to be thread safe.
        /// </summary>
        CtrieNode<T> RootNode { get; set; }

        /// <summary>
        /// Add a new value to the trie map with the given key. 
        /// This operation is thread safe.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Add(string key, T value);

        /// <summary>
        /// Returns true if the trie map contains a value mapped to the given key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool ContainsKey(string key);

        /// <summary>
        /// Returns a list of all nodes in the trie map
        /// </summary>
        /// <returns></returns>
        IEnumerable<CtrieNode<T>> GetAllNodes();

        /// <summary>
        /// Returns a specific node by key or null if that node does not exist
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        CtrieNode<T> GetNodeByKey(string key);

        /// <summary>
        /// Returns a list of nodes mapped to the given value. 
        /// Note that a single value can be mapped by multiple keys.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        IEnumerable<CtrieNode<T>> GetNodesByValue(T value);

        /// <summary>
        /// Returns the value mapped to the given key, or default(T) if it does not exist
        /// This operation is thread safe.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        T GetValue(string key);

        /// <summary>
        /// Returns all values in the trie starting at the given key. The values are returned in (key, value) tuples.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        IEnumerable<(string, T)> GetValues(string key);

        /// <summary>
        /// Removes the given key and value from the trie map.
        /// This operation is thread safe.
        /// </summary>
        /// <param name="key"></param>
        void Remove(string key);
    }
}