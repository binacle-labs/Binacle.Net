---
description: Manifest of every file under .agents/plans, grouped by area. Regenerate with just agents all.
---

# Agent Plans Index

Every plan in `.agents/plans/` (recursive), grouped by area. Plans are work not yet done — read the one
you need, and trim or delete it once the work lands. `state:` and `waits-on:` say where each one stands;
[README.md](README.md) defines the five states.

## General

```yaml
- file: architecture-checks.md
  description: "Generate the repo's dependency graph, draw it, and lint it with a small ruleset - plus three greps over api/src, and the one boundary violation to fix before building the check that watches it"
  state: idea
  waits-on: "v3.0.0 at the earliest, and a maybe even then"
  paths: ["**/*.csproj", "tooling/**"]
- file: comment-lint.md
  description: "A check that nothing outside the agent guidance directory points a reader into it."
  state: idea
  waits-on: "v3 at the earliest, and a maybe even then"
- file: image-base-slimming.md
  description: "Harden and slim the base image"
  state: idea
  waits-on: "nobody waiting - not in the near future"
- file: ruby-gem-coverage.md
  description: "The ten Ruby gems produce no coverage, so they are absent from the coverage table and from Sonar. Add simplecov per gem, without breaking the rule that a gem must be droppable into an unrelated site."
  state: ready
  waits-on: "nothing"
  paths: ["ruby/**", "tooling/**"]
- file: sonar-issue-triage.md
  description: "Sonar - re-read the open findings against the current project, then the one-line exclusion fix and the two rewrites behind it"
  state: blocked
  waits-on: "a re-read against SonarCloud - every count in this file predates the current project and the maintainer wants it revisited before the v3.0.0 tag"
  paths: ["tooling/sonar-analysis.xml"]
- file: testing-techniques.md
  description: "The testing techniques this repo does not use - property-based, fuzzing, load, mutation - what each buys, and the four yes-or-no answers"
  state: idea
  waits-on: "nobody waiting - future"
- file: todos.md
  description: "One-liners with a known answer - eight of them, across the image, the sites and the shared UI package"
  state: ready
  waits-on: "nothing"
- file: ui-test-harness.md
  description: "A test harness for the UI"
  state: blocked
  waits-on: "a Sonar run - state and blocker chosen by an agent, strike them if wrong"
- file: unwatched-gaps.md
  description: "Four repository gaps nothing watches - a pull request that runs no job, a generated copy on no drift check, a v4 caller inside the image, and an error page that answers 200"
  state: proposed
  waits-on: "a yes or a no per gap. State chosen by an agent to make the file legible - strike it if it is wrong."
  paths: ["sites/**", ".github/workflows/**", "tooling/**", "api/src/Binacle.Net.UIModule/**"]
```

## API

```yaml
- file: api/integration-test-additions.md
  description: "Integration tests: cover what the harness cannot see today"
  state: ready
  waits-on: "nothing - phase 1 is agreed and can start"
  paths: ["api/**"]
- file: api/nullable-enum-converter.md
  description: "The Kernel's nullable-enum converter accepts any string and returns default rather than refusing it - a leniency nobody chose, with one dead throw sitting on top of it"
  state: proposed
  waits-on: "a yes or a no on whether an unknown enum string is refused at deserialization or left to validation"
  paths: ["api/src/Binacle.Net.Kernel/Serialization/**"]
- file: api/pack-first-bin-endpoint.md
  description: "pack/first-bin endpoint"
  state: deferred
  waits-on: "v3.0.0. The v4 stable flip waits on this endpoint or on another candidate"
  paths: ["api/**"]
- file: api/packing-demo-bugs.md
  description: "Two open bugs in the shared packing demo - a partial result names no unfitted items, and the submit button can stick disabled on a page with no visualizer"
  state: deferred
  waits-on: "the v3.0.0 tag - the maintainer deferred it on 2026-08-27"
  paths: ["packages/binacle-net-ui/**", "api/src/Binacle.Net.UIModule/**", "sites/demo/**"]
- file: api/packing-only-image.md
  description: "a packing-only image variant, without the ServiceModule assemblies"
  state: idea
  waits-on: "nobody waiting - far future"
  paths: ["api/**"]
- file: api/servicemodule.md
  description: "How far ServiceModule is taken - one decision, and the three pieces of work behind it - collapsing the layering, a schema-migration path, and refresh tokens"
  state: deferred
  waits-on: "the maintainer. He said on 2026-08-27 that ServiceModule work is taken as one piece, not row by row"
  paths: ["api/src/Binacle.Net.ServiceModule/**", "api/src/Binacle.Net.ServiceModule.Domain/**", "api/src/Binacle.Net.ServiceModule.Infrastructure/**"]
- file: api/show-me-the-request.md
  description: "The packing demo shows the HTTP call it just made, against this host, ready to copy"
  state: deferred
  waits-on: "v3"
  paths: ["api/src/Binacle.Net.UIModule/**", "packages/binacle-net-ui/**"]
- file: api/ui-clients-off-v3.md
  description: "Migrate the shipped UI clients off the v3 API"
  state: deferred
  waits-on: "v3. The site half additionally needs api.binacle.net serving a v3.0.x image; the module half is only waiting on v3"
  paths: ["api/**", "packages/binacle-net-ui/**"]
- file: api/uimodule-instance-presets.md
  description: "The instance page reads its presets over HTTP from the browser - move it to server-side state"
  state: idea
  waits-on: "v3, and the maintainer expanding the idea"
  paths: ["api/src/Binacle.Net.UIModule/**", "api/src/Binacle.Net.Kernel/**"]
- file: api/v4-stable.md
  description: "v4 - flip from experimental to stable"
  state: deferred
  waits-on: "v3.0.0, and an endpoint added to v4 that reshapes no existing contract - none has been chosen"
  paths: ["api/**"]
```

