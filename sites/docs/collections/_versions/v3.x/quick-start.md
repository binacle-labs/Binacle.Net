---
title: Quick Start
description: >-
  Run Binacle.Net with one Docker command, open Swagger UI, Scalar UI and the web UI, then send your first
  packing request and read the answer.
nav:
  order: 1
  icon: 🚀
---

One command runs it, one request shows what it does. The setup below turns on **Swagger UI**, **Scalar UI**
and the **UI Module** so you can also try it from a browser. All three are off by default.

## 🖥️ Run it with Docker

##### 1️⃣ Install Docker

Download and install Docker from [docker.com](https://www.docker.com/get-started).

##### 2️⃣ Launch Binacle.Net

```bash
docker run -d --name binacle-net \
  -p 8080:8080 \
  -e SWAGGER_UI=True \
  -e SCALAR_UI=True \
  -e UI_MODULE=True \
  binacle/binacle-net:{{ page.version_tag }}
```

The tag `{{ page.version_tag }}` follows the newest patch in this line and never a breaking change. Pin it
rather than `latest`, which follows every release, including the next major.

##### 3️⃣ Open it

- Swagger UI: [http://localhost:8080/swagger/](http://localhost:8080/swagger/)
- Scalar UI: [http://localhost:8080/scalar/](http://localhost:8080/scalar/)
- UI Module: [http://localhost:8080/](http://localhost:8080/) - the packing demo and the ViPaq decoder

## 📦 Send a request

Two bins, three kinds of item. Which bin holds them, and where does each item go?

```bash
curl -s http://localhost:8080/api/v3/pack/by-custom \
  -H 'Content-Type: application/json' \
  -d '{
    "parameters": { "algorithm": "FFD" },
    "bins": [
      { "id": "small", "length": 10, "width": 40, "height": 60 },
      { "id": "large", "length": 20, "width": 40, "height": 60 }
    ],
    "items": [
      { "id": "box_1", "quantity": 2, "length": 2, "width": 5, "height": 10 },
      { "id": "box_2", "quantity": 1, "length": 12, "width": 15, "height": 10 },
      { "id": "box_3", "quantity": 1, "length": 12, "width": 10, "height": 15 }
    ]
  }'
```

The answer holds one result per bin. For each, `result` says whether everything fit, and `packedItems` says
where each item landed - its dimensions as placed, and the corner it sits at:

```json
{
  "result": "Success",
  "data": [
    {
      "result": "FullyPacked",
      "bin": { "id": "small", "length": 10, "width": 40, "height": 60 },
      "packedItems": [
        { "id": "box_2", "length": 10, "width": 12, "height": 15, "x": 0, "y": 0, "z": 0 },
        { "id": "box_3", "length": 10, "width": 12, "height": 15, "x": 0, "y": 12, "z": 0 },
        { "id": "box_1", "length": 2, "width": 5, "height": 10, "x": 0, "y": 0, "z": 15 },
        { "id": "box_1", "length": 2, "width": 5, "height": 10, "x": 0, "y": 24, "z": 0 }
      ],
      "unpackedItems": [],
      "packedItemsVolumePercentage": 100,
      "packedBinVolumePercentage": 15.83
    },
    { "result": "FullyPacked", "bin": { "id": "large", "length": 20, "width": 40, "height": 60 }, "...": "..." }
  ]
}
```

Both bins hold everything; the small one is the answer. `box_2` went in as 10 x 12 x 15 although it was sent
as 12 x 15 x 10 - items are rotated to fit.

That is V3, the stable API. [V4]({% vlink /api/v4.md %}) answers the "which bin" question in one call with
`pack/smallest-bin`, and is experimental.

## ➡️ Where to go next

- [Core Concepts]({% vlink /core-concepts.md %}) - what a dimension must be, and what the algorithms do.
- [API]({% vlink /api/index.md %}) - every endpoint, for V3 and V4.
- [Presets]({% vlink /configuration/core/presets.md %}) - your own bins by name, so a request need not carry them.
- [Samples]({% vlink /samples/index.md %}) - Docker Compose and Kubernetes setups to copy, including one for
  running behind your own backend.

Binacle.Net is one container, so it runs on any platform that runs one. Whatever the platform, pin the same
tag as above.
