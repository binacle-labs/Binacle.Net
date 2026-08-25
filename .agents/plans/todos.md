---
description: TODOs
state: ready
waits-on: "nothing"
---

# TODOs

One-liners with a known answer - the kind of thing that costs more to plan than to do. Anything here that grows
a decision or a set of sub-steps gets its own plan file instead.

---

## CI

**The OpenAPI lint moved out on 2026-08-17** and into the v3.0.0 release plan, which owns it whole along with
the `--fail-severity=warn` flag it needs. It had been written in both files and the two copies had already
started to differ.

## Comments

Found in a 2026-08-12 sweep of every comment outside `.agents`. The layer is in good shape overall - the
`just` modules, the workflows and the sample compose files carry "why" at the point of use, which is where it
has to stay. These two are the exceptions.

- `Dockerfile`, the line above `COPY ["artifacts/binacle-net", "."]`, reads "from the 'build' stage". **There is no
  build stage** - the publish happens outside the file, in `just build publish`. Say that instead, and that
  the path is hardcoded here and allowlisted in `.dockerignore`, so publishing elsewhere builds an empty image.

- **`Dockerfile:8` names two places for the description caption; there are three.** It is pinned in the
  `Dockerfile` label, in `release-docker-image.yml`, and in `api/src/Binacle.Net.Kernel/Metadata.cs`, which is
  what reaches Swagger UI, Scalar and both published OpenAPI documents. Change two of the three and the image
  label and the API document disagree, silently. **One comment**, and do it while the reason is in front of
  someone - the next person to change that string will read the comment and believe it.

## Ruby gems

- **No workflow runs the gem leaves.** The six `ruby-*-unit` leaves joined `just test all` on 24 Aug 2026, so
  a local run covers them, but the PR gate names its steps and none of them is ruby. **Whether they go on the
  gate is a separate call.**

- **Rubocop has never been run.** `ruby/.rubocop.yml` exists, rubocop is not in `ruby/Gemfile` and no recipe
  calls it. It lands red before it lands green, which is why it is not wired to anything yet.

## Sites

- **Three curly apostrophes and two lines of old-register prose on the docs site.** Found 2026-08-25 while
  the v3.0.x pages were being corrected, and left alone because they were outside that job.
  `configuration/ui-module/index.md:13-14` reads "provides a user-friendly interface" and "the system's
  capabilities" and carries a curly apostrophe; `configuration/index.md` still calls the UI module "packing
  demos and protocol decoding"; the docs landing has a curly apostrophe in "Binacle.Net's packing solutions".
  **The apostrophes break the plain-ASCII rule for user-facing text.** A site session.

- **The three sites still default to dark.** `default_theme: "dark"` in `sites/www/_config.yml`,
  `sites/docs/_config.yml` and `sites/demo/_config.yml`. **Decided 2026-08-25: every surface follows the
  machine**, and the UI module already does. Three lines to `"system"`. **A site session, not a coding
  session.**

## ServiceModule

- `api/src/Binacle.Net.ServiceModule/Services/ApiUsageRateLimitingPolicy.cs:34`
  Review JSON config for default rate limit policies (anonymous, subscription tiers).

- `api/src/Binacle.Net.ServiceModule/v0/Endpoints/AccountBindingResult.cs:57`
  The "no request body" path returns a raw `ProblemDetails`. Should be a proper typed response.
