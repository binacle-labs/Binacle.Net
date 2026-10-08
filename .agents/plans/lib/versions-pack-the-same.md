---
description: No test checks that two versions of one heuristic put every item in the same place; the tests and the measure compare status and fill only
state: idea
waits-on: "nobody - it is an idea"
horizon: undecided
paths:
  - "lib/test/**"
  - "lib/measure/**"
  - "lib/src/Binacle.Lib/Algorithms/**"
---

# Versions of one heuristic pack the same

The docs say every version of a heuristic produces the same results. Nothing checks that item by item.

- The unit tests check a scenario's status and metrics. v1 and v2 of BFD pack thpack7_45 to different fills
  (79.08 against 79.73) and both pass.
- `version-parity.md`, written by `just measure lib`, compares fill only. Two packings with the same fill and
  items in other places count as the same.

A check would pack each problem with each version and compare where every item landed: position and
orientation. It matters most when a new version changes how a space is picked, as the test-only v3 of BFD
and WFD does. Where two spaces tie, v1 and v3 take the earliest in the list, and v2 takes whichever its
unstable sort left first.

Open: whether it is a unit test, a measure file, or both; and whether a known difference such as thpack7_45
is listed as allowed or makes the check fail.

## Done when

- [ ] A check compares item positions between versions of each heuristic over the Bischoff suite.
      **By eye.** Find the check and read what it compares.
