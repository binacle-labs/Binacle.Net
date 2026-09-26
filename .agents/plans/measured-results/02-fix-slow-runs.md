---
description: Session 2 - build the slow-run fix chosen in session 1, and prove it with a small run before any long rerun
state: blocked
waits-on: "session 1 - the cause and the chosen fix"
horizon: undecided
paths: ["lib/bench/**", "vipaq/bench/**", "shared/test/Binacle.Benchmarking/**", "lib/src/Binacle.Lib/Algorithms/**"]
---

# 2 - Fix the slow runs

Build the fix session 1 chose, and nothing else. What it is, is only known once session 1 lands; its file says.

## The small run that proves it

A long rerun on an unproven fix costs hours and may crash the machine. So first:

- **BFD, th3_29, several launches.** Before the fault it was 215 or 370 us, by process. Pass: no spread over 1.3×.
- **ViPaq, `ViPaq_Row` on `5000 items, 8-bit`, five launches.** It was 183 or 279 us. Pass: no spread over 1.3×.
- **If the fix runs each case in several processes:** the report shows the spread for each case, and a slow
  process can be seen in it.

Write the numbers here.

## If the fix touches product code

- The result tests must pass: `just test cs_binacle-lib_unit`.
- The maintainer runs `just measure lib` and `just measure vipaq`. If a file under `measurements/` moves, the fix
  changed a packing: stop and tell him.

## Done when

- [ ] The fix is in, and every project it touches builds.
      `dotnet build <csproj>` on each project the fix touches, and on each project that uses it.
- [ ] The small run shows no spread over 1.3× on both cases, and its numbers are written here.
      **By eye.**
- [ ] If product code changed: the tests pass and the measurements did not move.
      `just test cs_binacle-lib_unit`; after `just measure lib` and `just measure vipaq`,
      `git status --short lib/results/measurements vipaq/results/measurements` prints nothing.
