# Binacle.Net

## 📝 Overview
Binacle.Net is an API created to address the 3D Bin Packing Problem in real time.

It is an ideal fit for e-commerce platforms offering parcel shipments to self-service locker systems,
providing optimal bin packing calculations to ensure efficient use of space and smooth customer experiences during checkout.

## 🚀 Quick Start
Simply execute the following command in your terminal:

```bash
docker run -d --name binacle-net -p 8080:8080 -e SWAGGER_UI=True -e UI_MODULE=True -e SCALAR_UI=True binacle/binacle-net:latest
```

> `latest` follows the newest release, and a new major version can break your integration. It is the right tag
> for trying Binacle.Net out. **Pin a version for anything you keep** - `binacle/binacle-net:3.0` will track
> fixes within the 3.0 line without ever changing behaviour under you.
>
> Until `3.0.0` is published, the only name that resolves is `binacle/binacle-net:3.0.0-beta.4`.

Images from `3.0.0` onward are signed and carry a bill of materials -
**[SECURITY.md](SECURITY.md#verifying-a-release)** has the two commands that check what you pulled.
### 🌐 Access the Interface
- Swagger UI (API Documentation): http://localhost:8080/swagger/
- Scalar UI (Alternative to Swagger): http://localhost:8080/scalar/
- UI Module & Packing Demo: http://localhost:8080/

Start exploring Binacle.Net now! 🚀

## 📂 Repository Structure

```text
/Binacle.Net      # Root directory
├── /api          # HTTP API — ASP.NET Core minimal APIs (v3, v4) and modules
├── /lib          # Core 3D bin-packing engine (Binacle.Lib)
├── /vipaq        # ViPaq — compact binary format for packing results
├── /shared       # Shared test kernel and benchmark data
├── /packages     # JavaScript/TypeScript packages (npm workspaces)
├── /ruby         # Ruby gems — Jekyll plugins for the sites
├── /sites        # Every published site (Jekyll)
│   ├── /docs     # Documentation site
│   └── /demo     # Binacle.Net demo site
├── /samples      # Docker Compose and Kubernetes deployment samples
├── /tooling      # Every task the repo can run - just modules, scripts, local compose
├── /assets       # Shared static assets copied into the sites at build time
├── /artifacts    # Build output - published app, sites, OpenAPI, test results, coverage
└── /results      # Benchmark and packing-efficiency output
```

Each slice folder has its own `README.md` with details.

## 🛠️ Building from source

You need the .NET SDK, Node, Ruby, `just` and - for the container image only - Docker.
**[DEVELOPMENT.md](DEVELOPMENT.md)** has the versions, the pin files and the install commands.

```bash
just install                     # npm workspaces, all three sites' gems, then the asset copy
just test all                    # every suite that needs nothing brought up
just build image                 # publish, then tag binacle-net:local
```

`just` with no arguments lists every task.

## 📄 License

This project carries more than one license. Which one applies depends on which part you are using.

| What | License |
|---|---|
| The code - API, packing engine, browser UI | [GPL-3.0-only](LICENSE.GPL-3.0) |
| Documentation, images and other content | [CC BY 4.0](https://creativecommons.org/licenses/by/4.0/) |
| The data formats - ViPaq, compact notation, [PROTOCOL.md](vipaq/PROTOCOL.md), its test vectors and the geometry primitives they use | [Apache-2.0](vipaq/src/Binacle.ViPaq/LICENSE) |
| The Ruby gems under [`ruby/`](ruby) | MIT |
| Sample deployments ([`samples/`](samples)) and build tooling ([`tooling/`](tooling)) | MIT |
| [`shared/src/Binacle.FluxResults`](shared/src/Binacle.FluxResults) | MIT |

`SPDX-License-Identifier: GPL-3.0-only AND CC-BY-4.0 AND Apache-2.0 AND MIT`

**Every part with its own license carries its own `LICENSE` file next to it.** The data formats and the
samples are permissive on purpose: reading a ViPaq string or a packing log, or copying a compose file into
your own project, should not put your work under this one.

The logo and the brand assets are not covered by the content license. [CONTENT-TERMS.md](CONTENT-TERMS.md)
is the plain-English summary and names what is excluded.

### 📦 Third-Party Libraries
Binacle.Net uses third-party libraries and dependencies. 

See the [NOTICE](NOTICE) file for the complete map - every license above, and every dependency with the
license it ships under.

## Security
See [SECURITY.md](SECURITY.md) for my security policy and how to report vulnerabilities.

---

Copyright (c) 2023-2026 Chris Mavrommatis. All rights reserved.
