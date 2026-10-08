namespace Binacle.ViPaq.EncodedSize.PreReportChecks;

// A fail-fast gate run before the reports: throws on failure, writes no report file (unlike `IReporter`). Registered
// via `AddPreReportChecks`, run by `RunPreReportChecks` before `Measure` runs the reporters.
internal interface IPreReportCheck
{
	void Run();
}
