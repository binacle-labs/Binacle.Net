namespace Binacle.TestsKernel.Algorithms.Models;

public class AlgorithmResult
{
	public AlgorithmResult(
		OperationResultStatus packingStatus,
		EarlyExitReason packingEarlyExitReason,
		OperationResultStatus fittingStatus,
		EarlyExitReason fittingEarlyExitReason
	)
	{
		this.PackingStatus = packingStatus;
		this.PackingEarlyExitReason = packingEarlyExitReason;
		this.FittingStatus = fittingStatus;
		this.FittingEarlyExitReason = fittingEarlyExitReason;
	}

	public OperationResultStatus PackingStatus { get; }
	public EarlyExitReason PackingEarlyExitReason { get; }
	public OperationResultStatus FittingStatus { get; }
	public EarlyExitReason FittingEarlyExitReason { get; }
}
