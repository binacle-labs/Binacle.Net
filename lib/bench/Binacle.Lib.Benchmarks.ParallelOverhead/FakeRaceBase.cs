using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Binacle.Benchmarking;
using Binacle.Data;
using Binacle.Lib.Abstractions;
using Binacle.Lib.AlgorithmProcessing;
using Binacle.Lib.Testing;
using Binacle.Packing;

namespace Binacle.Lib.Benchmarks.ParallelOverhead;

// What racing algorithms costs, with algorithms that do not pack. The three rows, and the setup every race
// class shares. `Bytes` is how much memory each fake algorithm walks: 0 is the pure machinery cost, above 0
// stands in for a real algorithm's working set.
[MemoryDiagnoser]
public abstract class FakeRaceBase
{
	private IAlgorithmProcessor loop = null!;
	private IAlgorithmProcessor parallelOneThread = null!;
	private IAlgorithmProcessor parallel = null!;
	private ScenarioBin bin = null!;
	private List<ScenarioItem> items = null!;

	// The algorithms the race is given. They never pack; only how many there are matters.
	protected abstract Algorithm[] Algorithms { get; }

	[Params(0, 65_536)]
	public int Bytes { get; set; }

	[GlobalSetup]
	public void GlobalSetup()
	{
		JobsByCoreCount.SetCoreCountAndCheck();

		// The smallest case there is. Nothing here packs it; the processors only carry it through.
		this.bin = IdenticalCase.Bin();
		this.items = IdenticalCase.Items(1);

		// One real result for every fake algorithm to hand back, because OperationResult cannot be built
		// outside Binacle.Lib.
		var realAlgorithm = AlgorithmFactories.FFD_v2(this.bin, this.items);
		var oneResult = realAlgorithm.Execute(Parameters());

		var factory = new FakeAlgorithmFactory(oneResult, this.Bytes);
		this.loop = new LoopAlgorithmProcessor(this.Algorithms, factory);
		// -1 lifts the processor's default cap, which is min(work, cores). The cap is the right production
		// default and it is what the packing projects measure; this project is measuring what a dispatch costs,
		// so it has to ask for the uncapped path or there is nothing left to measure at 1 CPU.
		this.parallel = new ParallelAlgorithmProcessor(this.Algorithms, factory, maxDegreeOfParallelism: -1);
		this.parallelOneThread = new ParallelAlgorithmProcessor(this.Algorithms, factory, maxDegreeOfParallelism: 1);

		var check = this.Run(this.parallel);
		if (check.Count != this.Algorithms.Length)
			throw new InvalidOperationException(
				$"The race returned {check.Count} results for {this.Algorithms.Length} algorithms.");
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

	private IDictionary<string, OperationResult> Run(IAlgorithmProcessor processor)
		=> processor.Process(this.bin, this.items, Parameters());
}
