# Binacle.Net.DiagnosticsModule integration tests

Boots the API with OpenTelemetry, health checks and packing logs on, and reads what the OpenTelemetry pipeline
would export: the resource, the spans, the metrics and the logs. The unit tests read spans and metrics with .NET
listeners, which skip the OpenTelemetry setup. This is where that setup is proved.

| Folder | What it is |
|---|---|
| `Telemetry/` | the pipeline tests |

```bash
just test cs_binacle-net-diagnostics-module_integration
```

**What will bite you.** The module reads the OpenTelemetry and packing-log switches while it registers, right
after it adds its own JSON file, so in-memory config loses there. The fixture sets those few as environment
variables, which are added after each JSON file. Everything else goes in as in-memory config, as in the other
test projects.

OpenTelemetry is only set up when an exporter is on. OTLP is on with all three signals off, so nothing leaves
the process, and the in-memory exporters are added beside it.

The packing call logs at `Debug`, so the fixture lowers `Binacle.Net` to `Debug` for that line to reach the log
bridge.

The exporters are shared by every test in the run. Each request sends its own `traceparent`, and a test reads
only the spans and logs with that trace id.
