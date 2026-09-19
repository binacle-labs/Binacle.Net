---
description: Orchestrator - measured results get a home in each slice, and the three tests kernels become Data and Testing projects first. Fourteen steps, one file each, the maintainer commits between them
state: ready
waits-on: "nothing - shape agreed 2026-09-19. Step 1 can start; each step settles its open details with the maintainer before it touches a file"
horizon: next-release
paths:
  - "shared/**"
  - "lib/**"
  - "vipaq/**"
  - "api/test/**"
  - "tooling/**"
  - "results/**"
  - ".agents/**"
  - "Binacle.Net.slnx"
  - "Directory.Build.props"
  - ".netconfig"
  - ".gitignore"
  - "justfile"
---

# Measured results, and the projects that feed them

One plan, too big for one file. The maintainer granted the topic folder on 2026-09-19; this file is the only
one that points at every file in it.

Two shapes, both settled, both in `measured-results/`:

- Support projects - the three tests kernels dissolved into a `Data` project per data folder, a `Testing`
  project per slice and `Binacle.Reporting`. Steps 1 to 6, landed 2026-09-20; the shape file is gone and
  its rules are the general design record's D9.
- [`results.md`](measured-results/results.md) - deterministic numbers written by a harness into
  `<slice>/results/`, timing keepers dated per family, one benchmark project per question, two `just`
  modules, the old vault converted in. Steps 7 to 14.

Support projects go first because the bench split in step 12 multiplies every copy they remove.

## How a session works this plan

- **A session takes one step or several; the maintainer commits between steps.** No session commits. A step
  is sized to be one reviewable commit - a rename, a move, a split, a harness - never a line in three files
  and never two ideas. Where a step says it is small, the maintainer may take it with the one before.
- **A session says when it should stop.** Its judgement, not a count of steps: when its context is heavy, or
  when the next step deserves a cold read. It says so at a step boundary, never in the middle of one, and
  names the step a fresh session picks up. The maintainer decides whether to start a new session or go on.
- **Pick up cold by running the gates.** Every row below has a gate: one command that is true once the step
  landed. Run them top to bottom; the first that fails is the next step. There is no progress table - the
  tree is the state, and a tick nobody verified is a claim.
- **Settle the open details first.** Each step file lists what it leaves open. Work those out with the
  maintainer, then start. A session that finds the shape wrong when it meets the code says so with the
  evidence and stops; a wrong decision is cheaper to change than to obey.
- **The sandbox denies `mv`, `rm` and `git mv`.** Hand the maintainer the lines; edit after they land.
  History follows the bigger half of a split.
- **Build only what moved.** `dotnet build <csproj>` on the project and each consumer. A solution build or a
  long run is the maintainer's; both have crashed the machine.
- **Every step leaves a working tree**: every project builds, every script or recipe that exists still runs.
  A step that has to break something fixes it in the same step.
- **Every step rewrites the doc lines it makes false**, in the same step. `just agents all` is the
  maintainer's; say when it is due.

## The steps

| # | File | In one line | Gate |
|---|---|---|---|
| 1 | [01-binacle-data](measured-results/01-binacle-data.md) | `shared/test/Binacle.TestsKernel` becomes `shared/data/Binacle.Data`, one reader; then demo-samples gets a provider and tests | `test -d shared/data/Binacle.Data` |
| 2 | [02-binacle-lib-data](measured-results/02-binacle-lib-data.md) | `lib/test/Binacle.Lib.TestsKernel` becomes `lib/data/Binacle.Lib.Data`, no reader of its own | `test -d lib/data/Binacle.Lib.Data` |
| 3 | [03-binacle-lib-testing](measured-results/03-binacle-lib-testing.md) | new `Binacle.Lib.Testing` takes the factories, the checks, the providers; `Binacle.Data` keeps only interfaces and enums from `Packing` | `test -d lib/test/Binacle.Lib.Testing` |
| 4 | [04-binacle-vipaq-data-and-testing](measured-results/04-binacle-vipaq-data-and-testing.md) | the ViPaq kernel splits into `Binacle.ViPaq.Data` and `Binacle.ViPaq.Testing` | `test -d vipaq/data/Binacle.ViPaq.Data` |
| 5 | [05-binacle-reporting](measured-results/05-binacle-reporting.md) | `Binacle.TestReporting` becomes `Binacle.Reporting` - small | `test -d shared/test/Binacle.Reporting` |
| 6 | [06-support-projects-record](measured-results/06-support-projects-record.md) | the folder rules into the design record, the dependency docs redrawn, the shape file deleted | `test ! -f .agents/plans/measured-results/support-projects.md` |
| 7 | [07-measure-projects](measured-results/07-measure-projects.md) | both PerformanceTests move to `<slice>/measure/` under their new names, point at `<slice>/results/`, get `measure.just`; the memory and D3 go | `test -f tooling/measure.just` |
| 8 | [08-lib-packing-efficiency](measured-results/08-lib-packing-efficiency.md) | one run, many views: the runner, the bag, the reporters, the three lib files | `test -f lib/results/version-parity.md` |
| 9 | [09-vipaq-gates-and-json](measured-results/09-vipaq-gates-and-json.md) | the two round-trip gates become unit tests over `ViPaq.Data`; the curated check stays | `test ! -f vipaq/measure/Binacle.ViPaq.EncodedSize/PreReportChecks/ReportPathRoundTripCheck.cs` |
| 10 | [10-vipaq-encoded-size](measured-results/10-vipaq-encoded-size.md) | JSON and compact encoders join protobuf; the ViPaq runner and reporters, `encoded-size.md` with its text columns, the README | `test -f vipaq/results/encoded-size.md` |
| 11 | [11-the-vault](measured-results/11-the-vault.md) | root `results/` goes, `just measure all` regrows both slices, every doc and config line that named the vault is rewritten | `test ! -d results` |
| 12 | [12-bench-split](measured-results/12-bench-split.md) | six benchmark projects, the config in the `Testing` projects, `bench.just`, the two benchmark scripts gone | `test -f tooling/bench.just && test ! -f tooling/benchmarks.lib.sh` |
| 13 | [13-convert-the-vault](measured-results/13-convert-the-vault.md) | every old keeper under its family with a date and its real class; both `benchmarks/README.md` | `test -f lib/results/benchmarks/README.md` |
| 14 | [14-first-keepers](measured-results/14-first-keepers.md) | the scaling class and the JSON timing; then `lib-fast`, `vipaq-encoding`, the bin threshold once, and its finding | `ls lib/results/benchmarks/threshold/*.md` |

## Done when

- [ ] Step 14's last box is ticked, and the maintainer has deleted the folder and this file.
      **By eye.** There is nothing left to check once the file is gone; the step files carried the checks.
