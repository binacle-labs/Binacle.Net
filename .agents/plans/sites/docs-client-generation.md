---
description: A docs page with copy-paste commands that generate a client from the published OpenAPI spec
state: ready
waits-on: "nothing"
paths:
  - "sites/docs/**"
---

# The client generation page

A site session in `sites/docs/`.

**The spec is published and nothing on the site links to it.** Verified: the per-version
`swagger/<api>.json` under `/version/<version>/` returns 200, and no page mentions it. **A client-generation
page is the payoff for publishing a spec at all.**

A short page with copy-paste commands - `hey-api` for TypeScript, `kiota` for C# - turns "there is a spec"
into "here is your client in thirty seconds".

## Three things it has to get right

- **Write the version as a placeholder the reader substitutes.** The commands work against every published
  version. Four near-identical pages drift apart; one does not.
- **Say that v4 is experimental** and its contracts are expected to move. Someone generating a client off
  `v4.json` should know before they build on it.
- **Decide where a page that is not version-specific lives.** Every page today sits under a version folder.
  **This is the only genuinely open question in the item**, and it is a structural call about the site - so
  settle it before writing the page, not during.

## What the generators actually do

From a real spike, and worth having on the page or near it:

- **Grouping comes from tags, method names from `operationId`.** Every generator groups operations by tag
  (`Pack` / `Fit` / `Presets` here). So the `client.pack....()` shape is driven by tags, which already exist.
- **v4 uses dot-notation operationIds** (`pack.customBin`), which reads as `client.pack.customBin()` with
  nesting-aware generators. The free `openapi-generator` sanitizes the dot.
- **Kiota ignores operationIds** and builds method names from the URL path
  (`client.Api.V4.Pack.Bin.PostAsync`). Cleaner output, no Java, but it does not use the naming.
- **Tool picks:** TypeScript - `hey-api`. C# - Kiota, free and clean. `openapi-generator` is the free
  multi-language option but needs Java. NSwag works but emits one very verbose file.

## No SDKs

**The deliverable is a published spec plus a generation guide, not shipped packages.** That is a recorded
decision and it is not this plan's to change.

## What will bite

**Do not paste a command you have not run.** A generation command that fails is worse than no page, because
the reader concludes the spec is broken rather than the docs.

**The version picker and the sitemaps are hand-maintained.** A new page needs a sitemap entry.

**The published spec is what the page must be proven against**, not `artifacts/openapi/`. The two can differ -
the site's copies are hand-carried.

## Done when

- [ ] One page exists, in the navigation and in the sitemap.
      `grep -rn 'client' sites/docs/_data/*.yml sites/docs/**/sitemap*` finds it in both.
- [ ] Both commands have been run against a published spec and their real output is what the page shows.
      **By eye.** Run each one in a scratch directory before pasting it.
