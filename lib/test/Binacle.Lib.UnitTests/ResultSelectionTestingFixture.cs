using Binacle.Lib.Abstractions;
using Binacle.Lib.Data.ResultSelection;

namespace Binacle.Lib.UnitTests;

public sealed class ResultSelectionTestingFixture : IDisposable
{

	public ResultSelectionTestingFixture()
	{

	}

	public void Dispose()
	{
	}

	// Act. Selects, then projects to the string the test compares. No assertion here - the test makes its
	// own Shouldly call, which Sonar already recognises, so this side needs no [AssertionMethod] hint.
	public string Select(
		Scenario scenario,
		IResultSelectionStrategy selectionStrategy,
		Func<OperationResult, string> resultSelector
		)
	{
		var selected = selectionStrategy.Select(scenario.Results);

		return resultSelector(selected);
	}
}
