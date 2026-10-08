---
description: A home for results that compare two slices - ViPaq's encode time against lib's pack time, what one request costs end to end
state: idea
waits-on: "nobody - it is an idea"
horizon: undecided
paths:
  - "lib/results/**"
  - "vipaq/results/**"
---

# Results across slices

Each slice's results files read only that slice's numbers, so a question that needs two slices has no home:
how ViPaq's encode time compares with lib's pack time, or what one request costs from items in to token out.
Neither `lib/results/` nor `vipaq/results/` may hold it. Where it lives, and whether the times can be set side
by side at all when they come from different runs, is open.
