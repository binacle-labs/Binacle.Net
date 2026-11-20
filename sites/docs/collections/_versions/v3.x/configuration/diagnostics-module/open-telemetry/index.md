---
title: Open Telemetry
description: >-
  Push traces, metrics and logs from Binacle.Net to Grafana, Jaeger, Prometheus or Azure Application Insights.
  The OpenTelemetry.json settings, the exporters, and the stability of Binacle's own names.
nav:
  parent: Diagnostics Module
  order: 4
  icon: 📡
---

**OpenTelemetry** offers distributed tracing, metrics, and logging, enabling you to monitor and
analyze your application's performance.

If Binacle.Net is part of your system, you can push telemetry data to popular platforms like
**Grafana**, **Jaeger**, **Prometheus**, and **Azure Application Insights** for real-time observability.

## 🛠️ Configuration
OpenTelemetry is configured via the `OpenTelemetry.json` file.

Below is the default configuration:

```json
{
  "OpenTelemetry": {
    "ServiceNamespace": "binacle",
    "ServiceInstanceId": null,
    "AdditionalAttributes": {

    },
    "Otlp": {
      "Enabled": false,
      "Endpoint": null,
      "Protocol": "grpc",
      "Metrics": true,
      "Tracing": true,
      "Logging": true
    },
    "AzureMonitor": {
      "Enabled": false,
      "ConnectionString": null,
      "EnableLiveMetrics": false,
      "SamplingRatio": 1.0
    },
    "Metrics": {
      "AdditionalMeters": [

      ]
    },
    "Tracing": {
      "AdditionalSources": [

      ]
    }
  }
}
```

You can modify OpenTelemetry configuration using **Production Overrides** by creating an
`OpenTelemetry.Production.json` file, or by using **Environment Variables**.

- 📁 **Location**: `/app/Config_Files/DiagnosticsModule`
- 📌 **Full Path**: `/app/Config_Files/DiagnosticsModule/OpenTelemetry.Production.json`

