using System;
using System.Collections.Generic;

namespace ConcurrentTrieMap
{
    public interface ICtrieNode<T>
    {
        char Char { get; }
        Dictionary<char, CtrieNode<T>> Children { get; set; }
        int Count { get; }
        bool HasValue { get; }
        string Key { get; }
        CtrieNode<T> Parent { get; }
        T Value { get; set; }
        CtrieNode<T> GetChild(char c);
        void Modify(Action<T> callback);
    }
}