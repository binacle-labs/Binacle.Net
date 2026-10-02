---
id: lib/decisions
description: Lib decisions ledger — why Algorithm.Best races a different set per path, where the packing vocabulary lives, why there are two data hubs, why old algorithm versions are kept, the parallel bench shape (a class per thing raced, one grid per scenario kind, why the line classes measure an expected nothing), how a kept baseline run is removed or replaced rather than dated and what a skip mark carries, and the open parallelization question.
verified: 2026-10-03
check: Algorithm sets match AlgorithmProcessorFactory.Create and BinProcessorFactory.CreateMultiAlgorithm; the project and fixture layout matches lib/ and shared/, and the folders embedded by shared/data/Binacle.Data/Binacle.Data.csproj match the Keys arrays in its BischoffSuite/DataProvider.cs and CustomProblems/DataProvider.cs
also_update:
  - lib/findings
paths:
  - "lib/**"
---

# Lib — decisions ledger

Decisions and open questions for `lib/src`, with the *why*. Measured evidence lives in `$lib/findings`.
This file is the "what was built and why", so a fresh session does not re-litigate it or "fix" a deliberate
choice.

## Decided

### D1 — `Algorithm.Best` races a different set depending on the path

**Decided (the maintainer, 2026-09-28):** "because it runs many bins i did not deem it worthy running wfd, many
algorithms x many bins, but this is subject to change if we find something better". Do not "align" the two
without asking him. The same parameter value means two things:

| `Best` on | Path | Races |
|---|---|---|
| `fit/bin`, `pack/bin` | `SingleBinAsync` → `AlgorithmProcessorFactory.Create` | FFD, WFD, BFD |
| `compare-bins`, `smallest-bin`, `best-bin` | `BinProcessorFactory.CreateMultiAlgorithm` | **FFD, BFD** |

**Why:** WFD is not worth choosing for auto **across many bins**. Adding it to FFD+BFD costs **+131% to +373%**
— roughly 2.3× to 4.7× the run time (`$lib/findings#F1`) — for an algorithm that rarely produces the winner.
That cost is paid **per bin**, so on a compare across a preset's bins it multiplies by the bin count. One bin
can afford the third algorithm; many bins cannot.

The asymmetry is the point: the single-bin path buys a small extra chance of a better answer for a bounded
cost, and the multi-bin path refuses the same trade because the cost is no longer bounded.

**Consequence for the docs:** any page describing `Best` must say **which set the route uses**, and why WFD is
dropped. "Runs all algorithms" is wrong on the multi-bin routes.

### D4 — old algorithm versions are kept

**Decided (the maintainer, 2026-09-28):** "old versions are kept. it shows my progress... when i need to start
improving i copy from the last and iterate on that... until i gained enough evidence thats stable then i
release it... if and when i decide to delete thats a different topic".

So each version stays in `lib/src/Binacle.Lib/Algorithms/`, the next one starts as a copy of the latest, and it
ships only once the evidence says it is stable. Deleting a version is a separate call, his.

### D5 — one bench project per parallel question, every class pinned

**Decided (the maintainer, 2026-09-29):** one project per question - `ParallelAlgorithms` for racing algorithms
on one bin, `ParallelBins` for one algorithm on many bins - "a full restructure of both binaries is in order".
Asked whether to pin every class in both, "yes common code to shared": `JobsByCoreCount` lives in
`shared/test/Binacle.Benchmarking`. `Full_Algorithms_Packing_v1` and `_v2` were dropped ("2 yes"): v1 against
v2 is the `Algorithms` project's question. The old bins classes were retired ("3 retire").

**Decided (the maintainer, 2026-09-30):** a baseline holds runs of classes that exist, nothing else - "what
eeds to run should not be in results besaline", "keeping the old in old". The retired threshold reports are
in git at commit `59e9dfb3`.

**"Keeping the old in old" is superseded by D7** (2026-10-03): there is no dated folder. A superseded report
is removed, and what it showed is written out in prose before it goes.

