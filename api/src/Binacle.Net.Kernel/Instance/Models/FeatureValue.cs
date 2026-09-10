namespace Binacle.Net.Kernel.Instance.Models;

// Shared by SwitchedOn and PathValue and nothing else, so EnabledFeatures and IsFeatureEnabled can tell a
// feature from a preset bag with a type check instead of a name list.
public abstract class FeatureValue : InstanceValue
{
}
