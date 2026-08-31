---
description: pack/first-bin endpoint
state: idea
waits-on: "nobody - it is an idea"
horizon: next-release
paths:
  - "api/**"
---

# Idea: pack/first-bin endpoint

A packing endpoint that answers with the first bin that succeeds rather than the best or the smallest one -
`POST .../pack/first-bin` for custom bins and `POST .../pack/first-bin/{preset}` for preset bins. The caller
supplies bins in the order they care about and gets back the first one the items fit into. The existing
selecting endpoints both optimize, and neither lets a caller say "I have a preference order, give me the first
that works"; a warehouse consuming a stack of box sizes in a set order cannot express that today. It is also
the only endpoint anyone has costed that could satisfy the condition the v4 stable flip waits on - an endpoint
added without reshaping an existing contract.

## Research

### Date not recorded - do not call this `first-fit`

That name collides with First Fit Decreasing, an algorithm already selectable through `Parameters.Algorithm`.
v4 shipped the same mistake once as `pack/best-fit` and had to rename it to `pack/best-bin`. A route names the
bin it returns; the algorithm is a parameter. `first-bin` follows `smallest-bin` and `best-bin`, and a
`FirstBin` strategy class would match.

### Date not recorded - "first success" means two different endpoints, and that is what blocked it

- **Selection only.** Run every bin, return the first successful one in request order. A small strategy class
  next to `BestBin` and `SmallestBin` in `lib/src/Binacle.Lib/ResultSelection/`, consistent with how the other
  selecting endpoints work. It saves no compute, so the name promises something the endpoint does not do.
- **Short-circuit.** Stop at the first bin that packs. This is the version that earns the name and the only one
  with a performance story. It needs a new bin processor: `IBinProcessor.Process` runs all bins today.

The second is the interesting one and the expensive one. Pick before writing code.

### Date not recorded - three open questions

- **Which version.** It was cut from v4. Landing it on v3 would reopen a frozen surface, so check that against
  how v3 is treated before assuming it is free.
- **Does short-circuit change the response shape?** Every other selecting endpoint has results for all bins
  available; this one would not.
- **"First" is caller-supplied order**, so the answer depends on request order in a way no other selecting
  endpoint does. Worth saying so in the endpoint description if this is ever built.
