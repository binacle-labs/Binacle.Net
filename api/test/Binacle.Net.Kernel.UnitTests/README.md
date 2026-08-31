# Binacle.Net.Kernel unit tests

One folder per Kernel feature, each holding its own `Tests/` and `Providers/`. The Kernel is shared by every
module, so a feature's tests stay next to that feature and nothing reaches across.

| Folder | Covers |
|---|---|
| `Logs/` | `LogsProcessor` and `LogsRetentionProcessor` - what reaches disk, and what retention will and will not delete |
| `Network/` | `IPEntry` - how a configured IP entry is read, and what each spelling admits |
| `OpenApi/` | The eleven document, operation and schema transformers - what each one writes into the generated document |
| `Paths/` | `ReservedPathOptions` - which paths modules reserve, and what each prefix matches |
| `Serialization/` | `JsonStringNullableEnumConverter` - how a request enum value is read, and what an unknown one does |
