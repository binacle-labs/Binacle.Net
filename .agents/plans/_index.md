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
- file: api/packing-demo-next.md
  description: "The next three pieces of work on the packing demo - name the items that did not fit, stop the submit button sticking, and show the visitor the HTTP call that was just made"
  state: proposed
  waits-on: "two answers - whether the request panel is a UI Module feature or a shared one, and which API version it prints. The other two need no decision. State chosen by an agent to make the file legible; strike it if it is wrong"
  paths: ["api/src/Binacle.Net.UIModule/**", "packages/binacle-net-ui/**", "sites/demo/**"]
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
- file: api/ui-clients-off-v3.md
  description: "Migrate the shipped UI clients off the v3 API"
  state: idea
  waits-on: "the shape - what the UI changes to and how is not worked out yet"
  horizon: near
  paths: ["api/**", "packages/binacle-net-ui/**"]
- file: api/uimodule-instance-presets.md
  description: "The instance page reads its presets over HTTP from the browser - move it to server-side state"
  state: idea
  waits-on: "nobody - it is an idea"
  horizon: next-release
  paths: ["api/src/Binacle.Net.UIModule/**", "api/src/Binacle.Net.Kernel/**"]
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
  description: "Seven open CI questions left by the platform sweep - Docker Hub OIDC, persist-credentials, one deploy workflow instead of three, scoping the registry credential, dropping setup-buildx-action, the Sonar wait, and the site half of the path filter. Six close on a sentence; one needs a dispatch"
  state: blocked
  waits-on: "the maintainer - findings 2, 3, 6, 9, 11 and the shellcheck gap are done; the rest are each a separate yes or no. State chosen by an agent, it was `in-progress` and that is not one of the five - strike it if wrong"
  paths: [".github/workflows/**", ".github/actions/**", "tooling/ci/**"]
- file: ci-cd/delete-the-beta-images.md
  description: "Eight 3.0.0 beta images are still pullable on Docker Hub. They go, deliberately later rather than now."
  state: deferred
  waits-on: "the maintainer, who chose to leave them a few months. Nothing depends on it and nothing decays"
  horizon: undecided
  paths: [".github/dockerhub-overview.md"]
- file: ci-cd/multi-arch-images.md
  description: "CI - publish the image for arm64 as well as amd64"
  state: idea
  waits-on: "someone asking for ARM - nobody has"
  horizon: on-demand
  paths: [".github/workflows/**"]
- file: ci-cd/what-the-pull-request-does-not-run.md
  description: "Two things a pull request does not run - the integration suites against the shipped module set, and Sonar, which is dispatch-only"
  state: idea
  waits-on: "nobody - it is an idea. horizon: near - chosen by an agent, strike it if wrong"
  horizon: near
  paths: [".github/workflows/**"]
```

## Shared

```yaml
- file: shared/fixture-fill-per-algorithm.md
  description: "The scenario fixtures record which algorithms succeed, not how full the bin got"
  state: idea
  waits-on: "nobody - it is an idea. horizon: future - chosen by an agent, strike it if wrong"
  horizon: future
  paths: ["shared/data/**", "shared/test/Binacle.TestsKernel/**"]
- file: shared/testskernel-data-extraction.md
  description: "TestsKernel - grow the shared fixture cases"
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
- file: tooling/where-benchmark-results-live.md
  description: "One unanswered question - where benchmark and performance results are persisted and in what shape - and the two mechanical jobs waiting behind it"
  state: idea
  waits-on: "a research session coming back with proposals. Nothing here can start until the maintainer picks one"
  horizon: next-release
  paths: ["tooling/**", "results/**", "lib/**"]
```
