namespace Binacle.Net.Telemetry;

internal static class PackingTags
{
	public const string Operation = "binacle.packing.operation";
	public const string Mode = "binacle.packing.mode";
	public const string RequestAlgorithm = "binacle.packing.request.algorithm";
	public const string ResultAlgorithm = "binacle.packing.result.algorithm";
	public const string ResultStatus = "binacle.packing.result.status";
	public const string BinCount = "binacle.packing.bin.count";
	public const string ItemCount = "binacle.packing.item.count";
	public const string ItemQuantity = "binacle.packing.item.quantity";

	public const string AllAlgorithms = "all";

	public static string ToTag(AlgorithmOperation operation) => operation switch
	{
		AlgorithmOperation.Fitting => "fit",
		AlgorithmOperation.Packing => "pack",
		_ => "unknown"
	};

	public static string ToTag(PackingMode mode) => mode switch
	{
		PackingMode.SingleBin => "single_bin",
		PackingMode.MultipleBins => "multiple_bins",
		PackingMode.SmallestBin => "smallest_bin",
		PackingMode.BestBin => "best_bin",
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