## CI/CD

```yaml
- file: ci-cd/ci-gates.md
  description: "Two gates the pull request still does not have - the integration suites against the module set the image ships, and a Sonar verdict without anyone pressing a button"
  state: idea
  waits-on: "nothing to gate yet. The maintainer called both a future idea on 2026-08-27"
  paths: [".github/workflows/**"]
- file: ci-cd/dockerhub-overview.md
  description: "The Docker Hub repository page - the quick start example is the last thing left, and it quotes a response from a tag that was deleted"
  state: ready
  waits-on: "nothing"
  paths: [".github/workflows/**"]
- file: ci-cd/dockerhub-tag-immutability.md
  description: "Turn on Docker Hub tag immutability, for release tags only"
  state: proposed
  waits-on: "the maintainer - he is rethinking it as of 27 Aug 2026"
  paths: [".github/workflows/**"]
- file: ci-cd/multi-arch-images.md
  description: "CI - publish the image for arm64 as well as amd64"
  state: idea
  waits-on: "nobody waiting - not in the near future"
  paths: [".github/workflows/**"]
- file: ci-cd/test-leaves-reach-ci.md
  description: "Ten of the twenty-six test leaves run on no pipeline, and rubocop has never been run at all. Give every leaf a step, group the leaves for a laptop, get rubocop running once, and add a check so the two lists cannot drift again."
  state: blocked
  waits-on: "the maintainer's instructions on how - he said on 2026-08-27 that he wants this done and to wait for them"
  paths: [".github/workflows/**", "tooling/**"]
- file: ci-cd/workflow-restructure.md
  description: "CI - what is left after the workflow restructure landed, and the gap the next workflows session inherits"
  state: blocked
  waits-on: "the maintainer - how to make the branch-protection change. The change itself is agreed"
  paths: [".github/**"]
```

## Lib

```yaml
- file: lib/parallel-processors-decision.md
  description: "Decide what happens to the three `Parallel*` processors"
  state: idea
  waits-on: "v3, and more time - it is an idea"
  paths: ["lib/**"]
```

## Shared

```yaml
- file: shared/fixture-fill-per-algorithm.md
  description: "The scenario fixtures record which algorithms succeed, not how well - so a sample that exists because one algorithm packs better cannot say so"
  state: idea
  waits-on: "nobody waiting - wanted sometime, not in the near future"
  paths: ["shared/data/**", "shared/test/Binacle.TestsKernel/**"]
- file: shared/testskernel-data-extraction.md
  description: "TestsKernel - grow the shared fixture cases"
  state: deferred
  waits-on: "the maintainer - he said \\"not yet\\" on 2026-08-27; it revives when he says the fixture cases are worth growing"
  paths: ["shared/**"]
```

## Sites

```yaml
- file: sites/code-blocks-and-wide-tables.md
  description: "Two framework defaults on the docs site - code samples had no named mono face (fixed), and wide tables are still clipped rather than scrolled"
  state: proposed
  waits-on: "a yes or no on wrapping each table in a scroll box - the only route left. State picked to make the file legible; strike it if it is wrong."
  paths: ["sites/docs/**"]
- file: sites/docs-client-generation.md
  description: "A docs page with copy-paste commands that generate a client from the published OpenAPI spec"
  state: blocked
  waits-on: "where a page that is not version-specific lives on the docs site - the maintainer said yes to the page on 2026-08-27 and that placement is the one thing still open"
  paths: ["sites/docs/**"]
- file: sites/docs-site-plain-ascii.md
  description: "The docs site's punctuation is inconsistent - en dashes used as list separators in four v3.0.x pages, two curly apostrophes, and a kramdown setting that rewrites straight quotes to curly at build"
  state: ready
  waits-on: "nothing"
  paths: ["sites/docs/**"]
- file: sites/docs-v3-deploy.md
  description: "What the v3.0.x docs pages still need - a worked example quoting a deleted tag, the release date and link - plus the live-site checks nothing else watches"
  state: blocked
  waits-on: "the v3.0.0 tag - the worked example quotes real output from the released image, and the release notes need its date and its release link"
  paths: ["sites/docs/**"]
```

## Tooling

```yaml
- file: tooling/typescript-linting.md
  description: "Answered no - linting is one decision for the whole repository, not a TypeScript one. Every language gets the same treatment or none does."
  state: deferred
  waits-on: "a decision to lint every language in this repository to the same standard - TypeScript alone is not the question"
  paths: ["packages/**", "sites/**", "api/src/Binacle.Net.UIModule/**", "vipaq/packages/**"]
- file: tooling/where-benchmark-results-live.md
  description: "One unanswered question - where benchmark and performance results are persisted and in what shape - and the two mechanical jobs waiting behind it"
  state: blocked
  waits-on: "the maintainer answering where benchmark and performance results live and what shape they take. They sit in results/ today and he does not want them there"
  paths: ["tooling/**", "results/**", "lib/**"]
```
