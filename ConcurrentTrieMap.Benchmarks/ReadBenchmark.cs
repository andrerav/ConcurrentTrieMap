using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using ConcurrentTrieMap;
using ConcurrentTrieMap.Tests;
using rm.Trie;

[SimpleJob(RuntimeMoniker.Net60)]
[RPlotExporter]
public class ReadBenchmark : AbstractTrieBenchmarks
{
	CtrieMap<int>? ctrie;
	TrieMap<int>? trieMap;

	[GlobalSetup]
	public override void Setup()
	{
		base.Setup();

		ctrie = new CtrieMap<int>(Environment.ProcessorCount, TestUtils.Chars.Length);
		Parallel.ForEach(Fasit, kvp =>
		{
			ctrie.Add(kvp.Key, kvp.Value);
		});

		trieMap = new TrieMap<int>();
		Parallel.ForEach(Fasit, kvp =>
		{
			lock (trieMap)
			{
				trieMap.Add(kvp.Key, kvp.Value);
			}
		});

	}

	[Benchmark]
	public void CtrieRead()
	{
		ArgumentNullException.ThrowIfNull(ctrie);

		Parallel.ForEach(Fasit, kvp =>
		{
			ctrie.GetValue(kvp.Key);
		});
	}

	[Benchmark]
	public void RmTrieRead()
	{
		ArgumentNullException.ThrowIfNull(trieMap);

		Parallel.ForEach(Fasit, kvp =>
		{
			lock (trieMap)
			{
				trieMap.ValueBy(kvp.Key);
			}
		});
	}
}