using System.Threading;
using Binacle.Lib.Abstractions;
using Binacle.Lib.Abstractions.Algorithms;

namespace Binacle.Lib.Testing;

// An algorithm factory whose algorithms do not pack, for measuring what a processor costs around the packing
// rather than the packing itself. Everything the processor does is still paid: Create allocates, Execute is a
// virtual call, the identifier name is built, and the result goes into the dictionary.
//
// `bytes` is how much memory each algorithm walks before returning. At 0 it walks none, which is the pure
// machinery cost. Above 0 it stands in for a real algorithm's working set: each instance gets its own buffer,
// as a real algorithm builds its own piece array, so a thread on a far CPU has to fetch data its cache does
// not hold - the cost no-work algorithms cannot show.
//
// Execute hands back a result made elsewhere, because OperationResult cannot be built outside Binacle.Lib. Run
// one real algorithm once and pass its result in.
public sealed class FakeAlgorithmFactory : IAlgorithmFactory
{
	private readonly OperationResult result;
	private readonly int bytes;

	public FakeAlgorithmFactory(OperationResult result, int bytes = 0)
	{
		if (bytes < 0)
			throw new ArgumentOutOfRangeException(nameof(bytes), "A fake algorithm cannot walk fewer than 0 bytes.");

		this.result = result;
		this.bytes = bytes;
	}

	public IPackingAlgorithm Create<TBin, TItem>(Algorithm algorithm, TBin bin, IList<TItem> items)
		where TBin : class, IWithID, IWithReadOnlyDimensions
		where TItem : class, IWithID, IWithReadOnlyDimensions, IWithQuantity
	{
		return new FakeAlgorithm(algorithm, this.result, this.bytes);
	}

	// The Algorithm it was asked for, so the identifier names stay distinct and the result dictionary holds one
	// entry per algorithm, as it would with real ones.
	private sealed class FakeAlgorithm : IPackingAlgorithm
	{
		// One cache line. Walking a line at a time touches every line without reading every byte, which is how
		// an algorithm walking a list actually hits memory.
		private const int Stride = 64;

		// Written to so the walk cannot be optimised away. Read by nothing.
		private static long sink;

		private readonly OperationResult result;
		private readonly byte[]? buffer;

		internal FakeAlgorithm(Algorithm algorithm, OperationResult result, int bytes)
		{
			this.Algorithm = algorithm;
			this.result = result;
			this.buffer = bytes > 0 ? new byte[bytes] : null;
		}

		public Algorithm Algorithm { get; }

		public int Version => 2;

		public OperationResult Execute(IOperationParameters parameters)
		{
			if (this.buffer is not null)
				Walk(this.buffer);

			return this.result;
		}

		private static void Walk(byte[] buffer)
		{
			long total = 0;
			for (var i = 0; i < buffer.Length; i += Stride)
				total += buffer[i];

			Volatile.Write(ref sink, total);
		}
	}
}