### D6 — the Identical scenario is one grid, and a class per thing raced

**Decided (the maintainer, 2026-10-03):** `ParallelAlgorithms` holds five classes - "we will split the alones
as well / 5 clases / FFD alone / WFD alone / BFD Alone / FFD & BFD / FFD & BFD & WFD / all wll have 2 methods
loop and paralel". `ParallelBins` holds six - "and for paralel bins / 6 / FFD Pieces / WFD Pieces / BFD Pieces
/ FFD Lines / BFD Lines / WFD Lines". Algorithm order in a name follows the code, FFD then WFD then BFD -
"algorithm = same as code".

**Why a class and not a row:** BenchmarkDotNet's `--filter` picks classes, not rows. As one class, the
two-algorithm race could not be run without paying for WFD, which costs about seven times FFD at 256 pieces.
`ParallelOverhead` already split by race width for the same reason.

**The cost of the split, measured 2026-10-03:** the alone rows used to share a report with the race rows, so
`Loop` against the sum of its algorithms could be checked under one run's conditions. **The split broke that
check as an equality.** `Loop` on a race class runs 1.10 to 1.21 times the sum of the alone classes, steadily
across the whole grid, while the allocation is additive to the kilobyte - same arrays, same work, 10 to 21 per
cent more time. An alone class's process only ever constructs one algorithm, so its factory and `Execute` call
sites are monomorphic and get devirtualized; the race's cannot be. **The alone classes therefore measure a
faster algorithm than the race contains.**

It is still the test of whether a run is clean, read as a ratio rather than an equality: about 1.2 is normal,
and a cell at 1.5 or more is a slow process. It only holds while every class of a scenario comes from one
invocation, which is why the recipes filter them all in.

**A one-algorithm class's `Parallel` row is not a race.** `ParallelLimits.Degree(1)` is 1, so `Parallel.For`
runs the body inline and nothing is dispatched: the row is what `Parallel.For` costs to set up, which
`$lib/findings` has measured straight in `ParallelOverhead`. It is kept because every class reading the same way
is worth more than three cheap rows cost - read it as the cap's receipt.

**Decided (the maintainer, 2026-10-03):** one grid for the whole scenario - "no for identical the params need
to be identiacal to all we need to measrrue Under the same Ideal conditions" - and, asked whether that meant a
cross product of every axis, "yes within each kind". So the three bins Pieces classes share a grid and the
three Lines classes share another. The grid lives on a base class, not on the leaves, so three classes of a
kind cannot drift apart and stop being comparable.

**Decided (the maintainer, 2026-10-03):** the classes sit in a folder per scenario and the namespace stays at
the project root - "we can put them in a folder and keep the namespace at root just for not cluttering".
`IDE0130` is `none` in `.editorconfig`. The scenario stays in the type name as well as the folder, because
BenchmarkDotNet names the report file after the type.

**Why the line classes exist when the answer is expected to be nothing:** they hand the same pieces over twice,
as one line of many and as many lines of one - "the idea is to provide same items in piece and as in lines to
see if they differ at all". They should come out flat, since the algorithm's constructor flattens quantity into
one array before sorting. He chose to measure it rather than read it off the code - "the way to go is add
them... see and proove tjhey do nothing... sometimes having proof it doesnt work is better" - because a measured
flat is what lets a parallelisation rule read pieces instead of lines, and an answered question still regresses.

### D7 — a kept run is removed or replaced, never dated, and a skip carries its reason

**Decided (the maintainer, 2026-10-03):** `baseline/` carries every bench class at the shape the code has now -
"in baseline we do no skip bnaseline must carryuy all and updated to the latest shapoe". Two acts and no third:
a class or shape that is gone has its report **removed**, and a class whose shape moved is **re-run and its
report replaced** - "if a report exist in the baseline forlder in which we have no shape it must be removed and
if a report exist and isnt ot in the right shape it must be run and be replaces".

