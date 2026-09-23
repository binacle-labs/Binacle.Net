using Binacle.Lib.ResultSelection;
using BestAlgorithm = Binacle.Lib.Data.ResultSelection.BestAlgorithm.DataProvider;
using BestBin = Binacle.Lib.Data.ResultSelection.BestBin.DataProvider;
using SmallestBin = Binacle.Lib.Data.ResultSelection.SmallestBin.DataProvider;

namespace Binacle.Lib.UnitTests;

[Trait("Scenario Tests", "Actual calculation for the algorithms.")]
public class ResultSelectionTests : IClassFixture<ResultSelectionTestingFixture>
{
	private readonly ResultSelectionTestingFixture fixture;

	public ResultSelectionTests(ResultSelectionTestingFixture fixture)
	{
		this.fixture = fixture;
	}

	[Theory]
	[MemberData(nameof(BestAlgorithm.TheoryNames), MemberType = typeof(BestAlgorithm))]
	public void BestAlgorithm_v1(string scenarioName)
	{
		var scenario = BestAlgorithm.GetByName(scenarioName);

		var selected = this.fixture.Select(scenario, new BestAlgorithm_v1(), x=> x.AlgorithmInfo.GetAlgorithmIdentifierName());

		selected.ShouldBe(scenario.ExpectedResult);
	}
    
	[Theory]
	[MemberData(nameof(BestAlgorithm.TheoryNames), MemberType = typeof(BestAlgorithm))]
	public void BestAlgorithm_v2(string scenarioName)
	{
		var scenario = BestAlgorithm.GetByName(scenarioName);

		var selected = this.fixture.Select(scenario, new BestAlgorithm_v2(), x=> x.AlgorithmInfo.GetAlgorithmIdentifierName());

		selected.ShouldBe(scenario.ExpectedResult);
	}
    
	[Theory]
	[MemberData(nameof(BestBin.TheoryNames), MemberType = typeof(BestBin))]
	public void BestBin_v1(string scenarioName)
	{
		var scenario = BestBin.GetByName(scenarioName);

		var selected = this.fixture.Select(scenario, new BestBin_v1(), x=> x.Bin.ID);

		selected.ShouldBe(scenario.ExpectedResult);
	}
    
	[Theory]
	[MemberData(nameof(BestBin.TheoryNames), MemberType = typeof(BestBin))]
	public void BestBin_v2(string scenarioName)
	{
		var scenario = BestBin.GetByName(scenarioName);

		var selected = this.fixture.Select(scenario, new BestBin_v2(), x=> x.Bin.ID);

		selected.ShouldBe(scenario.ExpectedResult);
	}
	
	[Theory]
	[MemberData(nameof(SmallestBin.TheoryNames), MemberType = typeof(SmallestBin))]
	public void SmallestBin_v1(string scenarioName)
	{
		var scenario = SmallestBin.GetByName(scenarioName);

		var selected = this.fixture.Select(scenario, new SmallestBin_v1(), x=> x.Bin.ID);

		selected.ShouldBe(scenario.ExpectedResult);
	}
    
	[Theory]
	[MemberData(nameof(SmallestBin.TheoryNames), MemberType = typeof(SmallestBin))]
	public void SmallestBin_v2(string scenarioName)
	{
		var scenario = SmallestBin.GetByName(scenarioName);

		var selected = this.fixture.Select(scenario, new SmallestBin_v2(), x=> x.Bin.ID);

		selected.ShouldBe(scenario.ExpectedResult);
	}
}
