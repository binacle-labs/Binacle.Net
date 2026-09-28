---
id: ci-cd/decisions/D32
description: the three site deploys are one workflow with a site chosen at dispatch
status: decided
verified: 2026-09-29
check: D32 against deploy-site.yml, whose dispatch must take a site choice of docs, demo and www and whose concurrency group must carry inputs.site
paths:
  - ".github/workflows/deploy-site.yml"
---

# D32 — the three site deploys are one workflow with a site chosen at dispatch

**Decided (the maintainer, 2026-09-12):** "do it" - to the offer to replace the shared workflow and its three
callers with one `deploy-site.yml` taking a `choice` input.

`deploy-site.yml` holds the three-job chain - site tests, build-check-deploy, tag - and its `workflow_dispatch`
takes one `choice` input, `site`, from `docs`, `demo` and `www`. `deploy-docs-site.yml`, `deploy-demo-site.yml` and
`deploy-www-site.yml` are gone. `$ci-cd/decisions/D17` is untouched: it is still by hand and never on a push.

**Why one input.** The three files were diffed on 2026-09-11 and everything that differed came from the
slug: the environment `binacle-net-<slug>`, the URL `https://<slug>.binacle.net`, the source `sites/<slug>`,
the wrangler config `tooling/cloudflare/<slug>.wrangler.jsonc`, the marker tag `<slug>-<run>`, the link check.
Three copies of that chain could not stay in step, and two of them had never run.

**Why a choice input and not a called workflow.** The first draft was `shared-deploy-site.yml` plus three
fifteen-line callers. The maintainer asked for the enum instead, and it is simpler on every count: one file
rather than four, no `workflow_call`, no secrets handed across by name, no permissions cap on the caller, and
none of the environment-secret precedence rules a called job with `environment:` brings.

**What it costs.** `github.run_number` counts per workflow, so the marker tags share one sequence -
`docs-40`, `demo-41`, `www-42` - where each site used to count on its own. A tag maps a site to the commit that
is live, which it still does; only the per-site numbering is gone. The concurrency group carries the site,
`${{ github.workflow }}-${{ inputs.site }}`, so one site still queues behind itself and two sites still deploy
side by side, and `run-name` names the site so the run list stays readable.

**What is unproved.** No site has deployed through this file - checked 2026-09-29: it is not on `main`, and
GitHub still lists only the three old deploy workflows, last run 2026-09-03. A `workflow_dispatch` workflow can
be dispatched only once it is on the default branch, so the first deploy through it comes after the merge to
`main`.
