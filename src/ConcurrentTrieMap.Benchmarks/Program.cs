using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using ConcurrentTrieMap;
using ConcurrentTrieMap.Tests;
using rm.Trie;

public static class Program
{
	public static void Main(string[] args)
	{
		var summary = BenchmarkRunner.Run<InsertBenchmark>();
	}
}
