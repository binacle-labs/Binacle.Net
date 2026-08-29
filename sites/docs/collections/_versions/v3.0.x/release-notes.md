---
title: Release Notes
description: >-
  What changed in the Binacle.Net v3.0.x line, newest first. v3.0.0 removes the V2 endpoints and changes ViPaq,
  so read the migration guide.
nav:
  order: 2
  icon: 🛠️
---

Release notes for the **v3.0.x** line, newest release first. Every patch in this line is on this page.

> **v3.0.0 introduces breaking changes.** Existing integrations must be reviewed and updated. V2 endpoints are
> removed, ViPaq strings from earlier versions no longer decode, and health check IP restrictions are matched
> differently. See the [Migration Guide](#migration-guide).
{: .block-warning}

> 🛡️ **The Service Module is exempt from these notes.** From v2.0.0 it is developed for the hosted service, so a
> breaking change to it is not documented here and does not force a major version increment. If you self-host
> with the Service Module enabled, read every release before upgrading - a minor or patch release can break it.
> Everything else on this page follows the usual rules. See the
> [Service Module]({% vlink configuration/service-module/index.md %}) page.
{: .block-note}

---

## v3.0.0

*Not released yet - the date and the release link are added when v3.0.0 is tagged. See the
[releases page](https://github.com/binacle-labs/Binacle.Net/releases) for what is published today.*

### 🔎 Overview
- **V2 endpoints** were removed.
- **V4 endpoints** were introduced as experimental.
- **V3 endpoints** remain stable and unchanged, and are the recommended version.
- **ViPaq** was rebuilt with a smaller, simpler format. Strings from earlier versions no longer decode.
- **ViPaq** left experimental status - the format is stable as of this release.
- **Algorithms** were unified - fitting and packing now share one implementation.
- **Packing Logs** configuration was flattened, with breaking changes for existing integrations.
- **Forwarded headers** are now supported, so the real caller is resolved when running behind a proxy or CDN.
- **Health check IP restrictions** are matched differently, with breaking changes for existing allow-lists.
- **The demo UI was rebuilt**, its page addresses changed, and it gained a page describing the instance you are
  on.
- **The image creates `/app/data`** and gives it to the app user, so a volume mounted there is writable.
- **The image is signed**, and carries an SBOM and build provenance, so you can verify what you pull.
- **The image is about a third smaller** - it uses the .NET runtime from its base image instead of bundling a
  second copy.
- The project was **restructured**, separating the API, library, and ViPaq into their own roots.
- **Versioned documentation** now covers every minor line, so older images keep their docs.
- **The project moved** to the `binacle-labs` organization. Links redirect; the signing identity does not.

### ⚙️ Core Changes
- Removal of all V2 endpoints.
- Added **16 experimental V4 endpoints**, covering everything V3 does.
- V4 splits a request into three shapes. **One bin, one answer** - `fit/bin`, `pack/bin`, and their
  `{preset}/{bin}` variants.
- **Many bins, one answer** - `pack/smallest-bin`, `pack/smallest-bin/{preset}`, `fit/smallest-bin`, and
  `fit/smallest-bin/{preset}` return the smallest bin that works; `pack/best-bin` and `pack/best-bin/{preset}`
  return the bin the items fill the most.
- **Many bins, every answer** - `fit/compare-bins`, `pack/compare-bins`, and their `{preset}` variants return
  one result per bin, in the order the bins were sent.
- Presets can be **listed** with `presets` or **fetched one at a time** with `presets/{preset}`.
- V4 is **experimental and can change at any time**. V3 remains stable and is the recommended version. See
  [Version 4]({% vlink /api/v4.md %}).
- V3 endpoints are unchanged and remain stable, apart from the ViPaq payload.
- **A non-string `algorithm` value now answers 422 rather than 400.** `"algorithm": 1`, `true` or a list used
  to come back as `Invalid JSON Format` with no field named. It now comes back as a validation error naming
  the field - the same answer a misspelled name such as `"FFDD"` has always given. Both statuses were already
  declared on these endpoints, so the contract has not moved; what changed is which one you get.
- **ViPaq is no longer experimental.** The format is settled as of this release, where it carried an
  experimental warning through v2.1.1. A future format change takes a new `Version` code rather than altering
  the current one, so an older decoder rejects a newer string outright instead of misreading it. See
  [ViPaq Protocol]({% vlink vipaq-protocol.md %}).
- Added **forwarded headers** support, configured in `Config_Files/ForwardedHeaders.json`. **Disabled by
  default.** See [Forwarded Headers]({% vlink /configuration/core/forwarded-headers.md %}).
- When enabled, the caller's address and scheme are resolved from `X-Forwarded-For` and `X-Forwarded-Proto`
  before anything reads them, so rate limiting and health check IP restrictions see the real caller rather than
  the proxy.
- Trust is explicit - a proxy on loopback or a private network is trusted by default, anything else must be
  named. The app **refuses to start** if nothing is trusted, because that would make every caller's header
  believable.
- A different header can be read instead, for CDNs that send one - `CF-Connecting-IP`, `X-Real-IP`,
  `X-Azure-ClientIP`.
- `ASPNETCORE_FORWARDEDHEADERS_ENABLED` is **ignored**. It switches the underlying middleware on with no proxy
  verification, which lets any caller choose their own address.
- `TrustedProxies` entries are **read exactly as written**, the same rule as health check `RestrictedIPs`.
  `010.10.10.10` used to be read as octal and trust `8.10.10.10`, and `172.17.1` used to mean `172.17.0.1`;
  both now fail startup validation rather than trusting a host you did not name.
- Added a **`/_debug` endpoint**, off by default, enabled with `DEBUG_ENDPOINT=True`. It echoes the caller's own
  request - connection address and headers - for working out what a proxy is sending.
- A **startup warning** when a forwarding header arrives and does not take effect, either because the feature is
  off or because the trust list does not name your proxy. Logged once. Without it both states are silent and the
  app quietly reads the proxy as the caller.
- **No `Strict-Transport-Security` header is sent.** The `UseHsts` call arrived with the demo UI's template and
  only ever ran when that UI was on. The image terminates no TLS, so the proxy or CDN in front of it is the only
  thing that knows whether HSTS is safe to pin, and a browser honours it until it expires.
- **The image now creates `/app/data` and gives it to the app user.** A volume mounted there is writable with no
  extra setup. Previously docker created the mount point as root, the app does not run as root, and packing logs
  and the SQLite database could not be written to a fresh named volume.
- The image ships `libgssapi-krb5-2`, so Npgsql stops printing `Cannot load library libgssapi_krb5.so.2` at
  every start. Nothing was broken - the app authenticates with a password, not Kerberos - but the message read
  like a fatal error.
- The image carries **OCI labels** - title, description, source, url, documentation, vendor, licence and base
  image - plus version, revision and created per build.
- **The description at the top of every API document changed.** The same one-line summary now appears in Swagger
  UI, Scalar and the image's `description` label.
- **The image is signed, and ships an SBOM and build provenance.** Signing is keyless, so there is no public key
  to fetch - the signature is checked against the workflow that produced it - and it covers the digest, so it
  holds for every tag pointing at that image:

  ```bash
  cosign verify binacle/binacle-net:3.0.0 \
    --certificate-identity-regexp '^https://github\.com/binacle-labs/Binacle\.Net/\.github/workflows/release-docker-image\.yml@' \
    --certificate-oidc-issuer https://token.actions.githubusercontent.com
  ```

  The SPDX SBOM and SLSA provenance travel inside the image index;
  `docker buildx imagetools inspect binacle/binacle-net:3.0.0` lists them. See
  [Verifying a Release]({% vlink verifying-a-release.md %}).
- **The project moved to the `binacle-labs` organization, and the signing identity moved with it.** The command
  above names the new organization. GitHub redirects a moved repository's links, but a certificate identity is
  written into the signature and does not redirect - a stale one fails the check rather than warning.
- **The image is smaller - around 108 MB, where the same image built the old way was 158 MB.** The app is
  published framework-dependent, so it runs on the .NET runtime already in the `aspnet:10.0` base image instead
  of carrying a second copy of it. Nothing about running the container changes.
- **One environment variable was removed - `BINACLEAPI_CONNECTION_STRING`.** The UI module used it to point the
  demo at another API host. The rebuilt module reads no configuration and always calls the API it is served
  from, so the variable is now ignored rather than rejected. `Config_Files/UiModule/ConnectionStrings.json` went
  with it, and the image no longer ships a `UiModule` config folder.
- Every other environment variable is unchanged.

### 🧪 Diagnostics Module
- Packing Logs configuration was **flattened** - `Path`, `FileName`, `DateFormat`, and `ChannelLimit` now sit
  directly under `PackingLogs`. See [Packing Logs]({% vlink /configuration/diagnostics-module/packing-logs.md %}).
- Removed the **fitting** configuration block, now that fitting and packing share one log.
- Implementations depending on the old nested shape must be updated, or startup validation will fail.
- The default log path changed from `data/pack-logs/packing/` to `data/pack-logs/`.
- Packing log entries now include a `Timestamp` field.
- Added **`RetentionDays`** to `PackingLogs`. When set, packing log files older than that many days are deleted
  once a day, and each deletion is logged. **Off by default** (`null`) - files are kept until you remove them
  yourself. Only files matching the configured `FileName` pattern in the configured `Path` are touched, and only
  at the top level.
- **`/_health` now reports what the instance is running.** Its `System` entry carried only `Processors`. It now
  also carries `Version`, `Environment`, `StartedAt` and `Uptime`, plus `Features` - the names of everything
  switched on, such as `HealthChecks`, `UIModule` or `SwaggerUI` - and `ReservedPaths`, the path prefixes that
  never answer with a web page. `Processors` is unchanged, so an existing parser keeps working; the new keys are
  there to check that a configuration arrived the way you meant it to.
- Health check **`RestrictedIPs` now uses CIDR notation correctly**. The value after `/` was previously read as
  an address mask, so `192.168.1.0/24` covered nearly the whole IPv4 range instead of 256 addresses. Existing
  CIDR entries are now **much narrower** than they were. See
  [Health Checks]({% vlink /configuration/diagnostics-module/health-checks.md %}).
- Health check `RestrictedIPs` now matches **IPv4 callers in containers**. Addresses arriving in IPv4-mapped
  IPv6 form are unmapped before comparison, which they previously were not - no IPv4 entry could match.
- Removed the **`start-end` range form** from `RestrictedIPs`. Entries such as `192.168.1.0-192.168.1.255` now
  fail startup validation. Use CIDR instead.
- `RestrictedIPs` entries are now **read exactly as written**. An IPv4 address must be four plain decimal parts
  with no leading zeros, and an IPv6 address must be in its short, lowercase form. `010.10.10.10` used to be read
  as octal and admit `8.10.10.10`; `10.1` used to mean `10.0.0.1`; `167772161` meant the same. All of these now
  fail startup validation instead of quietly admitting a host you did not name. `192.168.1.1/24` still means the
  whole `192.168.1.0/24` - that is what CIDR notation means - but the startup log now says so.

### 🛡️ Service Module
- The Service Module is **exempt from these notes** - see the note at the top of this page. One fix is worth
  calling out on its own.
- **The auth token rate limit no longer partitions on a caller-supplied header.** It partitions on the
  connection's remote address, which forwarded headers resolve to the real caller wherever a proxy is trusted.
  Before this, varying the header reset your own login throttle.

### 🎨 UI Module
- **The demo UI was rebuilt.** It was Blazor with an interactive server render mode; it is now Razor Pages, with
  everything interactive running in the browser. **No SignalR circuit and no WebSocket**, so the demo works
  behind a proxy or CDN that does not forward one.
- **The page addresses changed** - `/PackingDemo` is now `/packing`, `/ProtocolDecoder` is now `/vipaq`, and
  `/Error` is now `/error/{errorCode?}`. Old links no longer resolve.
- **The demo's static files moved under `/_content/Binacle.Net.UIModule/`.** `/favicon.ico`, `/css/main.css`,
  `/js/`, `/vendor/` and `/assets/` no longer answer at the root.
- **API paths never answer with the demo's error page.** A miss under `/api`, `/openapi`, `/swagger`, `/scalar`,
  `/_health`, `/_debug` or `/_content` returns the bare status or the JSON the endpoint wrote. Only `/api`,
  `/swagger` and `/scalar` were exempt before, so a miss on `/openapi` or a diagnostics path answered a caller
  with a web page.
- **The Protocol Decoder is now the ViPaq Decoder.** Same tool. It reads the **new ViPaq format only**, and
  strings from earlier versions are rejected. See [ViPaq Protocol]({% vlink vipaq-protocol.md %}).
- **A new page, `/instance`.** The version and environment this container is running, which features are
  switched on and where each one answers, and the presets it loaded - so you can see whether your configuration
  arrived the way you meant it to. It also links to GitHub Discussions.
- **The footer is one line** - copyright, version, licence, GitHub and Docker Hub. The Swagger UI and Scalar
  links that used to be footer badges are on the instance page, which also says whether each one is switched on.
- **Nothing on a page is fetched from the internet.** The footer's `img.shields.io` badges are gone, and the
  stylesheet, the icon font, the logos and the 3D library are all served from the image. An air-gapped install
  renders the same as any other.
- **The Packing Demo carries 20 worked examples instead of one**, and its two randomize buttons are now one. The
  same example always loads, so a link to the page shows everyone the same thing; Randomize moves to a different
  one. Each was checked against all three algorithms, and each is a set where the bins genuinely disagree -
  which is the comparison the page exists to show.
- **Add bin copies the bin above it, and Add item is sized to the bins you already have.** Both used to roll a
  fresh random box, which could add an item no bin on the page could hold.
- **The Packing Demo and ViPaq Decoder descriptions were rewritten.** What each tool does is unchanged.
- **Validation errors from the API are listed again.** The dialog built its message list wrong and always came
  up empty, so a rejected request opened a dialog with nothing in it.
- **A decoded bin with a zero side reads `0%` rather than `NaN%`** in the ViPaq Decoder. The bin comes out of a
  string a visitor pastes in, so a zero side is reachable.
- **The error page names the problem** - a separate line for 404, 403 and 500 instead of one sentence for all of
  them - and links back to the home page.
- **The demo follows your machine's light or dark setting on a first visit.** It used to start in light mode
  whatever the machine was set to. The switcher in the header still overrides it, and the choice is still kept
  in the same `theme` cookie, so anyone who already picked one keeps it.
- **The theme now sticks on an instance served over plain http.** The `theme` cookie was always written
  `Secure`, which a browser drops off https, so the demo reset to the default on every page load. It is marked
  `Secure` only where the page is served over https.
- **The module reads no configuration at all.** `UI_MODULE=True` is the whole setup - see Core Changes for the
  variable that went, and [UI Module]({% vlink /configuration/ui-module/index.md %}) for the page.

### 📈 Algorithms
- **Fitting and packing now share one algorithm.** Fitting stops early on the first item that does not fit.
- Packing results are unchanged - the shared algorithm is the previous packing implementation.
- The separate fitting algorithm family was retired.

### 🏗️ Internal Work
- Patched two **high-severity advisories** in transitive dependencies - `Microsoft.OpenApi` and the bundled
  **SQLite** native library.
- Extracted **Binacle.Geometry** into its own library.
- **Took `FluxResults` in-tree.** The result and union types the API returns are now part of the repository
  rather than a NuGet package. No behaviour changed.
- Reworked the packing log pipeline, moving the generic parts into the Kernel.

Everything below is work on the repository. None of it reaches the image.

- Restructured the repository - the API, library, ViPaq, and shared test data now live in their own roots. No
  route, contract or configuration moved with it, which is why it is listed here rather than as a change above.
- Added benchmark suites for algorithms, bin processing, result selection, and ViPaq.
- Added cross-language ViPaq interop tests between C# and TypeScript.
- **Rebuilt the release pipeline.** A release is dispatched with a version. It builds the image once, smoke
  tests it in a staging registry, copies the tested digest to Docker Hub, and creates the git tag and the
  GitHub release last - so what is published is bit for bit what passed, a failure anywhere leaves Docker Hub
  untouched, and no tag exists for a release that did not finish. The release body is the changelog, extracted
  by the workflow.
- Renamed two top-level folders - `config/` is now `tooling/`, and build output goes to `artifacts/` instead of
  `build/`.
- Every GitHub Action is pinned to a commit SHA, kept current by Dependabot.

### 📚 Versioned Docs
- Documentation is now versioned per minor line - `v1.3.x`, `v2.0.x`, `v2.1.x`, `v3.0.x` - so any image can be
  matched to its docs.
- Backfilled the `v2.0.x` and `v2.1.x` documentation, which was previously missing.
- The `latest` documentation now redirects to the current version, so existing links keep working.

### 🛠️ Migration Guide {#migration-guide}
To upgrade to **v3.0.0**, follow these steps:

1. **Remove all V2 usage**
   - Any calls to V2 endpoints must be removed or migrated.
   - Replace `/api/v2/presets`, `/api/v2/fit/by-custom`, `/api/v2/fit/by-preset/{preset}`,
     `/api/v2/pack/by-custom`, and `/api/v2/pack/by-preset/{preset}` with their V3 equivalents.

2. **Switch to V3 endpoints**
   - V3 requires an algorithm to be selected, where V2 used a fixed one, and drops V2's other parameters.
   - See the [v2.1.x documentation]({{ '/version/v2.1.x/' | relative_url }}) for the old contract, and
     [Version 3]({% vlink /api/v3.md %}) for the new one.

3. **Regenerate all ViPaq strings**
   - The format was rebuilt and is not backwards compatible.
   - Strings from earlier versions no longer decode, and there is no fallback reader.
   - Re-run the packing request to get a new one. Any stored string - a saved link or a bookmarked result - is
     stale.
   - This applies to V3 responses as well, even though V3 is otherwise unchanged.

4. **Do not mix versions**
   - Images before v3.0.0 produce the old ViPaq format; v3.0.0 onward produces and reads only the new one.
   - An encoder and a decoder on different sides of this release will not interoperate.

5. **Update Packing Logs configuration**
   - Move `Path`, `FileName`, `DateFormat`, and `ChannelLimit` out of the nested `Packing` block, directly under
     `PackingLogs`, and delete the `Fitting` block.
   - Left in the old shape with `Enabled: true`, startup validation now fails.
   - Repoint log collection from `data/pack-logs/packing/` to `data/pack-logs/`. The old `packing/` and
     `fitting/` directories are safe to remove.

6. **Review health check `RestrictedIPs`**
   - Replace any `start-end` entries with CIDR - `192.168.1.0-192.168.1.255` becomes `192.168.1.0/24`. Left as
     they are, startup validation now fails.
   - Re-check any CIDR entry. It now covers what it says, which is far less than before - confirm the addresses
     you expect are still inside it, or you will lock yourself out.
   - A range that does not line up with a CIDR boundary must be split into several entries, or widened to the
     enclosing subnet.
   - Drop any leading zeros - `010.10.10.10` becomes `10.10.10.10`, and note it used to admit `8.10.10.10`, so
     check that host was not the one you meant. Write IPv6 entries in the short lowercase form: `2001:0DB8::1`
     becomes `2001:db8::1`.
   - If Binacle.Net runs behind a proxy, load balancer or CDN, enable
     [Forwarded Headers]({% vlink /configuration/core/forwarded-headers.md %}) as well. Without it the list is
     compared against the proxy's address and can never match your monitoring system.

7. **Drop `BINACLEAPI_CONNECTION_STRING`**
   - Delete it from any compose file, Kubernetes manifest or environment file. It is ignored, not rejected, so
     nothing fails to start and nothing warns.
   - Only affects you if you set it. Pointing the shipped demo at a different API host is no longer possible;
     the demo calls the API it is served from.
   - Any `Config_Files/UiModule/` directory you mounted or edited can be removed. The image no longer reads it.
   - Bookmarks to `/PackingDemo` or `/ProtocolDecoder` need updating to `/packing` and `/vipaq`.

8. **Update any pinned `cosign verify` command**
   - The repository moved to the `binacle-labs` organization and the certificate identity moved with it. Replace
     `ChrisMavrommatis` with `binacle-labs` in `--certificate-identity-regexp`.
   - Only affects you if you verify signatures in a script or a pipeline. A stale identity **fails** the check,
     it does not warn - so it reads as a tampered image rather than an out-of-date command.
   - See [Verifying a Release]({% vlink verifying-a-release.md %}) for the full command.
