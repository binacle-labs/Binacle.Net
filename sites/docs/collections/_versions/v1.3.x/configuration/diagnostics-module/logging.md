---
title: Logging
meta_description: >-
  Serilog logging in Binacle.Net v1.3.x: console and daily file output, the supported settings, and where the
  log files are written.
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
log-{date}.txt
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
          "path": "data/logs/log-.txt",
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
refer to the [Configuration Basics]({% link _common_pages/configuration-basics.md %}#%EF%B8%8F-overriding-configuration) page.

> Modifying the logging configuration is not recommended unless you fully understand its implications,
> as improper configurations can impact application stability or even cause crashes.
{: .block-warning}

---

## Advanced Customization

For advanced customization, refer to the official Serilog documentation:
- 📖 [Configuration Basics](https://github.com/serilog/serilog/wiki/Configuration-Basics)
- 📖 [Serilog Settings Configuration](https://github.com/serilog/serilog-settings-configuration)
