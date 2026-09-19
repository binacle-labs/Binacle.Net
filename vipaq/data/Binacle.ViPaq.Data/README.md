# Binacle.ViPaq.Data

The packed results of `vipaq/data/packed` as C#. It embeds them and reads them into placed scenarios the ViPaq
harnesses run against. Not a test project: nothing in it asserts, no test SDK is referenced, and it does not
reference `Binacle.ViPaq` - it holds the inputs, not the encoders.

## 📂 What is in it

| Path | What it is |
|---|---|
| `Packed/` | One class per family - `BischoffSuite`, `CustomProblems`, `DemoSamples` - every pack of that family by name |
| `Scenario.cs` | A pack: the bin and its placed items, in `ushort` |
| `PackedDataReader.cs` | Reads a family's embedded files into scenarios |

The `Packed/` folders you see in the IDE are not on disk. The JSON lives in `../packed` and is linked in by the
csproj; regenerate it there, never edit it. A new file is embedded and read on its own.

## 🛠️ How you use it

```csharp
using Binacle.ViPaq.Data.Packed;

foreach (var scenario in BischoffSuite.All) { ... }
BischoffSuite.GetByName("OrLibrary_thpack1_2.ffd");
```

A scenario's name is `<problem>.<algorithm>`, so the same problem under three algorithms is three scenarios.

## ⚠️ What will bite you

The manifest name of each embedded file is `PackedData.<family>.<name>.<algorithm>.json`, set by `LogicalName`
in the csproj. The reader splits that name into its four parts, and a wrong name fails silently - the family
comes back empty. After touching the csproj, check with
`strings bin/Debug/net10.0/Binacle.ViPaq.Data.dll | grep PackedData`.

The embedded-resource reader is `Binacle.Data`'s; this project passes its own assembly to it.
