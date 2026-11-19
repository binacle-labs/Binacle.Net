---
id: api/modules/diagnostics
description: DiagnosticsModule — always-on logging, OpenTelemetry, health checks, and packing logs
verified: 2026-11-19
check: The Add/Use lists match ModuleDefinition.cs top to bottom, including what each branch is gated on; the InstanceOptions entries this module adds match its AddFeature calls; every config key matches its Configuration/Models/ class and its shipped JSON; the packing-log registration signature matches ExtensionMethods/LogProcessorServiceCollectionExtensions.cs; the scopes table matches the three Diagnostics.cs files (Binacle.Net, this module, Binacle.Lib) and each Create call's version
also_update:
  - api/configuration
paths:
  - "api/src/Binacle.Net.DiagnosticsModule/**"

---

# DiagnosticsModule

`api/src/Binacle.Net.DiagnosticsModule`

Always registered — no feature flag. Must be added before optional modules in `Program.cs`.

## Registration

```csharp
builder.BootstrapLogger();        // call first, before everything
builder.AddDiagnosticsModule();   // during builder phase
// ...
app.UseDiagnosticsModule();       // during app phase
```

## BootstrapLogger

Creates a Serilog bootstrap logger before the DI container exists.
Minimum level: `Information` (ASP.NET Core and Azure.Core suppressed to `Warning`).
Enriches with: machine name, process ID, thread ID, Binacle version.

Must be called before `AddDiagnosticsModule()` so module initialisation messages are captured.

## What AddDiagnosticsModule registers

**Logging** — replaces the bootstrap logger with a full Serilog instance configured from
`Config_Files/DiagnosticsModule/Serilog.json` (and `Serilog.{Environment}.json` if present).

**OpenTelemetry** — conditional on `OpenTelemetryConfigurationOptions.IsEnabled()` (config-driven).
When enabled: metrics (runtime + ASP.NET Core), tracing (ASP.NET Core + HTTP client), logging.
Exporters: OTLP and/or Azure Monitor — each enabled independently via config.

**Packing logs** — conditional on `PackingLogsConfigurationOptions.Enabled`.
When enabled: registers a channel-based log processor for algorithm operation logs.
`PackingLogBinacleService` writes to this channel via `IOptionalDependency<Channel<...>>` — it's a no-op when disabled.

**Health checks** — always registered. Includes the `SystemHealthCheck` class, registered under the
name `"System"` with tag `"Core"` (failure status `Unhealthy`). Use the name `"System"` — not the class
name — in config such as `RestrictedChecks`.
Path and enabled state are configured via `HealthCheckConfigurationOptions`.

## What UseDiagnosticsModule wires

In order:

- `RequestDebugMiddleware` — **first, and only when `DEBUG_ENDPOINT` is on.** Serves `/_debug`, which echoes the
  caller's own request. Unauthenticated; the startup log prints the path when it is live.
- `ForwardedHeadersDiagnosticsMiddleware` — always registered; warns once when a forwarding header arrived and
  did not take effect (see below)

The rest is inside `if (HealthChecks.Enabled)` — **with health checks disabled, which is the default, none of
this is wired at all** and the path 404s:

- `HealthChecksProtectionMiddleware` — restricts health endpoint access by caller address (`RestrictedIPs`)
- Maps `/_health` (configurable path) with full JSON response via `UIResponseWriter`
- Health status codes: `Healthy/Degraded → 200`, `Unhealthy → 503`
- The `RestrictedChecks` allow-list is applied as the map's `Predicate`
- `.DisableHttpMetrics()` on the map, so probes are not counted in the ASP.NET Core request metrics

This module registers two `InstanceOptions` entries, each with the path it answers on. `DebugEndpoint` is added
in the builder phase behind `DEBUG_ENDPOINT`, the same way `SWAGGER_UI` and `SCALAR_UI` do in `Program.cs`.
`HealthChecks` cannot be, because it is switched on in a config file rather than by a flag — it is bound through
`AddOptions<InstanceOptions>().Configure<IOptions<HealthCheckConfigurationOptions>>(...)` so it resolves after
build and carries the configured path, whatever it ended up being.

