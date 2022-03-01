# ConcurrentTrieMap
## Download
| Package | Link |
| ------- | ---- | 
| ConcurrentTrieMap | [![image](https://img.shields.io/nuget/v/ConcurrentTrieMap.svg)](https://www.nuget.org/packages/ConcurrentTrieMap/) |

## Quickstart
```csharp
// Build the trie
CtrieMap<int> ctrie = new CtrieMap<int>();
ctrie.Add("a", 1);
ctrie.Add("ab", 2);
ctrie.Add("abc", 3);

// Return a specific value
ctrie.GetValue("ab"); // Returns 2

// Return a list of tuples (key, values) in the trie starting at the specified prefix
ctrie.GetValues("ab"); // Returns [("ab", 2), ("abc", 3)]
```

## Description
ConcurrentTrieMap is a simple trie map implementation that guarantees thread safety on some basic operations such as adding, deleting and modifying entries in the map. This data structure is optimized for usage in scenarios where fast concurrent prefix lookups are required while still retaining excellent performance for updating the data structure with new data.
