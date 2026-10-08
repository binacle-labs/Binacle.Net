using System.Reflection;

namespace Binacle.Data.Files;

public static class EmbeddedResourceFileProvider
{
	// The caller names the assembly; the executing one would be this project, which holds only the shared sets.
	public static List<EmbeddedResourceFile> ByPrefix(Assembly assembly, string prefix)
	{
		return assembly.GetManifestResourceNames()
			.Where(name => name.StartsWith(prefix, StringComparison.Ordinal))
			.OrderBy(name => name, StringComparer.Ordinal)
			.Select(name => new EmbeddedResourceFile(assembly, name, name.Substring(prefix.Length)))
			.ToList();
	}
}