## Forwarded headers diagnostic

`ForwardedHeadersDiagnosticsMiddleware` exists because the two ways forwarded headers fail are both silent, and
both leave every component downstream — the health check allow-list, the login throttle, the logs — reading the
proxy as the caller:

| State | What it warns |
|---|---|
| Feature off, request carries the header | Something in front is rewriting the caller and the app ignores it |
| Feature on, header not applied | The trust list does not name the proxy (the framework says only `Unknown proxy`, at Debug) |

It reads `IOptions<ForwardedHeadersOptions>` — the framework type, not `Binacle.Net`'s config class, which the
module cannot see. `ConfigureForwardedHeaders` writes `ForwardedHeaders.None` when the feature is off, on
purpose, so `None` is the app's own answer to whether the feature is live. The same options give the configured
`ForwardedForHeaderName` (so a vendor header such as `CF-Connecting-IP` is watched instead of `X-Forwarded-For`)
and `OriginalForHeaderName`.

**`X-Original-For` is the signal that the header was applied**, not the presence of `X-Forwarded-For`: the
framework rewrites the forwarded header as it consumes entries and removes it once empty, so what is left says
nothing. Original-for is written only when an address was actually replaced.

Warns **once per process** — a misconfigured proxy sends the header on every request, and a warning per request
buries the log it is drawing attention to. Both states are fixed at startup, so one flag covers both. It runs
after `UseForwardedHeaders()` (`Program.cs`), which is the only place the outcome is visible.

Nothing here trusts a header; they are read only to decide whether to warn.

## Config files

| File | What it configures |
|---|---|
| `Config_Files/DiagnosticsModule/Serilog.json` | Serilog sinks, enrichers, log levels |
| `Config_Files/DiagnosticsModule/Serilog.{Environment}.json` | Environment override |
| `Config_Files/DiagnosticsModule/HealthChecks.json` | Health check path, IP restrictions, enabled state |
| `Config_Files/DiagnosticsModule/HealthChecks.{Environment}.json` | Environment override |
| `Config_Files/DiagnosticsModule/OpenTelemetry.json` | OTLP and Azure Monitor exporters |
| `Config_Files/DiagnosticsModule/OpenTelemetry.{Environment}.json` | Environment override |
| `Config_Files/DiagnosticsModule/PackingLogs.json` | Packing log channels and file paths |
| `Config_Files/DiagnosticsModule/PackingLogs.{Environment}.json` | Environment override |

**All four base files are required.** Three through `Optional => false` on `HealthCheckConfigurationOptions`,
`OpenTelemetryConfigurationOptions` and `PackingLogsConfigurationOptions`; `Serilog.json` through the
`optional: false` argument on its `AddJsonConfiguration` call, since it is read by Serilog and has no options
class. The module is always on, so a deploy missing any one of them fails to start — unlike the ServiceModule
files, which are optional or gated on the flag. Every `.{Environment}.json` sibling is optional.

## Logging

Default sinks: Console + rolling File.

- Console: outputs with ANSI theme and `[HH:mm:ss LVL] Message <s:SourceContext>` template.
- File: `/app/data/logs/{date}.ndjson`, daily rolling, 7-day retention (`retainedFileCountLimit: 7`).
  Format: `Serilog.Formatting.Compact.RenderedCompactJsonFormatter`.

Minimum level: `Information`.
Overrides: `Microsoft.AspNetCore → Warning`, `Azure.Core → Warning`.

Enrichers: `FromLogContext`, `WithMachineName`, `WithProcessId`, `WithThreadId`.
Property: `Application: Binacle.Net`.

Override logging by creating `Serilog.Production.json` at `/app/Config_Files/DiagnosticsModule/`.

