---
title: Core
description: >-
  The Core module: the API, the presets, the switches for Swagger UI, Scalar UI and the debug endpoint, and the
  pages for the three files it reads.
nav:
  parent: Configuration
  order: 1
  icon: 🏗️
---

Core is Binacle.Net itself: the [API]({% vlink /api/index.md %}), the presets, and a few switches. It reads
three files under `/app/Config_Files`, and each has its own page.

## 📂 Files

| File | Page | What it holds |
|---|---|---|
| `Presets.json` | [📖 Presets]({% vlink /configuration/core/presets.md %}) | Your bin set, so a request need not carry the bins |
| `ForwardedHeaders.json` | [🌐 Forwarded Headers]({% vlink /configuration/core/forwarded-headers.md %}) | The caller's real address when a proxy or CDN sits in front |
| `Cors.json` | [🌍 CORS]({% vlink /configuration/core/cors.md %}) | The browser origins allowed to call the API directly |

## 🎛️ Switches

Each one is an environment variable, and each is **off** unless you set it:

| Variable | Turns on |
|---|---|
| `SWAGGER_UI=True` | Swagger UI at `/swagger/` - explore and call the endpoints from a browser |
| `SCALAR_UI=True` | Scalar UI at `/scalar/` - the same, in a different reader |
| `DEBUG_ENDPOINT=True` | `/_debug`, which echoes your own request back: the address the app resolved you to and every header you sent |
| `ASPNETCORE_HTTP_PORTS=<port>` | The port inside the container. `8080` when unset |

> `/_debug` needs no authentication and echoes **every** header, including `Authorization`. It is the quickest
> way to see what a proxy is actually sending. Turn it on to read a value, then turn it off again. Do not
> leave it enabled where other people can reach it.
{: .block-warning}

To run on port `80` inside the container and still reach it on `8080`:

```bash
docker run --name binacle-net \
  -e ASPNETCORE_HTTP_PORTS=80 \
  -e SWAGGER_UI=True \
  -p 8080:80 \
  binacle/binacle-net:{{ page.version_tag }}
```
