using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Binacle.Reporting;

// Runs every IRunner, then every IReporter, then hands each file's sections to every IFileWriter.
// Register the bag, the runners, the reporters and the writers in DI; this ties them together.
public class Measure
{
	private readonly IServiceProvider serviceProvider;
	private readonly ILogger<Measure> logger;

	public Measure(
		IServiceProvider serviceProvider,
		ILogger<Measure> logger
	)
	{
		this.serviceProvider = serviceProvider;
		this.logger = logger;
	}

	public async Task RunAsync()
	{
		foreach (var runner in this.serviceProvider.GetServices<IRunner>())
		{
			runner.Run();
		}

		var fileWriters = this.serviceProvider.GetServices<IFileWriter>().ToArray();
		var reporters = this.serviceProvider.GetServices<IReporter>();
		var files = reporters.GroupBy(reporter => reporter.File);

		foreach (var fileReporters in files)
		{
			var file = fileReporters.Key;
			var sections = fileReporters.SelectMany(reporter => reporter.Report()).ToArray();
			foreach (var fileWriter in fileWriters)
			{
				await fileWriter.WriteAsync(file, sections);
			}

			this.logger.LogInformation("Wrote {Filename}.md", file.Filename);
		}
	}
}
