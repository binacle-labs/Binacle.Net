namespace Binacle.Net.Kernel.Instance.Models;

// The feature is on and answers at a path. Whoever switches it on records the path, because some of them are
// configurable and nobody else can know where one ended up.
public sealed class PathValue : FeatureValue
{
	public string Path { get; }

	public PathValue(string path)
	{
		this.Path = path;
	}
}
