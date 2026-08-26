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

## Tests

- **One flaky test, about 1 run in 19.** `packages/binacle-net-ui/tests/components/packingDemo.test.ts`,
  *"the new items fit the new largest bin"*. It randomizes once, then asserts every item fits the
  largest-by-volume bin side for side with no rotation. **That was a property of the old random roll**, where
  items were sized against the largest bin. The demo now carries hand-picked samples and does not have it:
  `07-tall-items` has bins 20x20x60 and 40x40x30 and an item 8x8x55, so the largest by volume is 40x40x30 and
  55 beats every side of it. **The sample is right and the assertion is wrong.** It will fail in CI one day on
  an unrelated commit. Found 2026-08-26.

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

- **The demo site's packing page still carries the pre-rewrite copy.** Found 2026-08-26. The image's version
  renders `AppletsService.cs`'s rewritten description; `sites/demo/pages/packing.html` hardcodes the old
  register in two places - the body `<p>` and the front-matter `excerpt:` - both reading "An interactive tool
  that lets you test different packing algorithms". **Two surfaces describe the same tool differently, and the
  site has the older one.** The front-matter `description:` is a meta description and is meant to differ; the
  body copy is not. A site session.

- **The three sites still default to dark.** `default_theme: "dark"` in `sites/www/_config.yml`,
  `sites/docs/_config.yml` and `sites/demo/_config.yml`. **Decided 2026-08-25: every surface follows the
  machine**, and the UI module already does. Three lines to `"system"`. **A site session, not a coding
  session.**

## ServiceModule

- `api/src/Binacle.Net.ServiceModule/Services/ApiUsageRateLimitingPolicy.cs:34`
  Review JSON config for default rate limit policies (anonymous, subscription tiers).

- `api/src/Binacle.Net.ServiceModule/v0/Endpoints/AccountBindingResult.cs:57`
  The "no request body" path returns a raw `ProblemDetails`. Should be a proper typed response.
