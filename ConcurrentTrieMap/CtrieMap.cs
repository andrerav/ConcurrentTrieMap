using System.Collections.Generic;
using System.Linq;

namespace ConcurrentTrieMap
{
    /// <summary>
    /// ConcurrentTrie -- a thread safe prefix tree (trie) data structure.
    /// This implementation is based on node-level locks
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CtrieMap<T>
    {
        protected CtrieOptions Options = null;
        public CtrieNode<T> RootNode { get; set; }
        public int Count => RootNode.Count;

        public CtrieMap()
        {
            RootNode = new CtrieNode<T>(' ', parent: null);
        }

        public CtrieMap(int concurrencyLevel, int initialChildNodeCapacity) : this()
        {
            Options = new(concurrencyLevel, initialChildNodeCapacity);
        }

        public CtrieMap(CtrieOptions options) : this()
        {
            Options = options;
        }

        public void Add(string key, T value)
        {
            var trieNode = RootNode;
            foreach (var c in key)
            {
                trieNode = trieNode.GetOrAddChild(c);
            }
            trieNode.Value = value;
        }

        public void Remove(string key)
        {
            var nodeToBeRemoved = GetNodeByKey(key);

            // Node not found
            if (nodeToBeRemoved == null)
            {
                return;
            }

            // Reset the current value and return if this node has children
            else if (nodeToBeRemoved.Children != null && nodeToBeRemoved.Children.Count > 0)
            {
                nodeToBeRemoved.ClearValue();
                return;
            }

            // Get the list of nodes that lead up to this node
            List<CtrieNode<T>> nodeChain = new();
            var currentNode = nodeToBeRemoved;
            while (currentNode.Parent != null && currentNode != RootNode)
            {
                nodeChain.Add(currentNode);
                currentNode = currentNode.Parent;
            }

            // Remove all parent nodes if there are no other children
            foreach (var c in nodeChain)
            {
                if (c == nodeToBeRemoved || (c.Count == 0 && !c.HasValue))
                {
                    c.ClearValue();
                    c.Parent.RemoveChild(c.Char);
                }
            }
        }

        public bool ContainsKey(string key)
        {
            CtrieNode<T> node = GetNodeByKey(key);

            return node != null && node.HasValue;
        }

        public T GetValue(string key)
        {
            CtrieNode<T> node = GetNodeByKey(key);
            if (node != null)
            {
                return node.Value;
            }
            return default(T);
        }

        public CtrieNode<T> GetNodeByKey(string key)
        {
            var node = RootNode;

            foreach (var c in key)
            {
                node = node?.GetChild(c);
                if (node == null) break;
            }

            return node;
        }

        public IEnumerable<CtrieNode<T>> GetNodesByValue(T value)
        {
            return GetNodesByValue(value, RootNode);
        }

        private IEnumerable<CtrieNode<T>> GetNodesByValue(T value, CtrieNode<T> node)
        {
            if (node.Value.Equals(value))
            {
                yield return node;
            }

            if (node.Children == null)
            {
                yield break;
            }

            foreach (var childNode in node.Children
                .SelectMany(child => GetNodesByValue(value, child.Value)))
            {
                yield return childNode;
            }
        }

        public IEnumerable<CtrieNode<T>> GetAllNodes()
        {
            return GetAllNodes(RootNode);
        }

        private IEnumerable<CtrieNode<T>> GetAllNodes(CtrieNode<T> node)
        {
            yield return node;

            if (node.Children == null)
            {
                yield break;
            }

            foreach (var childNode in node.Children
                .SelectMany(kvp => GetAllNodes(kvp.Value)))
            {
                yield return childNode;
            }
        }
    }
}