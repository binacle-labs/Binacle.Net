using Binacle.Reporting;
using Binacle.ViPaq.EncodedSize.ExtensionMethods;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Binacle.ViPaq.EncodedSize;

internal class Program
{
	static async Task Main(string[] args)
	{
		Log.Logger = new LoggerConfiguration()
			.MinimumLevel.Debug()
			.Enrich.FromLogContext()
			.Enrich.WithMachineName()
			.Enrich.WithThreadId()
			.WriteTo.Console(
				outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {NewLine}",
				theme: AnsiConsoleTheme.Code
			)
			.CreateBootstrapLogger();

		var builder = Host.CreateApplicationBuilder();
		builder.Logging.ClearProviders();
		builder.Logging.AddSerilog();

		// The reports are tracked files: the writer overwrites vipaq/results/ and a change shows up as a diff.
		var resultsDirectory = RepositoryRoot.Bind().Find("vipaq", "results");
		builder.Services.AddSingleton<IFileWriter>(new MarkdownFileWriter(resultsDirectory));
		builder.Services.AddTransient<Measure>();
		builder.Services.AddPreReportChecks();
		builder.Services.AddVipaqProtobufSizeComparisonTests();
		builder.Services.AddCodecCompressionCrossoverTests();

		IHost host = builder.Build();

		using var scope = host.Services.CreateScope();

		// Fail fast before the reports: run every registered gate (round-trip conformance, curated-pick resolution).
		scope.ServiceProvider.RunPreReportChecks();

		var measure = scope.ServiceProvider.GetRequiredService<Measure>();
		await measure.RunAsync();
	}
}
