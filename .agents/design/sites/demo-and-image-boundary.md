---
id: sites/demo-and-image-boundary
description: The two demo tools ship on two hosts from one implementation - what is shared, what diverges freely, and the test that keeps the line where it is
verified: 2026-11-09
check: the shared/not-shared table against shared/packages/apps, sites/demo and api/src/Binacle.Net.UIModule; the duplicated pairs - the packing and ViPaq pages, the errors dialog, 3D view and size rows partials, the packing and ViPaq page scripts - diff -w each to keep the list of differences honest
paths:
  - "sites/demo/**"
  - "shared/packages/apps/**"
  - "api/src/Binacle.Net.UIModule/**"
---

# What the demo site shares with the image

**Permanent.** The two demo tools ship inside the Docker image at `/`, behind the `UI_MODULE` flag, and on the
demo site. There is **one implementation and two hosts**.

| Shared - one source, `shared/packages/apps` | Not shared - diverges freely |
|---|---|
| the packing form, the ViPaq decoder, the errors dialog, the wiring of the 3D view | the home page, the navigation, the chrome, the copy, the metadata |

The look, and the 3D view itself, come from `shared/packages/design-system`, which both hosts read the same way.
*Everyday Items* is in the package too, but only the demo site hosts it.

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

**One live example: the request under the results.** The package keeps the call it just made as `lastRequest` -
method, path and body - and stops there. The image's own markup and script turn it into a curl command against
the page's origin, in a *Request* fold. The demo site does not show it. Because the package hands it out as
data, the demo site can add it later without touching the package.

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

**Anonymous callers share one rate-limit bucket**, so a 429 already happens. The errors dialog says so: a 429
with no body reads as rate limited, wait and try again.

## The ordering claim this makes

**Settle the duplicated markup before the demo host gains any state.** Today the two copies are the same page
wearing different chrome, which is why the decision keeps being deferrable. Put anything stateful around the
form on one side and they stop being the same page - and the cheapest way out stops being available.

## The half that is not shared and should be

**The markup exists twice.** `sites/demo/` and `api/src/Binacle.Net.UIModule/` each carry the packing page, the
ViPaq page and three partials - the errors dialog, the 3D view and the size rows - and the script behind each
of the two pages. The package underneath is shared and these are not, **which is the shape that breaks
silently**: the package is green, one host renders, the other does not.

**The stylesheets are no longer in it.** Each host's is one `main.scss` that pulls in the design system's
blocks; the two lists differ only by the blocks each host's pages use.

**No pair is identical, and today every difference is meant:**

- the template language - a Liquid include on one side, a Razor partial on the other. Razor reads `@` as its
  own, so the image writes Alpine's `@click` as `x-on:click`;
- the copy and metadata - front matter on the site, `AppletsService` in the image;
- the *Request* fold and its step line, in the image only;
- the demo's 3D view also listens for the item-type highlight, which *Everyday Items* sends;
- comments.

**Nothing checks that list.** The next difference that is a slip will look the same as these. **That is the
duplication working exactly as it fails** - nothing tells you which differences are deliberate.

**It ends in a decision, not a build.**

**It cost twice on 2026-08-26, which is what this section predicted.** Eight bugs were fixed in the packing
form. The behaviour fixes landed once, in the package, and reached both hosts. **Four markup fixes had to be
made twice** - `x-model.number` on seven inputs, the form-error region, `href` on the result rows, and the
status-text call. The second pass needed its own session, because one host is under `sites/` and the other is
not. **The shared half behaved exactly as designed; the duplicated half cost exactly what duplication costs.**
