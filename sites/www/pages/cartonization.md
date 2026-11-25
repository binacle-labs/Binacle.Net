---
permalink: /cartonization/
title: Cartonization API you can self-host
heading: Cartonization you can run yourself.
description: >-
  Cartonization is picking the right box for an order. Binacle.Net answers in milliseconds, as a free and
  open source API you host yourself.
---

# {{ page.heading }}
{: .band--split .split--text-left }

Cartonization is choosing which box an order goes in before anyone packs it. It is the name warehouse and shipping
software gives the job, and underneath it is one question: do these items fit in this box, and which box is the
smallest that holds them.

Binacle.Net answers that question in milliseconds, on hardware you control.
{: .muted }

{% include fit-row.html drawing="carton" row="cartons" %}

The order from the example below, against a preset of three boxes 60 by 40. Medium, the 20, is the smallest that
holds it. [See the request](#example).
{: .verdict }

## What it does
{: .band--wide .band--tint data-label="" }

Give it your box sizes and a list of items. It tells you which of those boxes hold the order and which is the
smallest that does. Ask the pack endpoints instead and it also returns the coordinates of every item inside the box.

The answer comes back as JSON your backend can act on: the box, the items that went in, the items that did not, and
how full the box ends up.

## One request, and the answer
{: .band--tint #example data-label="A worked example" }

Configure your box set once, then send nothing but the order.

{% include exchange.html block=site.data.exchange.carton fold=true %}

The Medium box is the smallest of the three that holds this order.
{: .verdict }

Every box in the set comes back with its own verdict, so your code can apply whatever rule it already has - smallest
by volume, cheapest to ship, whatever is in stock this morning.

It is free and open source, you host it yourself, and there is no per-call fee. Run it beside your order service and
your customers' dimensions never leave your network.

## Where it stops
{: .band--wide .prose--one-line data-label="Scope" }

Binacle.Net works one box at a time.

{% include scope.md %}

Carrier selection and rate shopping are a different job and Binacle.Net does not do them. It hands you a box and the
placement; what you spend on shipping it is yours to decide.

## Run it
{: .band--split .split--text-left #run data-label="Get started" }

One image, one port.

{% include command.html id="run" %}

Box sets are configured as presets, which
[the integration guide](https://docs.binacle.net/configuration/core/presets/#-choosing-your-bin-set) covers along
with how to get your product dimensions into a shape it can use.

## Where to go next
{: data-label="Next" }

- [Parcel lockers]({{ '/parcel-lockers/' | relative_url }}) - If your customers collect from a locker rather than a
  doorstep, the same call answers whether an order fits a locker compartment.
- [How it works]({{ '/how-it-works/' | relative_url }}) - If you are evaluating it, how the packing works covers the
  two functions, the three algorithms, and what a yes and a no each mean.
- [3D bin packing API]({{ '/' | relative_url }}) - Under the trade's word, this is a 3D bin packing API - the term
  most people search for, and the same problem.
{: .doors }
