namespace Binacle.Reporting;

// Reads the bag and returns the sections of one file. Reporters that share a File land in it one after the other.
public interface IReporter
{
	ResultFile File { get; }
	ReportSection[] Report();
}
