---
description: Manifest of every file under .agents/design, grouped by area. Regenerate with just agents all.
---

# Agent Design Index

Every design record in `.agents/design/`, grouped by area. The settled design behind the docs — the
decisions (why) and the findings (measured evidence). Permanent and citable; read the one you need.

## General

```yaml
- file: architecture-graph.md
  description: "Why the repo's dependency graph is generated rather than declared, what no generator can see, what an InternalsVisibleTo grant means for the graph, and the heavier architecture tools that were surveyed and not taken."
  paths: ["**/*.csproj", "Directory.Packages.props"]
- file: decisions.md
  description: "General decisions ledger — why the repository moved to the binacle-labs organization, what moved with it and what deliberately did not, the three signing identity bands, the rule that a version is named only where the version is the fact, why the licence file keeps its name, why only the current docs version is indexable and old ones are bug-fix only, how the agent reference layer is kept honest against the code, and what was deliberately not reduced to a shared model."
  paths: ["NOTICE", "README.md", "SECURITY.md", "CHANGELOG.md", "Dockerfile", "CONTENT-TERMS.md", "sites/docs/**", "shared/src/Binacle.Packing/**"]
- file: sonar-accepted-findings.md
  description: "The Sonar findings answered with a reason rather than a code change, why each one stands, and why this register has to live in the repository rather than in the SonarCloud UI."
  paths: ["lib/**", "packages/cookies/**", "api/src/Binacle.Net.Kernel/OpenApi/**"]
```

## API

```yaml
- file: api/decisions.md
  description: "API decisions ledger — why a module-off document carries no `429` and what guarantees it, what the generated documents are a document of, why the API sends no HSTS header, why the DiagnosticsModule alone is registered unconditionally, and why an unknown enum answers with the same error a missing one does."
  paths: ["api/**"]
```

## CI/CD

```yaml
- file: ci-cd/decisions.md
  description: "CI/CD decisions ledger — why a release is dispatched with a version and tagged last, why the pipeline stages on GHCR and copies to Docker Hub by digest, why the prerelease guard is metadata-action's rather than a job-level skip, why the notes come from CHANGELOG.md, the pinning rules, why lychee is a pinned binary rather than its own action, why the test suite is split in two by what ships, why the gem sources need a built project and what a slnx project type decides, why a workflow step calls a just recipe rather than inlining shell, how CodeQL is configured, what `just image verify` checks and in what order, and the open questions about the PR gate and supply-chain attestation."
  paths: [".github/workflows/**", "tooling/ci/**", "tooling/image.just"]
- file: ci-cd/github-surface.md
  description: "What GitHub offers a repository, what this one uses, and the ten Actions gotchas that fail quietly"
  paths: [".github/**"]
```

## Lib

```yaml
- file: lib/decisions.md
  description: "Lib decisions ledger — why Algorithm.Best races a different set per path, where the packing vocabulary lives, why there are two tests kernels, and the open parallelization question."
  paths: ["lib/**"]
- file: lib/findings.md
  description: "Lib findings — the measured evidence (algorithm racing cost, parallel racing gain) behind the decisions."
  paths: ["lib/**"]
```

## Ruby

```yaml
- file: ruby/decisions.md
  description: "Ruby gem decisions ledger — why one computation feeds every gem, the one key whose generator order is load-bearing, how a gem's config names are chosen, what the gem name says about portability, why a portable gem may not name the product, and what decides a tag against a generator."
  paths: ["ruby/**"]
```

## Sites

```yaml
- file: sites/decisions.md
  description: "Decisions behind the demo and documentation sites — the link-preview pair, title order, what the demo host calls itself, why the demo has no collections, and the two footer calls. What a review would otherwise re-litigate."
  paths: ["sites/demo/**", "sites/docs/**"]
- file: sites/demo-and-image-boundary.md
  description: "The two demo tools ship on two hosts from one implementation - what is shared, what diverges freely, and the test that keeps the line where it is"
  paths: ["sites/demo/**", "packages/binacle-net-ui/**", "api/src/Binacle.Net.UIModule/**"]
- file: sites/docs-and-demo.md
  description: "Why the docs and demo templates are shaped this way - the beercss and Alpine traps, the contrast measurements behind the component overrides, and the asset budget."
  paths: ["sites/demo/**", "sites/docs/**"]
- file: sites/packing-demo-set.md
  description: "Why the packing demo sizes its items against the largest bin, and how sizingBin and addBin relate - the reasoning behind the numbers a visitor arrives to"
  paths: ["packages/binacle-net-ui/**"]
- file: sites/webmanifest.md
  description: "Why the three sites ship a web app manifest rather than dropping the two android icons, where its colours come from, and why the UI module gets neither."
  paths: ["sites/**", "gulpfile.js"]
- file: sites/www.md
  description: "Why the www site's templates are shaped the way they are - the traps that bite silently, and the constraints a rewrite would break without noticing."
  paths: ["sites/www/**"]
```

## ViPaq

```yaml
- file: vipaq/decisions.md
  description: "ViPaq decisions ledger — the locked decisions and their reasons, plus the open questions."
  paths: ["vipaq/**"]
- file: vipaq/findings.md
  description: "ViPaq findings — the measured evidence (base64 size, encode/decode time) behind the decisions."
  paths: ["vipaq/**"]
- file: vipaq/history.md
  description: "ViPaq design history — superseded throwaway-prototype measurements (2026-07-05), the earlier framings of decisions that were later amended or reversed, and where the test files the v2 rebuild deleted ended up. Reference only, not current truth."
```
