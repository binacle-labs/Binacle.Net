---
title: Configuration
description: >-
  How Binacle.Net is configured: the files under /app/Config_Files, environment variable overrides, which one
  wins, and what the Core, Diagnostics, Service and UI modules each need.
nav:
  order: 6
  icon: 🔧
---

Binacle.Net turns on only what you ask for. Most of it is modules, each with its own files and switches.

This page is the configuration system: where the files are, the ways to override a setting, and which one
wins when two disagree. The settings themselves are on the module pages linked at the end.

## 📂 Configuration Files

Every configuration file lives under `/app/Config_Files`. This is the whole tree in {{ page.version_label }}:

```text
app
└── Config_Files
    ├── Presets.json
    ├── Cors.json
    ├── ForwardedHeaders.json
    └── DiagnosticsModule
        ├── HealthChecks.json
        ├── OpenTelemetry.json
        ├── PackingLogs.json
        └── Serilog.json
```

`ForwardedHeaders.json` ships with the feature turned off. `Cors.json` is not in the image at all - add it only
if a browser calls the API directly, since until you do, no origin is allowed through.

## ⚙️ Overriding Configuration

There are four ways to change a setting. Which one to use depends on what the setting is:

- 🔹 **Environment variables** - highest priority. Use them for secrets and anything that differs per deployment.
- 📝 **Production overrides** (`<filename>.Production.json`) - a file holding only the settings you change.
- 📄 **Direct file edits** - replace the file itself with a bind mount or a volume.
- 🔄 **Connection string fallbacks** - a dedicated environment variable for each connection string.

The examples below use this `Settings.json`:

```json
{
  "Settings": {
    "Enabled": false,
    "DataFolderPath": "/data",
    "Logs": {
      "FileFormat": "dd-MM-yyyy.txt",
      "Retention": 4
    }
  }
}
```

### 🌍 Environment Variables

An environment variable beats every file. Name it after the setting's path, with `__` between the levels:

```bash
Settings__Enabled=True
Settings__Logs__Retention=5
```

### 📝 Production Overrides

Put a `Settings.Production.json` next to `Settings.json` holding only what changes:

```json
{
  "Settings": {
    "Enabled": true,
    "Logs": {
      "Retention": 5
    }
  }
}
```

The two files are merged, so the rest of `Settings.json` still applies.

### 📄 Direct File Edits

Replace the whole file:

- **Docker**: a bind mount (`-v /host/path:/container/path`)
- **Kubernetes**: a volume (`hostPath` or a `ConfigMap`)

> The file you mount replaces every default in it, so a key you leave out is gone, not defaulted. Use this only
> when you mean to own the whole file - which is the normal way to supply `Presets.json`.
{: .block-warning}

### 🔄 Connection String Fallbacks

A connection string can also come from an environment variable named after the connection, uppercased, with
`_CONNECTION_STRING` on the end. This is the place for a connection string that holds credentials.

```bash
DATABASE_CONNECTION_STRING=endpoint=https://localhost:1413
```

## ⚖️ Configuration Precedence

When more than one method sets the same value, the highest row wins:

| Order | Method                     | Setting (`Logs.Retention`)      | Connection string (`ConnectionStrings.Database`)              |
|-------|----------------------------|---------------------------------|---------------------------------------------------------------|
| 1     | Environment variable       | `Settings__Logs__Retention=5`   | `ConnectionStrings__Database=endpoint=https://localhost:1413` |
| 2     | Production override        | `Settings.Production.json`      | `ConnectionStrings.Production.json`                           |
| 3     | Direct file edit           | `Settings.json`                 | `ConnectionStrings.json`                                      |
| 4     | Connection string fallback | -                               | `DATABASE_CONNECTION_STRING=endpoint=https://localhost:1413`  |

## 🔧 Modules

Each module adds something to Binacle.Net. Its page lists its files and switches.

### 🏗️ Core

The API itself, the presets, and the switches for Swagger UI, Scalar UI and the debug endpoint.

- [🔍 Core]({% vlink /configuration/core/index.md %})
- [📖 Presets]({% vlink /configuration/core/presets.md %})
- [🌐 Forwarded Headers]({% vlink /configuration/core/forwarded-headers.md %})
- [🌍 CORS]({% vlink /configuration/core/cors.md %})

### 📊 Diagnostics Module

Logging, health checks, packing logs and telemetry. Always on; only logging is enabled out of the box.

- [🔍 Diagnostics Module]({% vlink /configuration/diagnostics-module/index.md %})
- [📜 Logging]({% vlink /configuration/diagnostics-module/logging.md %})
- [❤️‍🩹 Health Checks]({% vlink /configuration/diagnostics-module/health-checks.md %})
- [📦 Packing Logs]({% vlink /configuration/diagnostics-module/packing-logs.md %})
- [📡 OpenTelemetry]({% vlink /configuration/diagnostics-module/open-telemetry.md %})

### 🛡️ Service Module

Accounts, JWT authentication and rate limiting, for callers you do not control. Built for the hosted service
and **not publicly documented**. A minor release can break it; a patch will not.

- [🔍 Service Module]({% vlink /configuration/service-module/index.md %})

### 🖥️ UI Module

Two browser pages: the packing demo and the ViPaq decoder. Off by default.

- [🔍 UI Module]({% vlink /configuration/ui-module/index.md %})
