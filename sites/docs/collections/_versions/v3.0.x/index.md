---
title       : Welcome to Binacle.Net Docs!
description: >-
  Documentation for Binacle.Net v3.0.x: quick start, the HTTP API, configuration, Docker and Kubernetes samples,
  and the release notes.
menu_title  : v3.0.x
permalink: /version/v3.0.x/
nav:
  exclude: true
  order: 1
  icon: 🏠
---


Binacle.Net answers which box an order goes in, in milliseconds. Give it your box sizes and a list of items
and it returns the smallest box that holds them, and where every item sits. It is a free and open source 3D
bin packing API that you run yourself, shipped as a Docker image.

These docs cover the HTTP API, configuration, deployment, and the ViPaq result format. If you just want to
see it work, start with the quick start.

---

## 🚀 [Quick Start]({% vlink /quick-start.md %})
One `docker run`, then a request. Start here if you have not run it yet.

## 🛠️ [Release Notes]({% vlink release-notes.md %})
See what's new in the {{ page.version }} version of Binacle.Net.

## 📡 [API]({% vlink /api/index.md %})
The endpoints, the request and response shapes, and how presets differ from sending your own bins.

Two versions are documented:
- ✅ [V3]({% vlink /api/v3.md %}): fitting and packing with a choice of algorithm. Stable, and the recommended version.
- 🧪 [V4]({% vlink /api/v4.md %}): 16 endpoints organized by the answer you want. **Experimental** - it can change at any time.

**V2 was removed in this version.** If you still call it, see the
[v2.1.x documentation]({{ '/version/v2.1.x/' | relative_url }}).

## 🗜️ [ViPaq Protocol]({% vlink vipaq-protocol.md %})
The compact format the packing endpoints return. The format changed in v3.0.0 and is stable from this release.

## 🔧 Configuration
Customize Binacle.Net to suit your environment. Explore the following configuration modules:

- [🏗️ Core]({% vlink /configuration/core/index.md %}): Provides essential API functionality, including Presets and running behind a proxy.
- [📊 Diagnostics Module]({% vlink /configuration/diagnostics-module/index.md %}): Configure logging, health checks, and telemetry.
- [🛡️ Service Module]({% vlink /configuration/service-module/index.md %}): Allows Binacle.Net to run as a Service. Built for the hosted service - **no public documentation from v2.0.0 onward**.
- [🖥️ UI Module]({% vlink /configuration/ui-module/index.md %}): Turn on the browser demo - packing and the ViPaq decoder.

## 📦 [Samples]({% vlink /samples/index.md %})
Docker Compose and Kubernetes setups to copy and edit, including one for running the API behind your own backend.
