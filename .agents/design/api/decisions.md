---
id: api/decisions
description: API decisions ledger — why a module-off document carries no `429` and what guarantees it, what the generated documents are a document of, why the API sends no HSTS header, why the DiagnosticsModule alone is registered unconditionally, why an unknown enum answers with the same error a missing one does, why the shipped image calls the experimental v4 API, why the instance page renders its presets from a startup snapshot rather than a live provider, why OpenTelemetry's switches, endpoint paths and resource order are what they are, why telemetry has a meter per module and wraps the service, and why Binacle's own telemetry names are experimental and shaped as they are.
verified: 2026-11-19
check: D1 against api/src/Binacle.Net.Kernel/OpenApi/Transformers/RateLimiterResponseOperationTransformer.cs, which must check EnableRateLimitingAttribute and nothing else; against a grep for EnableRateLimitingAttribute and RequireRateLimiting over api/src, which must land only inside Binacle.Net.ServiceModule; and against ApiDocument.Transform for the relative servers entry and the GitHub/Docker Hub description. D2 against a grep for UseHsts over api/src, which must return nothing; D3 against Program.cs, where AddDiagnosticsModule and UseDiagnosticsModule must carry no Feature.IsEnabled guard while the other two modules do; D4 against BindingProblem, which must match JsonEnumValueException before JsonException, and against JsonEnumValueException.GetValidationSummary, whose key must come from the request type rather than the wire path; D5 against a grep for api/v4 over api/src/Binacle.Net.UIModule, which must now return no call - the one remaining hit is a comment in Instance.cshtml.cs naming the endpoint this page can go stale against - and note this grep never saw the demo's v4 call, which reaches the image through the bundle rather than the module's source; D6 against the absence of api/src/Binacle.Net.UIModule/_js/instance.js and of an instance entry in that module's webpack.config.js, against InstanceOptions.EnabledFeatures filtering on FeatureValue rather than on a name list, and against Binacle.Net.Kernel.csproj referencing Binacle.Packing neither directly nor through what it references; the BinOption-to-InstancePresetBin projection must live in api/src/Binacle.Net/ExtensionMethods/BinPresetOptionsExtensions.cs and nowhere else; D7 against OtlpExporterExtensions.UseOtlpExporter, which must keep the SDK's UseOtlpExporter when all three Otlp switches are on, against OtlpExporterConfigurationOptions adding /v1/<signal> for HTTP only, against OpenTelemetryConfigurationExtensions.ConfigureResource, where AddEnvironmentVariableDetector must sit after AddHostAndProcess and before AddOptionalAdditionalAttributes, against the health-check map in ModuleDefinition.cs carrying DisableHttpMetrics, against OpenTelemetryMetricsExtensions and OpenTelemetryTracingExtensions passing TelemetryNames.Wildcard to AddMeter and AddSource, and against AddBinacleServices registering PackingLogBinacleService around InstrumentedBinacleService around BinacleService; the D7 naming entries against the names in api/src/Binacle.Net/Telemetry/PackingMetrics.cs, api/src/Binacle.Net.DiagnosticsModule/Telemetry/PackingLogMetrics.cs and lib/src/Binacle.Lib/Diagnostics.cs
paths:
  - "api/**"
---

# API — decisions ledger

Why the API is shaped the way it is where the shape is not obvious from the code. What it *does* is `$api` and
the docs under it; this file is the reasoning, so a later session does not undo a deliberate choice.

## Decided

### D5 — the shipped image calls the experimental v4 API, and that is accepted

**Decided (the maintainer, 2026-09-09):** "we do a gull rebuild from v3 to v4" (answering: move the demo UI to v4)
The maintainer, 2026-09-29: "D5 yes its used as a testing ground".

**One caller ships in the image. Until 2026-09-10 there were two.**

**The packing demo calls `/api/v4/pack/compare-bins`** through `packages/binacle-net-client`, which both hosts
bundle - so the shipped `/packing` page is a v4 consumer, where until 2026-09-09 it called v3.

**The second one is gone.** `api/src/Binacle.Net.UIModule/_js/instance.js` fetched `/api/v4/presets` from the
browser until D6 replaced it with server-side state on 2026-09-10 and deleted the file. The reasoning below
about its blast radius is kept because it is why the call was tolerated for as long as it was, not because
the call still exists.

The marketing site tells a reader v4 can change in a patch release and not to integrate against it by
accident, so the image doing exactly that is worth writing down rather than leaving for someone to find.

