using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using System.Collections.Immutable;

namespace Binacle.Benchmarking;

public class AttributeOrderer : IOrderer
{
	public bool SeparateLogicalGroups => true;

	public IEnumerable<BenchmarkCase> GetExecutionOrder(ImmutableArray<BenchmarkCase> benchmarksCase, IEnumerable<BenchmarkLogicalGroupRule>? order = null)
	{
		return benchmarksCase;
	}

	public string GetHighlightGroupKey(BenchmarkCase benchmarkCase)
	{
		return GetGroupKey(benchmarkCase);
	}

	public IEnumerable<BenchmarkCase> GetSummaryOrder(ImmutableArray<BenchmarkCase> benchmarksCases, Summary summary)
	{
		return benchmarksCases
			.GroupBy(b => b.Parameters.DisplayInfo)
			.SelectMany(byParameters => byParameters
				.GroupBy(b => b.Job.DisplayInfo)
				.OrderBy(byJob => byJob.Key, StringComparer.Ordinal)
				.SelectMany(byJob => byJob.OrderBy(b => GetBenchmarkOrder(b))));
	}

	// Each job and category is its own group, so a ratio is taken against its own baseline.
	private static string GetGroupKey(BenchmarkCase benchmarkCase)
	{
		return $"{benchmarkCase.Parameters.DisplayInfo} | {benchmarkCase.Job.DisplayInfo} | {string.Join(",", benchmarkCase.Descriptor.Categories)}";
	}

	private static int GetBenchmarkOrder(BenchmarkCase benchmarkCase)
	{
		var orderAttr = benchmarkCase.Descriptor.WorkloadMethod
			.GetCustomAttributes(typeof(BenchmarkOrderAttribute), false)
			.FirstOrDefault() as BenchmarkOrderAttribute;

		return orderAttr?.Order ?? int.MaxValue;
	}

	public string? GetLogicalGroupKey(ImmutableArray<BenchmarkCase> allBenchmarksCases, BenchmarkCase benchmarkCase)
	{
		return GetGroupKey(benchmarkCase);
	}

	public IEnumerable<IGrouping<string, BenchmarkCase>> GetLogicalGroupOrder(IEnumerable<IGrouping<string, BenchmarkCase>> logicalGroups, IEnumerable<BenchmarkLogicalGroupRule>? order = null)
	{
		return logicalGroups;
	}

}
