---
title: Core Concepts
description: >-
  What Binacle.Net needs from your dimensions, the two questions it answers - fit and pack - and what the
  FFD, WFD, BFD and Best algorithms each do.
nav:
  order: 3
  icon: 🔍
---

Binacle.Net answers one question: given some bins and some items, which bin holds them, and where does each
item go. This page is what you need to know before calling it - the shape of the data, the two operations,
and the algorithms behind them.

## 📏 Dimensions

Every bin and every item is a box: a length, a width and a height. Binacle.Net knows nothing else about them.

- **Integers, in one unit.** Centimetres are assumed, but any unit works as long as every bin and every item
  uses the same one. Convert before you send, and round up - a value rounded down describes an item that is
  smaller than the one in the warehouse.
- **Box the irregular ones.** A bottle, a tube or a bag is sent as the smallest box it fits in.
- **Weight is not a dimension.** Binacle.Net does not read it. If a carrier has a weight limit, check it in
  your own code before or after the call.

## 🧩 Fit and Pack

Binacle.Net does two things, and every endpoint is one or the other.

**Fitting** answers yes or no: do these items fit in this bin? It stops as soon as it knows, which makes it
the cheap call - the one to make at checkout, before offering a delivery option that depends on the answer.

**Packing** goes on to place every item and returns where each one sits. If not everything fits, it packs
what it can and names what was left over. This is the call that draws a picture, feeds a packing station, or
is stored as [ViPaq]({% vlink vipaq-protocol.md %}).

A fit that says yes is reliable: the items were placed. A no is a no from that algorithm, not proof that no
arrangement exists - see below.

## 🧠 Algorithms

Binacle.Net uses heuristics: rules that place items quickly rather than searching every possible
arrangement. They do not always find the best packing that exists, and in rare cases one misses a fit that
another would find. That is the trade for answering in milliseconds.

Every algorithm sorts the items largest first, then places them one by one. They differ in which space each
item goes into.

### ⚖️ First Fit Decreasing (FFD)
Each item goes into the first space it fits in.

- ✅ Fast - it never compares spaces.
- ⚖️ Can leave room unused, because it takes the first space and not the tightest one.

### 🧊 Worst Fit Decreasing (WFD)
Each item goes into the space that leaves the most room behind.

- ✅ Spreads items out, which helps when the leftover space matters more than the fit.
- ⚖️ Rarely the tightest packing.

### 📏 Best Fit Decreasing (BFD)
Each item goes into the space that leaves the least room behind.

- ✅ The tightest placement it can find for each item.
- ⚖️ Looks at every candidate space for every item, so it is the slowest of the three.

### 🏆 Best
Runs more than one of the three and keeps the best result. The response says which one produced it. Only
[V4]({% vlink /api/v4.md %}) offers it, and its page says which heuristics each route runs.

> Which algorithm is fastest, or packs tightest, depends on your bins and items. These descriptions say what
> each one does, not how they rank. If it matters to your workload, measure all of them on your own data.
{:.block-note }

## ➡️ Where to go next

- [API]({% vlink /api/index.md %}) - the endpoints, and where the algorithm is chosen.
- [Presets]({% vlink /configuration/core/presets.md %}) - your bin set, so a request need not carry the bins.
