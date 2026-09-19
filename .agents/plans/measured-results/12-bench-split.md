---
description: Step 12 - one benchmark project per question, six of them, the config in the two Testing projects, bench.just, the two benchmark scripts gone
state: ready
waits-on: "step 11's gate"
horizon: next-release
paths: ["lib/**", "vipaq/**", "tooling/**", "Binacle.Net.slnx", "justfile"]
---

# Step 12 - the bench split

Shape: [results.md](results.md), "Benchmark projects" and "Recipes". Protocol: the orchestrator.

## The step

- `lib/test/Binacle.Lib.Benchmarks` becomes four projects under `lib/bench/`: `.Algorithms` (FastValidation
  and BischoffSuite in tiers), `.ResultSelection`, `.Racing`, `.Threshold`. `vipaq/test/Binacle.ViPaq.Benchmarks`
  becomes two under `vipaq/bench/`: `.Encoding` (curated encode, decode, CompressionCost) and `.Scale`
  (synthetic at 2,000 and 5,000). No new benchmark class here: the scaling curve and the JSON timing are
  step 14's.
- The BDN config moves into `Binacle.Lib.Testing` and `Binacle.ViPaq.Testing`, one copy each, and every
  bench project calls it. `AttributeOrderer` and `BenchmarkOrderAttribute` go with the lib copy.
- The friend grants: `Binacle.Lib` and `Binacle.ViPaq` each grant `$(ProjectName).Benchmarks` today, and
  `CompressionCostBenchmarks` constructs internal codecs. The one grant becomes one per bench project that
  needs it.
- Naming pass: one class-name rule, `<Family>_<Operation>_<Variant>`; one namespace per project; the file
  `Benchmarks/Fitting/FastValidation/FastValidation_SpecializedBaseline_Packing.cs` holds the `_Fitting`
  class and is renamed; `Generator.cs` is unused and goes; every `_v1` baseline carries one comment saying
  it is deleted with v1.
- `tooling/bench.just`: `default` lists each project with its cost; one recipe per project, the family is
  the recipe name; `lib-fast`, `lib-all` (says "hours" first), `vipaq-all`; `*args` passes BDN flags after
  `--`. `benchmarks.lib.sh` and `benchmarks.vipaq.sh` are absorbed and deleted. The root `justfile` gets the
  `mod` line and loses "benchmark and performance runs are still shell scripts".
- `Binacle.Net.slnx`: `/lib/bench/` and `/vipaq/bench/` folders. Docs: the lib tests doc (its class list
  and aliases), the tooling doc, the commands doc, both slice READMEs, a README in each `bench/` folder.
- Two commits: the lib split and the vipaq split, each with its half of `bench.just`. The tree builds after
  each.

## Open before starting

- How the full 700-scenario tier in `lib-algorithms` is named: a BDN filter, a second recipe, or an
  environment variable. Whichever, the full run never happens without being asked.
- The BDN mechanics the shape names: `--join`, `--exporters json` for anything a script reads, and whether a
  `Job` is set at all (none is today).

## Done when

- [ ] `ls lib/bench` lists `Binacle.Lib.Benchmarks.Algorithms Binacle.Lib.Benchmarks.ResultSelection Binacle.Lib.Benchmarks.Racing Binacle.Lib.Benchmarks.Threshold`;
      `ls vipaq/bench` lists `Binacle.ViPaq.Benchmarks.Encoding Binacle.ViPaq.Benchmarks.Scale`.
- [ ] `ls lib/test vipaq/test` lists no `Benchmarks`.
- [ ] `grep -r "^namespace" lib/bench vipaq/bench --include=*.cs | awk -F: '{split($1,p,"/"); print p[2], $2}' | sort -u`
      shows one namespace per project.
- [ ] `grep -l "ManualConfig" lib/bench/*/Program.cs vipaq/bench/*/Program.cs` is empty - the config is in the
      two `Testing` projects.
- [ ] `grep -c "Benchmarks\." vipaq/src/Binacle.ViPaq/Binacle.ViPaq.csproj` is the number of vipaq bench projects
      that touch internals, and `Benchmarks"` alone appears nowhere.
- [ ] `test -f tooling/bench.just && test ! -f tooling/benchmarks.lib.sh && test ! -f tooling/benchmarks.vipaq.sh`
- [ ] `just bench` lists every project with a cost; `just bench lib-result-selection` runs to a report.
- [ ] `grep -n "still shell scripts" justfile` is empty; `grep -n "benchmarks\." tooling/README.md` is empty.
