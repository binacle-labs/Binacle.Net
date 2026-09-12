---
title       : Welcome to Binacle.Net Docs!
description: >-
  Documentation for Binacle.Net: quick start, the V1, V2 and V3 APIs, configuration, Docker samples, and
  the release notes.
breadcrumbs : false
menu_title  : v1.x
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
One `docker run`, then the URLs to open. Start here if you have not run it yet.

## 🛠️ [Release Notes]({% vlink release-notes.md %})
What changed in {{ page.version_label }}.

## 🔍 [Core Concepts]({% vlink /core-concepts.md %})
What a dimension must be, fit against pack, and what each algorithm does.

## 📡 [API]({% vlink /api/index.md %})
The endpoints and their shapes. V1 is deprecated, V2 is current, V3 is experimental, and Users needs the Service Module.

## 🔧 [Configuration]({% vlink /configuration/index.md %})
The files under `/app/Config_Files`, how to override a setting, and one page per module.

## 📦 [Samples]({% vlink /samples/index.md %})
Docker Compose setups to copy.

## 🗜️ [ViPaq Protocol]({% vlink vipaq-protocol.md %})
The compact format the V3 packing endpoint returns. Experimental in this line.
