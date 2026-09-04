---
description: Release - Binacle.Net v3.1.0. The release that makes v4 the version the product itself uses, plus the two gaps the pipeline does not cover.
---

# Release - Binacle.Net v3.1.0

**Status:** scope named by the maintainer on 2026-09-05. **Nothing started.** No branch, no beta, no date.

**What this release is.** v4 ships experimental, the shipped UI calls v3, and the only v4 call anywhere in the
image is a browser fetch that one of these rows deletes. So the product uses the version it tells people is
the older one. **This release makes v4 the version the product itself uses, then flips it stable.**

**It is a minor version, so nothing here reshapes an existing contract.** The packing-only image split does
not belong in it for that reason - removing the ServiceModule assemblies changes what a self-hoster pulls.

**This file points; it does not hold work it can point at.** Each row names the plan and the slice this
release takes. The plan holds the work, the traps and the research. **This file holds the order and the
dependencies.** Where a row has no plan, it is release paperwork and lives here because nothing else owns it.

**The scope is the maintainer's.** Rows marked *(chosen by an agent)* are orderings or readiness calls written
to make the file legible - strike any of them.

---

## Before anything else

**`## [Unreleased]` has to go back into `CHANGELOG.md`.** The v3.0.0 release renamed it. Nothing can be logged
for this version until it is there, and `just changelog check Unreleased` fails on a missing section and on an
empty one alike, so it goes in with the first real entry rather than on its own.

**The staging repository goes first, or it does nothing for this release.** `plans/ci-cd/prerelease-staging-repository.md`.
The whole point is that this version's own prereleases and branch builds never reach the repository users pull
from. **Land it after the first `3.1.0-beta` and it has missed the thing it exists for.** *(ordering chosen by
an agent)*

Its two open questions have to be answered before it is built: whether it replaces the GHCR staging step or
sits after it, and what the published verify command covers - **a branch build signs under that branch's ref
and fails the command printed in `SECURITY.md`**, which is exactly why betas 1 to 4 read as tampered.

---

## The v4 chain, in this order

**The order is a dependency, not a preference.** v4 cannot be flipped stable until an endpoint has been added
to it that reshapes no existing contract, and only one has been costed.

| # | Plan | The slice this release takes |
|---|---|---|
| 1 | `plans/api/pack-first-bin-endpoint.md` | the endpoint, on v4. **One decision first: selection-only or short-circuit.** Only the second earns the name and it needs a new `IBinProcessor` - `Process` runs all bins today |
| 2 | `plans/api/ui-clients-off-v3.md` | the whole plan. One line in `packages/binacle-net-ui/src/core/packingDemo.ts`, both hosts rebuilt |
| 3 | `plans/api/uimodule-instance-presets.md` | the whole plan. It deletes `_js/instance.js` and its webpack entry, and removes the only v4 call shipped in the image |
| 4 | `plans/api/v4-stable.md` | the flip. **Row 1 is what makes this legal** |

**One answer unblocks rows 2, 4 and part of the demo work: v4 is what the product uses and what it prints.**
Until that sentence exists, `ui-clients-off-v3` has nothing to change to and the demo's request panel does not
know which version to teach.

**Do not edit the v4 call in `_js/instance.js` on the way past.** Row 3 deletes the file.

---

## The two gaps the pipeline does not cover

| Plan | The slice this release takes |
|---|---|
| `plans/api/integration-tests-cover-shipped-modules.md` | **the CORS assertion at minimum.** A configured origin comes back in `Access-Control-Allow-Origin`, an unconfigured one does not. The rest of the plan - the module matrix - needs its research question answered first and does not have to be in this release *(split chosen by an agent)* |
| `plans/ci-cd/what-the-pull-request-does-not-run.md` | both halves. Sonar off a button press, and the integration suites on every pull request. **The second cannot close before the harnesses have the optional modules on**, so it follows the row above |

**Why CORS is the one to take even if the rest slips.** `Program.cs` always registers the policy and every
core endpoint requires it, the origins come from an optional `Cors.json`, and with none present the fallback
allows nothing. **Nothing anywhere asserts any of it**, and it is the one thing in this project that has
actually broken in public: a preflight from the demo origin came back without the header, measured 2026-09-01,
fixed 2026-09-02.

**Neither of these goes in `shared-image-tests.yml`.** The release calls that file whole and takes no inputs,
so a step added there is a step every release pays for. They belong in `pull-request.yml`.

---

## The demo surface

| Plan | The slice this release takes |
|---|---|
| `plans/api/packing-demo-next.md` | items 1 and 2 - the unpacked-items tooltip and the stuck submit button. **Item 3, the request panel, waits on the v4 answer above** *(split chosen by an agent)* |

**Item 1 needs no decision.** The four helpers and their ten tests are already in
`packages/binacle-net-ui/src/core/packingDemo.ts`; the work is a tooltip reachable by hover, touch and
keyboard. **Do not rebuild the inline version** - it shipped on 2026-08-27 and was pulled the same day because
it changes the height of the result row.

**Item 2 is a latent defect.** `submitting` is cleared only inside the thunk handed to
`$dispatch('update-scene', …)`, and nothing runs that thunk unless a visualizer is listening. Both packing
pages have one today, so nothing is broken in public.

---

## Not in this release, and why

**`plans/api/packing-only-image.md` and `plans/api/servicemodule.md`.** Both answered 2026-08-31 and both
`proposed`. The image split changes what a self-hoster pulls, which a minor version may not do. They are the
next major.

**Everything with a `future`, `long`, `on-demand` or `undecided` horizon.** Read `plans/_index.md`.

---

## Done when

- [ ] `CHANGELOG.md` has an `## [Unreleased]` section describing this release.
      `just changelog check Unreleased` passes.
- [ ] A prerelease of this version exists and it is not in the release repository.
      `docker buildx imagetools inspect binacle/binacle-net:3.1.0-beta.1` fails.
- [ ] Every row above is either ticked with a date, or moved out of this file with a reason.
      **By eye.** A row that is neither is the state this file exists to refuse.
- [ ] v4 is no longer marked experimental anywhere a reader meets it.
      `grep -n 'IsExperimental' api/src/Binacle.Net/v4/ApiV4Document.cs` returns `false`, and the published
      v4 spec carries no experimental banner.
- [ ] The shipped UI calls v4 on both hosts.
      **By eye** in `packages/binacle-net-ui/src/core/packingDemo.ts`, with both bundles rebuilt.
- [ ] CORS is asserted in a test.
      `grep -rn "Access-Control-Allow-Origin" api/test` matches.

**Delete this file once v3.1.0 is out and verified.** What outlives it goes to the docs and the decision
ledgers, not here.
