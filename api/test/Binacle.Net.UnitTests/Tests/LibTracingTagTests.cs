using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Channels;
using Binacle.Net.DiagnosticsModule.Logs.Models;
using Binacle.Net.ExtensionMethods;
using Binacle.Net.Services;
using Binacle.Net.Telemetry;
using Binacle.Net.v4.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Binacle.Net.UnitTests;

[Trait("Behavioral Tests", "Ensures Binacle.Lib spans use the same tag values as the API")]
public class LibTracingTagTests : IDisposable
{
	private readonly ServiceProvider services;
	private readonly ActivitySource testSource = new("Binacle.Net.UnitTests.LibTracing");
	private readonly ActivityListener listener;
	private readonly ConcurrentQueue<Activity> activities = new();
	private ActivityTraceId traceId;

	public LibTracingTagTests()
	{
		this.services = new ServiceCollection()
			.AddLogging()
			.AddMetrics()
			.AddTransient(typeof(IOptionalDependency<>), typeof(OptionalDependency<>))
			.AddSingleton(Channel.CreateUnbounded<AlgorithmOperationLogChannelRequest>())
			.AddBinacleServices()
			.BuildServiceProvider();

		// Other tests in the run also call the lib, so only spans under this test's root are kept.
		this.listener = new ActivityListener
		{
			ShouldListenTo = source => source.Name == "Binacle.Lib" || source == this.testSource,
			Sample = (ref ActivityCreationOptions<ActivityContext> options) =>
				options.Source == this.testSource || options.Parent.TraceId == this.traceId
					? ActivitySamplingResult.AllDataAndRecorded
					: ActivitySamplingResult.None,
			ActivityStopped = activity => this.activities.Enqueue(activity)
		};
		ActivitySource.AddActivityListener(this.listener);
	}

	public void Dispose()
	{
		this.listener.Dispose();
		this.testSource.Dispose();
		this.services.Dispose();
	}

	private static Bin SmallBin() => new() { ID = "small", Length = 4, Width = 4, Height = 4 };

	private static List<Box> Items() =>
	[
		Box.From("a", 5, 5, 5, 3),
		Box.From("b", 2, 2, 2, 4)
	];

	[Theory]
	[InlineData(Binacle.Packing.AlgorithmOperation.Fitting)]
	[InlineData(Binacle.Packing.AlgorithmOperation.Packing)]
	public async Task Lib_spans_send_the_api_values(Binacle.Packing.AlgorithmOperation operation)
	{
		var service = this.services.GetRequiredService<IBinacleService>();
		var parameters = operation == Binacle.Packing.AlgorithmOperation.Fitting
			? new OperationParameters { Algorithm = null }.ForFittingOperation()
			: new OperationParameters { Algorithm = null }.ForPackingOperation();

		Binacle.Packing.OperationResult result;
		using (var root = this.testSource.StartActivity("test")!)
		{
			this.traceId = root.TraceId;
			result = await service.SingleBinAsync(SmallBin(), Items(), parameters, TestContext.Current.CancellationToken);
		}

		var lib = this.activities.Where(x => x.Source.Name == "Binacle.Lib").ToList();

		var processor = lib.Where(x => x.DisplayName == "process algorithms").ShouldHaveSingleItem();
		processor.GetTagItem(PackingTags.Operation).ShouldBe(PackingTags.ToTag(operation));

		var built = lib.Where(x => x.DisplayName == "build result").ToList();
		built.Select(x => (string?)x.GetTagItem(PackingTags.ResultAlgorithm)).ShouldBe(
			Enum.GetValues<Binacle.Packing.Algorithm>().Select(PackingTags.ToTag),
			ignoreOrder: true
		);

		var selected = built.Single(x =>
			(string?)x.GetTagItem(PackingTags.ResultAlgorithm) == PackingTags.ToTag(result.AlgorithmInfo.Algorithm));
		selected.GetTagItem(PackingTags.ResultStatus).ShouldBe(PackingTags.ToTag(result.Status));
	}
}
