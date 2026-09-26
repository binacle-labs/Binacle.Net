---
description: Measure MessagePack, CBOR and a columnar protobuf beside ViPaq in the encoded-size files, in the form people actually use, and lean the JSON baseline
state: idea
waits-on: "nobody - it is an idea. horizon: undecided, an agent did not judge the distance"
horizon: undecided
paths:
  - "vipaq/test/Binacle.ViPaq.Testing/**"
  - "vipaq/measure/Binacle.ViPaq.EncodedSize/**"
  - "vipaq/results/**"
---

# Measure the off-the-shelf binary formats

"Why not just use something standard instead of your own format?" has no answer in the measurements. Protobuf
is there, but it needs a schema and code generation, which is easy to argue against. MessagePack and CBOR need
neither, so they are what a caller actually reaches for, and neither is measured. Adding both as columns in the
encoded-size files answers the question with numbers instead of an argument.

## What was tried, and why it was pulled back

Built and run on 2026-09-25, then reverted the same day - the comparison set was growing faster than the story
it was meant to support, and the results README could not hold both. Nothing below needs re-deriving.

- **Encode them the way the libraries steer you, or the numbers are worthless.** Written first as maps with
  the JSON's field names, MessagePack measured a mean of 3,703 characters and CBOR 4,110 against ViPaq's 954 -
  a flattering result and a wrong one. The canonical form is integer keys, which serialize as positional
  arrays with no names on the wire: **MessagePack 938, CBOR 1,345**. The string-keyed form is the opt-in
  convenience mode, not the default.
- **Canonical MessagePack is smaller than an uncompressed ViPaq token** - 938 against 954, ViPaq 1.5% larger.
  MessagePack spends one byte on any value under 128; ViPaq picks one width for the whole token and pays it on
  every coordinate.
- **The columnar layout is what ViPaq actually wins on.** Deflated, ViPaq columnar is 304 against MessagePack's
  383, but ViPaq *row* is 362 - a 5.8% edge. The gap widens with pack size: 14% under 10 items, 53% over 200.
  Worst packing, deflated: ViPaq 704, MessagePack 1,308.
- **Packages:** `MessagePack` 3.1.10 and `System.Formats.Cbor` 10.0.12, both in the support-libraries group.
  Write through `MessagePackWriter` and `CborWriter` rather than the attribute path, so the shape stays visible
  beside the JSON encoder's.
- **A namespace ending in `MessagePack` hides the package's.** `using MessagePack;` binds to the local one;
  qualify the writer type.
- **Verify by size, against the specs.** An independent model of both specs - container headers, string
  headers, shortest integer forms - predicted all 2,322 packings to the character, twice. Neither encoder has a
  decoder, so nothing round-trips; that gap stays until one is written.

## A columnar protobuf

The protobuf baseline is a row message only - `vipaq/test/Binacle.ViPaq.Testing/Protobuf/packing.proto` says so
in its own comment. ViPaq's compressed win is largest in the columnar layout, and protobuf never got that
layout, so part of that win is the layout, not the format. A columnar message - one packed repeated field per
dimension and per coordinate - is the harder, smaller baseline. Measure it beside the row one, through the same
three codecs. Raised by the maintainer, 2026-09-26.

## Two smaller things that rode along

- **Lean the JSON baseline.** The measured JSON spells out `length`, `width` and `x`, which invites the charge
  that it is a strawman. One-letter minified JSON measures 3,582 against 4,702, moving ViPaq's lead from 80% to
  74%. Going further - bare arrays, no keys - lands within 1% of compact notation, so that is the floor and
  compact already measures it.
- **Rename the `Compact` column to `CN`**, and say in the table caption that every rival is at its smallest
  sensible setting.

## What it costs elsewhere

The size files at the root of `vipaq/results/` - `format-size.md`, `row-*.md`, `columnar-*.md` - tell the size
story against JSON, compact notation and protobuf. Every headline in them moves if this lands, and the honest framing changes with them: the claim stops being "smallest" and becomes
"holds its shape as packings grow, and has a ceiling nothing else has".
