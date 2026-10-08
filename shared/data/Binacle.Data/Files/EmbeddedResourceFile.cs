using System.Reflection;

namespace Binacle.Data.Files;

// One embedded resource. RelativeName is the manifest name after the prefix it was found by, unsplit: the
// shape of that name is the caller's to know.
public sealed class EmbeddedResourceFile
{
	private readonly Assembly assembly;

	internal EmbeddedResourceFile(Assembly assembly, string resourceName, string relativeName)
	{
		this.assembly = assembly;
		this.ResourceName = resourceName;
		this.RelativeName = relativeName;
	}

	public string ResourceName { get; }
	public string RelativeName { get; }

	public Stream OpenRead()
		=> this.assembly.GetManifestResourceStream(this.ResourceName)
		   ?? throw new FileNotFoundException($"Resource {this.ResourceName} not found in {this.assembly.GetName().Name}");
}
