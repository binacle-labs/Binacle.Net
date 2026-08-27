---
id: sites/demo-and-image-boundary
description: The two demo tools ship on two hosts from one implementation - what is shared, what diverges freely, and the test that keeps the line where it is
verified: 2026-08-27
check: the shared/not-shared table against packages/binacle-net-ui, sites/demo and api/src/Binacle.Net.UIModule - the eight duplicated file pairs are the row that moves; diff -w each pair to keep the identical-vs-drifted split honest
paths:
  - "sites/demo/**"
  - "packages/binacle-net-ui/**"
  - "api/src/Binacle.Net.UIModule/**"
---

# What the demo site shares with the image

**Permanent.** The two demo tools ship inside the Docker image at `/`, behind the `UI_MODULE` flag, and on the
demo site. Since the rebuild there is **one implementation and two hosts**.

| Shared - one source, `packages/binacle-net-ui` | Not shared - diverges freely |
|---|---|
| the packing form, the 3D visualizer, the errors dialog, the ViPaq decoder | the index page, the navigation, the chrome, the copy, the metadata |

**Why the line sits there.** Nobody minds two different home pages. **Everyone minds fixing the packing form
twice.**

## The rule that keeps it holding

**Pass data in, do not fork.** `PackingDemoOptions` is the seam. A difference between the two hosts arrives as
a value handed to the same component, **never as a second copy of it.**

## The test for any new idea

**Is it around the tool, or inside it?**

**Around it** - a share link, a "run this yourself" block, a scenario gallery - touches the site only and
costs the image nothing.

**Inside it** is a change to the shared package. It lands on the image on its next build, so the image is part
of the decision even when the work looks like site work.

**One live example.** Printing the HTTP call the demo just made is a panel inside the tool, so it lands on
both hosts - and the host named in the snippet differs between them, which is the thing to decide before
building it.

## The two hosts serve different states

**This is the line every allocation falls out of.**

**The image's `/` is reached by someone who is already running it.** They have a container and are checking
their own configuration arrived, which is why `This Instance` belongs there.

**`demo.binacle.net` is reached by anyone.** It has no container behind it belonging to the reader, so
nothing on it can assume one, and `This Instance` means nothing there.

So: **anything that only makes sense after installation goes in the image and nowhere else.** The tool in the
middle is shared.

## If the demo host ever becomes stateful, the seam is what holds

**Sessions, storage and a backend are the first thing one host could have that the other structurally cannot.**
The image ships as one container with no state behind it.

**`PackingDemoOptions` is the seam, and anything of that kind has to arrive through it as data.** One more
value handed to the same component; the image passes nothing and behaves exactly as it does today.
**A second copy of the packing form with that behaviour baked into it is the failure**, and it is the one that
will look like the easy option on the day.

**Anything of that kind is chrome around the tool, not inside it.**

**It also makes the rate limit worth rendering properly.** Anonymous callers share one bucket, so a 429
already happens and currently reads as "nothing happened". The errors dialog has to say what actually failed.

## The ordering claim this makes

**Settle the duplicated markup before the demo host gains any state.** Today the two copies are the same page
wearing different chrome, which is why the decision keeps being deferrable. Put anything stateful around the
form on one side and they stop being the same page - and the cheapest way out stops being available.

## The half that is not shared and should be

**The markup exists twice.** `sites/demo/` and `api/src/Binacle.Net.UIModule/` carry the same packing page,
ViPaq page, **four** scss files and two partials - eight file pairs, 576 lines on the demo side, measured
2026-08-27. The TypeScript underneath is shared and the markup is not, **which is the shape that breaks
silently**: the package is green, one host renders, the other does not.

**Two of the eight are byte-identical and four are not.** `_theme.scss` (121 lines) and `_theme-modes.scss`
(20) match; `_components.scss`, `_forms.scss` and both pages have each drifted by a handful of lines. **That
is the duplication working exactly as it fails** - nothing tells you which differences are deliberate.

**It ends in a decision, not a build.**

**It cost twice on 2026-08-26, which is what this section predicted.** Eight bugs were fixed in the packing
form. The behaviour fixes landed once, in the package, and reached both hosts. **Four markup fixes had to be
made twice** - `x-model.number` on seven inputs, the form-error region, `href` on the result rows, and the
status-text call. The second pass needed its own session, because one host is under `sites/` and the other is
not. **The shared half behaved exactly as designed; the duplicated half cost exactly what duplication costs.**
