using System.Collections;

namespace Binacle.Net.Kernel.UnitTests.Serialization.Providers;

// Values that must be refused rather than read as absent. Row: the value as the client wrote it, ready to be
// pasted into a JSON body.
internal class RejectedEnumValueProvider : IEnumerable<object[]>
{
	public IEnumerator<object[]> GetEnumerator()
	{
		yield return ["\"invalid\""];
		yield return ["\"FF D\""]; // a real name with a stray space
		yield return ["\"FFDD\""]; // a real name with a stray character

		// Tokens the BCL reads as an enum and this converter does not, each landing somewhere the client did
		// not name.
		yield return ["1"]; // the ordinal
		yield return ["0"];
		yield return ["-1"]; // not an ordinal at all, still read by the BCL

		// Tokens no enum spelling can arrive as.
		yield return ["true"];
		yield return ["[\"FFD\"]"];
		yield return ["{\"value\":\"FFD\"}"];
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
