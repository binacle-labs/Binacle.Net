# ViPaq data

The frozen packing results the ViPaq harnesses measure against, and the project that reads them. They live
here and not in `shared/data` because ViPaq is their only reader, and because they carry placed items with
coordinates, which the shared scenario format does not.

| Folder | What it is |
|---|---|
| [`packed/`](packed/README.md) | Generated placed results, one file per problem and algorithm. Regenerate, never edit. |
| [`Binacle.ViPaq.Data/`](Binacle.ViPaq.Data/README.md) | The C# project that embeds `packed/` and reads it into scenarios. |
