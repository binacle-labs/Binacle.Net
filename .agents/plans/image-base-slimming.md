---
description: Harden and slim the base image - the base is now 90% of it
state: idea
waits-on: "nobody - not near future"
---

# Harden and slim the base image

The finding this was opened for is fixed: dropping `--self-contained` on 2026-08-10 took the image from
**150.2 MB to 103.2 MB**. What is left is the base itself, which is now roughly 90% of the image.

**Measured 2026-08-10. Do not re-measure.**

| Where the 103.2 MB sits | | Base image options | |
|---|---|---|---|
| base `aspnet:10.0` | **93.7 MB** | `aspnet:10.0` (current) | 93.7 MB |
| the app layer | 18.4 MB | `aspnet:10.0-noble-chiseled-extra` | 67.5 MB |
| `libgssapi-krb5-2` | 2.5 MB | `aspnet:10.0-noble-chiseled` | 52.6 MB |
| | | `runtime-deps:10.0-noble-chiseled` | 5.5 MB |

**Self-contained is not a route to a smaller image.** That publish is 123 MB and the whole `aspnet` base is
93.7 MB, so even the 5.5 MB chiseled base lands around 130 MB, worse than today. It is only a route to a
*generic* base, which matters for Docker Hardened Images. It would need trimming or Native AOT to pay, and
this app is a poor trimming candidate - Azure SDK, Npgsql and OpenTelemetry are reflection-heavy and Razor
Pages resolves types by name.

**The recommendation: framework-dependent on a chiseled `aspnet` tag.** ~73 to ~88 MB. **Do it for the attack
surface, take the size as a bonus** - no shell, no package manager, non-root by default. Two blockers, and
the first picks the tag:

- **ICU.** Plain chiseled has none, so it needs `InvariantGlobalization=true`, which changes string comparison
  app-wide. **Take `-extra` unless someone checks that is safe** - a wrong answer is subtly different sorting,
  not a crash.
- **`libgssapi-krb5-2` cannot be apt-installed** on chiseled. Copy the `.so` in from a builder stage, or drop
  it - it exists only to silence a cosmetic Npgsql log line that looks fatal and is not.

**Verify with the smoke suite.** Expect some structure assertions to fail on paths or a shell chiseled does
not have; that is the suite working. Confirm `APP_UID` still resolves and `/app/data` is still writable.

**DHI: chiseled first, and probably chiseled only.** The gap to DHI is mostly attestation paperwork, and
`sbom: true` plus `provenance: mode=max` supplies part of it free. Revisit if someone actually asks for VEX,
and confirm the catalog carries an ASP.NET runtime image before planning around it.
