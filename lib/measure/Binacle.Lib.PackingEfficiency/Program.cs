using Binacle.Lib.PackingEfficiency.Reporters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Binacle.Lib.PackingEfficiency;

internal class Program
{
	static async Task Main(string[] args)
	{
		var builder = Host.CreateApplicationBuilder();

		// The files are tracked: the writer overwrites lib/results/ and a change shows up as a diff.
		var resultsDirectory = RepositoryRoot.Bind().Find("lib", "results");
		builder.Services.AddSingleton<IFileWriter>(new MarkdownFileWriter(resultsDirectory));

		builder.Services.AddSingleton<PackingBag>();
		builder.Services.AddTransient<IRunner, PackingRunner>();
		builder.Services.AddTransient<IReporter, ReadmeReporter>();
		builder.Services.AddTransient<IReporter, PackingEfficiencyReporter>();
		builder.Services.AddTransient<IReporter, VersionParityReporter>();
		builder.Services.AddTransient<Measure>();

		IHost host = builder.Build();

		using var scope = host.Services.CreateScope();
		var measure = scope.ServiceProvider.GetRequiredService<Measure>();
		await measure.RunAsync();
	}
}
