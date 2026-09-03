# Binacle.Net

**An order is ready to ship and something has to decide which box it goes in.** Binacle.Net answers that in
milliseconds: give it your box sizes and a list of items and it returns the smallest box that holds them, and
where every item sits.

It is a free and open source 3D bin packing API that you host yourself. Nothing about your customers' orders
leaves your network.

Built for checkout: your customer picks a locker or a box, and Binacle.Net says whether the order fits before
they pay.

## 🚀 Quick Start
One command:

```bash
docker run -d --name binacle-net -p 8080:8080 -e SWAGGER_UI=True -e UI_MODULE=True -e SCALAR_UI=True binacle/binacle-net:latest
```

> `latest` follows the newest release, and a new major version can break your integration. It is the right tag
> for trying Binacle.Net out. **Pin a version for anything you keep** - `binacle/binacle-net:3.0` will track
> fixes within the 3.0 line without ever changing behaviour under you.

Images from `3.0.0` onward are signed and carry a bill of materials -
**[SECURITY.md](SECURITY.md#verifying-a-release)** has the two commands that check what you pulled.
### 🌐 Access the Interface
The three flags in that command turn on three optional pages:

- <http://localhost:8080/> - the packing demo
- <http://localhost:8080/swagger/> - Swagger UI
- <http://localhost:8080/scalar/> - Scalar, an alternative to Swagger

The API itself is under `/api/v3` and `/api/v4` and needs none of them.

## 📐 What it answers

Binacle.Net works **one box at a time**.

| It answers | It does not answer |
|---|---|
| Does this order fit in this box? | How do I split an order across boxes? |
| Which of my boxes is the smallest that holds it? | How many boxes do I need? |
| Where does every item sit? | Which carrier or rate is cheapest? |

**The algorithms are heuristics.** A yes is reliable - if it says the items fit, they fit, and the pack
endpoints show you how. A no is not a proof: there may be an arrangement it did not find.

One box at a time is the single-container case, which is a 3D knapsack problem rather than bin packing
proper. The logistics trade has its own word for the job, cartonization. Almost everyone looking for it
searches for 3D bin packing, so that is the term used here.

## 📂 Repository Structure

```text
/Binacle.Net      # Root directory
├── /api          # HTTP API - ASP.NET Core minimal APIs (v3, v4) and modules
├── /lib          # Core 3D bin-packing engine (Binacle.Lib)
├── /vipaq        # ViPaq - compact binary format for packing results
├── /shared       # Shared test kernel and benchmark data
├── /packages     # JavaScript/TypeScript packages (npm workspaces)
├── /ruby         # Ruby gems - Jekyll plugins for the sites
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
| The code - API, packing engine, browser UI | [AGPL-3.0-only](LICENSE.AGPL-3.0) |
| Documentation, images and other content | [CC BY 4.0](https://creativecommons.org/licenses/by/4.0/) |
| The data formats - ViPaq, compact notation, [PROTOCOL.md](vipaq/PROTOCOL.md), its test vectors and the geometry primitives they use | [Apache-2.0](vipaq/src/Binacle.ViPaq/LICENSE) |
| The Ruby gems under [`ruby/`](ruby) | MIT |
| Sample deployments ([`samples/`](samples)) and build tooling ([`tooling/`](tooling)) | MIT |
| [`packages/theme-switcher`](packages/theme-switcher) | MIT |
| [`shared/src/Binacle.FluxResults`](shared/src/Binacle.FluxResults) | MIT |

`SPDX-License-Identifier: AGPL-3.0-only AND CC-BY-4.0 AND Apache-2.0 AND MIT`

**Every part with its own license carries its own `LICENSE` file next to it.** The data formats and the
samples are permissive on purpose: reading a ViPaq string or a packing log, or copying a compose file into
your own project, should not put your work under this one.

The logo and the brand assets are not covered by the content license. [CONTENT-TERMS.md](CONTENT-TERMS.md)
is the plain-English summary and names what is excluded.

### 📦 Third-Party Libraries

Binacle.Net uses third-party libraries and dependencies. See the [NOTICE](NOTICE) file for the complete map -
every license above, and every dependency with the license it ships under.

## 🛡️ Security
See [SECURITY.md](SECURITY.md) for my security policy and how to report vulnerabilities.

## 💬 Who is running this?

If Binacle.Net is running in something you built, say hello in a
[discussion](https://github.com/binacle-labs/Binacle.Net/discussions). Almost nobody does, and it is the only
way this gets built for real use instead of guesses.

[CONTRIBUTING.md](CONTRIBUTING.md) says what else is welcome, and why pull requests are closed for now.

## 🔗 Quick reference

- **Documentation:** <https://docs.binacle.net>
- **Website:** <https://www.binacle.net>
- **Demo:** <https://demo.binacle.net>
- **Deployment samples:** [`samples/`](samples) - Docker Compose and Kubernetes
- **Docker image:** <https://hub.docker.com/r/binacle/binacle-net>
- **Releases:** <https://github.com/binacle-labs/Binacle.Net/releases> - what changed in each is in the
  [changelog](CHANGELOG.md)
- **File an issue:** <https://github.com/binacle-labs/Binacle.Net/issues>

---

Copyright (c) 2023-2026 Chris Mavrommatis. All rights reserved.
