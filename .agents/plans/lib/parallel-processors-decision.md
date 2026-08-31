---
description: Decide what happens to the three `Parallel*` processors - wire the threshold up, or delete them
state: idea
waits-on: "nobody - it is an idea"
paths:
  - "lib/**"
---

# Decide what happens to the three `Parallel*` processors

`BinProcessorFactory.Create` takes `binCount` and `itemCount` and ignores both, always returning the `Loop`
variants. Nothing outside test code constructs the three `Parallel*` classes. Wire the threshold up or delete
them - three unreachable processors invite someone to fix a path that never runs, and keep two parameters in
a public factory signature that do nothing.

**Measured:** on the FFD + BFD set production uses, parallel algorithm racing runs 0.93x to 1.48x, so it only
wins when the two algorithms take very unequal time.

**`ParallelBinProcessor` has never been measured.** It parallelises across bins rather than algorithms, so it
is the one that could still pay. Deleting it without measuring is a guess.
