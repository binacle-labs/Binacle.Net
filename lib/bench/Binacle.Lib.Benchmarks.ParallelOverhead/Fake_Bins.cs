using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Binacle.Benchmarking;
using Binacle.Data;
using Binacle.Lib.Abstractions;
using Binacle.Lib.Testing;
using Binacle.Packing;

namespace Binacle.Lib.Benchmarks.ParallelOverhead;

// What packing many bins at once costs, with algorithms that do not pack. The same three rows as the race
// classes, over the bin processors instead of the algorithm ones.
//
// Bins is a parameter rather than a class because a request carries any number of them, where a race is only
// ever 2 or 3 wide.
[MemoryDiagnoser]
public class Fake_Bins
{
	private IBinProcessor loop = null!;
	private IBinProcessor parallelOneThread = null!;
	private IBinProcessor parallel = null!;
	private List<ScenarioBin> bins = null!;
	private List<ScenarioItem> items = null!;

	[Params(1, 2, 8, 32)]
	public int Bins { get; set; }

	[Params(0, 65_536)]
	public int Bytes { get; set; }

	[GlobalSetup]
	public void GlobalSetup()
	{
		JobsByCoreCount.SetCoreCountAndCheck();

		var dimensions = IdenticalCase.Bin();
		this.bins = Enumerable.Range(1, this.Bins)
			.Select(i => new ScenarioBin($"bin{i}", dimensions))
			.ToList();
		this.items = IdenticalCase.Items(1);

		var realAlgorithm = AlgorithmFactories.FFD_v2(this.bins[0], this.items);
		var oneResult = realAlgorithm.Execute(Parameters());

		var factory = new FakeAlgorithmFactory(oneResult, this.Bytes);
		this.loop = new LoopBinProcessor(factory);
		// -1 lifts the processor's default cap, which is min(bins, cores) - see FakeRaceBase for why.
		this.parallel = new ParallelBinProcessor(factory, maxDegreeOfParallelism: -1);
		this.parallelOneThread = new ParallelBinProcessor(factory, maxDegreeOfParallelism: 1);

		var check = this.Run(this.parallel);
		if (check.Count != this.Bins)
			throw new InvalidOperationException($"{check.Count} results came back for {this.Bins} bins.");
	}

	[Benchmark(Baseline = true)]
	[BenchmarkOrder(10)]
	public IDictionary<string, OperationResult> Loop()
		=> this.Run(this.loop);

	[Benchmark]
	[BenchmarkOrder(20)]
	public IDictionary<string, OperationResult> Parallel_OneThread()
		=> this.Run(this.parallelOneThread);

	[Benchmark]
	[BenchmarkOrder(30)]
	public IDictionary<string, OperationResult> Parallel()
		=> this.Run(this.parallel);

	private static TestOperationParameters Parameters()
		=> new() { Operation = AlgorithmOperation.Packing };

	private IDictionary<string, OperationResult> Run(IBinProcessor processor)
		=> processor.Process(Algorithm.FFD, this.bins, this.items, Parameters());
}