**Note the grep in `check:` does not see the demo's call.** It searches the module's own source, and the demo's
call lives in `packages/binacle-net-ui` and `packages/binacle-net-client`, reaching the image only through
the bundle. Anyone re-verifying this record from the grep alone will count one caller and be wrong.

**It is accepted, not overlooked.** Three reasons.

**Moving our own UI onto v4 is the point, not an accident.** It is the adoption that earns calling v4 stable
later, and the flip is a separate piece of work needing an endpoint v4 does not yet have. A demo on the
version we tell readers is older argues against our own recommendation.

**The blast radius of the presets call is one page.** `/instance` lists the presets the running container was
configured with. If a v4 change broke the shape, that page stops listing presets. No packing call, no result,
no data written.

**The demo's blast radius is bigger and is the accepted cost.** A v4 change to `pack/compare-bins` breaks the
`/packing` page for everyone running the image, and the contract test in `packages/binacle-net-client` is
what turns that from a silent break into a failing build - it validates the hand-written types against the
committed copy of the v4 document.

**Moving it to v3 would have been work thrown away**, which is why it was never done: the rewrite that
deleted the file was already planned, and editing the version string first would have read afterwards as a
deliberate v3 choice rather than as a stopgap. That rewrite landed as D6.

**What would change this.** The presets half is closed. For the demo, v4 going stable removes the question
entirely.

### D6 — the instance page renders its presets from a startup snapshot, not a live provider

**Decided (the maintainer, 2026-09-10):** "on presets  we go route A" (answering: (a) fill the presets once at startup, or (c) a live provider)

**Decided 2026-09-10.** `_js/instance.js` fetched `GET /api/v4/presets` from the browser and built the table
in DOM calls. It now renders server-side and the file is deleted, along with its webpack entry and its script
tag.

**Three reasons, and they are independent.** The server is holding the answer while it renders the page, so
the call was the process asking itself a question over the network. A browser fetch dies behind an auth layer,
a proxy or a CORS rule - which is the situation an operator is in when they open a diagnostics page, so it
failed exactly when it was needed. And it was the last v4 call made from a browser inside the image, which
D5 above had to carry.

**The route the obvious fix cannot take, measured 2026-08-22.** `BinPresetOptions` lives in the entry project,
which references the UI module, so a project reference back is a cycle - `Instance.cshtml.cs` cannot inject
`IOptions<BinPresetOptions>` the way it injects the switch list. **And the type cannot simply move**:
`BinOption` implements `IIdentifiableBin` and `IWithDimensions`, from `Binacle.Packing` and `Binacle.Geometry`.
`Binacle.Net.Kernel` does not reach `Binacle.Packing`; it gets `Binacle.Geometry` only through
`Binacle.CompactNotation`. Pulling `Binacle.Packing` in would have been the largest cost in the job by
far, so `PresetsValue` carries a name and, per bin, an id and three ints. `BinPresetOptionsExtensions` in the
entry project does the projection and is the one place a `Bin` type meets one that is not - it extends the
nullable type, because the configuration section is optional and reads back null.

**What was built.** A `Kernel/Instance/` slice, holding what a running instance reports about itself.
`FeatureOptions` moved into it as `InstanceOptions`, and its `Dictionary<string, string?>` became a closed
value hierarchy: `InstanceValue`, with `FeatureValue` for the switched-on things - `SwitchedOn` and
`PathValue` - and `PresetsValue` beside them.

**`FeatureValue` is not decoration, it is what keeps the wire honest.** `EnabledFeatures` is published in the
health check payload and dumped by the debug middleware. Keyed on names alone, a presets entry would have made
the health check report `Presets` as a switched-on feature and `IsFeatureEnabled("Presets")` return true and
mean nothing. It filters on `is FeatureValue` for that reason, and a Kernel unit test holds it.

**The snapshot is the accepted cost.** `BinPresetOptions.ReloadOnChange` is `true`, so an operator who edits
`Presets.json` sees `/instance` disagree with `/api/v4/presets` until the process restarts - and that operator
is the page's whole audience. **A live provider was the alternative and was rejected on cost**: it needs an
interface in `Kernel` implemented in the entry project reading `IOptionsMonitor` per render, to close a window
that ends at the next restart on a page that already reports version and environment, which also need one.
The comment at the fill point in `Program.cs` says so, so the staleness reads as chosen rather than missed.

