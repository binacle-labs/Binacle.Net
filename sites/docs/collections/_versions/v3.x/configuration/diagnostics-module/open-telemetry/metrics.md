---
title: OpenTelemetry Metrics
menu_title: Metrics
description: >-
  The metrics Binacle.Net sends on its own meters - packing time, items, results and the packing log queue -
  with their units, attributes and values.
nav:
  parent: OpenTelemetry
  order: 2
  icon: 📈
---

Beside the .NET runtime and ASP.NET Core metrics, Binacle.Net sends its own on two meters. Their names are
experimental in 3.x - see [Stability]({% vlink /configuration/diagnostics-module/open-telemetry/index.md %}#stability).

## 📦 Packing

On the `Binacle.Net` meter, always on:

| Metric | Type | Unit | Attributes |
|---|---|---|---|
| `binacle.packing.operation.duration` | histogram | `s` | operation, mode, request.algorithm; `error.type` on a failure |
| `binacle.packing.operation.items` | histogram | `{item}` | operation, mode |
| `binacle.packing.results` | counter | `{result}` | operation, mode, request.algorithm, result.algorithm, result.status |

- `binacle.packing.operation.duration` is the time spent packing, in seconds.
- `binacle.packing.operation.items` is the number of items in an operation, counting quantity.
- `binacle.packing.results` counts the results returned. It shows which algorithm wins when none is given, and
  how often items do not fit.

### 🏷️ Attributes

Each attribute key is under `binacle.packing.`, such as `binacle.packing.mode`:

| Attribute | Values |
|---|---|
| `operation` | `fit`, `pack` |
| `mode` | `single_bin`, `multiple_bins`, `smallest_bin`, `best_bin` |
| `request.algorithm` | `ffd`, `wfd`, `bfd`, or `best` when none is given |
| `result.algorithm` | `ffd`, `wfd`, `bfd` - under `best`, the one that was picked |
| `result.status` | `fully_packed`, `partially_packed`, `not_packed`, `early_exit` |

## 📜 Packing Logs

With [packing logs]({% vlink /configuration/diagnostics-module/packing-logs.md %}) on, the
`Binacle.Net.DiagnosticsModule` meter adds:

| Metric | Type | Unit | Shows |
|---|---|---|---|
| `binacle.packing_log.processed` | counter | `{entry}` | every entry, by `error.type` |
| `binacle.packing_log.queue.size` | up-down counter | `{entry}` | how full the queue is |
| `binacle.packing_log.queue.capacity` | up-down counter | `{entry}` | the queue's limit, only when `ChannelLimit` is set |

`error.type` on `binacle.packing_log.processed` says what happened to the entry:

- none - written.
- `queue_full` - dropped because the queue was full.
- the exception's full type name - lost because the write failed.
- `already_shutdown` - not written because the packing log had stopped. After too many failed writes in a row the
  packing log stops and takes no more entries.
