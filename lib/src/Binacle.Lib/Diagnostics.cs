using System.Diagnostics;

namespace Binacle.Lib;

public static class Diagnostics
{
	public static readonly ActivitySource ActivitySource = new("Binacle.Lib");
}

// The API sends the same values from its own constants, so the two must agree.
internal static class DiagnosticsTags
{
	public const string Operation = "binacle.packing.operation";
	public const string Processor = "binacle.lib.processor";
	public const string ResultAlgorithm = "binacle.packing.result.algorithm";
	public const string ResultStatus = "binacle.packing.result.status";

	public const string Loop = "loop";
	public const string Parallel = "parallel";

	public static string ToTag(AlgorithmOperation operation) => operation switch
	{
		AlgorithmOperation.Fitting => "fit",
		AlgorithmOperation.Packing => "pack",
		_ => "unknown"
	};

	public static string ToTag(Algorithm algorithm) => algorithm switch
	{
		Algorithm.FFD => "ffd",
		Algorithm.WFD => "wfd",
		Algorithm.BFD => "bfd",
		_ => "unknown"
	};

	public static string ToTag(OperationResultStatus status) => status switch
	{
		OperationResultStatus.FullyPacked => "fully_packed",
		OperationResultStatus.PartiallyPacked => "partially_packed",
		OperationResultStatus.NotPacked => "not_packed",
		OperationResultStatus.EarlyExit => "early_exit",
		_ => "unknown"
	};
}

internal static class DiagnosticsActivityExtensions
{
	public static Activity? StartProcessorActivity(
		this ActivitySource source,
		string name,
		string processor,
		AlgorithmOperation operation
	)
	{
		var activity = source.StartActivity(name);
		if (activity is not null)
		{
			activity.SetTag(DiagnosticsTags.Processor, processor);
			activity.SetTag(DiagnosticsTags.Operation, DiagnosticsTags.ToTag(operation));
		}
		return activity;
	}
}
