---
title: UI Module
description: >-
  The UI Module adds a browser interface for trying Binacle.Net without writing API calls. Off by default, and
  it reads no configuration.
permalink: /version/v3.0.x/configuration/ui-module/
nav:
  parent: Configuration
  order: 4
  icon: 🖥️
---

The UI Module adds two pages to the API: a packing demo and a ViPaq decoder. Both run in your browser, so you
can try Binacle.Net without writing a request.

> This module is disabled by default.
{: .block-note}

## 📦 Packing Demo
Put in your own bins and items, pick an algorithm, and watch it pack.

- 🔹 Step through it one item at a time.
- 🔹 Watch each item land in the bin as it is placed.

## 📜 ViPaq Decoder
Paste a ViPaq string and see the packing drawn. Step through the layout to check how it was arranged.

- 📌 To obtain ViPaq data, set `includeViPaqData` on a packing request in
  [Version 3]({% vlink /api/v3.md %}) or [Version 4]({% vlink /api/v4.md %}) of the API.
- 📌 The decoder reads the **v3.0.0 format only**. Data produced by v2.1.1 and earlier is rejected - see
  [ViPaq Protocol]({% vlink vipaq-protocol.md %}).


## 🔧 Activating the UI Module
To enable the UIModule, set the environment variable:
```bash
UI_MODULE=True
```

That is the whole setup. The module reads no configuration at all - both demos run in your browser and call
the API they are served from, over relative URLs, so there is nothing to point anywhere.
