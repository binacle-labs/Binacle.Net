using Binacle.Data;

namespace Binacle.Lib.UnitTests;

[Trait("Scenario Tests", "Actual calculation for the algorithms.")]
public class FittingCustomProblemsTests : IClassFixture<CommonTestingFixture>
{
	private CommonTestingFixture Fixture { get; }
	public FittingCustomProblemsTests(CommonTestingFixture fixture)
	{
		this.Fixture = fixture;
	}

	[Theory]
	[MemberData(nameof(CustomProblems.DataProvider.TheoryNames), MemberType = typeof(CustomProblems.DataProvider))]
	public void CustomProblems_Fitting_FFD_v1(string scenario)
	{
		var testScenario = this.Fixture.GetScenarioByName(scenario);

		var result = this.Fixture.Run(AlgorithmFactories.FFD_v1, testScenario, AlgorithmOperation.Fitting);

		this.Fixture.AssertResult(testScenario, result);
	}

	[Theory]
	[MemberData(nameof(CustomProblems.DataProvider.TheoryNames), MemberType = typeof(CustomProblems.DataProvider))]
	public void CustomProblems_Fitting_FFD_v2(string scenario)
	{
		var testScenario = this.Fixture.GetScenarioByName(scenario);

		var result = this.Fixture.Run(AlgorithmFactories.FFD_v2, testScenario, AlgorithmOperation.Fitting);

		this.Fixture.AssertResult(testScenario, result);
	}

	[Theory]
	[MemberData(nameof(CustomProblems.DataProvider.TheoryNames), MemberType = typeof(CustomProblems.DataProvider))]
	public void CustomProblems_Packing_WFD_v1(string scenario)
	{
		var testScenario = this.Fixture.GetScenarioByName(scenario);

		var result = this.Fixture.Run(AlgorithmFactories.WFD_v1, testScenario, AlgorithmOperation.Fitting);

		this.Fixture.AssertResult(testScenario, result);
	}

	[Theory]
	[MemberData(nameof(CustomProblems.DataProvider.TheoryNames), MemberType = typeof(CustomProblems.DataProvider))]
	public void CustomProblems_Fitting_WFD_v2(string scenario)
	{
		var testScenario = this.Fixture.GetScenarioByName(scenario);

		var result = this.Fixture.Run(AlgorithmFactories.WFD_v2, testScenario, AlgorithmOperation.Fitting);

		this.Fixture.AssertResult(testScenario, result);
	}

	[Theory]
	[MemberData(nameof(CustomProblems.DataProvider.TheoryNames), MemberType = typeof(CustomProblems.DataProvider))]
	public void CustomProblems_Fitting_BFD_v1(string scenario)
	{
		var testScenario = this.Fixture.GetScenarioByName(scenario);

		var result = this.Fixture.Run(AlgorithmFactories.BFD_v1, testScenario, AlgorithmOperation.Fitting);

		this.Fixture.AssertResult(testScenario, result);
	}

	[Theory]
	[MemberData(nameof(CustomProblems.DataProvider.TheoryNames), MemberType = typeof(CustomProblems.DataProvider))]
	public void CustomProblems_Fitting_BFD_v2(string scenario)
	{
		var testScenario = this.Fixture.GetScenarioByName(scenario);

		var result = this.Fixture.Run(AlgorithmFactories.BFD_v2, testScenario, AlgorithmOperation.Fitting);

		this.Fixture.AssertResult(testScenario, result);
	}
}
