---
description: Harden and slim the base image - the base is now 90% of it
state: idea
waits-on: "nobody - it is an idea. horizon: future - chosen by an agent, strike it if wrong"
horizon: future
---

# Harden and slim the base image

The image is 103.2 MB and roughly 90% of that is the `aspnet:10.0` base. Moving to a chiseled `aspnet` tag
would land it around 73 to 88 MB, but the reason to do it is the attack surface, not the size: no shell, no
package manager, non-root by default. Take the size as a bonus.

## Research

### 2026-08-10 - measured. Do not re-measure

Dropping `--self-contained` took the image from **150.2 MB to 103.2 MB**, which closed the finding this was
opened for. What is left is the base itself.

| Where the 103.2 MB sits | | Base image options | |
|---|---|---|---|
| base `aspnet:10.0` | **93.7 MB** | `aspnet:10.0` (current) | 93.7 MB |
| the app layer | 18.4 MB | `aspnet:10.0-noble-chiseled-extra` | 67.5 MB |
| `libgssapi-krb5-2` | 2.5 MB | `aspnet:10.0-noble-chiseled` | 52.6 MB |
| | | `runtime-deps:10.0-noble-chiseled` | 5.5 MB |

### 2026-08-10 - self-contained is not a route to a smaller image

That publish is 123 MB and the whole `aspnet` base is 93.7 MB, so even the 5.5 MB chiseled base lands around
130 MB, worse than today. It is only a route to a *generic* base, which matters for Docker Hardened Images. It
would need trimming or Native AOT to pay, and this app is a poor trimming candidate: Azure SDK, Npgsql and
OpenTelemetry are reflection-heavy and Razor Pages resolves types by name.

### Date not recorded - two blockers, and the first picks the tag

- **ICU.** Plain chiseled has none, so it needs `InvariantGlobalization=true`, which changes string comparison
  app-wide. **Take `-extra` unless someone checks that is safe** - a wrong answer is subtly different sorting,
  not a crash.
- **`libgssapi-krb5-2` cannot be apt-installed** on chiseled. Copy the `.so` in from a builder stage, or drop
  it - it exists only to silence a cosmetic Npgsql log line that looks fatal and is not.

### Date not recorded - verify with the smoke suite

Expect some structure assertions to fail on paths or a shell chiseled does not have; that is the suite working.
Confirm `APP_UID` still resolves and `/app/data` is still writable.

### Date not recorded - DHI: chiseled first, and probably chiseled only

The gap to Docker Hardened Images is mostly attestation paperwork, and `sbom: true` plus
`provenance: mode=max` supplies part of it free. Revisit if someone actually asks for VEX, and confirm the
catalog carries an ASP.NET runtime image before planning around it.
