---
title: Configuration Basics
description: >-
  How Binacle.Net is configured: the files under /app/Config_Files, environment variable overrides, production
  overrides, and which one wins.
nav:
  order: 3
  icon: 🔧
---

Binacle.Net is designed for flexibility, allowing you to enable only the features you need. Most functionality is
provided through modules, each with its own requirements, configuration options, and dependencies.

This guide covers the configuration system, including file-based settings, environment variables, and overrides, so you
can tailor Binacle.Net to your specific needs.

See the dedicated configuration documentation for your Binacle.Net version for version-specific details.

## 📖 Table of Contents

- [📂 Configuration Files](#-configuration-files)
    - [📑 Example Directory Structure](#-example-directory-structure)
- [⚙️ Overriding Configuration](#%EF%B8%8F-overriding-configuration)
    - [🌍 Environment Variables](#-environment-variables)
    - [📝 Production Overrides](#-production-overrides)
    - [📄 Direct File Edits](#-direct-file-edits)
    - [🔄 Connection String Fallbacks](#-connection-string-fallbacks)
- [⚖️ Configuration Precedence](#%EF%B8%8F-configuration-precidence)

---

## 📂 Configuration Files

All configuration files are located in `/app/Config_Files`.
Below is an example structure; specific modules may have additional files and folders.
See the dedicated configuration documentation for your Binacle.Net version for the exact structure.

### 📑 Example Directory Structure
```text
app
└── Config_Files
    ├── Presets.json
    ├── AModule
    │   ├── FeatureA.json
    │   ├── FeatureB.json
    │   ├── FeatureC.json
    └── BModule
        └── FeatureA.json
    
```

---

## ⚙️ Overriding Configuration

Binacle.Net allows multiple ways to override configuration settings. The recommended approach depends on your use case:

- 🔹 **Environment Variables**: Highest priority, ideal for secrets and dynamic configurations.
- 📝 **Production Overrides** (`<filename>.Production.json`): Override settings without modifying the default
  configuration.
- 📄 **Direct File Edits**: Modify configuration files directly (use bind mounts in Docker or volumes in Kubernetes).
- 🔄 **Connection String Fallbacks**: A dedicated method for defining connection strings.

For demonstration, we will use the following `Settings.json`:

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

Environment variables take precedence over configuration files and are recommended for secrets and dynamic settings.

To override a setting, replace nested properties with double underscores (`__`):

```bash
Settings__Enabled=True
Settings__Logs__Retention=5
```

### 📝 Production Overrides

AKA: `<filename>.Production.json`

Instead of modifying `Settings.json` directly, create a `Settings.Production.json` file with only the necessary changes:

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

Place this file in the same directory as `Settings.json`.

### 📄 Direct File Edits

You can manually modify configuration files or replace them entirely using:

- **Docker**: Bind mounts (`-v /host/path:/container/path`)
- **Kubernetes**: Volumes (e.g., `hostPath` or `ConfigMaps`)

> Not recommended for production.
>
> This approach overrides all default settings, which may lead to unexpected behavior.
> Only use this if you want a completely custom configuration.
{: .block-warning}


### 🔄 Connection String Fallbacks

Connection strings can be configured using dedicated environment variables that act as a fallback mechanism. This is the
recommended way for connection strings when they contain sensitive credentials.

- 🔹 **Format**: Uppercase connection name + `_CONNECTION_STRING`
- 🔹 **Example**: `DATABASE_CONNECTION_STRING=endpoint=https://localhost:1413`

---

## ⚖️ Configuration Precidence

When multiple configuration methods define the same setting, the following priority order applies (higher wins):

| Order | Method                     | Example Setting <br> (`Logs.Retention`) | Example Connection String <br> (`ConnectionStrings.Database`) |
|-------|----------------------------|-----------------------------------------|---------------------------------------------------------------|
| 1     | Environment Variable       | `Settings__Logs__Retention=5`           | `ConnectionStrings__Database=endpoint=https://localhost:1413` |
| 2     | Production Override        | `Settings.Production.json`              | `ConnectionStrings.Production.json`                           |
| 3     | Direct File Edit           | `Settings.json`                         | `ConnectionStrings.json`                                      |
| 4     | Connection String Fallback | N/A                                     | `DATABASE_CONNECTION_STRING=endpoint=https://localhost:1413`  |

---