**To OpenTelemetry:** `writeToProviders: true` hands every Serilog event to the OTel logger provider, so
Serilog's levels decide first. Each record carries the template's values and the Serilog properties as
attributes, plus the trace and span id. `IncludeFormattedMessage` is set in `OpenTelemetryLoggingExtensions`;
without it the body is the raw template, `pack single_bin completed in {OperationDurationMs}ms`. Seen in the
Aspire dashboard, 2026-11-19.

The body is Serilog's own rendering, which quotes string values: `Now listening on: "https://localhost:7194"`.
Serilog has no setting for it. The console drops the quotes because its template uses `{Message:lj}`.

**The packing timed line** - `{operation} {mode} completed in {OperationDurationMs}ms` - is Debug, so it is off
at the default level. `Override: Binacle.Net.Telemetry → Debug` brings it back. `OperationDurationMs` stays in
milliseconds: it is a log value, not a metric, and changing it breaks anyone grepping the file.

## Health checks

Default: **disabled**. Default path: `/_health`.

```json
{
  "HealthChecks": {
    "Enabled": false,
    "Path": "/_health",
    "RestrictedIPs": [],
    "RestrictedChecks": []
  }
}
```

`RestrictedIPs` accepts a single address or a CIDR range. Empty means no restriction:

```json
"RestrictedIPs": [
  "192.168.1.1",
  "192.168.1.0/24"
]
```

Entries are parsed by `IPEntry.TryParse` (`$api/kernel`, Network) into `System.Net.IPNetwork`; a single address
becomes a `/32` or `/128` so the middleware matches one kind of entry. An invalid entry fails startup validation
— which spellings are invalid, and why, is the Kernel's business and is documented there.

`HealthCheckAllowList` holds the parsed list and answers `RestrictsNobody` and `Allows(caller)`. It is nested in
`HealthChecksProtectionMiddleware`, whose `BuildAllowList` is the only thing that constructs it — so moving to
`IOptionsMonitor` is a second call to that method and nothing else. `BuildAllowList` also logs a warning for an
entry whose host bits were masked off: `192.168.1.1/24` covers 256 addresses and does not look like it.

The caller's address is normalised with `IPEntry.Normalize` before matching, which unmaps IPv4-mapped IPv6
addresses. The server listens on a dual-mode socket, so an IPv4 caller arrives as `::ffff:x.x.x.x` and no IPv4
entry would match without it.

**Behind a proxy this list matches the proxy, not the caller** — it compares `Connection.RemoteIpAddress`, which
is the proxy's address until forwarded headers resolve it. See `$api/configuration` (Forwarded headers).

`RestrictedChecks` is an **allow-list**, not a skip-list. When empty (the default), all checks run.
When non-empty, ONLY the checks whose name is in the list run — everything else is filtered out.
Example: `["Database"]` means "run only the Database check", not "skip Database".
Match on the registered name (e.g. `"System"`, `"Database"`), not the class name.

Built-in checks (by registered name):
- `System` — always present (the `SystemHealthCheck` class, tag `"Core"`).
- `Database` — available only when `ServiceModule` is active.

## OpenTelemetry

Config file: `Config_Files/DiagnosticsModule/OpenTelemetry.json`.

Top-level fields:
- `ServiceNamespace` — service namespace label (default: `"binacle"`).
- `ServiceInstanceId` — unique instance ID (default: `null`, so the SDK generates a new one each start).
- `AdditionalAttributes` — key-value attributes attached to all telemetry.

The resource always carries `host.name`, `process.pid` and `deployment.environment.name`, the host environment
lowercased (`production`, `development`). Who wins, lowest first:

1. Binacle's values: `service.name` `"Binacle.Net"`, `ServiceNamespace`, `ServiceInstanceId`, `host.name`,
   `process.pid`, `deployment.environment.name`.
