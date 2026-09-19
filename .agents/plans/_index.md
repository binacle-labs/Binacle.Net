---
description: Manifest of every file under .agents/plans, grouped by area. Regenerate with just agents all.
---

# Agent Plans Index

Every plan in `.agents/plans/` (recursive), grouped by area. Plans are work not yet done. Read the one
you need, and trim or delete it once the work lands. `state:` and `waits-on:` say where each one stands;
[README.md](README.md) defines the five states.

## General

```yaml
- file: architecture-checks.md
  description: "Generate the repo's dependency graph, draw it, lint it - plus three greps over api/src, and the one boundary violation to fix first"
  state: idea
  waits-on: "nobody - it is an idea"
  horizon: near
  paths: ["**/*.csproj", "tooling/**"]
- file: comment-lint.md
  description: "A check that nothing outside the agent guidance directory points a reader into it"
  state: idea
  waits-on: "nobody - it is an idea. horizon: future - chosen by an agent, strike it if wrong"
  horizon: future
- file: image-base-slimming.md
  description: "Harden and slim the base image - the base is now 90% of it"
  state: idea
  waits-on: "nobody - it is an idea. horizon: future - chosen by an agent, strike it if wrong"
  horizon: future
- file: measured-results.md
  description: "Orchestrator - measured results get a home in each slice, and the three tests kernels become Data and Testing projects first. Fourteen steps, one file each, the maintainer commits between them"
  state: ready
  waits-on: "nothing - shape agreed 2026-09-19. Step 1 can start; each step settles its open details with the maintainer before it touches a file"
  horizon: next-release
  paths: ["shared/**", "lib/**", "vipaq/**", "api/test/**", "tooling/**", "results/**", ".agents/**", "Binacle.Net.slnx", "Directory.Build.props", ".netconfig", ".gitignore", "justfile"]
- file: testing-techniques.md
  description: "The testing techniques this repo does not use - property-based, fuzzing, load, mutation - and the four yes-or-no answers"
  state: idea
  waits-on: "nobody - it is an idea"
  horizon: undecided
```

## API

```yaml
- file: api/integration-tests-cover-shipped-modules.md
  description: "Integration tests that exercise the module set the image ships, not core modules only"
  state: idea
  waits-on: "nobody - it is an idea. horizon: near - chosen by an agent, strike it if wrong"
  horizon: near
  paths: ["api/**"]
- file: api/pack-first-bin-endpoint.md
  description: "pack/first-bin endpoint"
  state: idea
  waits-on: "nobody - it is an idea"
  horizon: next-release
  paths: ["api/**"]
- file: api/packing-only-image.md
  description: "The public image becomes packing-only and the Service Module moves to its own image"
  state: proposed
  waits-on: "nothing. The tag landed 2026-09-01. It still needs a yes from the maintainer, which is what `proposed` means"
  paths: ["api/**"]
- file: api/servicemodule.md
  description: "How far ServiceModule is taken - answered. One store, one project, refresh tokens"
  state: proposed
  waits-on: "nothing. The tag landed 2026-09-01. It is answered together with the packing-only image split, and still needs a yes, which is what `proposed` means"
  paths: ["api/src/Binacle.Net.ServiceModule/**", "api/src/Binacle.Net.ServiceModule.Domain/**", "api/src/Binacle.Net.ServiceModule.Infrastructure/**"]
- file: api/v4-stable.md
  description: "v4 - flip from experimental to stable"
  state: idea
  waits-on: "an endpoint added to v4 that reshapes no existing contract - none has been chosen"
  horizon: near
  paths: ["api/**"]
```

## CI/CD

