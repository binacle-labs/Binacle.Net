using Binacle.ViPaq.EncodedSize.ExtensionMethods;
using Binacle.ViPaq.EncodedSize.Reporters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Binacle.ViPaq.EncodedSize;

internal class Program
{
	static async Task Main(string[] args)
	{
		var builder = Host.CreateApplicationBuilder();

		// The files are tracked: the writer overwrites vipaq/results/ and a change shows up as a diff.
		var resultsDirectory = RepositoryRoot.Bind().Find("vipaq", "results");
		builder.Services.AddSingleton<IFileWriter>(new MarkdownFileWriter(resultsDirectory));

		builder.Services.AddPreReportChecks();
		builder.Services.AddSingleton<EncodingBag>();
		builder.Services.AddTransient<IRunner, EncodingRunner>();
		builder.Services.AddTransient<IReporter, EncodedSizeReporter>();
		builder.Services.AddTransient<Measure>();

		IHost host = builder.Build();

		using var scope = host.Services.CreateScope();

		// Fail fast before anything is encoded: every curated pick must still name a real pack.
		scope.ServiceProvider.RunPreReportChecks();

		var measure = scope.ServiceProvider.GetRequiredService<Measure>();
		await measure.RunAsync();
	}
}
