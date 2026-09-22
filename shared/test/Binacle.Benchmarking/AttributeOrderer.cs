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
		return benchmarkCase.Parameters.DisplayInfo;
	}

	public IEnumerable<BenchmarkCase> GetSummaryOrder(ImmutableArray<BenchmarkCase> benchmarksCases, Summary summary)
	{
		return benchmarksCases
			.GroupBy(b => b.Parameters.DisplayInfo)
			.SelectMany(group => group.OrderBy(b => GetBenchmarkOrder(b)));
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
		return benchmarkCase.Parameters.DisplayInfo;
	}

	public IEnumerable<IGrouping<string, BenchmarkCase>> GetLogicalGroupOrder(IEnumerable<IGrouping<string, BenchmarkCase>> logicalGroups, IEnumerable<BenchmarkLogicalGroupRule>? order = null)
	{
		return logicalGroups;
	}

}
