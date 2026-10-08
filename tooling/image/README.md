# Image

What `just image` runs: the local stacks, the checks against a published image, and the Docker Hub page.
One script per operation, so every recipe in `tooling/image.just` is a named command you can also run in a
terminal.

Nothing is written inside the `.just` file, because a recipe body cannot be run on its own and cannot be
handed to shellcheck. A `.sh` file is both.

## 📂 What is in it

| File | What it does |
|---|---|
| `stack.sh` | Brings a local stack up or down - `full`, `volume` or `bind`. Holds the one table that maps a stack name to its compose file, project name and folders |
| `verify.sh` | The front door of `just image verify`: checks the arguments, runs the checks asked for, OR-s their exit codes so the first failure cannot hide the rest |
| `verify-tags.sh` | Which tags in the repository are this same image. Docker Hub through its web API; GHCR through the OCI registry API, where a prerelease stops |
| `verify-signature.sh` | `cosign verify` against the release workflow's identity on the given ref |
| `verify-attestations.sh` | The SPDX SBOM package count and the SLSA provenance builder |
| `verify-metadata.sh` | The OCI labels, then what the container reports from a throwaway run |
| `dockerhub-overview.sh` | Renders `.github/dockerhub-overview.md` for a version and prints it. Refuses a prerelease |

The two compose files the stacks read, `image.full.yml` and `image.local.yml`, sit one level up at the
tooling root.

## 🚀 How you run one

From the repo root, through the `image` module. `just --list image` prints the recipes with their arguments.

```bash
just image up bind
just image verify 3.0.0
just image verify 3.1.0-beta.1 all refs/heads/main ghcr.io/binacle-labs/binacle-net
just image dockerhub-overview 3.1.0
```

## ⚠️ What will bite you

**Every path is written from the repo root**, and the module sets `working-directory` so a recipe starts
there. Run a script by hand from the root too.

**`verify.sh` runs without `set -e` on purpose.** Every check runs and the codes are OR-ed at the end, so
the first failure cannot hide the answers that explain it. The checks themselves fail on their own.

**No `docker login`, anywhere in `verify-*.sh`.** These are the commands a user runs against a public
artifact, and a check that only passes with a credential is not checking a public artifact. GHCR works
because the package is public.

**The stack name is positional.** `just image up -d` reads `-d` as the stack name and is rejected; write
`just image up full -d`.

**They must pass `shellcheck` clean.** `just check scripts` covers this folder.
