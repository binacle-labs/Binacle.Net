---
title: Logging
description: >-
  Binacle.Net logs with Serilog to the console and to daily NDJSON files. The supported settings, log levels,
  and where the files are written.
nav:
  parent: Diagnostics Module
  order: 1
  icon: 📜
---


Binacle.Net uses Serilog as its logging framework to provide structured, flexible, and efficient logging capabilities.

While Serilog offers many advanced features, Binacle.Net supports a predefined subset to ensure stability.

It is recommended to stick with the provided configuration and avoid using unsupported Serilog features.

## ⚙️ Default Logging Behavior
By default, Binacle.Net logs to both the Console and File outputs, ensuring real-time monitoring and historical log retention.

## 📟 Console Logging
✅ Logs are output to the console, allowing you to monitor real-time activity.

## 📁 File Logging
✅ Logs are stored in the `/app/data/logs/` directory. Logs are created daily with the following naming format:

```bash
{date}.ndjson
```

⏳ Retention Policy:
- Logs older than 7 days are automatically deleted to save storage.
- No manual log cleanup is necessary.

## 🛠️ Configuration
The default Serilog configuration for Binacle.Net is as follows:
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft.AspNetCore": "Warning",
        "Azure.Core": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "theme": "Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme::Code, Serilog.Sinks.Console",
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} <s:{SourceContext}>{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "data/logs/.ndjson",
          "formatter": "Serilog.Formatting.Compact.RenderedCompactJsonFormatter, Serilog.Formatting.Compact",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 7
        }
      }
    ],
    "Enrich": [
      "FromLogContext",
      "WithMachineName",
      "WithProcessId",
      "WithThreadId"
    ],
    "Properties": {
      "Application": "Binacle.Net"
    }
  }
}
```

You can modify the logging configuration through **Production Overrides** by creating a `Serilog.Production.json` file.

- 📁 **Location**: `/app/Config_Files/DiagnosticsModule`
- 📌 **Full Path**: `/app/Config_Files/DiagnosticsModule/Serilog.Production.json`

For more details on overriding configurations,
refer to the [Configuration]({% vlink /configuration/index.md %}#%EF%B8%8F-overriding-configuration) page.

> Modifying the logging configuration is not recommended unless you fully understand its implications,
> as improper configurations can impact application stability or even cause crashes.
{: .block-warning}

## 📡 Sent over OpenTelemetry {#sent-over-opentelemetry}

With [OpenTelemetry]({% vlink /configuration/diagnostics-module/open-telemetry/index.md %}) on, every log event
is also sent there, after the levels above have filtered it. The OTLP `Logging` switch turns this off for OTLP.

- **The body is the finished message**, with the values and the trace id as attributes. Before 3.2.0 it was the
  template, such as `pack single_bin completed in {OperationDurationMs}ms`.
- **Text values appear in quotes**, as Serilog renders them: `Now listening on: "https://localhost:7194"`. The
  console shows them without quotes.

## ⏱️ Per-Operation Line

Each fit or pack logs a line such as `pack single_bin completed in 12.3ms`. Since 3.2.0 it is logged at Debug, so
at the default level it does not show in the console, the log file or OpenTelemetry. The span and the
`binacle.packing.operation.duration` metric carry the same time. To see it again, add this override:

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Override": {
        "Binacle.Net.Telemetry": "Debug"
      }
    }
  }
}
```

`OperationDurationMs` stays in milliseconds.

---

## Advanced Customization

For advanced customization, refer to the official Serilog documentation:
- 📖 [Configuration Basics](https://github.com/serilog/serilog/wiki/Configuration-Basics)
- 📖 [Serilog Settings Configuration](https://github.com/serilog/serilog-settings-configuration)
