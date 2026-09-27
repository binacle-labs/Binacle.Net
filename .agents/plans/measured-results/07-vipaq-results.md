---
description: Session 7 - fill the seven ViPaq results files from the kept runs, the same way the lib files were filled; the table shapes are in the files, as comments over fake sample tables
state: blocked
waits-on: "session 6 - how the numbers get in"
horizon: undecided
paths: ["vipaq/results/**"]
---

# 7 - The ViPaq results files

## Where it stands

The seven files in `vipaq/results/` are placeholders, like lib's: a comment per table as its spec, a sample table
with fake numbers under it. The numbers get in the way session 6 settled.

## The decisions the files serve

1. Is the format worth having - how much smaller than the alternatives?
2. Is encode and decode speed acceptable, or worth work?
3. Should the default layout stay row, or switch to columnar?
4. Should tokens be compressed, with which codec, and from what pack size?

One file per question, or per layout and codec; the README combines them.

## The raw files

| File | Holds |
|---|---|
| `vipaq/results/measurements/encoded-size/<ffd|wfd|bfd>/<row|columnar>-<group>.md` | 54 files; groups `thpack1`..`thpack7`, `custom-problems`, `demo-samples`. Three tables each - `## Raw`, `## Deflate`, `## Gzip` - one row per pack: Scenario, Items, Widths, ViPaq, Proto, JSON, Compact, ViPaq/Proto. Lengths in characters: ViPaq and protobuf as base64, JSON and compact as text |
| `vipaq/results/benchmarks/<run>/encoding/Sample_Encode.md` | 12 packs; Protobuf (baseline), ViPaq_Row, ViPaq_Columnar, Json |
| `.../Sample_Decode.md` | the same packs, no Json - the test JSON encoder cannot decode |
| `.../Sample_CompressionCost_Encode.md`, `_Decode.md` | two real FFD packs, row-major: thpack4_1 (low win) and thpack1_2 (high win); NoOp (baseline), Deflate, Gzip |

Two categories, and no file crosses them: **size** (`measurements/encoded-size/`) feeds `format-size.md` and
the four codec files; **encoding** (`benchmarks/<run>/encoding/`) feeds `encode-cost.md` and `decode-cost.md`.

The cost files read the kept sample run.

## Rules particular to ViPaq

- **Size, per pack, then averaged.** ViPaq ÷ the other format on that one pack, then the mean per group. Rows
  thpack1..7, custom problems, demo samples, All, with a Packs column. **Algorithms are never
  mixed** (the maintainer, 2026-09-26): each size file carries its tables once per algorithm - FFD, then WFD,
  then BFD.
- **A loss shows.** Any average above 1.00× is bold, and a column "ViPaq larger than protobuf on" counts the
  packs where ViPaq lost. A JSON count only if some pack has ViPaq larger than JSON. Leaning
  yes (the maintainer, 2026-09-26: "yes both.,,, i think for now").
- **Widths everywhere**, because width drives ViPaq's size and cost. `Widths` is bin / item / coordinate bits,
  e.g. `16/8/16`.
- **Row is the default layout** (`ViPaqSerializationOptions.Layout = RowMajor`) and the API sets no other, so
  row is what gets sent. The earlier README said the wire carries columnar; it was wrong.
- **Cost files hold cost only.** No size column in a cost file, even for the same pack; the README puts size
  saved beside time paid.
- **Say how a number was made when it is not obvious**: compact notation joins the bin and the items with `;`
  because it has no whole-pack form.

## Open points in the shapes, found 2026-09-26

Each is the maintainer's to settle, one per turn.

- **The "All" row** of a size table now holds one algorithm's 774 packs, not all 2,322.
- **Raw row and raw columnar are the same length** on all 2,322 packs (checked 2026-09-26). The question in
  `format-size.md` is answered, and the Raw table of every `columnar-*` measurement file is an exact copy of the
  row file's. Drop it from the measure output, or keep it so each layout has its own file?
- **Unused measure columns:** Compact in the Deflate and Gzip tables, and ViPaq/Proto everywhere (a rounded copy
  of what the tables compute). Drop, or keep? Items is unused too and stays as context.
- **The protobuf gap in the codec files.** "Part of the win is the layout" fits the columnar files only; the row
  files say "no columnar protobuf".
- **Which file owns** "no MessagePack or CBOR" (now in all five size files) and "no earlier format" (now in
  `format-size.md` only).
- **The one-process gap** is named in `encode-cost.md` but not `decode-cost.md`. Slow processes are not
  chased, so the gap stays.
- **Kind** (real or synthetic) is not in the report; it comes from the pack name.

## Gaps each file names

- Compression cost is measured on two packs only; nothing says from what size compressing pays for its time.
- No columnar protobuf, MessagePack or CBOR.
- ViPaq has no "before" to show its direction is sound, the way v1 against v2 does for lib. Is there an earlier
  format worth measuring, or is "against protobuf" the whole story?
- Session 8's README must not claim "smallest" from `format-size.md` alone. A run on 2026-09-25, reverted the same
  day, measured canonical MessagePack at 938 mean characters against ViPaq's 954 raw; ViPaq won clearly only
  compressed and columnar (304 against 383 deflated).

## What the removed ViPaq README said (2026-09-22)

Kept as a check on the new numbers, not as a source. Every number here was typed; recompute before use.

| Format | Mean | Max |
|---|---|---|
| JSON | 4,702 | 22,048 |
| Compact notation | 1,669 | 7,904 |
| Protobuf raw | 1,472 | 7,168 |
| Protobuf deflate | 529 | 1,656 |
| ViPaq deflate, row | 362 | 1,248 |
| ViPaq deflate, columnar | 304 | 704 |

- ViPaq deflate columnar as a share of: JSON 7% (1-22%), compact 21%, protobuf raw 24%, protobuf deflate 58%.
- ViPaq over protobuf under the same codec: raw 0.65, deflate row 0.68, deflate columnar 0.58, gzip 0.60-0.70.
- Deflate is smallest on 2,265 packs (row) and 2,275 (columnar); gzip never.
- Every real pack deflates to at most 1,248 base64 characters (row) or 704 (columnar).
- Compression pays from very small packs, but not always: raw still wins on some packs up to 6 items (row)
  and 2 (columnar). The removed README said "1 item", which was base64 rounding on one demo pack - do not
  repeat it.
- Missing from the removed README: gzip rows in the per-format table.

## Done when

- [ ] Every ViPaq results file has real numbers, and its words and gaps are written from them.
      `! grep -l "is fake" vipaq/results/*.md`, then **by eye** - every number in the words is in a table.
