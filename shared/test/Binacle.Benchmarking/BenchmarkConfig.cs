using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Reports;

namespace Binacle.Benchmarking;

// The one BDN config every bench project runs with.
public static class BenchmarkConfig
{
	public static IConfig Create()
	{
		// The defaults, but the only export is the GitHub markdown report.
		var defaults = DefaultConfig.Instance;
		var config = ManualConfig.CreateEmpty()
			.AddColumnProvider(defaults.GetColumnProviders().ToArray())
			.AddLogger(defaults.GetLoggers().ToArray())
			.AddAnalyser(defaults.GetAnalysers().ToArray())
			.AddValidator(defaults.GetValidators().ToArray())
			.AddExporter(MarkdownExporter.GitHub)
			.WithOptions(ConfigOptions.DisableLogFile)
			.WithBuildTimeout(TimeSpan.FromMinutes(20))
			.WithSummaryStyle(SummaryStyle.Default.WithMaxParameterColumnWidth(50))
			.WithOrderer(new AttributeOrderer());

		// Reports land beside the calling project, not in the current directory.
		config.ArtifactsPath = Path.GetFullPath(
			Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "BenchmarkDotNet.Artifacts"));

		return config;
	}
}
