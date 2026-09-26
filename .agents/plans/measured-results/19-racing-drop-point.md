---
description: Step 19 - a racing bench that finds the drop point where running the algorithms at the same time starts to beat running them one after another, on 2, 4, 8 and 12 cores, over 30 locked Bischoff problems
state: ready
waits-on: "a session of its own - the maintainer says when"
horizon: now
paths: ["lib/bench/Binacle.Lib.Benchmarks.Racing/**", "lib/test/Binacle.Lib.Testing/**", "shared/test/Binacle.Benchmarking/**", "tooling/bench.just"]
---

# Step 19 - the racing drop point

## Why

Loop against parallel exists to find the drop point: where parallel starts to win, and the point from which it
always wins by a meaningful amount. A cost function then picks loop or parallel at run time. Racing goes first;
the bins test waits (the maintainer, 2026-09-25). The threshold benches stay as they are.

## What the kept runs already say (checked by script, 2026-09-26)

- **The 0.50 "near tie" racing win is a measuring fault.** Its v2 loop row is 656 us against FFD 36 + BFD 294
  run alone; the other four problems' loop is 1.11 to 1.19 times the sum. A normal row would give about 0.96.
- **Parallel racing pays when the faster algorithm alone takes longer than the parallel overhead**, about
  21 us here - the median FFD time, about 130 to 160 items. Not "when one algorithm is much slower", as the
  lib findings record says; fix that line once this run confirms it.
- **The parallel overhead is not fixed.** It grows with the job: 2 us at 3 items, 14 at 79, 20 to 48 on real
  problems.
- **The win is small at best** for FFD+BFD: about 0.85 predicted at best. The run may show there is no
  "always wins by a meaningful amount" point; report that rather than force one.
- **Three algorithms won on all five kept problems** (0.53 to 0.87), but the three-algorithm loop took 1.39 to
  1.66 times the sum of the three alone, which flatters parallel. The alone rows settle it.
- **This machine is a KVM guest.** Its 12 CPUs are virtual; the host decides where they run.

## The test

- Race FFD+BFD, and FFD+WFD+BFD, loop against parallel, v2 only, packing, one bin.
- Also each algorithm alone - FFD, WFD, BFD - so a loop row that is not about the sum of its parts shows up.
- On 2, 4, 8 and 12 cores. No 1 core: the rule returns loop. Core count should matter little for FFD+BFD above
  2 cores, and a lot for three algorithms on 2; keep all four unless the maintainer narrows it.
- Over the 30 problems below. Per core count: 30 x (2 races x 2 + 3 alone) = 210 cases; 840 for four core
  counts - about 70 minutes at the short job, 3.5 hours at the default job.

## Settled with the maintainer, 2026-09-26

- Class `Cores_Packing`, recipe `lib-racing-cores`: short job by default, `precise` for the default job,
  `[confirm]` with 840 cases.
- The alone rows sit in the same class. Each block is a `[BenchmarkCategory]`, and the orderer's group key
  takes the category as well as the job, so each race has its own Loop baseline and the alone rows have none.
- The 30 problems live in a new `CoresSet.cs` beside `RacingSet`, which stays as it was.
- The old racing classes and their recipes are gone, so only `Cores_Packing` runs. `BenchmarkBase` and
  `RacingSet` stay, unused.
- Each case pins every thread itself before the check: on Linux BDN's mask reaches only
  the main thread, so a check alone would fail every case.

## Where it stands, 2026-09-26

Built; the racing project and `Binacle.Benchmarking` build clean. The other five bench projects have not been
built against the shared change. The maintainer started the full test on 2026-09-26:

```
just bench lib-racing-cores precise      # 840 cases, about 3.5 hours
```

Two checks already ran. A dry run of the FFD+BFD rows (240 cases): every pinning check passed, every Loop is
1.00 over its own Parallel, cores sort 2, 4, 8, 12. A short-job preview over five problems (th1_72, th1_44,
th5_41, th2_30, th1_65 - th7_56 was in the filter but did not run; not looked into):