```yaml
- file: ci-cd/ci-open-questions.md
  description: "Seven open CI questions left by the platform sweep - Docker Hub OIDC, persist-credentials, one deploy workflow instead of three, scoping the registry credential, dropping setup-buildx-action, the Sonar wait, and the site half of the path filter. All seven close on a sentence; all were re-verified on 2026-09-11"
  state: ready
  waits-on: "the first release run from main - it is the first to run the changed publish job, since a prerelease stops at staging. All six approved findings landed 2026-09-11 and 2026-09-12, the OIDC connection exists since 2026-09-14, and 7 is rejected"
  paths: [".github/workflows/**", ".github/actions/**", "tooling/ci/**"]
- file: ci-cd/multi-arch-images.md
  description: "CI - publish the image for arm64 as well as amd64"
  state: idea
  waits-on: "someone asking for ARM - nobody has"
  horizon: on-demand
  paths: [".github/workflows/**"]
- file: ci-cd/prerelease-staging-repository.md
  description: "Branch builds go to the staging registry on dispatch, the way a prerelease now does - so an image from a branch can be tried without ever reaching the repository users pull from"
  state: idea
  waits-on: "the signing story for a branch-built image. horizon: undecided - chosen by an agent, strike it if wrong"
  horizon: undecided
  paths: [".github/workflows/**", "tooling/ci/**", "tooling/image.just", "tooling/image/**"]
```

## Measured-results

```yaml
- file: measured-results/01-binacle-data.md
  description: "Step 1 - shared/test/Binacle.TestsKernel becomes shared/data/Binacle.Data with one reader that takes an Assembly; then demo-samples gets a provider and its own tests"
  state: ready
  waits-on: "nothing - first step"
  horizon: next-release
- file: measured-results/02-binacle-lib-data.md
  description: "Step 2 - lib/test/Binacle.Lib.TestsKernel becomes lib/data/Binacle.Lib.Data and uses the one reader"
  state: ready
  waits-on: "step 1's gate"
  horizon: next-release
- file: measured-results/03-binacle-lib-testing.md
  description: "Step 3 - new Binacle.Lib.Testing takes the factories, the checks and the benchmark providers; Binacle.Data names no result type after it"
  state: ready
  waits-on: "step 2's gate"
  horizon: next-release
- file: measured-results/04-binacle-vipaq-data-and-testing.md
  description: "Step 4 - the ViPaq kernel splits into Binacle.ViPaq.Data (the packs) and Binacle.ViPaq.Testing (the encoders, the picks, the generator)"
  state: ready
  waits-on: "step 3's gate"
  horizon: next-release
- file: measured-results/05-binacle-reporting.md
  description: "Step 5 - Binacle.TestReporting becomes Binacle.Reporting; small, may ride with step 4"
  state: ready
  waits-on: "step 4's gate"
  horizon: next-release
- file: measured-results/06-support-projects-record.md
  description: "Step 6 - the folder rules go into the design record, the dependency docs are redrawn end to end, the shape file is deleted"
  state: ready
  waits-on: "step 5's gate"
  horizon: next-release
- file: measured-results/07-measure-projects.md
  description: "Step 7 - both PerformanceTests projects move to <slice>/measure/ under their new names, point their writer at <slice>/results/, and get measure.just; the memory and D3 that said otherwise go the same day"
  state: ready
  waits-on: "step 6's gate"
  horizon: next-release
- file: measured-results/08-lib-packing-efficiency.md
  description: "Step 8 - one run, many views - the lib runner packs every scenario once into a bag, one reporter per file writes the three files, the README holds the summaries"
  state: ready
  waits-on: "step 7's gate"
  horizon: next-release
- file: measured-results/09-vipaq-gates-and-json.md
  description: "Step 9 - the two round-trip gates become unit tests over ViPaq.Data and run under just test; the curated-picks check stays in the measure project"
  state: ready
  waits-on: "step 8's gate"
  horizon: next-release
- file: measured-results/10-vipaq-encoded-size.md
  description: "Step 10 - JSON and compact-notation encoders join protobuf, then the ViPaq runner and reporters write encoded-size.md with its text columns and the README with the largest-token line"
  state: ready
  waits-on: "step 9's gate"
  horizon: next-release
- file: measured-results/11-the-vault.md
  description: "Step 11 - every doc, record and config line that named the vault is rewritten; results/ itself stays on disk until the new benchmarks have run"
  state: ready
  waits-on: "step 10's gate"
  horizon: next-release
- file: measured-results/12-bench-split.md
  description: "Step 12 - one benchmark project per question, six of them, the config in the two Testing projects, bench.just, the two benchmark scripts gone"
  state: ready
  waits-on: "step 11's gate"
  horizon: next-release
- file: measured-results/13-convert-the-vault.md
  description: "Step 13 - every old keeper lands under its family folder with a date and a line naming its real class; both benchmarks/README.md are written"
  state: ready
  waits-on: "step 12's gate"
  horizon: next-release
- file: measured-results/14-first-keepers.md
  description: "Step 14 - the scaling class and the JSON timing are written, then the first keepers - lib-fast, vipaq-encoding, the bin threshold once on a quiet machine - and its finding"
  state: ready
  waits-on: "step 13's gate, and a quiet machine for the threshold run"
  horizon: next-release
- file: measured-results/results.md
  description: "Measured results get a home in each slice, a harness that writes the verdict, benchmark projects split by question, two just recipes, and the old vault converted in"
  state: ready
  waits-on: "nothing - the reviewer pass landed 2026-09-19; the support projects (steps 1 to 6 of the orchestrator) go first"
  horizon: next-release
  paths: ["tooling/**", "results/**", "lib/**", "vipaq/**", "shared/test/**"]
```

