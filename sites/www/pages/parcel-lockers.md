---
permalink: /parcel-lockers/
title: Check if an order fits a parcel locker
heading: Which locker size does this order need?
description: >-
  Check whether an order fits a parcel locker before your customer pays. A free and open source API you host
  yourself, answering in milliseconds.
---

# {{ page.heading }}
{: .band--split .split--text-left }

A customer at checkout picks a locker to collect from. Your carrier's API tells you where the lockers are and which
compartments are free. It does not tell you whether the order fits one.

Send Binacle.Net the compartment sizes and the items, and it answers before the customer commits.
{: .muted }

{% include fit-row.html drawing="lockers" row="lockers" %}

The kettle from the example below, against three compartments 61 wide. [See the request](#example).
{: .verdict }

## What it costs when nobody checks
{: .band--wide .band--tint data-label="The problem" }

The order is packed and labelled for a compartment it does not go into. Someone finds that out at the locker, not at
the desk. The parcel is rerouted to an address or comes back to you, and the customer has already paid for the
delivery they chose.

The check itself is one call, and it happens while the locker options are still on screen.

## One order, three compartment sizes
{: .band--tint #example data-label="A worked example" }

Send the compartments you can actually book, and the items in the cart.

{% include exchange.html block=site.data.exchange.locker fold=true %}

Only the large compartment takes this order. Offer that one, or offer a courier.
{: .verdict }

Every compartment comes back with its own verdict, so the checkout page can grey out the sizes that will not work
instead of letting the customer pick one that fails later.

## What it needs from you
{: .band--wide data-label="Integrating" }

- Use centimetres for all measurements, and keep the values as integers.
- Represent products as rectangular prisms - length, width, height. Irregular shapes have to be boxed first.
- Use the maximum measurement of a product, not the nominal one. A box that is 24 cm on the label and 24.6 cm in the
  hand is the one that comes back.
- Track weight yourself. Binacle.Net measures space and nothing else, so the carrier's weight limit is a check your
  code still has to make.

[The integration guide](https://docs.binacle.net/configuration/core/presets/#-choosing-your-bin-set) has the rest,
including the two ways to choose a bin set and where the call goes in a checkout flow.

## Compartment sizes come from your carrier
{: .band--compact data-label="A limit" }

The numbers in the request above are the shape a locker bank tends to have. They are not any carrier's published
dimensions, and Binacle.Net ships no list of them. Get the real sizes from whoever runs the lockers you ship to, and
configure them as your bin set.

There is no integration with any locker network, and none is implied. Binacle.Net answers the geometry question;
booking the compartment stays between you and your carrier.

## Run it
{: .band--split .split--text-left #run data-label="Get started" }

One image, one port.

{% include command.html id="run" %}

If your team calls this [picking the right box for an order]({{ '/cartonization/' | relative_url }}), it is the same
call with your own boxes in place of the compartments.

The rest of what it does is on the [homepage]({{ '/' | relative_url }}).
