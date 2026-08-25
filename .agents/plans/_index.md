---
description: Manifest of every file under .agents/plans, grouped by area. Regenerate with just agents all.
---

# Agent Plans Index

Every plan in `.agents/plans/` (recursive), grouped by area. Plans are work not yet done — read the one
you need, and trim or delete it once the work lands. `state:` and `waits-on:` say where each one stands;
[README.md](README.md) defines the five states.

## General

```yaml
- file: agents-doc-drift.md
  description: "The guidance drift a third site left, and the repository gaps nothing watches. A findings list - each item is fixed, moved onto the plan it concerns, or written down as a decision not to act."
  state: ready
  waits-on: "nothing"
  paths: [".agents/**", "sites/**", ".github/workflows/**"]
- file: architecture-checks.md
  description: "Derive the repo's dependency graph into a generated file, draw it, and lint it with a small ruleset."
  state: ready
  waits-on: "nothing - state chosen by an agent, strike it if wrong"
- file: comment-lint.md
  description: "A check that nothing outside the agent guidance directory points a reader into it."
  state: ready
  waits-on: "nothing - state chosen by an agent, strike it if wrong"
- file: image-base-slimming.md
  description: "Harden and slim the base image"
  state: ready
  waits-on: "nothing"
- file: mutation-testing.md
  description: "mutation testing with Stryker.NET"
  state: idea
  waits-on: "nobody waiting"
- file: ruby-gem-coverage.md
  description: "The ten Ruby gems produce no coverage, so they are absent from the coverage table and from Sonar. Adding simplecov collides with the rule that a gem must be droppable into an unrelated site."
  state: proposed
  waits-on: "a yes or a no - it is ten gems' published surface, and the alternative is writing down that Ruby coverage is deliberately absent"
  paths: ["ruby/**", "tooling/**"]
- file: sonar-issue-triage.md
  description: "Sonar - what is left after the 2026-08-09 sweep"
  state: ready
  waits-on: "nothing"
- file: testing-techniques.md
  description: "testing techniques not in use"
  state: idea
  waits-on: "nobody waiting"
- file: todos.md
  description: "TODOs"
  state: ready
  waits-on: "nothing"
- file: ui-test-harness.md
  description: "A test harness for the UI"
  state: blocked
  waits-on: "a Sonar run - state and blocker chosen by an agent, strike them if wrong"
```

## API

```yaml
- file: api/integration-test-additions.md
  description: "Integration tests: cover what the harness cannot see today"
  state: ready
  waits-on: "nothing"
  paths: ["api/**"]
- file: api/pack-first-bin-endpoint.md
  description: "pack/first-bin endpoint"
  state: idea
  waits-on: "nobody waiting - but v4-stable is waiting on this one, or on another candidate"
  paths: ["api/**"]
- file: api/packing-demo-bugs.md
  description: "Ten correctness and accessibility bugs in the shared packing demo component - most of them ship inside the image as well as on the demo site"
  state: ready
  waits-on: "nothing"
  paths: ["packages/binacle-net-ui/**", "api/src/Binacle.Net.UIModule/**", "sites/demo/**"]
- file: api/packing-only-image.md
  description: "a packing-only image variant, without the ServiceModule assemblies"
  state: idea
  waits-on: "nobody waiting"
  paths: ["api/**"]
- file: api/refresh-token-endpoint.md
  description: "add refresh-token support to ServiceModule"
  state: idea
  waits-on: "the maintainer - how far the ServiceModule is taken"
  paths: ["api/**"]
- file: api/schema-migrations.md
  description: "a schema-migration path for the ServiceModule store"
  state: idea
  waits-on: "the maintainer - how far the ServiceModule is taken"
  paths: ["api/**"]
- file: api/servicemodule-simplification.md
  description: "simplify ServiceModule - collapse the ceremony, keep the provider seam"
  state: idea
  waits-on: "the maintainer - how far the ServiceModule is taken. The other two api/ plans wait on the same answer"
  paths: ["api/**"]
- file: api/show-me-the-request.md
  description: "The packing demo shows the HTTP call it just made, against this host, ready to copy"
  state: idea
  waits-on: "nobody waiting"
  paths: ["api/src/Binacle.Net.UIModule/**", "packages/binacle-net-ui/**"]
- file: api/ui-clients-off-v3.md
  description: "Migrate the shipped UI clients off the v3 API"
  state: blocked
  waits-on: "the site half waits on api.binacle.net serving a v3.0.x image; the module half can start today"
  paths: ["api/**", "packages/binacle-net-ui/**"]
- file: api/uimodule-instance-presets.md
  description: "The instance page reads its presets over HTTP from the browser - move it to server-side state"
  state: ready
  waits-on: "nothing"
  paths: ["api/src/Binacle.Net.UIModule/**", "api/src/Binacle.Net.Kernel/**"]
- file: api/v4-stable.md
  description: "v4 - flip from experimental to stable"
  state: blocked
  waits-on: "an endpoint added to v4 that reshapes no existing contract - none has been chosen"
  paths: ["api/**"]
```

