using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Binacle.ViPaq.Testing.ViPaq;

namespace Binacle.ViPaq.Benchmarks;

// Report columns read off the case's pack, beside its ScenarioName.
public static class PackColumns
{
	public static IColumn[] All { get; } =
	[
		new PackColumn("Items", 1, true, "Items in the pack",
			scenario => scenario.ItemCount.ToString()),
		// The same label the size results print; widths do not depend on the layout.
		new PackColumn("Widths", 2, false, "Bin / item / coordinate bits ViPaq picks for the pack",
			scenario => ViPaqHeader.Create(scenario, EncoderInfo.RowMajor).ToWidthsLabel()),
	];

	// The set each class loads from in its setup.
	private static Scenario ScenarioOf(BenchmarkCase benchmarkCase)
	{
		var name = (string)benchmarkCase.Parameters[nameof(BenchmarkBase.ScenarioName)]!;
		return benchmarkCase.Descriptor.Type.IsAssignableTo(typeof(CompressionCostBase))
			? CompressionCostSet.GetByName(name)
			: TimingSet.GetByName(name);
	}

	private sealed class PackColumn(
		string columnName,
		int priority,
		bool isNumeric,
		string legend,
		Func<Scenario, string> valueOf
	) : IColumn
	{
		public string Id => $"{nameof(PackColumn)}.{columnName}";
		public string ColumnName => columnName;
		public bool AlwaysShow => true;
		public ColumnCategory Category => ColumnCategory.Params;
		public int PriorityInCategory => priority;
		public bool IsNumeric => isNumeric;
		public UnitType UnitType => UnitType.Dimensionless;
		public string Legend => legend;

		public string GetValue(Summary summary, BenchmarkCase benchmarkCase)
			=> valueOf(ScenarioOf(benchmarkCase));

		public string GetValue(Summary summary, BenchmarkCase benchmarkCase, SummaryStyle style)
			=> this.GetValue(summary, benchmarkCase);

		public bool IsDefault(Summary summary, BenchmarkCase benchmarkCase) => false;
		public bool IsAvailable(Summary summary) => true;
	}
}