**No dated folder for the superseded ones** - "folder = no delete or fold into plans for history if we need
them". What a removed report showed is written out in prose first, where it is still wanted. This reverses the
dated-folder half of D5 and the agent preference recorded with it. Applied on 2026-10-03: all three parallel
packing reports were removed when the two projects were reshaped.

**Decided (the maintainer, 2026-10-03):** a class whose question is answered can be marked skip, and the mark
carries a reason - "sure no skip flag and do skip with a reason". There is no flag that includes the skipped
ones, because `baseline/` never skips, so the run mode is the only switch needed.

`SkipAttribute` is built and **nothing reads it** (2026-10-03, on his "build the skip attribute do not wire
it"). When it is wired, three things follow from the decisions above: a `baseline` run must ignore the mark,
a `dry` run must ignore it too or a skipped class rots unnoticed - it still compiles, then fails at
`GlobalSetup` - and the reader is where an empty reason should be rejected, not the attribute's constructor,
which would throw during reflection. **Suggested (agent, 2026-10-03):** a `baseline` mode word carrying the
default job, since `JobsByCoreCount` puts the mode word into the job id and a kept report's header would then
say what it is.

### D8 — ParallelOverhead is fake algorithms and fake bins, read as gaps

**Decided (the maintainer, 2026-10-02):** the project is "paralel ovehear = fake algoirithms + bins we test
both... we start with no work and some work?", and the names have to be plain - "make sure u use names that are
clear and people undersntand what theu do". So `Fake_TwoAlgorithms`, `Fake_ThreeAlgorithms` and `Fake_Bins`,
three rows each - `Loop`, `Parallel_OneThread`, `Parallel` - over a working set of 0 or 64 KB. Core counts 1,
2, 4, 8 and 12: the set every other project runs, plus 1, which stays on his word - "2 keep it we need a
baseline maybe the run was noisy maybe it was a dry run".

**The rows are read as gaps, not as times.** `Loop` to `Parallel_OneThread` is what `Parallel.For` costs to
set up and does not move with the machine; `Parallel_OneThread` to `Parallel` is what handing work to another
CPU costs, and that one does. A `Mean` here is not a packing time - it is scaffolding around work that does
not exist.

**A class per race width, a parameter for bins:** a production race is only ever 2 or 3 wide, and `--filter`
picks classes rather than parameter values, so each width can be run alone. A request carries any number of
bins, so that has to be a parameter.

**Names rejected on the way**, so they are not revisited: `Stub_` (jargon, and it describes how the double is
built rather than what the case is), `NoPacking_` (collides with `OperationResultStatus.NotPacked`, a real
result), `Empty_` (reads as an empty bin or an empty request), `NoWork_` (accurate until the algorithms started
walking memory, which made no-work one value of a sweep rather than the case).

**Why the project exists at all:** the cost of parallelising used to be read as the gap between two packing
rows, and subtracting two numbers of similar size is the least reliable way to get one. Here it is measured
straight. What it found is in `$lib/findings`.

## Pending

What the code does and why. No quote from the maintainer covers these yet.

### D2 — the packing vocabulary lives in `shared`, and there is no abstractions assembly

`Binacle.Lib.Abstractions` was three things wearing one name: pure geometry, the packing vocabulary, and the
packer's engine interfaces. It was broken up on 2026-08-13. The geometry half folded into `Binacle.Geometry`,
the vocabulary became `shared/src/Binacle.Packing`, and the engine interfaces went into `Binacle.Lib` under its
own `Abstractions/` folder. `lib/src` now holds one project. See `$lib/dependencies`.

**Why the line falls where it does.** `Binacle.Geometry` holds what the notation and the wire format need;
`Binacle.Packing` holds what only the packer and its consumers need. That is a fact about usage, not taste.
The quantity interfaces are used by `Binacle.CompactNotation`, the OR-Library converter and the notation unit
tests — consumers that never touch packing — so they are correctly in Geometry. All three name
`IWithReadOnlyQuantity<T>`, the read-only base, not `IWithQuantity<T>` itself; the pair moves together.
`IWithID` is used by none of those three, only by
`api/src`, `lib/src`, `lib/test` and the data projects. That is why identity is packing vocabulary and not
geometry.

**Why there is no separate abstractions assembly.** After the extraction the engine interfaces had exactly one
direct consumer, `Binacle.Lib` itself; `api/src/Binacle.Net` resolves them transitively, and every test project
references `Binacle.Lib` only. Nothing in the repo is packable — no `IsPackable`, `PackageId` or
`GeneratePackageOnBuild` anywhere — so the contract-without-implementation case does not apply. Publishing would
not change it: the package an external consumer would want *is* `Binacle.Packing`, the result types they read
back.

**What the collapse cost.** `IPackingAlgorithm` could not name a concrete algorithm before — separate assembly,
one-way reference, enforced by the compiler. Now only convention stops it. That is the sharpest candidate rule
for any type-level architecture tool: types in namespace `Binacle.Lib.Abstractions` may not reference types
outside it.

**An unused reference is not a dead reference.** `Binacle.Net.Kernel` referenced `Binacle.Lib.Abstractions` and
no file under it named `Binacle.Lib` — but `Binacle.Net.DiagnosticsModule` was resolving four types *through* it
with no reference of its own. Removing it surfaced an undeclared dependency rather than creating one. "No file
in this project names the assembly" proves the reference is unused by that project, not that it is unused; only
a build after removal proves that.

### D3 — two data hubs, split by who reads the fixtures

Split on 2026-08-13. `shared/data/Binacle.Data` (the shared tests kernel until 2026-09-19) keeps
the algorithm fixtures, which the api integration suite reads as well as the lib tests.
`lib/data/Binacle.Lib.Data` (the lib tests kernel until 2026-09-20) holds result selection,
which **nothing outside the lib slice reads**, so its fixtures live in `lib/data` rather than `shared/data`.

**The rule that falls out:** a fixture set lives in `shared/data` when more than one slice reads it, and in the
slice otherwise. Bischoff and custom-problems qualify twice over — two slices read them through it, and
the ViPaq packed-data generator reads the same files by path at run time — so they stay put. ViPaq had already
settled this shape with its own `vipaq/data/packed`.

**`shared/data/demo-samples/` was added under the same rule.** The demo component generates its sample set
from it and the ViPaq packed-data generator packs it. Until 2026-09-20 `Binacle.Data` embedded it with no key
set, so nothing read the embed; it now has `DemoSamples.DataProvider`, is in `All`, and `PackingDemoSamplesTests`
runs every version of every algorithm over it. Its `Result` came from the packer, so that class is a
regression baseline, not an independent check.

**The friend grant is preferred to a shared bin model.** `OperationResultHelper` bridges through Packing's
internal `Dimensions` rather than taking `ScenarioBin` from `Binacle.Data`. `PackedBin`'s constructor is
internal, so the grant is needed either way; borrowing the type would add a dependency without removing one.
Since 2026-09-20 `Binacle.Lib.Data` references `Binacle.Data` anyway, for its reader - the argument now rests
on the constructor alone.

**One embedded-resource reader, and the caller names the assembly (since 2026-09-19).** It replaced the
per-kernel copies. Until then each kernel owned one, because `Assembly.GetExecutingAssembly()` inside a shared reader resolves to the shared
assembly, which embeds nothing. `Binacle.Data`'s `EmbeddedResourceFileProvider.ByPrefix(assembly, prefix)`
takes the assembly instead, so that failure cannot happen, and hands the manifest name back unsplit so each
data project keeps its own name shape. `Binacle.Lib.Data` reads through it since 2026-09-20.

**The manifest prefix names the purpose, not the assembly.** `ResultSelection.<case>.<file>`, following ViPaq's
`PackedData.<family>.<file>`, so an assembly rename cannot silently break the manifest. A broken manifest name
fails **silently** — verify with `strings <dll> | grep <prefix>` after any change here.

## Open

### O1 — the `Parallel*` processors are unreachable

`BinProcessorFactory.Create` and `CreateMultiAlgorithm` take `binCount` and `itemCount` and **ignore both** —
they always return the `Loop` variants. Nothing in `lib/src` or `api/src` constructs `ParallelBinProcessor` or
`ParallelAlgorithmProcessor`; only the benchmarks and one cancellation test do.

**`ParallelMultiAlgorithmBinProcessor` is not even that** — checked 2026-09-04, the only file that names it
is its own. It is a public type nothing anywhere constructs, so the measurement below does not cover it and
nothing would notice if it broke.

The signature promises a decision that is never made.

**And the measurement argues against wiring the algorithm one up.** On the set production actually uses
(FFD+BFD), parallel *algorithm* racing runs **0.93× to 1.48×** on the curated problems (`$lib/findings#F2`) and
**1.11× to 2.55× — always slower** over the whole item ladder (`$lib/findings#F2a`). It is only ahead where the
two algorithms take very unequal time. Two algorithms cap the win at 2× before overhead. **D1 is what makes
this so: the decision that makes racing cheap is the decision that makes parallelising it pointless.**

**`ParallelBinProcessor` does pay, above a threshold, and the threshold is now measured**
(`$lib/findings#F4`): 1 bin is always a loss (1.02× to 4.60×), 3 and 7 items never win at any bin count, and
from 13 items the crossover walks in with size — FFD wins from 7 bins at 17 items, 5 at 23, 2 at 47; BFD about
two bins earlier. At 79 items over 7 bins it reaches 0.60 (FFD) and 0.52 (BFD) on 12 cores, for 1.03× the
allocation.

**So a threshold is defensible, and it is a surface, not a number.** It needs both a bin count and an item
count, measured on the machine it will run on — a 12-core result says nothing about 4 cores, and F4 shows the
threshold moving *outward* as the algorithms get faster (at the small end v1 crosses over earlier than v2;
FFD at 37 and 47 items crosses earlier in v2). A wrong
threshold is worse than none: below it, parallel costs up to 4.6× on exactly the small requests the demo makes
(median 13 items).

**Undecided:** wire the bin threshold up, or delete the classes. F4 says what the threshold would have to be,
not whether the routes are worth the branch. Leaving three
unreachable processors in place invites someone to "fix" a path that never runs in production.

**The processors were guarded on 2026-10-02**, which changes what wiring up still needs. `concurrencyLevel`
left all three constructors and is worked out instead — one writer per work item, capped by the threads that
can run — and `maxDegreeOfParallelism` is now the one optional argument, defaulting to `ParallelLimits.Degree`,
which is `min(work items, Environment.ProcessorCount)` and never below 1. Pass `ParallelLimits.NoLimit` to lift
it. So the old objection, that `concurrencyLevel` never reached `MaxDegreeOfParallelism`, is answered.

**It is a cap, not a decision** (the maintainer, 2026-10-02: "the factory will choose but that we better guard
the processors"). No processor branches to a loop on its own; whether to parallelise at all stays with the
factory, in one place. What the cap prevents is the pathological case: on one CPU an uncapped race measured
1,231 ns against the loop's 74, so parallel was 16 times dearer than not bothering, and capped it is 296 ns —
still dearer than looping, which is why the factory and not the cap is what keeps production off that path.

**What a threshold still cannot see.** `BinProcessorFactory.Create` takes `binCount` and `itemCount`, and
`itemCount` is the item *lines* — one line of 500 pieces and one line of 2 look identical to it. Every drop
point measured so far is in pieces, so wiring one up means either giving `Create` the piece count (summing
quantities is a handful of nanoseconds against a 300 ns algorithm) or accepting that the case parallel wins
hardest is the case the rule cannot detect. **Suggested (agent, 2026-10-03):** the first.
