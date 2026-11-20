---
id: api/service
description: IBinacleService — method reference for SingleBinAsync, MultipleBinsAsync, SmallestBinAsync, BestBinAsync; return types, call pattern, and algorithm selection
verified: 2026-11-19
check: The Telemetry section matches api/src/Binacle.Net/Telemetry/ - names, tags, units and buckets - and the wrapping order in AddBinacleServices(); Method signatures, including the trailing CancellationToken, match api/src/Binacle.Net/Services/IBinacleService.cs; the call pattern matches a v4 endpoint handler; GetAlgorithm and ForFittingOperation/ForPackingOperation match api/src/Binacle.Net/v4/Contracts/OperationParameters.cs
paths:
  - "api/src/Binacle.Net/Services/**"
  - "api/src/Binacle.Net/Telemetry/**"

---

# IBinacleService

Defined in `api/src/Binacle.Net/Services/IBinacleService.cs`. The interface is `internal` — it lives in the
`Binacle.Net` assembly, not in the packer or `Binacle.Packing`. Endpoint handlers inject it and call the appropriate
method. They do not touch processors or factories directly.

## Methods

Every method is **generic** and **async**. The full shape is:

```csharp
ValueTask<OperationResult> SingleBinAsync<TBin, TBox, TParams>(
    Algorithm algorithm, TBin bin, List<TBox> items, TParams parameters,
    CancellationToken cancellationToken = default)
    where TBin : class, IWithID, IIdentifiableBin
    where TBox : class, IWithID, IWithQuantity, IIdentifiableItem
    where TParams : class, IOperationParameters, ILogParametersProvider;
```

The same generic signature and constraints apply to every method below (the `MultipleBinsAsync` overloads
return `ValueTask<IDictionary<string, OperationResult>>`). The table simplifies argument names for readability.

`IIdentifiableBin` / `IIdentifiableItem` (in `Binacle.Packing`) are the read-only markers the
packing log reads through — see Models (`$lib/models`). `ILogParametersProvider` lets the params render
themselves into the log. All three exist so the service output can flow into the log channel with no copy.

| Method | Returns | What it does |
|---|---|---|
| `SingleBinAsync(algorithm, bin, items, params)` | `ValueTask<OperationResult>` | Runs one specific algorithm on one bin |
| `SingleBinAsync(bin, items, params)` | `ValueTask<OperationResult>` | Runs all algorithms on one bin, picks `BestAlgorithm` |
| `MultipleBinsAsync(algorithm, bins, items, params)` | `ValueTask<IDictionary<string, OperationResult>>` | Runs one specific algorithm on each bin, returns all results keyed by bin ID |
| `MultipleBinsAsync(bins, items, params)` | `ValueTask<IDictionary<string, OperationResult>>` | Runs all algorithms on each bin, picks best per bin, returns all results keyed by bin ID |
| `SmallestBinAsync(algorithm, bins, items, params)` | `ValueTask<OperationResult>` | Runs one algorithm across all bins, picks `SmallestBin` |
| `SmallestBinAsync(bins, items, params)` | `ValueTask<OperationResult>` | Runs all algorithms across all bins, picks `SmallestBin` |
| `BestBinAsync(algorithm, bins, items, params)` | `ValueTask<OperationResult>` | Runs one algorithm across all bins, picks `BestBin` |
| `BestBinAsync(bins, items, params)` | `ValueTask<OperationResult>` | Runs all algorithms across all bins, picks `BestBin` |

`SmallestBinAsync` and `BestBinAsync` are the same method with a different selector — every bin is run either
way, and only the winner differs (`$lib/result-selection`).

## Choosing the right method

| Endpoint scenario | Method to use |
|---|---|
| Single bin, user picked an algorithm | `SingleBinAsync(algorithm, bin, items, params)` |
| Single bin, auto-select best | `SingleBinAsync(bin, items, params)` |
| All bins, return all results, user picked algorithm | `MultipleBinsAsync(algorithm, bins, items, params)` |
| All bins, return all results, auto-select best | `MultipleBinsAsync(bins, items, params)` |
| Many bins, return smallest that fits | `SmallestBinAsync(algorithm/bins, items, params)` |
| Many bins, return the one the items fill most | `BestBinAsync(algorithm/bins, items, params)` |

## Calling it

The handler reads the algorithm from `request.Parameters.GetAlgorithm()`.
`GetAlgorithm()` returns `null` only for `Best` — that means use the auto-select overload.
(Null from the request body fails the `NotNull()` validator and never reaches the handler.)

