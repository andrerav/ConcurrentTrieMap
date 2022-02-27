using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using ConcurrentTrieMap;
using ConcurrentTrieMap.Tests;
using rm.Trie;

[SimpleJob(RuntimeMoniker.Net60)]
[RPlotExporter]
public class InsertBenchmark : AbstractTrieBenchmarks
{

	[GlobalSetup]
	public override void Setup()
	{
		base.Setup();
	}

	[Benchmark]
	public void CtrieInsertAndRead()
	{
		var ctrie = new CtrieMap<int>(Environment.ProcessorCount, TestUtils.Chars.Length);
		Parallel.ForEach(Fasit, kvp =>
		{
			ctrie.Add(kvp.Key, kvp.Value);
			ctrie.GetValue(kvp.Key);
		});
	}

	[Benchmark]
	public void RmTrieInsertAndRead()
	{
		var trieMap = new TrieMap<int>();
		Parallel.ForEach(Fasit, kvp =>
		{
			lock (trieMap)
			{
				trieMap.Add(kvp.Key, kvp.Value);
				trieMap.ValueBy(kvp.Key);
			}
		});
	}
}