```
dotnet run -c Release --project lib/bench/Binacle.Lib.Benchmarks.Racing -- --job short --filter '*"th1_72 *' '*"th1_44 *' '*"th5_41 *' '*"th7_56 *' '*"th2_30 *' '*"th1_65 *'
```

What the preview showed - short job, so a hint, not a result:

- **The faulty Loop row is back, and it is not a one-off.** Loop runs on one thread, so core count should not
  move it. th2_30's Loop FFD+BFD is about 360 us on 2, 4, 8 cores and 659 on 12; th1_65's about 810 and 1497
  on 12. Also three-algorithm Loops: th1_44 297 / 452 / 288 / 424, th5_41 about 500 then 664 on 12. Every
  large "win" (0.44 to 0.54) is one of these rows. 12 cores, the one job with no spare CPUs, is where it hits
  most. If `precise` still shows it, find the cause before reading a drop point.
- **The three-algorithm Loop is 1.4 to 1.6 times the sum of its three alone rows**, on every problem and core
  count (sum/Loop 0.63 to 0.78). Too steady to be noise; the kept runs showed it too. It flatters Parallel for
  FFD+WFD+BFD until explained. The FFD+BFD Loop is 0.92 to 0.96 of the sum - normal.
- **FFD+BFD where the Loop is sane:** Parallel 0.89 to 0.97 from th1_44 up; on th1_72 it loses above 2 cores
  (1.10, 1.56, 1.79). Fits "small win at best".

The full run overwrites the preview report; the numbers above are the only copy.

## How cores are pinned

- One BDN job per core count. Each sets the affinity mask **and** `DOTNET_PROCESSOR_COUNT`: BDN pins the child
  after it starts, and .NET reads its CPU count once at start-up, so the mask alone can come too late.
- BDN adds a CLI `--job short` beside declared jobs instead of applying it to them. The racing `Program.cs`
  reads `--job` itself, builds the four jobs from it, and drops it from the args. The recipes stay as they are.
- `shared/test/Binacle.Benchmarking/AttributeOrderer.cs` groups by parameters, job and category, and sorts jobs
  by name (`02 cores` to `12 cores`). With one job and no category nothing changes. `BenchmarkProgram.Run`
  takes an optional config.
- A "Cores" column; hide the Job, Affinity and EnvironmentVariables columns. The BDN header still says 12.
- In `[GlobalSetup]`, fail the case if any thread escaped the mask or `ProcessorCount` does not match it.
- A pinned run is kinder than a real small VM: the OS and the BDN host run on the spare CPUs.

## The 30 problems - locked 2026-09-26

Picked by an agent from the v2 times in `lib/results/benchmarks/baseline/algorithms/Full_*_Packing.md`, spread by
FFD+BFD time from the smallest job to the largest, denser where the crossover should be, with an even and a
lopsided problem in each band. The five problems of the kept racing run are in. Name is the problem, then items
and item types. Times are the v2 short-job means the pick used.

