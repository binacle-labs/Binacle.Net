---
id: api/findings
description: API findings — the scratch-test evidence behind the OpenTelemetry decisions (exporter switching, endpoint paths, env vars, resource order).
verified: 2026-11-17
check: Each row was observed in a throwaway app outside the repo, so it cannot be re-run from here. Re-test a row before relying on it after an OpenTelemetry package bump; the 2026-11-17 rows were on 1.21.
also_update:
  - api/decisions
paths:
  - "api/src/Binacle.Net.DiagnosticsModule/**"
---

# API — findings (the measured evidence)

What was observed, behind `$api#D7`. The apps were throwaway console and web apps wired the way Binacle wires
OpenTelemetry, with a fake OTLP receiver where traffic mattered. Nothing here is a guarantee for a later package
version.

## Exporters and endpoints - 2026-11-15 and 2026-11-16

OTel 1.18 and 1.21, same results. Serilog wired as in Binacle.

| Case | Result |
|---|---|
| `UseOtlpExporter`, a signal given no sources | tracing stops; **metrics still send near-empty requests; logs still go out** (Serilog skips the filter) |
| `AddOtlpExporter` per signal, signal not added | nothing sent, for all three |
| `AddOtlpExporter`, `Endpoint` in code, `http/protobuf` | sent to `/`, so 404s. The code must add `/v1/metrics`, `/v1/traces`, `/v1/logs` |
| adding the path with `new Uri(base, "v1/traces")` | drops a path already on the endpoint (`/otlp` lost). `TrimEnd('/') + "/v1/traces"` keeps it |
| gRPC | works with the endpoint as given. Adding `/v1/...` breaks it |
| no `Endpoint`, `OTEL_EXPORTER_OTLP_ENDPOINT` set | works. The SDK adds the path itself |
| `OTEL_EXPORTER_OTLP_HEADERS` | works |
| `OTEL_EXPORTER_OTLP_TRACES_ENDPOINT` and the other per-signal vars | **ignored under `AddOtlpExporter`.** Read under `UseOtlpExporter` |
| Azure Monitor on, OTLP metrics only | OTLP gets metrics only. **Azure still gets all three** |
| `UseOtlpExporter(protocol, endpoint)` plus `OTEL_EXPORTER_OTLP_TRACES_ENDPOINT` | **the env var wins for traces**; metrics and logs use the code endpoint. gRPC and HTTP the same |
| `UseOtlpExporter(protocol, endpoint)` plus `OTEL_EXPORTER_OTLP_ENDPOINT` | **the code endpoint wins** for all three |
| `UseOtlpExporter(protocol, endpoint)`, endpoint with a path | keeps it (`/smth/v1/traces`) - 2026-11-17 |
| `OTEL_TRACES_EXPORTER=none` and the metrics and logs ones | **ignored.** All three still sent |
| `OTEL_TRACES_SAMPLER=traceidratio` (arg 0.0) or `always_off` | **works.** No spans; metrics and logs still sent |
| Serilog with `writeToProviders: true` | every enricher and template property is a log record attribute. **The resource had no `host.name` or `process.pid`** |

**Why .NET needs a switch of its own:** `OTEL_TRACES_EXPORTER` and kin are standard and stable in the spec, but
the .NET SDK does not read them - `-` in the spec compliance matrix, and opentelemetry-dotnet #1453, open since
2020 ("not in the immediate plan", Sep 2024).

## Resource order - 2026-11-17

OTel 1.21. The resource chain as in `ConfigureResource`, read back through `TracerProvider.GetResource()`.

| Case | Without `AddEnvironmentVariableDetector()` | With it, after host and pid |
|---|---|---|
| no env vars | Binacle's values, a random instance id | the same |
| `OTEL_RESOURCE_ATTRIBUTES` sets host, pid, instance id | ignored | **all three win** |
| `OTEL_SERVICE_NAME` | ignored | **renames the service** |
| `service.name` inside `OTEL_RESOURCE_ATTRIBUTES` | - | wins |
| both name variables | - | `OTEL_SERVICE_NAME` wins |
| env var plus the `ServiceInstanceId` key | - | the env var wins |
| env var plus `AdditionalAttributes` | - | `AdditionalAttributes` wins |
| `AddService` with no instance id | a new random id each start | the same |
