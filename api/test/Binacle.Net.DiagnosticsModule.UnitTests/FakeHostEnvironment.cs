using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace Binacle.Net.DiagnosticsModule.UnitTests;

// The debug endpoint reads EnvironmentName and nothing else.
internal sealed class FakeHostEnvironment : IHostEnvironment
{
	public string EnvironmentName { get; set; } = "Test";
	public string ApplicationName { get; set; } = "Binacle.Net";
	public string ContentRootPath { get; set; } = string.Empty;
	public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
}
