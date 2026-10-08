---
title       : Welcome to Binacle.Net Docs!
seo_title   : Open source 3D bin packing API - Binacle.Net Docs
breadcrumbs : false
description: >-
  Documentation for Binacle.Net: quick start, the HTTP API, configuration, Docker and Kubernetes samples,
  and the release notes.
menu_title  : v3.x
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
What changed in {{ page.version_label }}, and how to move from the version before.

## 🔍 [Core Concepts]({% vlink /core-concepts.md %})
What a dimension must be, fit against pack, and what each algorithm does.

## 📡 [API]({% vlink /api/index.md %})
The endpoints and their shapes. V3 is stable; V4 is experimental and adds the `Best` algorithm.

## 🧰 [Generate a Client]({% vlink /generate-a-client.md %})
A typed client from the published OpenAPI documents - hey-api for TypeScript, Kiota for C#.

## 🔧 [Configuration]({% vlink /configuration/index.md %})
The files under `/app/Config_Files`, how to override a setting, and one page per module.

## 📦 [Samples]({% vlink /samples/index.md %})
Docker Compose and Kubernetes setups to copy, including one for running behind your own backend.

## 🗜️ [ViPaq Protocol]({% vlink vipaq-protocol.md %})
The compact format the packing endpoints return. Stable from v3.0.0.

## 🔏 [Verifying a Release]({% vlink /verifying-a-release.md %})
Check the signature and read the bill of materials of a published image.
