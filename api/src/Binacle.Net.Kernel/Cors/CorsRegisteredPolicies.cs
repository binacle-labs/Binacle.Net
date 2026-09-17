namespace Binacle.Net.Kernel.Cors;

// Every policy name an owner registered, so the validator can name a key nobody asked for.
public sealed class CorsRegisteredPolicies
{
	public HashSet<string> Names { get; } = new(StringComparer.OrdinalIgnoreCase);
}
