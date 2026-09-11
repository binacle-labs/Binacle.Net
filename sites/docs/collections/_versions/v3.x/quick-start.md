---
title: Quick Start
description: >-
  Run Binacle.Net with one Docker command, with Swagger UI, Scalar UI and the web UI switched on so you
  can try it from a browser, or deploy the same container to Azure, AWS, Google Cloud, Koyeb or Digital Ocean.
nav:
  order: 1
  icon: 🚀
---

Getting started with Binacle.Net is simple.

The setup below turns on **Swagger UI**, **Scalar UI** and the **UI Module** so you can try it out from a
browser. All three are off by default.

## 🖥️ Run Locally with Docker

##### 1️⃣ Install Docker

Download and install Docker from [docker.com](https://www.docker.com/get-started).

##### 2️⃣ Launch Binacle.Net

Run this command in your terminal:

```bash
docker run -d --name binacle-net \
  -p 8080:8080 \
  -e SWAGGER_UI=True \
  -e SCALAR_UI=True \
  -e UI_MODULE=True \
  binacle/binacle-net:{{ page.version_tag }}
```

This starts Binacle.Net with Swagger UI, Scalar UI and the UI Module enabled on port 8080.

The tag `{{ page.version_tag }}` is the minor tag: it follows the newest patch in this line and never a
breaking change.

##### 3️⃣ Access Locally

- Swagger UI: [http://localhost:8080/swagger/](http://localhost:8080/swagger/)
- Scalar UI: [http://localhost:8080/scalar/](http://localhost:8080/scalar/)
- UI Module: [http://localhost:8080/](http://localhost:8080/)

## ☁️ Run in the Cloud

Binacle.Net is one container, so it runs on every platform that runs one. Pick by what you already use:

| Deployment     | Best Use Case                                | Platform URL                                                                        |
|----------------|----------------------------------------------|-------------------------------------------------------------------------------------|
| Local (Docker) | Quick development, testing, demos            | [Docker](https://www.docker.com/)                                                   |
| Azure          | Microsoft stack integration, scalable apps   | [Azure App Service](https://azure.microsoft.com/en-us/products/app-service/)        |
| AWS            | Large-scale microservices, container scaling | [AWS ECS](https://aws.amazon.com/ecs/) / [Fargate](https://aws.amazon.com/fargate/) |
| Google Cloud   | Serverless, efficient API deployment         | [Google Cloud Run](https://cloud.google.com/run)                                    |
| Koyeb          | Simple, cost-effective small workloads       | [Koyeb](https://www.koyeb.com/)                                                     |
| Digital Ocean  | Easy, affordable for SMB apps                | [Digital Ocean](https://www.digitalocean.com/products/app-platform/)                |

Whatever the platform, pin the same tag as above and never `latest` - it follows the newest release, and a
major release can bring breaking changes.

## ➡️ Where to go next

- [API]({% vlink /api/index.md %}) - the endpoints, for V3 and experimental V4.
- [Presets]({% vlink /configuration/core/presets.md %}) - replace the example bins with your own.
- [Samples]({% vlink /samples/index.md %}) - Docker Compose and Kubernetes setups to copy, including one for
  running behind your own backend.
