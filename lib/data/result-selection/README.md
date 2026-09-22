# Result Selection

Hand-authored result-selection scenarios, embedded by `Binacle.Lib.Data`. Read by the lib unit tests and
`lib/bench/Binacle.Lib.Benchmarks.ResultSelection` only - not by the api suite, and not by ViPaq.

These fixtures exercise how the lib picks a single winning result out of many candidate packings. Unlike the
algorithm fixtures (Bischoff suite, custom-problems) these do not describe a packing problem. Each case lists a set
of already-computed results and the one the selector is expected to choose. One folder per selector:

- `BestAlgorithm/` - pick the best result across algorithms.
- `BestBin/` - pick the best bin.
- `SmallestBin/` - pick the smallest bin that still fits.

Each folder has a single `baseline.json` today, so coverage is thin. A new JSON file is embedded on its own, but
is not read until its key is added to that set's `Scenarios.Keys` in `Binacle.Lib.Data`.

## 🧾 Format

A JSON array of scenarios. Each scenario names the expected winner and the candidate results to choose from:

```json
{
  "Name": "one full winner",
  "ExpectedResult": "60x40x30",
  "Results": {
    "60x40x10": "60x40x10 FFD_v2 PartiallyPacked 40 60",
    "60x40x20": "60x40x20 FFD_v2 PartiallyPacked 80 70",
    "60x40x30": "60x40x30 FFD_v2 FullyPacked 95 100"
  }
}
```

- `Name` - short, and the benchmark's column header as is. Names repeat across folders, so a name is looked
  up through its folder's `Scenarios` class, never across all three.
- `ExpectedResult` - the bin key the selector under test must choose.
- `Results` - candidate results keyed by bin; each value is a compact operation result
  `Bin Algorithm PackingStatus <metric> <metric>`.

This set uses its **own** provider/reader/model (`ResultSelection/ScenarioCollectionsProvider.cs`, its own
`Scenario` model and the set classes), a different shape from the algorithm fixtures; the two are kept separate.

This folder is the single source: `Binacle.Lib.Data` embeds these files directly (via `Link`/`LogicalName` in
`Binacle.Lib.Data/Binacle.Lib.Data.csproj`) under the manifest name `ResultSelection.<Case>.<file>`, so there is
no separate copy.

It lives in the `lib` slice because the `lib` slice is its only consumer. The fixture sets that more than one
slice reads stay in `shared/data`.
