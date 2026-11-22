---
title: OpenTelemetry Resource
menu_title: Resource
description: >-
  The resource attributes every Binacle.Net trace, metric and log record carries, which setting wins when two
  set the same one, and how to name the server yourself.
nav:
  parent: Open Telemetry
  order: 1
  icon: 🖥️
---

Every trace, metric and log record Binacle.Net sends carries the same resource attributes. They say which
service, which copy of it and which machine the data came from.

## 🖥️ Attributes

- `service.name` - `Binacle.Net`
- `service.namespace` - `ServiceNamespace`
- `service.instance.id` - `ServiceInstanceId`, or a new id each start
- `service.version` - the Binacle version
- `host.name` - the machine name
- `process.pid` - the process id
- `deployment.environment.name` - the host environment in lowercase, such as `production` or `development`

`host.name`, `process.pid` and `deployment.environment.name` are new in 3.2.0.

## ⚖️ Who Wins

When more than one place sets the same attribute, the later one in this list wins:

1. Binacle's own values, above.
2. The `OTEL_RESOURCE_ATTRIBUTES` and `OTEL_SERVICE_NAME` environment variables. They also win over
   `ServiceInstanceId`. `OTEL_SERVICE_NAME` wins over a `service.name` inside `OTEL_RESOURCE_ATTRIBUTES`.
   With Azure Monitor, `OTEL_SERVICE_NAME` also changes the role name.
3. The top-level `AdditionalAttributes` in `OpenTelemetry.json`.

## ✏️ Naming the Server

To name the server yourself, set `host.name` through either one:

```bash
OTEL_RESOURCE_ATTRIBUTES=host.name=my-server
```

```json
{
  "OpenTelemetry": {
    "AdditionalAttributes": {
      "host.name": "my-server"
    }
  }
}
```

## ☁️ Azure Monitor

Azure Monitor maps only the `service.*` and `k8s.*` attributes onto its records. `host.name`, `process.pid` and
`deployment.environment.name` reach it only through its `_OTELRESOURCE_` metric, which the
`OTEL_DOTNET_AZURE_MONITOR_ENABLE_RESOURCE_METRICS` environment variable turns on. See
[Open Telemetry]({% vlink /configuration/diagnostics-module/open-telemetry/index.md %}).
