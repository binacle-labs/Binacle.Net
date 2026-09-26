using BischoffSuite = Binacle.Data.BischoffSuite.DataProvider;

namespace Binacle.Lib.Testing;

// Thirty of the 700, spread by FFD+BFD time from the smallest job to the largest, with an even and a lopsided
// problem at each size. The name printed is "<problem> (<items>i/<item types>t)".
public static class CoresSet
{
	private static readonly (string Name, string Id)[] picks =
	[
		("th1_72 (74i/3t)", "thpack1_72"),
		("th2_61 (83i/5t)", "thpack2_61"),
		("th5_93 (84i/12t)", "thpack5_93"),
		("th1_7 (126i/3t)", "thpack1_7"),
		("th1_44 (142i/3t)", "thpack1_44"),
		("th2_21 (84i/5t)", "thpack2_21"),
		("th4_22 (111i/10t)", "thpack4_22"),
		("th3_67 (169i/8t)", "thpack3_67"),
		("th2_35 (147i/5t)", "thpack2_35"),
		("th3_17 (86i/8t)", "thpack3_17"),
		("th7_18 (103i/20t)", "thpack7_18"),
		("th5_35 (143i/12t)", "thpack5_35"),
		("th5_41 (133i/12t)", "thpack5_41"),
		("th4_2 (123i/10t)", "thpack4_2"),
		("th2_65 (187i/5t)", "thpack2_65"),
		("th3_13 (199i/8t)", "thpack3_13"),
		("th6_39 (183i/15t)", "thpack6_39"),
		("th2_13 (228i/5t)", "thpack2_13"),
		("th7_56 (162i/20t)", "thpack7_56"),
		("th3_23 (130i/8t)", "thpack3_23"),
		("th1_68 (238i/3t)", "thpack1_68"),
		("th4_84 (125i/10t)", "thpack4_84"),
		("th1_85 (319i/3t)", "thpack1_85"),
		("th3_29 (194i/8t)", "thpack3_29"),
		("th5_84 (138i/12t)", "thpack5_84"),
		("th2_30 (196i/5t)", "thpack2_30"),
		("th1_56 (408i/3t)", "thpack1_56"),
		("th1_30 (405i/3t)", "thpack1_30"),
		("th1_39 (243i/3t)", "thpack1_39"),
		("th1_65 (476i/3t)", "thpack1_65"),
	];

	private static readonly Dictionary<string, string> scenarios
		= picks.ToDictionary(p => p.Name, p => $"OrLibrary_{p.Id}");

	public static IEnumerable<string> Names
		=> picks.Select(p => p.Name);

	public static Scenario GetByName(string name)
		=> BischoffSuite.GetByName(scenarios[name]);
}