2. `OTEL_RESOURCE_ATTRIBUTES` and `OTEL_SERVICE_NAME`. The SDK reads them before the code runs, so
   `ConfigureResource` reads them again with `AddEnvironmentVariableDetector()` after step 1. Without that they
   lose to step 1. They also beat the `ServiceInstanceId` key. `OTEL_SERVICE_NAME` beats a `service.name` inside
   `OTEL_RESOURCE_ATTRIBUTES`.
3. The top-level `AdditionalAttributes`.

The detector reads `IConfiguration`, so the tests feed it in-memory values instead of real environment variables.

OTLP exporter (`Otlp`):
- `Enabled` (bool, default false)
- `Endpoint` (string, e.g. `http://localhost:4317`). With `Enabled`, it must be an absolute `http` or `https` URL.
  The module reads these options while registering services, before `ValidateOnStart` runs, so it calls
  `OpenTelemetryConfigurationOptionsValidator.ThrowIfInvalid` first. Without that, `new Uri()` throws a raw error.
- `Protocol` — `"grpc"` (default) or `"http/protobuf"`. The older `"httpProtobuf"` is still accepted. Anything else fails validation at startup.
- `Metrics`, `Tracing`, `Logging` (bool, default true) — per-signal OTLP switches. Azure Monitor ignores them.
  - All three on: `UseOtlpExporter`. It also reads the per-signal `OTEL_EXPORTER_OTLP_<SIGNAL>_*` variables.
  - Any off: `AddOtlpExporter` for each signal still on. The per-signal variables are then not read.
  - All three off: a startup warning, nothing sent over OTLP.

With `Endpoint` set and `http/protobuf`, each signal goes to `/v1/metrics`, `/v1/traces` or `/v1/logs` after any
path already on the endpoint (`http://host:4318/smth/` gives `/smth/v1/traces`). gRPC uses the endpoint as given.
With no `Endpoint` in config, the SDK reads `OTEL_EXPORTER_OTLP_ENDPOINT` and `OTEL_EXPORTER_OTLP_PROTOCOL` itself,
and `Protocol` from config is not used.

