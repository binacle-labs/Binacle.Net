---
permalink: /how-it-works/
title: How the packing works
heading: How Binacle.Net packs a box.
description: >-
  Binacle.Net runs three heuristic packing algorithms. What a yes means, what a no does not mean, and where
  the API stops.
---

# {{ page.heading }}
{: .band--split .split--text-left }

Two functions and three algorithms. Fit answers whether a set of items goes into a box. Pack answers the same
thing and works out where each item sits.

Both are part of the same [3D bin packing API]({{ '/' | relative_url }}): one image, and you call whichever
answer you need.
{: .muted }

{% include picture.html name="pack" caption="locker-M · FullyPacked" size="40 x 30 x 20" %}

Every position is from the pack response below. [See the request](#example).
{: .verdict }

## Fit and pack
{: .band--tint data-label="The two functions" }

**Fit** is the pre-check. It takes your boxes and your items and returns a verdict per box, with the items that
went in and the items that did not. Use it at checkout, where you need a yes or a no and nothing else.

**Pack** runs the same algorithms and returns the position of every item as well. Use it when someone downstream
has to build the box, or when you want to draw the layout.

A fit response has no coordinates in it. If you need placement, call pack.

## A pack request, and where every item lands
{: .band--tint #example data-label="A worked example" }

Same shape as a fit request. The difference is in the response.

{% include exchange.html block=site.data.exchange.pack %}

All three items are placed, and the response says exactly where each one sits.
{: .verdict }

The coordinates are the corner of each item inside the box, in the same units you sent. The first item sits at
the origin; the two after it are placed against what is already there.

## The three algorithms
{: .band--wide .columns data-label="Algorithms" }

All three sort the items by decreasing size and then differ in where they put each one. You pick one per request.

### First Fit Decreasing

`FFD`

- Places each item in the first space it finds that holds it, without looking for a better one
- **Costs** May leave space unused that a slower search would have filled
{: .labelled-list }

### Worst Fit Decreasing

`WFD`

- Places each item in the space that leaves the most room around it
- **Costs** Spreads items out, which helps with distribution more often than with space
{: .labelled-list }

### Best Fit Decreasing

`BFD`

- Keeps the space left around each placement as small as it can
- **Costs** Examines the candidate spaces for each item rather than taking the first that fits
{: .labelled-list }

---

Which one is fastest, or packs tightest, depends on your data and on the version you run. These describe what each
algorithm does, not how they rank against each other. If it matters to your workload, measure all three on your own
boxes and items.
{: .prose .note }

## What a yes means, and what a no does not
{: data-label="What the answer means" }

All three are heuristics. They are built to run in the time a checkout page has, and that choice has a consequence
in each direction.

**A yes is reliable.** When Binacle.Net says the items fit, they fit. It does not report a placement it could not
make, and the pack endpoints will show you the placement it found.

**A no is weaker.** It means this algorithm did not find an arrangement, not that no arrangement exists. In rare
cases another algorithm, or the same items in a different order, finds one. That is the trade the speed is bought
with.

If a no is expensive for you, run a second algorithm before you act on it. If a yes is expensive to get wrong, you
are on the safe side already.

## Which version to call
{: .band--tint data-label="Versions" }

**Use v3.** It is stable, it is what every example on this site uses, and it is what the docs cover.

**v4 is experimental.** It ships in the same image, under `/api/v4`, and its routes and responses can change in a
patch release. Do not integrate against it by accident.

[The API reference](https://docs.binacle.net/api/v3/) covers both, and the
[v3 OpenAPI document](https://docs.binacle.net/swagger/v3/) is the contract itself.

## Read the code
{: .band--wide #run data-label="Get started" }

All of it is in [one repository](https://github.com/binacle-labs/Binacle.Net), under AGPL-3.0 - the packing engine,
the API, and the tests that hold the algorithms to what this page says they do.

If you would rather watch it than read it, [the packing demo](https://demo.binacle.net/packing/) runs the three
algorithms in the browser on boxes and items you type in.

Or run the image and point your own data at it:

{% include command.html id="run" class="band__command" %}

Start with [the quick start](https://docs.binacle.net/quick-start/), or
[core concepts](https://docs.binacle.net/core-concepts/) for the longer version of this page.
