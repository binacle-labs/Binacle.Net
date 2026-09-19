# Binacle.ViPaq.Testing

What the ViPaq benchmarks and performance tests share and nothing else needs: the harness's own encoders and
the scenarios it picks. Not a test project: nothing in it asserts on its own, and no test SDK is referenced.
The unit tests never reference it - they are the spec gate and must not lean on a rival encoder.

## 📂 What is in it

| Path | What it is |
|---|---|
| `ViPaq/` | `ViPaqEncoder` and `ViPaqHeader` - the door into ViPaq's internal encoder, so every codec and layout can be forced |
| `Protobuf/` | `ProtobufEncoder` and `packing.proto` - the rival the size reports compare against |
| `EncoderInfo.cs` | Which layout to encode with, handed out as two ready-made instances |
| `Providers/` | The curated picks per family, the merged curated set, and the synthetic scenarios for the speed benchmarks |

The packs themselves are in `Binacle.ViPaq.Data`; the curated providers resolve their picks by name there.

## 🛠️ How you use it

```csharp
using Binacle.ViPaq.Testing.Providers;
using Binacle.ViPaq.Testing.ViPaq;

var scenario = CuratedScenarioProvider.GetScenarioByName(name);
var token = new ViPaqEncoder(codec).Encode(scenario, EncoderInfo.RowMajor);
var header = ViPaqHeader.Create(scenario, EncoderInfo.RowMajor);
```

## ⚠️ What will bite you

`ViPaqEncoder` drives internals of `Binacle.ViPaq`, so this project is a friend of it. A stale curated pick
fails in `CuratedPicksCheck` before any benchmark runs, not inside one.

Never use the synthetic scenarios for size or compression. Random data has nothing for a codec to grip, so it
reports the opposite of real behaviour.