For more information on overriding configurations, refer to the
[Configuration]({% vlink /configuration/index.md %}#%EF%B8%8F-overriding-configuration) page.

## 🔧 Configuration Options

### 🌍 General Settings
- `ServiceNamespace` (_string_): Defines the namespace for your service.
- `ServiceInstanceId` (_string?_, default: `null`): Unique identifier for the service instance. Left `null`, each
  start gets a new id of its own. Set it to keep one id across restarts.
- `AdditionalAttributes` (_object_): Custom key-value attributes attached to every signal - traces, metrics and
  logs. See [Resource]({% vlink /configuration/diagnostics-module/open-telemetry/resource.md %}) for what wins
  when two places set the same attribute.

> Before 3.2.0, `ServiceInstanceId` defaulted to `"1"`, which made every running copy look like the same one.
> If a dashboard or alert filters on `"1"`, change it, or set `ServiceInstanceId` yourself.
{: .block-note}

### 📊 Metrics
- `AdditionalMeters` (_array_): Extra meters to collect, beside the ones Binacle always collects.

### 🎯 Tracing
- `AdditionalSources` (_array_): Extra trace sources to collect, beside the ones Binacle always collects. List
  `Binacle.Lib` here to get the
  [library spans]({% vlink /configuration/diagnostics-module/open-telemetry/traces.md %}#library-spans).

> **Deprecated in 3.2.0:** the `AdditionalAttributes` under `Metrics`, `Tracing` and `Logging`. They still work,
> and a warning is logged at startup when one has values. Move the values to the top-level
> `AdditionalAttributes`, which applies to every signal. They will be removed in 4.0.0.
{: .block-warning}

### 🌐 OTLP (OpenTelemetry Protocol) Exporter
The OTLP Exporter is the recommended way to send OpenTelemetry data.

It allows you to export traces, metrics, and logs to platforms like
**Grafana**, **Jaeger**, **Tempo**, **Prometheus**, and more.

- `Enabled` (_boolean_): Enables or disables the OTLP exporter.
- `Endpoint` (_string?_): The OTLP collector URL (e.g., `http://localhost:4317`). With OTLP enabled, it must be an
  absolute `http` or `https` URL, or startup stops with a message naming the key.
- `Protocol` (_string_): Supported values:
    - `"grpc"` (__default__)
    - `"http/protobuf"`
- `Metrics`, `Tracing`, `Logging` (_boolean_, default: `true`): Turn one signal off for OTLP. A signal turned off
  is not exported over OTLP at all. With all three off, OTLP sends nothing and a warning is logged at startup.
  Added in 3.2.0.

With `http/protobuf`, each signal goes to `/v1/metrics`, `/v1/traces` or `/v1/logs`, after any path already on the
endpoint: `http://host:4318/otel/` sends traces to `http://host:4318/otel/v1/traces`. With `grpc` the endpoint is
used as given.

The recommended way to configure the OTLP exporter is via the officially supported environment variables.

For a full list of available variables, check the [OpenTelemetry .NET OTLP Exporter Docs](https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/src/OpenTelemetry.Exporter.OpenTelemetryProtocol/README.md#environment-variables).

### 🔄 OTLP Environment Variables

- **`OTEL_EXPORTER_OTLP_ENDPOINT` and `OTEL_EXPORTER_OTLP_PROTOCOL`** are read when `Endpoint` is not set in the
  config. `Protocol` from the config is then not used.
- **The per-signal variables**, such as `OTEL_EXPORTER_OTLP_TRACES_ENDPOINT`, work only while `Metrics`,
  `Tracing` and `Logging` are all on.

> **With any of the three switches off, no `OTEL_EXPORTER_OTLP_<SIGNAL>_*` variable is read.** The signals still
> on share the one endpoint, from `Endpoint` or `OTEL_EXPORTER_OTLP_ENDPOINT`.
{: .block-warning}

### ☁️ Azure Monitor Exporter
The Azure Monitor Exporter sends telemetry to Azure Application Insights for monitoring and live diagnostics.

- `Enabled` (_boolean_): Enables or disables the Azure Monitor exporter.
- `ConnectionString` (_string?_): The Azure Monitor connection string.
- `EnableLiveMetrics` (_boolean_): Enables live metrics streaming in Application Insights.
- `SamplingRatio` (_float_, default: `1`): Defines how much telemetry data to sample
  (1 = 100% of requests, 0.5 = 50%, etc.).

🔹 Azure Monitor Exporter requires an Application Insights resource.
You can retrieve your connection string from the Azure Portal under the "Instrumentation Key"
section of your Application Insights instance.

Azure Monitor always gets all three signals; the OTLP `Metrics`, `Tracing` and `Logging` switches do not apply
to it. It maps only the `service.*` and `k8s.*` resource attributes onto its records. `host.name`, `process.pid`
and `deployment.environment.name` reach it only through its `_OTELRESOURCE_` metric, which the
`OTEL_DOTNET_AZURE_MONITOR_ENABLE_RESOURCE_METRICS` environment variable turns on.

## 📡 What Binacle Sends

With OpenTelemetry on, Binacle sends:

- **Metrics** from the .NET runtime and ASP.NET Core, plus Binacle's own -
  [Metrics]({% vlink /configuration/diagnostics-module/open-telemetry/metrics.md %}).
- **Traces** from ASP.NET Core and outgoing HTTP calls, plus Binacle's own -
  [Traces]({% vlink /configuration/diagnostics-module/open-telemetry/traces.md %}).
- **Logs** - every log event, after Serilog's levels have filtered it -
  [Logging]({% vlink /configuration/diagnostics-module/logging.md %}#sent-over-opentelemetry).

Every signal carries the same
[resource attributes]({% vlink /configuration/diagnostics-module/open-telemetry/resource.md %}).

### 🧩 Scopes

| Scope | Meter | Trace source | Collected |
|---|---|---|---|
| `Binacle.Net` | yes | yes | always |
| `Binacle.Net.DiagnosticsModule` | yes | no | always |
| `Binacle.Lib` | no | yes | only when `Tracing.AdditionalSources` lists it |

The `Binacle.Net` and `Binacle.Net.DiagnosticsModule` scopes carry the Binacle version, so a backend can tell
which release produced a series.

### 🩺 Health Checks Left Out

With [health checks]({% vlink /configuration/diagnostics-module/health-checks.md %}) on, requests to the
health-check path make no span and are not counted in the HTTP request metrics, so probes do not bury real
traffic. The health checks themselves are unchanged. Added in 3.2.0.

## 🧪 Stability {#stability}

**Binacle's own names are experimental in 3.x.** That covers every `binacle.*` metric and attribute, the span
names on the `Binacle.Net` and `Binacle.Lib` sources, and the `Binacle.Net`, `Binacle.Net.DiagnosticsModule` and
`Binacle.Lib` scope names. A minor release may change one, and the
[release notes]({% vlink /release-notes.md %}) will say so.

**The built-in names follow OpenTelemetry and .NET**, not Binacle: `http.*`, `aspnetcore.*`, `dotnet.*`,
`kestrel.*` and `db.*`, such as `http.server.request.duration`.