**What would change this.** An operator hitting the stale window in practice, or presets becoming editable
from the page. Either makes the provider worth its cost.

### D7 — OpenTelemetry: what is code, what is config, and who wins

What it does is `$api/modules/diagnostics`. This is why. The evidence is `$api/findings`, scratch tests on OTel
1.18 and 1.21 between 2026-11-15 and 2026-11-17.

**The per-signal switches sit under `Otlp`, not on `Metrics`, `Tracing` and `Logging`.** They only control
OTLP. Azure Monitor sets up its own signals and still sends all three with an OTLP switch off (tested), so
`Tracing.Enabled: false` would lie to an Azure user. The OTel spec also ties "off" to the exporter
(`OTEL_TRACES_EXPORTER=none`), which the .NET SDK does not read - hence a key of Binacle's own.
**Decided (the maintainer, 2026-11-16):** "i want those 3 options in otlp".

**All three on keeps `UseOtlpExporter`; any off uses `AddOtlpExporter` per signal.** `UseOtlpExporter` cannot
leave a signal out: with no sources, metrics still send near-empty requests and logs still go out. Only
`UseOtlpExporter` reads the per-signal `OTEL_EXPORTER_OTLP_<SIGNAL>_*` variables, so keeping it when all are on
means no existing setup breaks. With a signal off those variables are not read - a known gap, documented
rather than coded. **Decided (the maintainer, 2026-11-16):** "we do json key lets leave individual endpoint out
of it now.maybe we will add them in json in the future".

**The code adds `/v1/<signal>` for HTTP, and only for HTTP.** `AddOtlpExporter` with an endpoint set in code uses
it as given, so HTTP would 404. gRPC takes no path. `new Uri(base, "v1/traces")` drops a path already on the
endpoint; trimming `/` and appending keeps it. The maintainer, 2026-11-16: "take care a user may do endpoint
http:host:2222/smth/ then v1/traces".

**Resource order, lowest first: Binacle's values, then `OTEL_RESOURCE_ATTRIBUTES` and `OTEL_SERVICE_NAME`, then
`AdditionalAttributes`.** The SDK reads the two variables before the code runs, so anything the code sets beats
them - which the spec allows, but the spec means the app's developer. Binacle ships as an image; the person
running it has only env vars and config, and expects the standard variables to work. So `ConfigureResource`
reads them again with `AddEnvironmentVariableDetector()` after Binacle's values. That also lets the env var
beat the `ServiceInstanceId` key, matching .NET's usual "env beats JSON", and lets `OTEL_SERVICE_NAME` rename
the service - the standard variable, so no `ServiceName` key is needed.
**Decided (the maintainer, 2026-11-17):** "Yes go and we need to document them", after "yes i think it is ok"
to both effects.

**`ServiceInstanceId` defaults to `null`.** The semantic conventions say the id must be unique per running copy.
`"1"` for every copy merged their data. A new id each start makes a new series in a metrics store; queries that
sum by service do not see it, and an operator who wants one id sets the key.
**Decided (the maintainer, 2026-11-16):** "2 opk null".

**`host.name` and `process.pid` go on the resource, in code, with no detector package.** Before, only log records
carried them, through the Serilog enrichers. In a container they say little - pid is usually 1 and the host is
the container id - and `container.id` needs a beta detector package. An operator names the real server through
either override above. **Decided (the maintainer, 2026-11-16):** "4 ok we add".

**Binacle's own trace sources and meters are added in code; `AdditionalSources` and `AdditionalMeters` are for extras.** An app turns on its own
instrumentation in code. Listing it only in the Development file meant production never sent the pack and fit
spans. **Decided (the maintainer, 2026-11-16):** "1 ok we can fix".

**Health-check requests are left out of traces and of the HTTP request metrics.** Each probe was a span and a
counted request. Traces filter in the ASP.NET Core instrumentation; metrics use `.DisableHttpMetrics()` on the
map, the .NET 9 switch made for this. `/_debug` stays traced: whoever calls it wants the trace. The maintainer,
2026-11-17: "debug on traces". **Decided (the maintainer, 2026-11-17):** "ok
gilters", then "3 ok lets fix this" for the metrics.

**Per-signal `AdditionalAttributes` are deprecated, removed in 4.0.0.** Resource attributes say who sent the
data, which is the same for all three signals from one process; a set per signal makes them harder to join in
a backend. **Decided (the maintainer, 2026-11-16):** "yes", to a startup warning and removal in 4.0.0.

