namespace Binacle.ViPaq.UnitTests;

// Marks a helper that does the asserting on a test's behalf. See the copy in Binacle.Lib.UnitTests for the
// full story - the analyser matches the attribute by name alone, so each unit-test project declares its own.
[AttributeUsage(AttributeTargets.Method)]
public sealed class AssertionMethodAttribute : Attribute
{
}
