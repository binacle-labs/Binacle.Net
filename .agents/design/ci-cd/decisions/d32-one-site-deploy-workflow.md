---
id: ci-cd/decisions/D32
description: the three site deploys are one workflow with a site chosen at dispatch
status: decided
verified: 2026-10-08
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

**What it costs.** The marker tag is `<site>-<run id>`, so it reads `demo-18234567890` where each site used to
count on its own. A tag maps a site to the commit that is live, which it still does; only the short per-site
numbering is gone. The concurrency group carries the site,
`${{ github.workflow }}-${{ inputs.site }}`, so one site still queues behind itself and two sites still deploy
side by side, and `run-name` names the site so the run list stays readable.

**Why the run id and not the run number.** The tag was first `<site>-${{ github.run_number }}`, on the
reading that the number would carry on past the old workflows' tags. It does not: a new workflow file counts
from 1, and the old files had already made `docs-1` to `docs-10`, `demo-1` to `demo-5` and `www-1` to `www-6`.
The second dispatch, `demo` on 2026-10-08, deployed and then failed at the tag with `Reference already exists`
on `demo-2`. `github.run_id` is unique across the repository and never repeats. The maintainer, 2026-10-08,
picking it from four options: "go with 1".

**Proved 2026-10-08.** The third dispatch, `demo` from `main` on `17d4d78e`, was green on all three jobs, and
`docs` followed green. The first had failed at `Deploy to Cloudflare`: Dependabot had moved
`cloudflare/wrangler-action` to v4.1.1, a release Cloudflare marks broken because its tag has no `dist/`. The pin
is v4.1.3. The second deployed and failed at the tag, as above.