```csharp
var algorithm = request.Parameters.GetAlgorithm();

OperationResult result;
if (algorithm.HasValue)
{
    result = await binacleService.SingleBinAsync(
        algorithm.Value, bin, request.Items, request.Parameters.ForFittingOperation(),
        cancellationToken
    );
}
else
{
    result = await binacleService.SingleBinAsync(
        bin, request.Items, request.Parameters.ForFittingOperation(), cancellationToken
    );
}
```

Always call `.ForFittingOperation()` or `.ForPackingOperation()` on `Parameters` before passing them in.
This sets the `AlgorithmOperation` that the algorithm uses — it is not in the JSON body.

> **Warning:** these methods mutate the `OperationParameters` instance. Do not call both on the same object —
> the second call will overwrite the first. Each endpoint should call exactly one.

See Processors (`$lib/processors`) for how the service uses factories internally.
See Result Selection (`$lib/result-selection`) for how `BestAlgorithm`, `SmallestBin`, and `BestBin` strategies are scored.

## Telemetry

`IBinacleService` resolves to three classes, each wrapping the next:

| Class | File | Adds |
|---|---|---|
| `PackingLogBinacleService` | `Services/` | the packing log write, after the call returns |
| `InstrumentedBinacleService` | `Telemetry/` | the span, the timed log line and the metrics |
| `BinacleService` | `Services/` | nothing - packing only |

The order is set in `AddBinacleServices()` with `Decorate` (`$api/kernel`); the last call is the outermost. The log write is outside the instrumented call, so the duration is
the packing alone, and the `queue packing_log` span is a sibling of the operation span, not its child.

`InstrumentedBinacleService` opens a `PackingScope` (`Telemetry/PackingScope.cs`) per call. The scope starts the
span through `BeginTimedActivityOperation`, records the items, times `RunAsync(...)` around the inner call, then
`Record(...)` takes the result or results. The span is named `{operation} {mode}` from the two tag values, e.g.
`pack single_bin`. The same text is the timed log line's message.

One request makes this span tree, all on the `Binacle.Net` source under the HTTP server span:
- `{operation} {mode}` - the packing call.
- `queue packing_log` - the packing log write (`ExtensionMethods/LogChannelExtensions.cs`), with packing logs on.
- `create response` - building the response, in each v3 and v4 endpoint handler.

The endpoints start no span of their own. The HTTP server span already carries the route.

The API's source and meter are both `Binacle.Net` (`Telemetry/Diagnostics.cs`, `internal`). The meter is created through `IMeterFactory` in `PackingMetrics` (`Telemetry/PackingMetrics.cs`):

| Instrument | Type | Unit | Tags |
|---|---|---|---|
| `binacle.packing.operation.duration` | histogram, buckets 0.001-10 | `s` | operation, mode, request.algorithm; `error.type` on a throw |
| `binacle.packing.operation.items` | histogram, buckets 1-10000 | `{item}` | operation, mode |
| `binacle.packing.results` | counter, one per result returned | `{result}` | operation, mode, request.algorithm, result.algorithm, result.status |

The duration buckets are `HistogramBuckets.Seconds` from the Kernel (`$api/kernel`, Telemetry).

Tag keys and values are in `PackingTags` (`Telemetry/PackingTags.cs`), each key under `binacle.packing.`:
- `operation`: `fit`, `pack`.
- `mode`: `single_bin`, `multiple_bins`, `smallest_bin`, `best_bin`.
- `request.algorithm`: `ffd`, `wfd`, `bfd`, or `best` when none is given. Every method without an algorithm picks one
  result. `MultipleBinsAsync` picks the best algorithm for each bin, so it returns one result per bin, not one per
  algorithm.
- `result.algorithm`: `ffd`, `wfd`, `bfd`. Under `best` it is the one the selector picked.
- `result.status`: `fully_packed`, `partially_packed`, `not_packed`, `early_exit`.

The items value is the sum of `Quantity`, not the number of item lines.

The span also gets `bin.count`, `item.count` and `item.quantity`, and for a single result `result.algorithm` and
`result.status`. These are set only when the span records data. On a throw inside `RunAsync`, the span status is
`Error` with the exception message, the exception is added as an event, and the span gets the same `error.type`
as the duration. Both take it from `TelemetryNames.ErrorTypeOf` (`$api/kernel`, Telemetry).

Tested in `api/test/Binacle.Net.UnitTests/Tests/PackingTelemetryTests.cs`, including the span name for each
method, that the SDK uses the bucket advice and that the log write is outside the operation span.