**Each module has its own meter and trace source, named after it, and the setup collects `Binacle.Net*`.** One
meter and one source per component is the .NET convention, and a dashboard can then tell the modules apart. The
wildcard means a new module needs no change to the OpenTelemetry setup. Only the shared
parts sit in the Kernel; the Kernel does not reference `Binacle.Packing` (D6), so nothing about packing goes there.
The maintainer, 2026-11-17: "i can see a folder of telemetry in each module add it and chared to kernel".
**Decided (the maintainer, 2026-11-17):** "One per module (Recommended)", picked from the two options offered.

**Telemetry wraps `IBinacleService` as a decorator, and the packing log is a second decorator outside it.**
`BinacleService` then only packs. With the log write inside the timed call, the duration would also count building
the log entry, which copies every bin, item and result. The cost: the `queue packing_log` span is no longer a
child of the operation span, and each decorator repeats every method signature. The maintainer, 2026-11-17: "the
code is a mess i expected a clean separation". **Decided (the maintainer, 2026-11-17):** "go with two decorators".

**Binacle's own names follow OpenTelemetry's naming rules, and are experimental in 3.x.** No Binacle-named
metric or span had reached a production user: the spans ran only in Development before 3.2.0, and the metrics
are new in it. A rename costs nothing now; once shipped it breaks someone's dashboard or alert. Experimental
says a minor release may still change one. OpenTelemetry has no field for it that a backend reads, so the docs
and the changelog are the marking. **Decided (the maintainer, 2026-11-19):** "yes do all proposal scope is
everything service module too... but... what will provide value and according to conventions.... yes we will
mark excperimental". That covers each change below.

- **Every duration is in seconds.** The maintainer's reason: one unit, so a dashboard puts
  `http.server.request.duration` and Binacle's own durations on one panel and in one query.
- **`binacle.packing.operation.items`, not `.size`.** In OpenTelemetry's names `size` is bytes, as in
  `http.server.request.body.size`. The value is a count of items.
- **One `binacle.packing_log.processed` counter with `error.type`, plus `queue.size` and `queue.capacity`.** It
  replaces `dropped_entries` and `queued_entries`, and copies the shape of OpenTelemetry's own log processor
  metrics. One counter split by `error.type` gives the failure rate in one query. That shape is still
  "development" in OpenTelemetry, so it is a pattern to copy, not a standard.
- **A stopped packing log counts as `already_shutdown`.** The maintainer, 2026-11-19: "use the exception's full
  type name for write failures... When the processor stops, complete the channel, count what's left as
  already_shutdown, and count later writes the same way, as OTel's log processor does."
- **A failure sets `error.type`**, the OpenTelemetry attribute for it, on the metric and the span.
- **Attribute values are fixed lowercase lists, and per-request numbers go on spans only.** Each distinct value
  on a metric is one more series to store.
- **Span names are short, lowercase and fixed, such as `pack single_bin`.** The endpoint spans, such as
  `Pack Custom Bin: v4`, are gone: the HTTP server span already carries the route and the version.
- **`deployment.environment.name` on the resource**, the semantic-conventions attribute, so a dashboard can split
  production from development.
- **The `Binacle.Lib` source has no version.** Its assembly is not stamped, so it would always say `1.0.0.0`, and
  `service.version` already names the release. **Decided (the maintainer, 2026-11-19):** "No version".
- **Npgsql's spans and metrics are collected.** The maintainer, 2026-11-17: "not now lets adress later", then
  "scope is everything service module too" on 2026-11-19.

