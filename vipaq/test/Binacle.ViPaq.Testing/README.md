# Binacle.ViPaq.Testing

What the ViPaq measure and bench projects share and nothing else needs: the harness's own encoders and
the scenarios it picks. Not a test project: nothing in it asserts on its own, and no test SDK is referenced.
The unit tests never reference it - they are the spec gate and must not lean on a rival encoder.

## 📂 What is in it

| Path | What it is |
|---|---|
| `ViPaq/` | `ViPaqEncoder` and `ViPaqHeader` - the door into ViPaq's internal encoder, so every codec and layout can be forced |
| `Protobuf/` | `ProtobufEncoder` and `packing.proto` - the rival the size reports compare against |
| `Json/` | `JsonEncoder` - the bin and placed items as the JSON a user's token replaces; text, not base64 |
| `Compact/` | `CompactEncoder` - the same in compact notation, items joined by `;`; text, not base64 |
| `EncoderInfo.cs` | Which layout to encode with, handed out as two ready-made instances |
| `BischoffTimingSet.cs`, `CustomProblemsTimingSet.cs` | The packs the timing benchmarks run on, one set per family, keyed by the column the report prints |
| `TimingSet.cs` | Those two plus the synthetic curve, joined in report order - what Encode and Decode run over |
| `CompressionCostSet.cs` | The two packs the CompressionCost benchmarks run on: the low and the high end of deflate's win |
| `SyntheticGenerator.cs` | Scenarios built from an item count, past any real pack. Speed and memory only, never size |

The packs themselves are in `Binacle.ViPaq.Data`; every set resolves its picks by name there, so a pick that
no longer exists is caught by the gate in `Binacle.ViPaq.EncodedSize` rather than mid-run. Each set answers
`Names` (the columns), `GetByName(column)` and `PackNames` (the picks behind them, for that gate).

## 🛠️ How you use it

```csharp
using Binacle.ViPaq.Testing;
using Binacle.ViPaq.Testing.ViPaq;

var scenario = TimingSet.GetByName(column);
var token = new ViPaqEncoder(codec).Encode(scenario, EncoderInfo.RowMajor);
var header = ViPaqHeader.Create(scenario, EncoderInfo.RowMajor);
```

## ⚠️ What will bite you

`ViPaqEncoder` drives internals of `Binacle.ViPaq`, so this project is a friend of it. A stale curated pick
fails in `CuratedPicksCheck` when `just measure vipaq` runs, before any report is written. The benchmarks do
not run that check, so there a stale pick fails inside the run.

Never use the synthetic scenarios for size or compression. Random data has nothing for a codec to grip, so it
reports the opposite of real behaviour.
