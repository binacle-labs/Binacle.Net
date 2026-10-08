namespace Binacle.Benchmarking;

// Marks a bench class whose question is answered: kept so the answer cannot regress quietly, and not worth
// machine time on an ordinary run. Nothing reads it yet, so marking a class changes nothing.
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class SkipAttribute : Attribute
{
	public SkipAttribute(string reason)
	{
		this.Reason = reason;
	}

	// What the run proved, so a reader knows why the class stopped running and can tell when that stops holding.
	public string Reason { get; }
}