## CI/CD

```yaml
- file: ci-cd/ci-gates.md
  description: "CI - make the PR gate mean something"
  state: deferred
  waits-on: "gate 2 the all-modules integration tests, gate 3 the UI test harness - neither has anything to gate yet"
  paths: [".github/workflows/**"]
- file: ci-cd/dockerhub-overview.md
  description: "The Docker Hub repository page"
  state: ready
  waits-on: "nothing"
  paths: [".github/workflows/**"]
- file: ci-cd/dockerhub-tag-immutability.md
  description: "Turn on Docker Hub tag immutability, for release tags only"
  state: blocked
  waits-on: "a shipped release behind the rule"
  paths: [".github/workflows/**"]
- file: ci-cd/multi-arch-images.md
  description: "CI - publish the image for arm64 as well as amd64"
  state: blocked
  waits-on: "an answer to whether anyone runs this on ARM"
  paths: [".github/workflows/**"]
- file: ci-cd/test-leaves-reach-ci.md
  description: "Ten of the twenty-six test leaves run on no pipeline. Give every leaf a step, group the leaves for a laptop, and add a check so the two lists cannot drift again."
  state: ready
  waits-on: "nothing"
  paths: [".github/workflows/**", "tooling/**"]
- file: ci-cd/workflow-restructure.md
  description: "CI - what is left after the workflow restructure landed, and the gap the next workflows session inherits"
  state: blocked
  waits-on: "branch protection pointing at Pull Request / Gate - state chosen by an agent, strike it if wrong"
  paths: [".github/**"]
```

## Lib

```yaml
- file: lib/benchmark-ledger.md
  description: "Refresh the curated lib benchmark ledger"
  state: deferred
  waits-on: "someone needing the numbers"
  paths: ["lib/**"]
- file: lib/parallel-processors-decision.md
  description: "Decide what happens to the three `Parallel*` processors"
  state: ready
  waits-on: "nothing"
  paths: ["lib/**"]
```

## Shared

```yaml
- file: shared/extend-shared-models.md
  description: "take the shared model leaf further"
  state: idea
  waits-on: "nobody waiting"
  paths: ["shared/**"]
- file: shared/testskernel-data-extraction.md
  description: "TestsKernel - grow the shared fixture cases"
  state: ready
  waits-on: "nothing"
  paths: ["shared/**"]
```

## Sites

```yaml
- file: sites/code-blocks-and-wide-tables.md
  description: "Two framework defaults nobody overrode - code samples on the docs site render in the body sans-serif, and wide tables are clipped rather than scrolled"
  state: proposed
  waits-on: "a yes or a no from the maintainer"
  paths: ["sites/docs/**"]
- file: sites/docs-client-generation.md
  description: "A docs page with copy-paste commands that generate a client from the published OpenAPI spec"
  state: ready
  waits-on: "nothing"
  paths: ["sites/docs/**"]
- file: sites/docs-v3-deploy.md
  description: "The v3.0.x corrections the docs site needs and the deploy that publishes them - five pages describing configuration the image no longer ships, a worked example quoting a deleted tag, and two stale OpenAPI copies"
  state: ready
  waits-on: "the v3.0.0 tag - three of its items quote the released tag, its date or its digest"
  paths: ["sites/docs/**"]
```

## Tooling

```yaml
- file: tooling/scripts-to-just-recipes.md
  description: "Convert the last `tooling/*.sh` scripts to `just` recipes"
  state: ready
  waits-on: "nothing"
  paths: ["tooling/**"]
- file: tooling/typescript-linting.md
  description: "No linter exists for any TypeScript or JavaScript in the repository - decide whether one lands, and what it gates"
  state: idea
  waits-on: "a yes or a no, and it is not urgent - nothing is broken by the gap"
  paths: ["packages/**", "sites/**", "api/src/Binacle.Net.UIModule/**", "vipaq/packages/**"]
```
