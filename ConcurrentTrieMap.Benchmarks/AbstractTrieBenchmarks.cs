using BenchmarkDotNet.Attributes;
using ConcurrentTrieMap.Tests;

public abstract class AbstractTrieBenchmarks
{
	[Params(3,6,12)]
	public int TrieWordLength;

	[Params(10000)]
	public int TrieWords;

	protected static Dictionary<string, int> Fasit = new Dictionary<string, int>();

	public virtual void Setup()
	{
		// This loop simply builds a "fasit" of sorts that will be used by the benchmarks to build
		// trie structures and verify results.
		while (true)
		{
			Fasit[TestUtils.RandomString(TrieWordLength)] = TestUtils.RandomInt();
			if (Fasit.Count >= TrieWords) break;
		}
	}
}
