---
description: Manifest of every file under .agents/design, grouped by area. Regenerate with just agents all.
---

# Agent Design Index

Every design record in `.agents/design/`, grouped by area. The settled design behind the docs — the
decisions (why) and the findings (measured evidence). Permanent and citable; read the one you need.

## General

```yaml
- file: decisions.md
  description: "General decisions ledger — why the repository moved to the binacle-labs organization, what moved with it and what deliberately did not, the three signing identity bands, the rule that a version is named only where the version is the fact, why the licence file keeps its name, why only the current docs version is indexable and old ones are bug-fix only, and how the agent reference layer is kept honest against the code."
  paths: ["NOTICE", "README.md", "SECURITY.md", "CHANGELOG.md", "Dockerfile", "CONTENT-TERMS.md", "sites/docs/**"]
```

## API

```yaml
- file: api/decisions.md
  description: "API decisions ledger — why a module-off document carries no `429` and what guarantees it, what the generated documents are a document of, and why the API sends no HSTS header."
  paths: ["api/**"]
```

## CI/CD

```yaml
- file: ci-cd/decisions.md
  description: "CI/CD decisions ledger — why the release pipeline is tag-triggered, stages on GHCR and copies to Docker Hub by digest, why the prerelease guard is metadata-action's rather than a job-level skip, why the notes come from CHANGELOG.md, the pinning rules, why lychee is a pinned binary rather than its own action, and the open questions about the PR gate and supply-chain attestation."
  paths: [".github/workflows/**"]
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
  description: "ViPaq design history — superseded throwaway-prototype measurements (2026-07-05) that informed the locked decisions. Reference only, not current truth."
```
