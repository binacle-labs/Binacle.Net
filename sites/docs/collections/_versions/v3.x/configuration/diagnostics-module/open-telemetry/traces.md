---
title: OpenTelemetry Traces
menu_title: Traces
description: >-
  The spans Binacle.Net sends for each fit and pack request, their attributes, and the packing library's own
  spans and how to turn them on.
nav:
  parent: Open Telemetry
  order: 3
  icon: 🎯
---

Beside the ASP.NET Core and outgoing HTTP spans, Binacle.Net sends its own. Their names are experimental in
3.x - see [Stability]({% vlink /configuration/diagnostics-module/open-telemetry/index.md %}#stability).

## 🎯 Spans

One fit or pack request makes these spans on the `Binacle.Net` source, under the HTTP request span:

- **`{operation} {mode}`**, such as `pack single_bin` - the packing itself.
- **`queue packing_log`** - handing the entry to the packing log, only with packing logs on.
- **`create response`** - building the response.

The HTTP request span already carries the route, so the endpoints add no span of their own.

The `{operation} {mode}` span carries `binacle.packing.operation`, `binacle.packing.mode`,
`binacle.packing.request.algorithm`, `binacle.packing.bin.count`, `binacle.packing.item.count` and
`binacle.packing.item.quantity`. For a single result it also carries `binacle.packing.result.algorithm` and
`binacle.packing.result.status`. The values are the ones on the
[Metrics]({% vlink /configuration/diagnostics-module/open-telemetry/metrics.md %}) page.

A failed operation marks its span as an error, records the exception, and sets `error.type`.

## 🧠 Library Spans {#library-spans}

The packing library, `Binacle.Lib`, has its own spans, under the `{operation} {mode}` span:

| Span | Attributes |
|---|---|
| `process algorithms` | `binacle.lib.processor`, `binacle.packing.operation` |
| `process bins` | `binacle.lib.processor`, `binacle.packing.operation` |
| `process multi_algorithm_bins` | `binacle.lib.processor`, `binacle.packing.operation` |
| `build result` | `binacle.packing.result.algorithm`, `binacle.packing.result.status` |

`binacle.lib.processor` is `loop` or `parallel`.

They are off by default, because there is one span per processor call and one `build result` per result: a
smallest-bin request over 50 bins with 3 algorithms sends about 150 spans. Turn them on with:

```json
{
  "OpenTelemetry": {
    "Tracing": {
      "AdditionalSources": [
        "Binacle.Lib"
      ]
    }
  }
}
```

## 🩺 Health Checks

With health checks on, requests to the health-check path make no span. Added in 3.2.0.
