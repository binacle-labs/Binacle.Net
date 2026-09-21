namespace Binacle.Lib.Testing.Providers;

public static class BischoffCuratedProblemsProvider
{
	// Five scenarios curated out of the full OR-library suite, chosen so each one stresses something
	// different. Numbers are the fill percentage each algorithm reaches on that scenario.
	//
	//   scenario     BFD      FFD      WFD     covers
	//   thpack1_7    80.30%   78.08%   78.08%  typical container
	//   thpack1_44   83.86%   62.65%   69.43%  BFD wins big
	//   thpack2_30   88.17%   87.75%   87.40%  near tie
	//   thpack2_35   85.86%   75.77%   56.82%  WFD falls over
	//   thpack7_56   79.18%   69.10%   68.19%  most item types (20)
	public static Dictionary<string ,string> ScenarioDescriptions { get; }
		= new()
		{
			{ "typical container", "OrLibrary_thpack1_7" },
			{ "BFD wins big", "OrLibrary_thpack1_44" },
			{ "near tie", "OrLibrary_thpack2_30" },
			{ "WFD falls over", "OrLibrary_thpack2_35" },
			{ "most item types", "OrLibrary_thpack7_56" },
		};
	
	public static string[] GetBenchmarkScenarios()
		=> ScenarioDescriptions.Keys.ToArray();
}
