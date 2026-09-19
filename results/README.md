# Results

The old hand-kept records: benchmark keepers and the last hand-copied size, fill and timing reports.
Everything measured today is written by the measure projects into [`lib/results/`](../lib/results) and
[`vipaq/results/`](../vipaq/results), where a change is a diff and nothing is copied by hand.

| Folder | What |
|---|---|
| [lib/](lib/) | `Binacle.Lib` - dated BenchmarkDotNet keepers, and the last fill and timing reports |
| [vipaq/](vipaq/) | `Binacle.ViPaq` - the last size and crossover reports; the benchmarks folder is empty |

These stay until the benchmark projects have run and have their own keeper folders under
`lib/results/benchmarks/` and `vipaq/results/benchmarks/`. Each file here is then filed under its family with
its date, or dropped. Nothing writes here and nothing reads it at build time.

## 📊 Comparing numbers honestly

Only compare within the same ruler. Speed and time depend on the runtime and the machine. A Windows run and a
Linux run, or net9 and net10, are not the same measurement. Fill rate and encoded size are stable across
machines and can be compared freely.
