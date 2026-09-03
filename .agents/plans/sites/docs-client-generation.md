---
description: A docs page with copy-paste commands that generate a client from the published OpenAPI spec
state: ready
waits-on: "one docs deploy. The page is written and builds - 2026-09-04. Every box but the live one passes"
paths:
  - "sites/docs/**"
---

# The client generation page

A site session in `sites/docs/`.

**Agreed by the maintainer, 2026-08-27.** The page gets written. **Where it goes was settled by the site on
2026-09-02** - see below. Nothing is open.

**The spec is published and nothing on the site links to it.** Verified: the per-version
`swagger/<api>.json` under `/version/<version>/` returns 200, and no page mentions it. **A client-generation
page is the payoff for publishing a spec at all.**

A short page with copy-paste commands - `hey-api` for TypeScript, `kiota` for C# - turns "there is a spec"
into "here is your client in thirty seconds".

## Where it lives - answered by the site, 2026-09-02

**This plan was blocked on a premise that is false.** It said every page on the docs site sits under a version
folder. **The `common_pages` collection is a non-versioned home and it already exists**, with
`permalink: /:path/` in `sites/docs/_config.yml:44`. Seven pages are in it, including `core-concepts.md` and
`quick-start.md`.

**Measured on the live host:** `docs.binacle.net/core-concepts/` answers 200 and
`docs.binacle.net/version/v3.0.x/core-concepts/` answers 404. The page is served once, outside the version
tree, exactly as the first row of the table below wanted.

**And the two costs that row named are already paid.** The nav is front matter, not a hand-kept list - a
`nav:` block with `order:` and `icon:` is all `core-concepts.md` carries. The sitemap is generated: the
`pages` file in the `sitemaps:` block at `sites/docs/_config.yml:276` already includes the `common_pages`
collection, so a new page in it is listed without touching the config.

**So there is nothing to decide.** The table below is kept only as the record of what was weighed, and the
first row is the one the site already implements:

| Where | What it costs |
|---|---|
| **A home outside the version tree** - a top-level page beside the version folders | The version picker and both sitemaps are hand-maintained and every entry in them today is versioned. A page with no version is a new case for both, and for whatever navigation include builds the picker. It is the only option where the page exists once. |
| **A copy under every version** | Nothing structural changes - it is the shape the site already has. The cost is that four near-identical pages drift apart, and each new version adds a fifth copy somebody has to remember to write. The plan already rejects that shape for the version string inside the page; this is the same argument one level up. |
| **Somewhere else on the site** - the landing page, or a section that is not the versioned docs | Puts a developer task on a page that is not the docs, and readers looking for it will look under the version they are on. Cheapest to build, hardest to find. |

**Row 1 won and it cost nothing**, because the two prices it was marked up with - a hand-kept picker and a
hand-kept sitemap - are not what the site does.

## Two things it has to get right

- **Write the version as a placeholder the reader substitutes.** The commands work against every published
  version. Four near-identical pages drift apart; one does not.
- **Say that v4 is experimental** and its contracts are expected to move. Someone generating a client off
  `v4.json` should know before they build on it.

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

**What it takes: one docs session and one deploy.** One new file, one `nav:` block, two commands run in a
scratch directory before anything is pasted. No config edit, no decision left. `sites/docs/` is published, so
it needs the site grant - `rules/never-edit-published-sites.md`.

- [x] **Where the page lives is settled - 2026-09-02.** The `common_pages` collection, `permalink: /:path/`,
      already serving seven non-versioned pages. Measured on the live host, not read off the config.
- [x] **The page exists as a common page - 2026-09-04.** `generate-a-client.md`, `nav: order 4, icon 🧰`,
      matching `core-concepts.md`'s shape.
- [x] **It is in the nav and the generated sitemap, with no config edited - 2026-09-04.**
      `git diff sites/docs/_config.yml` is empty and `artifacts/docs/sitemap/pages.xml` names the path.
- [x] **Both commands were run against the published spec - 2026-09-04**, not against `artifacts/openapi/`.
      Both generated, both built, and both printed `FullyPacked 3` against a live API.
- [x] **The page says v4 is experimental**, in a `.block-warning` using the site's existing wording.
- [ ] The page is live.
      `curl -s -o /dev/null -w '%{http_code}' https://docs.binacle.net/generate-a-client/` returns 200. The
      docs deploy is `workflow_dispatch` only, so nothing fails if this is skipped.
