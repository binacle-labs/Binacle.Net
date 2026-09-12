---
title: Core
description: >-
  The Core module: the API, the presets, and the switches for Swagger UI. The one file it reads and its page.
nav:
  parent: Configuration
  order: 1
  icon: 🏗️
---

Core is Binacle.Net itself: the [API]({% vlink /api/index.md %}), the presets, and a few switches. It reads
one file under `/app/Config_Files`.

## 📂 Files

| File | Page | What it holds |
|---|---|---|
| `Presets.json` | [📖 Presets]({% vlink /configuration/core/presets.md %}) | Your bin set, so a request need not carry the bins |

## 🎛️ Switches

Each one is an environment variable, and each is **off** unless you set it:

| Variable | Turns on |
|---|---|
| `SWAGGER_UI=True` | Swagger UI at `/swagger/` - explore and call the endpoints from a browser |
| `ASPNETCORE_HTTP_PORTS=<port>` | The port inside the container. `8080` when unset |

To run on port `80` inside the container and still reach it on `8080`:

```bash
docker run --name binacle-net \
  -e ASPNETCORE_HTTP_PORTS=80 \
  -e SWAGGER_UI=True \
  -p 8080:80 \
  binacle/binacle-net:{{ page.version_tag }}
```