| Name | Problem | FFD us | WFD us | BFD us | Why |
|---|---|---|---|---|---|
| th1_72 (74i/3t) | OrLibrary_thpack1_72 | 7.0 | 21.6 | 21.6 | smallest job of the 700 |
| th2_61 (83i/5t) | OrLibrary_thpack2_61 | 10.6 | 74.5 | 26.1 | tiny job, FFD and BFD close |
| th5_93 (84i/12t) | OrLibrary_thpack5_93 | 11.4 | 77.7 | 41.5 | small job, 12 types |
| th1_7 (126i/3t) | OrLibrary_thpack1_7 | 12.0 | 47.4 | 53.2 | kept from the old racing run; typical balance, below the crossover |
| th1_44 (142i/3t) | OrLibrary_thpack1_44 | 21.7 | 152.0 | 48.3 | kept; FFD right at 21 us, on the crossover |
| th2_21 (84i/5t) | OrLibrary_thpack2_21 | 9.9 | 84.1 | 60.6 | same work as th1_44 but lopsided; balance test |
| th4_22 (111i/10t) | OrLibrary_thpack4_22 | 15.6 | 111.3 | 60.3 | typical balance just below the crossover |
| th3_67 (169i/8t) | OrLibrary_thpack3_67 | 32.9 | 349.5 | 61.4 | near the most even of all 700; best case for parallel at this size |
| th2_35 (147i/5t) | OrLibrary_thpack2_35 | 22.2 | 151.8 | 72.2 | kept; on the crossover |
| th3_17 (86i/8t) | OrLibrary_thpack3_17 | 10.7 | 55.7 | 86.0 | same work as th2_35, lopsided; balance test |
| th7_18 (103i/20t) | OrLibrary_thpack7_18 | 19.6 | 143.6 | 83.2 | same work as th2_35 with fewer items and 20 types; items against work |
| th5_35 (143i/12t) | OrLibrary_thpack5_35 | 35.7 | 361.2 | 79.4 | even at medium work; predicted clear win |
| th5_41 (133i/12t) | OrLibrary_thpack5_41 | 25.0 | 206.6 | 101.1 | the median problem, just past the crossover |
| th4_2 (123i/10t) | OrLibrary_thpack4_2 | 15.8 | 138.0 | 122.8 | bigger job that should still lose because FFD is small |
| th2_65 (187i/5t) | OrLibrary_thpack2_65 | 27.6 | 210.6 | 119.4 | typical balance, larger job |
| th3_13 (199i/8t) | OrLibrary_thpack3_13 | 44.6 | 420.5 | 105.3 | among the best predicted wins |
| th6_39 (183i/15t) | OrLibrary_thpack6_39 | 39.2 | 414.9 | 117.1 | the only 15-type case |
| th2_13 (228i/5t) | OrLibrary_thpack2_13 | 49.9 | 484.8 | 121.4 | big FFD share; predicted win |
| th7_56 (162i/20t) | OrLibrary_thpack7_56 | 38.7 | 358.2 | 144.6 | kept; the one real measured win (0.92) |
| th3_23 (130i/8t) | OrLibrary_thpack3_23 | 18.3 | 143.2 | 172.8 | large job predicted not to win; sharpest balance test |
| th1_68 (238i/3t) | OrLibrary_thpack1_68 | 36.4 | 445.6 | 163.0 | typical balance at about 200 us |
| th4_84 (125i/10t) | OrLibrary_thpack4_84 | 23.1 | 193.3 | 201.1 | few items but a long job; items do not predict time here |
| th1_85 (319i/3t) | OrLibrary_thpack1_85 | 60.0 | 692.7 | 167.1 | biggest FFD among the even problems; fills the 243-319 item gap |
| th3_29 (194i/8t) | OrLibrary_thpack3_29 | 38.4 | 436.7 | 228.1 | the only 8-type case above 250 us |
| th5_84 (138i/12t) | OrLibrary_thpack5_84 | 27.8 | 265.8 | 268.1 | huge job with few items; checks the ceiling of the win |
| th2_30 (196i/5t) | OrLibrary_thpack2_30 | 35.9 | 278.9 | 293.5 | kept; re-racing it checks the faulty loop row |
| th1_56 (408i/3t) | OrLibrary_thpack1_56 | 67.3 | 542.8 | 284.8 | most even of the huge jobs |
| th1_30 (405i/3t) | OrLibrary_thpack1_30 | 71.3 | 725.2 | 389.7 | top end, on the always-wins side |
| th1_39 (243i/3t) | OrLibrary_thpack1_39 | 47.5 | 487.6 | 503.4 | most lopsided of all 700 |
| th1_65 (476i/3t) | OrLibrary_thpack1_65 | 77.3 | 657.4 | 664.6 | largest job of the 700 |

## Done when

- [ ] The racing bench runs the test above and writes one report per class with a Cores column, and Loop is
      1.00 within each problem and core count.
      **By eye.** Open the report; every Parallel ratio sits under a Loop of its own core count.
- [ ] The maintainer has run it and the report is kept under `lib/results/benchmarks/`.
      `ls lib/results/benchmarks/*/racing/`
- [ ] The drop point is read out of it, or the report shows there is none, and the lib findings record says
      so - including the fixed line on when racing pays.
      **By eye.**
