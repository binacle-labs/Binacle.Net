using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace Binacle.Net.Kernel.UnitTests.Logs;

// Both processors resolve their directory as ContentRootPath + options.Path and then touch real files, so the
// tests give them a real throwaway one. Dispose deletes it.
internal sealed class LogsHost : IHostEnvironment, IDisposable
{
	public const string LogsFolder = "logs";

	public LogsHost()
	{
		this.ContentRootPath = Path.Combine(Path.GetTempPath(), $"binacle-logs-{Guid.NewGuid():N}");
		Directory.CreateDirectory(this.ContentRootPath);
	}

	public string LogsPath => Path.Combine(this.ContentRootPath, LogsFolder);

	public string EnvironmentName { get; set; } = "Test";
	public string ApplicationName { get; set; } = "Binacle.Net";
	public string ContentRootPath { get; set; }
	public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();

	public string[] LogFiles()
		=> Directory.Exists(this.LogsPath)
			? Directory.GetFiles(this.LogsPath).Select(Path.GetFileName).OfType<string>().Order().ToArray()
			: [];

	public void Dispose()
	{
		try
		{
			Directory.Delete(this.ContentRootPath, recursive: true);
		}
		catch (IOException)
		{
			// A leftover temp folder is not worth failing a green run over.
		}
	}
}
