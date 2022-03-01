using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ConcurrentTrieMap
{
    [DebuggerDisplay("{Char}")]
    public class CtrieNode<T> : ICtrieNode<T>
    {
        private readonly char _c;
        private readonly object _lockObject = new object();
        private Dictionary<char, CtrieNode<T>> _children = null;
        private bool _hasValue;
        private CtrieOptions _options = null;
        private CtrieNode<T> _parent = null;
        private T _t;

        public CtrieNode()
        {
        }

        public CtrieNode(char c, CtrieNode<T> parent)
        {
            _c = c;
            _parent = parent;
        }

        public CtrieNode(char c, CtrieNode<T> parent, CtrieOptions options) : this(c, parent)
        {
            _options = options;
        }

        /// <summary>
        /// Builds and returns the key for this node
        /// </summary>
        public string Key
        {
            get
            {
                var node = this;
                var buffer = new Stack<char>();
                while (node.Parent != null)
                {
                    buffer.Push(node.Char);
                    node = node.Parent;
                }

                return new string(buffer.ToArray());
            }
        }

        public char Char => _c;

        /// <summary>
        /// Returns the number of child nodes that has a value starting from and including this node
        /// </summary>
        public int Count => (_hasValue ? 1 : 0) + (_children?.Sum(c => c.Value?.Count) ?? 0);

        /// <summary>
        /// A dictionary containing the child nodes referenced from this node
        /// </summary>
        public Dictionary<char, CtrieNode<T>> Children
        {
            get
            {
                return _children;
            }
            set
            {
                _children = value;
            }
        }

        public bool HasValue => _hasValue;

        /// <summary>
        /// Returns the parent node for this node
        /// </summary>
        public CtrieNode<T> Parent => _parent;

        /// <summary>
        /// Removes the current value from this node and resets the HasValue flag
        /// </summary>
        internal void ClearValue()
        {
            lock (_lockObject)
            {
                _hasValue = false;
                _t = default(T);
            }
        }

        /// <summary>
        /// Returns the node value. Note that the node value object itself will not be protected by locks, and
        /// the caller is responsible for handling the value object in a thread safe manner.
        /// </summary>
        public T Value
        {
            get => _t;
            set
            {
                // Reference assignments are atomic
                _t = value;
                _hasValue = true;
            }
        }

        /// <summary>
        /// Removes the given child from this node
        /// </summary>
        /// <param name="char"></param>
        internal void RemoveChild(char @char)
        {
            lock (_lockObject)
            {
                _children.Remove(@char);
            }
        }

        /// <summary>
        /// Returns the specified child node.
        /// This operation will lock the current node while accessing the _children collection.
        /// </summary>
        /// <param name="c"></param>
        /// <returns>A CtrieNode of type T, or null if the child node does not exist</returns>
        public CtrieNode<T> GetChild(char c)
        {
            lock (_lockObject)
            {
                return _children?.GetValueOrDefault(c);
            }
        }

        /// <summary>
        /// Sets a lock on this node and lets the caller perform arbitrary
        /// modifications on the node value through a callback.
        /// </summary>
        /// <param name="callback"></param>
        public void Modify(Action<T> callback)
        {
            lock (_lockObject)
            {
                callback(_t);
            }
        }

        /// <summary>
        /// Adds the given child to the trie and returns it or returns the child if it already exists.
        /// This operation will lock the current node while accessing the _children collection.
        /// </summary>
        /// <param name="c"></param>
        /// <returns>A CtrieNode of type T</returns>
        internal CtrieNode<T> GetOrAddChild(char c)
        {
            lock (_lockObject)
            {
                if (_children == null)
                {
                    if (_options != null)
                    {
                        _children = new Dictionary<char, CtrieNode<T>>(
                                _options.InitialChildNodeCapacity);
                    }
                    else
                    {
                        _children = new Dictionary<char, CtrieNode<T>>();
                    }
                }

                if (!_children.ContainsKey(c))
                {
                    _children[c] = new CtrieNode<T>(c, parent: this, _options);
                }

                return _children[c];
            }
        }
    }
}