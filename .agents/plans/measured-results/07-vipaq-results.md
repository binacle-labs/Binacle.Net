---
description: Session 7 - pin the shape of each ViPaq results file with the maintainer, then fill them by hand from the kept runs
state: blocked
waits-on: "sessions 4 to 6 - the maintainer, 2026-09-29: 'waits on the rest'"
horizon: undecided
paths: ["vipaq/results/**"]
---

# 7 - The ViPaq results files

## Where it stands

The files in `vipaq/results/` are placeholders, like lib's: a comment per table as its spec, a sample table
with fake numbers under it.

## Goal

**(the maintainer, 2026-09-29):** the shapes are not pinned down yet - "some sample files exist in /lib/results/
but not pinned down same holsds true for vipaq". Pin each file's shape first, then fill it by hand, as lib's
("6 is manual for now").

## The decisions the files serve - suggested (agent), to reinvestigate

1. Is the format worth having - how much smaller than the alternatives?
2. Is encode and decode speed acceptable, or worth work?
3. Should the default layout stay row, or switch to columnar?
4. Should tokens be compressed, with which codec, and from what pack size?

One file per question, or per layout and codec; the README combines them.

## Fact

**Row is the default layout** (`ViPaqSerializationOptions.Layout = RowMajor`) and the API sets no other, so row
is what gets sent. The earlier README said the wire carries columnar; it was wrong.

## Rules particular to ViPaq - suggested (agent), to reinvestigate

The main plan's rules hold the maintainer's own: algorithms never mixed, a loss shows.

- **Size, per pack, then averaged.** ViPaq ÷ the other format on that one pack, then the mean per group. Rows
  thpack1..7, custom problems, demo samples, All, with a Packs column. Each size file carries its tables once
  per algorithm - FFD, then WFD, then BFD.
- **A loss shows** this way: any average above 1.00× is bold, and a column "ViPaq larger than protobuf on"
  counts the packs where ViPaq lost. A JSON count only if some pack has ViPaq larger than JSON.
- **Widths everywhere**, because width drives ViPaq's size and cost. `Widths` is bin / item / coordinate bits,
  e.g. `16/8/16`.
- **Cost files hold cost only.** No size column in a cost file, even for the same pack; the README puts size
  saved beside time paid.
- **Say how a number was made when it is not obvious**: compact notation joins the bin and the items with `;`
  because it has no whole-pack form.

## Open points in the shapes - suggested (agent, 2026-09-26), to reinvestigate

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

## Gaps each file names - suggested (agent), to reinvestigate

- Compression cost is measured on two packs only; nothing says from what size compressing pays for its time.
- No columnar protobuf, MessagePack or CBOR.
- ViPaq has no "before" to show its direction is sound, the way v1 against v2 does for lib. Is there an earlier
  format worth measuring, or is "against protobuf" the whole story?
- Session 8's README must not claim "smallest" from `format-size.md` alone. A run on 2026-09-25, reverted the same
  day, measured canonical MessagePack at 938 mean characters against ViPaq's 954 raw; ViPaq won clearly only
  compressed and columnar (304 against 383 deflated).

## What the removed ViPaq README said (2026-09-22) - suggested (agent), to check new numbers against

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

- [ ] The maintainer pinned each file's shape.
      **By eye.** His yes for each file, quoted and dated, in this file.
- [ ] Every ViPaq results file has real numbers, filled by hand, and its words and gaps are written from them.
      `! grep -l "is fake" vipaq/results/*.md`, then **by eye**.
- [ ] The old numbers above are used to check the new ones, or dropped with a reason.
      **By eye.**
