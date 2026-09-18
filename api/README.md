# Binacle.Net (API)

The HTTP API. ASP.NET Core (.NET 10) Minimal APIs that expose the bin-packing engine in [`lib/`](../lib)
over versioned REST endpoints.

## 📦 Projects

**Core**

| Path | What it is |
|---|---|
| `src/Binacle.Net` | Entry point - `Program.cs`, versioned endpoint groups (v3, v4) |
| `src/Binacle.Net.Kernel` | Shared building blocks - endpoint registration, validation, OpenAPI, feature flags, CORS |

**Modules**

| Path | Flag | What it is |
|---|---|---|
| `src/Binacle.Net.DiagnosticsModule` | always on | Logging, OpenTelemetry, health checks |
| `src/Binacle.Net.ServiceModule` | `SERVICE_MODULE` | JWT auth, rate limiting, account management |
| `src/Binacle.Net.ServiceModule.Domain` | - | Domain layer for ServiceModule |
| `src/Binacle.Net.ServiceModule.Infrastructure` | - | Database backends (SQLite / PostgreSQL / Azure Tables) |
| [`src/Binacle.Net.UIModule`](src/Binacle.Net.UIModule) | `UI_MODULE` | Razor Pages demo host - packing demo and ViPaq decoder |

## 🔢 API versions

- **v3** (`/api/v3`) - stable. Do not modify.
- **v4** (`/api/v4`) - active development.

Each version offers **fit** (do all items fit?) and **pack** (pack as many as you can). See the
root [README](../README.md) for what those mean.

## 🚀 Run it

```bash
just serve api [N|S|U|All]
```

`N` core only · `S` with ServiceModule · `U` with UIModule · `All` everything.

Once running, the API serves under `/api/v3` and `/api/v4`. With the UIs enabled, Swagger is at
`/swagger/`, Scalar at `/scalar/`, and the packing demo at `/`.

## 🧪 Tests

| Project | Run with | Covers |
|---|---|---|
| [`test/Binacle.Net.IntegrationTests`](test/Binacle.Net.IntegrationTests) | `just test cs_binacle-net_integration` | HTTP tests for v3 and v4 endpoints |
| `test/Binacle.Net.ServiceModule.IntegrationTests` | `just test cs_binacle-net-service-module_integration` | Auth and rate limiting |
| [`test/Binacle.Net.UIModule.IntegrationTests`](test/Binacle.Net.UIModule.IntegrationTests) | `just test cs_binacle-net-ui-module_integration` | The pages, with the module on and off |
| `test/Binacle.Net.UnitTests` | `just test cs_binacle-net_unit` | Presets, forwarded headers, request converters |
| [`test/Binacle.Net.Kernel.UnitTests`](test/Binacle.Net.Kernel.UnitTests) | `just test cs_binacle-net-kernel_unit` | OpenAPI transformers, CORS, instance options |
| `test/Binacle.Net.DiagnosticsModule.UnitTests` | `just test cs_binacle-net-diagnostics-module_unit` | Health checks, logging and telemetry options, middleware |
| `test/Binacle.Net.ServiceModule.UnitTests` | `just test cs_binacle-net-service-module_unit` | JWT options, password hashing, rate limit policies |
| [`test/Binacle.Net.UIModule.UnitTests`](test/Binacle.Net.UIModule.UnitTests) | `just test cs_binacle-net-ui-module_unit` | Page models and the applets service |

## 🧩 Other

- `requests/` - `.http` files for manual testing, grouped by version (`v3`, `v4`, `Service`).