## Shared

```yaml
- file: shared/fixture-fill-per-algorithm.md
  description: "The scenario fixtures record which algorithms succeed, not how full the bin got"
  state: idea
  waits-on: "nobody - it is an idea. horizon: future - chosen by an agent, strike it if wrong"
  horizon: future
  paths: ["shared/data/**", "shared/data/Binacle.Data/**"]
- file: shared/grow-the-fixture-cases.md
  description: "Binacle.Data - grow the shared fixture cases"
  state: idea
  waits-on: "nobody - it is an idea. horizon: future - chosen by an agent, strike it if wrong"
  horizon: future
  paths: ["shared/**"]
```

## Sites

```yaml
- file: sites/code-blocks-and-wide-tables.md
  description: "Two framework defaults on the docs site - code samples had no named mono face (fixed), and wide tables are still clipped rather than scrolled"
  state: proposed
  waits-on: "a yes or no on wrapping each table in a scroll box - the only route left. State picked to make the file legible; strike it if it is wrong."
  paths: ["sites/docs/**"]
- file: sites/docs-current-at-root.md
  description: "The docs site keeps one folder per major, renders the current one at the site root, and drops the common-page layer. A minor stops moving every URL."
  state: ready
  waits-on: "the docs deploy - everything on the branch landed 2026-09-12, both open questions answered the same day; what is left needs the deployed site (the redirect curls, the selector click, the 301 flip) or the release (the major tag manifest)"
  horizon: now
  paths: ["sites/docs/**", "ruby/binacle-docs-versions/**", "tooling/openapi.just", ".github/workflows/release-docker-image.yml"]
```

## Tooling

```yaml
- file: tooling/linting.md
  description: "Answered no - linting is one decision for the whole repository, not a per-language one. TypeScript has nothing, Ruby has a config nobody runs, C# has SonarCloud but no in-build linter. Every language gets the same treatment or none does."
  state: deferred
  waits-on: "a decision to lint every language in this repository to the same standard - TypeScript alone is not the question"
  horizon: undecided
  paths: ["packages/**", "sites/**", "api/src/Binacle.Net.UIModule/**", "vipaq/packages/**", "ruby/**", ".editorconfig"]
- file: tooling/regen-check-runs-nowhere.md
  description: "`just regen check` is called by no workflow, and two of the files it covers cannot pass it - .NET's deflate output moves between SDK patch versions and nothing pins the SDK"
  state: ready
  waits-on: "nothing. Answered 2026-09-04: stop byte-comparing the two ViPaq vector files and compare what they decode to. The SDK stays unpinned"
  paths: ["tooling/**", "vipaq/test-vectors/**", ".github/workflows/**"]
```
