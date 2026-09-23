using Binacle.ViPaq.Data.Packed;

namespace Binacle.ViPaq.EncodedSize.PreReportChecks;

// Every pack must land in a file. A new Bischoff set or a fourth algorithm would otherwise be encoded and then
// silently dropped, because the file list is fixed.
internal sealed class GroupCoverageCheck : IPreReportCheck
{
	public void Run()
	{
		var slugs = Groups.All.Select(group => group.Slug).ToHashSet();

		Assert(Groups.BischoffFamily, BischoffSuite.Names, slugs);
		Assert("custom-problems", CustomProblems.Names, slugs);
		Assert("demo-samples", DemoSamples.Names, slugs);
	}

	private static void Assert(string family, IEnumerable<string> names, HashSet<string> slugs)
	{
		foreach (var name in names)
		{
			var slug = Groups.Of(family, name);
			if (!slugs.Contains(slug))
			{
				throw new InvalidOperationException(
					$"Pack '{name}' belongs to group '{slug}', which has no file. Add it to Groups.All.");
			}

			var algorithm = EncodingRunner.AlgorithmOf(name);
			if (!Algorithms.All.Contains(algorithm))
			{
				throw new InvalidOperationException(
					$"Pack '{name}' was packed by {algorithm}, which has no file. Add it to Algorithms.All.");
			}
		}
	}
}
