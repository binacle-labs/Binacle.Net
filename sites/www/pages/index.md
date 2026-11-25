---
permalink: /
title: 3D bin packing API you can self-host
heading: Will it fit in the box?
description: >-
  Binacle.Net answers which box an order goes in, in milliseconds. A free and open source 3D bin packing API
  you host yourself, in one Docker command.
structured_data:
  type: SoftwareApplication
  name: Binacle.Net
---

# {{ page.heading }}
{: .hero }

Binacle.Net answers which box an order goes in, in milliseconds. Give it your box sizes and a list of items and it
returns the smallest box that holds them, and where every item sits. It is a free and open source 3D bin packing API
that you host yourself.

Built for checkout: your customer picks a locker or a box, and Binacle.Net says whether the order fits before they
pay.
{: .muted }

[Try the demo](https://demo.binacle.net){: .button .button--primary }
{: .actions }

{% include command.html id="hero" class="hero__command" %}

[Read the source on GitHub](https://github.com/binacle-labs/Binacle.Net)
{: .hero__source }

{% include picture.html name="hero" cropped="hero-crop" caption="compartment-L · AllItemsFit" size="kettle-boxed · 24 x 20 x 28" %}

1. [The problem](#problem)
2. [Scope](#scope)
3. [Trade-offs](#trade-offs)
4. [The API](#api)
5. [Use cases](#use-cases)
6. [Get started](#run)
{: .path aria-label="On this page" }

## Checkout has to answer before the customer pays.
{: .band--numbered .band--split .band--number-in-row .band--tint .split--text-left #problem data-label="The problem" }

Your customer picks a locker size or a box, and something has to say yes or no while they are still on the page. Not
in a warehouse an hour later.

Get it wrong and someone repacks the order, prints a second label, or sends a parcel to a locker that will not take
it. The customer has already paid.

Working out whether the items go in is a 3D packing problem, and checkout does not have long to wait.

{% include fit-row.html drawing="lockers" row="lockers" %}

The kettle from the locker example, against three compartments 61 wide. Only the large one takes it.
[See the request]({{ '/parcel-lockers/' | relative_url }}#example).
{: .verdict }

## What it answers
{: .band--numbered .band--wide .band--number-in-column .prose--one-line #scope data-label="Scope" }

Binacle.Net works one box at a time.

{% include scope.md %}

## A yes is reliable. A no is not a proof.
{: .statement .band--deep }

[What a yes and a no each mean]({{ '/how-it-works/' | relative_url }})

## What you get, and what it costs
{: .band--numbered .band--number-in-column .columns #trade-offs data-label="Trade-offs" }

### You run it yourself

- Your customers' dimensions never leave your network
- **Costs** Uptime is yours
{: .labelled-list }

### A yes is reliable

- If it says the items fit, they fit, and the pack routes show you how
- **Costs** A no is not a proof - there may be an arrangement it did not find
{: .labelled-list }

### Free and open source

- AGPL-3.0, no signup, no tier
- **Costs** Support is a GitHub discussion, not an SLA
{: .labelled-list }

---

Strictly, that makes this the single-container case - a 3D knapsack rather than bin packing, and the word the
logistics trade uses is cartonization. Most people looking for it search for 3D bin packing, so that is the term used
here.
{: .prose .muted }

## What you can ask it
{: .band--numbered .band--wide .band--number-in-row .band--tint #api data-label="The API" }

Two functions. **Fit** checks whether a set of items goes into a box. **Pack** works out where each one sits.

Either takes your boxes in the request - `by-custom` - or by the name of a preset you configured once -
`by-preset/{preset}`.

The pack endpoints run the same algorithms and also return the position of every item. A fit response tells you what
went in and what did not; it has no coordinates in it.

{% include exchange.html block=site.data.exchange.fit fold=true %}

The medium locker holds all three items. The small one takes only one of them.
{: .verdict }

## Where people use it
{: .band--numbered .band--split .band--number-in-row .band--tint .split--text-left #use-cases data-label="Use cases" }

The same 3D bin packing API answers three different questions, depending on who is asking.

- {% include door-picture.html name="door-carton" %}
  [Cartonization]({{ '/cartonization/' | relative_url }}) - Warehouse and shipping teams call it picking the right box
  for an order.
- {% include door-picture.html name="door-locker" %}
  [Parcel lockers]({{ '/parcel-lockers/' | relative_url }}) - If your customers collect from a locker, the question is
  whether an order fits a parcel compartment before they choose one.
- {% include door-picture.html name="door-pack" %}
  [How it works]({{ '/how-it-works/' | relative_url }}) - If you are deciding whether to integrate it, read how the
  packing itself works - the two functions, the three algorithms, and what a yes and a no each mean.
{: .doors .doors--pictured }

## Who makes this
{: .band--compact data-label="The maintainer" }

[Chris Mavrommatis](https://github.com/ChrisMavrommatis) builds Binacle.Net. Every commit since February 2023 is his.
Questions go to [GitHub Discussions](https://github.com/binacle-labs/Binacle.Net/discussions).

## Run it
{: .band--numbered .band--split .band--number-in-row .split--text-left #run data-label="Get started" }

One image, one port.

The three flags turn on three optional pages, and the API needs none of them.

{% include command.html id="run" %}

| Flag | Page | Path |
|---|---|---|
| `UI_MODULE` | The packing demo | `/` |
| `SWAGGER_UI` | Swagger UI | `/swagger/` |
| `SCALAR_UI` | Scalar | `/scalar/` |

The API itself is under `/api/v3`. Everything else is optional, and
[the quick start](https://docs.binacle.net/quick-start/) has the rest.

Try it out.
