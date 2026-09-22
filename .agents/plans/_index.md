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
  description: "Orchestrator - steps 1 to 11 landed; what is left is the bench tooling rework, the findings from the review of steps 1 to 12, and the first keepers. The maintainer commits between steps"
  state: ready
  waits-on: "the maintainer deletes the old bench files named in the findings file; the other findings can start now"
  horizon: next-release
  paths: ["shared/**", "lib/**", "vipaq/**", "tooling/**", "results/**", ".agents/**"]
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
- file: measured-results/12-bench-split.md
  description: "Step 12 - one benchmark project per question, five of them, each with its tiers and scenarios settled, the config in shared/test/Binacle.Benchmarking, bench.just, the two benchmark scripts gone"
  state: ready
  waits-on: "the maintainer deletes the old files named in findings.md section 1, and runs the smoke recipes"
  horizon: next-release
- file: measured-results/14-first-keepers.md
  description: "Step 14 - the scaling class and the JSON timing are written, then the first keepers - lib-algorithms-smoke, vipaq, the bin threshold once on a quiet machine - their benchmarks README, and the bin-threshold finding"
  state: ready
  waits-on: "step 12's tooling rework, and a quiet machine for the threshold run"
  horizon: next-release
- file: measured-results/consistent-provider-names.md
  description: "One naming rule for every scenario provider in the Data and Testing projects - the namespace says what kind, the class says which one, every class has the same members"
  state: proposed
  waits-on: "the maintainer picks the scope (the table only, or the data projects' members too); he wants consistent names, 2026-09-22"
  horizon: undecided
- file: measured-results/findings.md
  description: "Findings from the 2026-09-22 reviews of steps 1 to 12 as landed - the bench tooling the maintainer wants reworked, what broke or lost coverage, what text is false, where the build drifted from the shape. A finding leaves here when it is fixed."
  state: ready
  waits-on: "nothing - the bench tooling shape was picked and built 2026-09-22; the old files wait on the maintainer"
  horizon: now
- file: measured-results/fixes.md
  description: "Open fixes in the measure and bench tooling that wait on the maintainer's answer, settled together with the results story and the provider names"
  state: proposed
  waits-on: "the maintainer's answer on each item. horizon was set by an agent, strike it"
  horizon: undecided
- file: measured-results/results-story.md
  description: "The results READMEs as a story for a human - \"X is N% faster, cheaper or smaller than Y\" - written from the raw files the measure and bench projects produce, in a session of its own; holds what the removed READMEs said and what the old vault could still prove"
  state: ready
  waits-on: "a session of its own - the maintainer says when. horizon was set by an agent, strike it"
  horizon: undecided
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
- file: tooling/just-recipes-cleanup.md
  description: "The just modules get the fixes the bench module got - recipes listed in file order, a word checked by just before anything runs, short scripts folded back into their recipe, one way to name a private recipe"
  state: proposed
  waits-on: "a yes from the maintainer, item by item. State picked by an agent to make the file legible; strike it if wrong"
  horizon: undecided
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
