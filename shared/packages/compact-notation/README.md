# compact-notation

`@binacle/compact-notation`. Parses and writes the compact strings for bins and items - `"60x40x30"` for a
size, `"108x76x30 [40]"` for forty items of that size. The TypeScript twin of the C# parser in
[`shared/src/Binacle.CompactNotation`](../../src/Binacle.CompactNotation). Apache-2.0, like the C# one.

## 📂 What is in it

| Folder or file | What it is |
|---|---|
| `src/compactNotation.ts` | The `parse` functions, the `format` functions, and `detect`, which says which block a string is |
| `src/types.ts` | `Dimensions`, `Coordinates` and `Item`, as plain shapes |
| `index.ts` | The single entry point |
| `tests/` | The jest tests |

## 🚀 How you use it

```ts
import {parseItems, formatDimensions} from "@binacle/compact-notation";

parseItems("108x76x30 [40]");   // forty items
formatDimensions({length: 60, width: 40, height: 30});
```

## 🧪 Tests

```bash
just test ts_binacle-compact-notation_unit
```

## ⚠️ What will bite you

**It has to agree with the C# parser by hand.** Nothing generates one from the other. A change to the grammar
is made in both, with a test on each side.
