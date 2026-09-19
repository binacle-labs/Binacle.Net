---
id: lib/decisions
description: Lib decisions ledger — why Algorithm.Best races a different set per path, where the packing vocabulary lives, why there are two data hubs, and the open parallelization question.
verified: 2026-09-20
check: Algorithm sets match AlgorithmProcessorFactory.Create and BinProcessorFactory.CreateMultiAlgorithm; the project and fixture layout matches lib/ and shared/, and the folders embedded by shared/data/Binacle.Data/Binacle.Data.csproj match the Keys arrays in its BischoffSuite/Scenarios.cs and CustomProblems/Scenarios.cs
also_update:
  - lib/findings
paths:
  - "lib/**"
---

# Lib — decisions ledger

Locked decisions and open questions for `lib/src`, with the *why*. Measured evidence lives in `$lib/findings`.
This file is the "what we settled and why", so a fresh session does not re-litigate it or "fix" a deliberate
choice.

## Locked

### D1 — `Algorithm.Best` races a different set depending on the path

**This is deliberate. Do not "align" the two.** The same parameter value means two things, on purpose:

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
`api/src`, `lib/src`, `lib/test` and the fixture kernels. That is why identity is packing vocabulary and not
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
the algorithm fixtures, which the api integration suite reads in 25 files as well as the lib tests.
`lib/data/Binacle.Lib.Data` (the lib tests kernel until 2026-09-20) holds result selection,
which **nothing outside the lib slice reads**, so its fixtures live in `lib/data` rather than `shared/data`.

**The rule that falls out:** a fixture set lives in `shared/data` when more than one slice reads it, and in the
slice otherwise. Bischoff and custom-problems qualify twice over — two slices read them through it, and
the ViPaq packed-data generator reads the same files by path at run time — so they stay put. ViPaq had already
settled this shape with its own `vipaq/data/packed`.

**`shared/data/demo-samples/` was added under the same rule.** The demo component generates its sample set
from it and the ViPaq packed-data generator packs it. Until 2026-09-20 `Binacle.Data` embedded it with no key
set, so nothing read the embed; it now has `DemoSamples.Scenarios`, is in `All`, and `PackingDemoSamplesTests`
runs every version of every algorithm over it. Its `Result` came from the packer, so that class is a
regression baseline, not an independent check.

**The friend grant is preferred to a shared bin model.** `OperationResultHelper` bridges through Packing's
internal `Dimensions` rather than taking `ScenarioBin` from `Binacle.Data`. `PackedBin`'s constructor is
internal, so the grant is needed either way; borrowing the type would add a dependency without removing one.
Since 2026-09-20 `Binacle.Lib.Data` references `Binacle.Data` anyway, for its reader - the argument now rests
on the constructor alone.

**One embedded-resource reader, and the caller names the assembly. Superseded 2026-09-19.** Until then each
kernel owned a copy, because `Assembly.GetExecutingAssembly()` inside a shared reader resolves to the shared
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

**And the measurement argues against wiring it up.** On the set production actually uses (FFD+BFD), parallel
*algorithm* racing runs **0.93× to 1.48×** — slower than `Loop` on the cheapest scenario, and only clearly
ahead where the two algorithms take very unequal time (`$lib/findings#F2`). Two algorithms cap the win at 2×
before overhead. **D1 is what makes this so: the decision that makes racing cheap is the decision that makes
parallelising it pointless.**

The untested axis is `ParallelBinProcessor` — many *bins* at once, which scales with bin count rather than
algorithm count. That is the one that could still pay, and it has no finding yet.

**Undecided:** wire the threshold up, or delete the classes. Leaving three unreachable processors in place
invites someone to "fix" a path that never runs in production. Also `ParallelBinProcessor.concurrencyLevel`
only sizes the `ConcurrentDictionary` — it never reaches `MaxDegreeOfParallelism`, so the name overpromises.