**What would change this.** The .NET SDK reading `OTEL_TRACES_EXPORTER` and kin (opentelemetry-dotnet #1453)
would make the `Otlp` switches redundant with a standard variable.

## Pending

What the API does and why. No quote from the maintainer covers these yet.

### D1 — a module-off document must carry no `429`, and the metadata is what guarantees it

`RateLimiterResponseOperationTransformer` documents `429 Too Many Requests` when the operation carries
`[EnableRateLimiting]`, and that single check is enough because **only `AddServiceModule` ever attaches it**.
The core endpoints call `.RateLimited()`, a marker naming no policy; the module's `IEndpointConvention` turns
that marker into the attribute. Metadata present therefore *means* a limiter is registered.

**The invariant is the assembly, not the convention.** The module's own `/api/auth/token` calls
`.RequireRateLimiting("AuthToken")` directly, which attaches the same attribute without going through the
convention — and that is fine, because the endpoint only exists in a build where the module is on. What must
stay true is that no attach point sits outside `Binacle.Net.ServiceModule`. A `.RequireRateLimiting(...)` added
to a core v3 or v4 endpoint file is the exact regression this decision exists to prevent, and it would pass a
check that only watched the convention.

**It took two guards until 2026-08-14, and the second one was load-bearing.** The v3/v4 endpoint files used to
call `.RequireRateLimiting("ApiUsage")` directly, naming a policy only the module supplies. With the module off
the call was a no-op but the metadata was still there, so the transformer also had to check the `"RateLimiter"`
feature — otherwise the document advertised a response the build cannot emit, which is a false statement in a
contract. **Moving the policy name into the module removed the need for that guard rather than the guard
alone.** Do not read its deletion as a decision that metadata was always sufficient: it was not, and the
difference is which assembly puts the attribute there.

**What the generated documents are a document of.** `just openapi generate` builds them from a host with no
launch profile, so the ServiceModule is off, and the output describes **the image a self-hoster runs at its
defaults** — not the hosted deployment. Two things confirm the reading: `servers` is the single relative `/`,
and `info.description` points at GitHub and Docker Hub rather than at any host. That document already omits
everything else the module contributes — the whole `v0` ServiceModule document, `/api/auth/token` — so a `429`
inside it is the one inconsistency, describing a shape that exists nowhere.

**This was reversed once, in both directions.** The feature guard was removed on 2026-07-19 in "open api
improvements", on the reasoning that `429` is part of the endpoint's contract and a generated client should be
told it can happen. That argument fails on its own terms: a client generated from the module-off document also
has no `/api/auth/token`, which is the thing it would need in order to stop being rate-limited, so the
document serves neither the self-hoster nor a caller of the hosted service. The guard went back in on
2026-08-13, before v3.0.0 shipped. The removal reached the v3.0.0 betas, whose published swagger copies carry
the `429`; regenerating them takes it out and returns the v3 document to the shape v2.1.x published. On
2026-08-14 the guard came out for the last time, because by then nothing was left for it to guard.

**If a contract for the hosted service is ever wanted, it is a second document**, generated with the module on,
carrying the auth paths and the `429` together. Do not approximate it by loosening this one.

### D2 — the API sends no HSTS header, and TLS is the proxy's job

**`UseHsts()` was deleted from `UseUIModule` on 2026-08-22.** Nothing in the API sends
`Strict-Transport-Security` now, and nothing should add it back without reversing this entry.

**Where it came from.** It is the ASP.NET Core Razor Pages and MVC template block, `!IsDevelopment()` guard
and all, and it arrived with the module when the module became Razor Pages. **Microsoft's own API templates
leave HSTS out**, and say why: "The default API projects don't include HSTS because HSTS is generally a
browser only instruction. Other callers, such as phone or desktop apps, do not obey the instruction."
(<https://learn.microsoft.com/aspnet/core/security/enforcing-ssl>.) This repo is an API that grew a Razor
Pages module, so it inherited the web-app template's answer to a question the API template had already
answered the other way.

**It was inert where it stood.** The published image terminates no TLS — it has no certificate, and no sample
or smoke profile sets an HTTPS port; every one of them listens on plain 8080. A browser ignores
`Strict-Transport-Security` on a plain-HTTP response, so the call could only ever fire for someone who gave
Kestrel a certificate **and** switched the demo on, a combination nothing in the repo describes.

**Three reasons it is the wrong thing for this image to send, and they outlive the placement:**

- **The header belongs to whatever owns the hostname.** Where there is TLS it is terminated by a proxy,
  ingress or CDN, and that is the only thing that knows whether `includeSubDomains` and `preload` are safe.
  ASP.NET's default — 30 days, no subdomains — is a guess made by the wrong party.
- **It is the one header that cannot be taken back.** A browser honours it for its max-age, and the only undo
  is serving `max-age=0` over HTTPS from the same host. An image people run on internal hostnames must not
  pin that by default.
- **The callers are servers.** This is the API behind someone's backend; HSTS is a browser mechanism and an
  HTTP client ignores it.

**It was also on the wrong switch.** Inside `UseUIModule` it existed only when the demo UI was on — absent
from exactly the deployments most likely to face the internet — and it ran after eight other middlewares, so
anything answering earlier never got it either. Both faults predate the module rebuild; the call moved with
the file rather than being introduced by it.

**Expect an analyser to ask for it.** A missing HSTS call is a standard ASP.NET finding. The answer is this
entry: the repo answers findings where a reader can see the answer, never with a suppression rule.

**The rejected alternative was an opt-in `Hsts__Enabled`** in `Program.cs` beside `UseHttpsRedirection`. That
is an options class, a config file, a documented key and a feature switch, for a header the proxy in front
sets in one line.

**The same page says the proxy settles it**: "If the proxy server also handles writing HSTS headers ... HSTS
middleware isn't required by the app."

**`UseHttpsRedirection()` stays**, untouched and unexamined by this decision. With no HTTPS port configured it
logs that it cannot determine one and does nothing, so it costs nothing today. **What is worth knowing before
anyone changes that**: Microsoft warns that an app calling both `UseHttpsRedirection` and `UseHsts` "put[s] a
site into an infinite loop if deployed to an Azure Linux App Service, Azure Linux virtual machine (VM), or
behind any other reverse proxy besides IIS. The reverse proxy terminates TLS, and Kestrel isn't made aware of
the correct request scheme." (<https://learn.microsoft.com/aspnet/core/host-and-deploy/proxy-load-balancer>.)
`ForwardedHeaders` ships **off** in this image, so anyone who configures an HTTPS port behind a proxy walks
into exactly that. Deleting the HSTS half removes one leg of it.

### D3 — DiagnosticsModule is always on, and the asymmetry is deliberate

**Since 2026-08-17.** `builder.AddDiagnosticsModule()` and `app.UseDiagnosticsModule()` are unconditional in
`Program.cs`, while ServiceModule and UIModule sit behind `Feature.IsEnabled`.

**That is by design and is not evidence of anything.** An earlier draft of the architecture-check work read it
as a sign a boundary was being crossed, and that argument is withdrawn. **So "every module is registered
behind a feature check" is not a rule anyone should write** — it would be red on the choice this entry
records.

**`IOptionalDependency<T>` is still doing real work**, which is a separate thing. The module is always
registered; the packing log processor inside it is not — `ModuleDefinition.cs` registers that only when its
configuration turns it on. So the channel can be absent while the module is present, which is exactly what
that abstraction covers.

### D4 — an unknown enum answers with the error a missing one gives, keyed the same way

**Since 2026-08-27.** A value the enum does not have is refused during binding, and the 422 it produces
carries the same key and the same message the validator produces when the field is absent.

**Refusing it is the first half.** Reading an unknown value as null would make it indistinguishable from
absent, and a request that only requires one field of several would then be accepted with the field the client
sent silently dropped. A PATCH is where that bites: send a good status and a bad role and the patch applies
with the role gone. So the converter throws `JsonEnumValueException` rather than returning null.

**Answering it the same way is the second half, and it is the part that is easy to get wrong.** The two checks
see different names for the same field. FluentValidation reports the CLR property path, `Parameters.Algorithm`.
`JsonException.Path` reports what the client wrote on the wire, `parameters.algorithm`. Left alone that is two
keys and two messages for one field, only one of them documented — a client cannot key off either with
confidence. `JsonEnumValueException.GetValidationSummary` takes the request type and maps the wire path back,
so both checks answer `Parameters.Algorithm`.

**The message is deliberately identical, not merely similar.** `'Algorithm' is required and must be one of the
following values: ...` for both. "Required" is loose for a value that was sent, and a distinct message was the
rejected alternative: it adds a second response shape to document for a client whose next move is the same
either way — send one of these values. One shape, one documented example, and the generated document did not
move.

**Two things hold this.** `BindingProblem` is the single place a failed bind becomes a response, so the enum
branch cannot be fixed on one route and missed on the next — it was, for exactly as long as there were two
copies. And `JsonEnumValueException` derives from `JsonException`, so `BindingProblem` has to match the derived
type first; matched the other way round every unknown enum falls into the 400 branch and this decision is
silently undone.

**What guards it.** `RequestEnumConverterTests` in the two unit suites finds every nullable enum on every
contract by reflection and fails if one does not carry the converter — a new field is covered the day it is
added, without anyone remembering to add a test. The integration suites assert the 422 body, not just the
status, because the status was right while the key was wrong.