The recommended way to configure OTLP is via the standard OpenTelemetry .NET SDK env vars
(e.g. `OTEL_EXPORTER_OTLP_ENDPOINT`). See the [OTLP exporter docs](https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/src/OpenTelemetry.Exporter.OpenTelemetryProtocol/README.md#environment-variables).

Azure Monitor exporter (`AzureMonitor`):
- `Enabled` (bool, default false)
- `ConnectionString` (string)
- `EnableLiveMetrics` (bool, default false)
- `SamplingRatio` (float, default 1 = 100%)

Azure Monitor maps only the `service.*` and `k8s.*` resource attributes onto its records (exporter 1.8.3).
`deployment.environment.name`, `host.name` and `process.pid` reach it only through its `_OTELRESOURCE_` metric,
which `OTEL_DOTNET_AZURE_MONITOR_ENABLE_RESOURCE_METRICS` turns on.

Every meter and trace source named `Binacle.Net*` is always on, added in code through `TelemetryNames.Wildcard`
(`$api/kernel`, Telemetry). That covers the API's `Binacle.Net` source and meter (`$api/service`, Telemetry) and
this module's `Binacle.Net.DiagnosticsModule` meter. Tested in
`api/test/Binacle.Net.DiagnosticsModule.UnitTests/Tests/ModuleTelemetryWildcardTests.cs`.
Requests to the health-check path are left out of traces when health checks are on (`HealthChecks:Path`, matched
by segment). The map also disables the request metrics for that path (see the health-check list above). `/_debug` is traced.
`Metrics.AdditionalMeters` and `Tracing.AdditionalSources` list extra meters and sources to include.

**`Binacle.Lib` spans are off by default.** The lib's source is not `Binacle.Net*`, so the wildcard misses it. That
is on purpose: there is one span per processor call and one `build result` per result, so a smallest-bin run over
50 bins with 3 algorithms sends about 150 spans for one request. Turn them on with
`Tracing.AdditionalSources: ["Binacle.Lib"]`. `OpenTelemetry.Development.json` does, so they show in dev.
The span names and tags are in `$lib/processors#diagnostics`.

`Metrics`, `Tracing` and `Logging` each still bind `AdditionalAttributes`, marked `[Obsolete]` (deprecated in
3.2.0, removed in 4.0.0). They are no longer in the shipped JSON. The top-level `AdditionalAttributes` covers
every signal. When OpenTelemetry is on, a startup warning names each one that has values.

### Binacle's own names

**Every name Binacle makes up is experimental in 3.x:** each `binacle.*` metric and attribute, each span name on
the `Binacle.Net` and `Binacle.Lib` sources, and the scope names. A minor release may change one, and the
changelog says so when it does. Nothing in code marks them. OpenTelemetry has no field for it that a backend
reads, so the docs and the changelog are the marking. The built-in names - `http.*`, `aspnetcore.*`, `dotnet.*`,
`kestrel.*`, `db.*` and Npgsql's own - follow OpenTelemetry's and .NET's stability, not Binacle's.

**Scopes:**

| Scope | Meter | Trace source | Version | Collected |
|---|---|---|---|---|
| `Binacle.Net` | yes | yes | `Metadata.Version` | always, through the wildcard |
| `Binacle.Net.DiagnosticsModule` | yes | no | `Metadata.Version` | always, through the wildcard |
| `Binacle.Lib` | no | yes | none | only when `Tracing.AdditionalSources` lists it |

`Metadata.Version` is also the resource's `service.version`. `IMeterFactory` caches by name, version and tags, so
every `Create` of one meter must pass the same version. The `Binacle.Lib` source has no version: its assembly is
not stamped, so it would always say `1.0.0.0`. The ServiceModule has no meter or source of its own
(`$api/modules/service`, Telemetry).

**Where each name is listed:**

| Names | Doc |
|---|---|
| `binacle.packing.*` metrics and attributes with their values; the `{operation} {mode}`, `create response` and `queue packing_log` spans | `$api/service`, Telemetry |
| `binacle.packing_log.*` metrics and their `error.type` values | Packing logs, below |
| `Binacle.Lib` spans and `binacle.lib.processor` | `$lib/processors#diagnostics` |
| resource attributes | the resource list above |
| `error.type`, the scope prefix and wildcard, the seconds buckets | `$api/kernel`, Telemetry |

**A new name follows the same rules** (why: `$api/decisions#D7`):

- A duration is in seconds, with `HistogramBuckets.Seconds` as bucket advice.
- Lowercase and dotted, under `binacle.`. `_` joins words inside one segment: `packing_log`, `single_bin`.
- A metric name is plural only when its unit is an annotation such as `{result}`. That is OpenTelemetry's rule.
- A failure sets `error.type` on the metric and the span: the exception's full type name, or a short fixed word
  where there is no exception, such as `queue_full`.
- Attribute values come from fixed lowercase lists. A per-request number, such as a bin or item count, goes on the
  span only, never on a metric, so the number of metric series stays small.
- Built-in names are not renamed or wrapped.

## Packing logs

Default: **disabled**. Format: NDJSON. Config file: `Config_Files/DiagnosticsModule/PackingLogs.json`.

### How it's wired

The **generic** log pipeline lives in the Kernel — `ILogEntryConvertible<TLog>`, `LogsProcessor<TRequest, TLog>`,
`LogsProcessorOptions`, and `AddLogProcessor<TChannelRequest, TLog>` (see `$api/kernel`, Logs). The **concrete** packing feature lives here, in one file,
`DiagnosticsModule/Logs/Models/AlgorithmOperationLogChannelRequest.cs`:

- `AlgorithmOperationLogChannelRequest : ILogEntryConvertible<PackingLogEntry>` — the channel message. Carries
  `Bins` (`IReadOnlyCollection<IIdentifiableBin>`), `Items` (`IReadOnlyCollection<IIdentifiableItem>`),
  `Parameters` (`ILogParametersProvider?`), and `Results` (`IDictionary<string, OperationResult>`). Its static
  `From<TBin, TItem, TParams>(...)` builds it with **no copy** (covariant read-only collections); `ToLogEntry(timestamp)`
  maps it to a `PackingLogEntry` in the background — the request thread only enqueues references. No `UserId`
  (per-user logging is a ServiceModule concern).
- `PackingLogEntry` (record) — the JSON line: `Timestamp`, `Parameters` (`IReadOnlyList<string>?`, omitted when
  null), `Bins`/`Items` (compact strings keyed by id, e.g. `"small-box" -> "10x10x10"`), and `Results`
  (`IReadOnlyDictionary<string, LogResult>`). Compact strings come from `CompactNotationFormatter`.
- `LogResult` (record) — one algorithm's result: `Status`, `PackedBinVolumePercentage`,
  `PackedItemsVolumePercentage`, and `PackedItems`/`UnpackedItems` (compact strings grouped by id).

Registration: `AddOptionsBasedPackingLogProcessor()` — no arguments — in DiagnosticsModule
`ExtensionMethods/LogProcessorServiceCollectionExtensions.cs`. It builds both factories the Kernel's
`AddLogProcessor<AlgorithmOperationLogChannelRequest, PackingLogEntry>` wants, resolving
`IOptions<PackingLogsConfigurationOptions>` inside each so the config is read from DI rather than passed in.
Gated by `PackingLogs.Enabled`.

Both factories resolve `PackingLogMetrics` (`Telemetry/PackingLogMetrics.cs`, a singleton), whose instruments are
on the `Binacle.Net.DiagnosticsModule` meter (`Telemetry/Diagnostics.cs`):
- `binacle.packing_log.processed` (counter, `{entry}`) - one per entry, by `error.type`:
  - none when written - the options' `OnWritten` hook;
  - `queue_full` when dropped - the `itemDropped` callback of `Channel.CreateBounded`, so only with `ChannelLimit`
    set. `DropWrite` reports success to the writer, so without the count a full queue loses entries unseen;
  - the exception's full type name when the write threw - the `OnWriteFailed` hook;
  - `already_shutdown` when the processor has stopped - the `OnDiscarded` hook for what was left in the queue, and
    `RefusalCountingChannel` (`Telemetry/`) for each later write the completed channel refuses. The factory
    returns the channel wrapped in it. The app writes with `TryWrite`, so a refused write is not logged.
- `binacle.packing_log.queue.size` (observable up-down counter, `{entry}`) - `channel.Reader.Count`. The
  unbounded channel is built with `SingleReader = false`, because a single-reader unbounded channel cannot count.
- `binacle.packing_log.queue.capacity` (observable up-down counter, `{entry}`) - `ChannelLimit`, only when the
  queue is bounded.

Tested in `api/test/Binacle.Net.DiagnosticsModule.UnitTests/Tests/PackingLogMetricsTests.cs`.

### Config

Flat — `PackingLogs` has `Enabled`, `Path` (default `data/pack-logs/`), `FileName` (default `{0}.ndjson`),
`DateFormat` (default `yyyyMMdd`), `ChannelLimit` and `RetentionDays`. Both fit and pack flow through the one
channel and land there. `{0}` is replaced by the date.

`RetentionDays`:
- `null` (the default, and what ships) — keep every file. These logs are an archive, so pruning is opt-in and
  deletion is left to an external sweep.
- a number — the Kernel's `LogsRetentionProcessor` deletes local files older than that many days, on its own
  loop so a slow sweep never stalls the write channel (`$api/kernel`).

`ChannelLimit`:
- `0` or absent — unbounded; limited only by available memory.
- `> 0` — bounded queue with drop-newest (`DropWrite`). If the writer falls behind, newest entries are dropped
  to prevent overload. Default is `100`.
