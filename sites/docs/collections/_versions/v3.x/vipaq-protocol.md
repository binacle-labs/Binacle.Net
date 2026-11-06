---
title: ViPaq Protocol
description: >-
  ViPaq packs one bin and its items into a single copy-pastable string. What it carries, and the wire format
  as Binacle.Net writes and reads it: header, body and compression. The format changed in v3.0.0.
nav:
  order: 9
  icon: 🗜️
status:
  - label: Format 0
  - label: Stable from
    value: v3.0.0
---

**ViPaq** packs one bin and its items into a single copy-pastable string.
{:.lede}

Binacle.Net's packing responses get large with many items; ViPaq condenses one result - a single bin plus the
items packed into it - into a string that is small to store, cheap to send and easy to share. It carries:

- **Bin**: its dimensions - Length, Width, Height
- **Items**: each item's dimensions (L, W, H) and its position in the bin (X, Y, Z)

That is everything needed to redraw the packing, and nothing else. Items come back in the order they were sent.
This page is the wire format as Binacle.Net {{ page.version_label }} writes and reads it.

> 🚨 **The format changed in v3.0.0.** Strings produced by v2.1.1 and earlier do not decode here, and strings
> produced here do not decode there. There is no fallback reader. An old string is rejected with a format error
> rather than misread, so the failure is visible - but stored strings are worth regenerating after the upgrade.
{: .block-warning}

## 📌 Structure

A ViPaq blob is a two byte header followed by a body:

<div class="bit-diagram" style="--cols:4" role="img" aria-label="A two byte header, never compressed, then the body, compressed if smaller." markdown="0">
{% include bit-field.html size="2 B" name="Header" note="Never compressed" %}
{% include bit-field.html size="varies" name="Body" note="Compressed if smaller" span=3 %}
</div>

- The header is **never compressed**. The body is built first, compressed if that makes it smaller, and the
  header is put in front afterwards.
- Every blob is therefore self-describing: a decoder reads the two header bytes, then reads the body exactly as
  they say.
- All multi-byte integers are **unsigned** and **little-endian**.

## 🧾 Header

Two bytes, one job each. Byte 0 says **how to read** the body, byte 1 says **how wide** its integers are.

##### Byte 0 - form

<div class="bit-diagram" role="img" aria-label="Byte 0: bits 7 and 6 Version, bit 5 Compressed, bit 4 Layout, bits 3 to 0 reserved." markdown="0">
{% include bit-field.html size="7-6" name="Version" note="0 for this format" span=2 %}
{% include bit-field.html size="5" name="Compressed" note="raw or compressed" %}
{% include bit-field.html size="4" name="Layout" note="rows or columns" %}
{% include bit-field.html size="3-0" name="reserved" note="always 0" span=4 kind="reserved" %}
</div>

| Bits | Field | Values |
|---|---|---|
| `7-6`{:.mono} | `Version`{:.mono} | `0` for this format. `1`-`3` are reserved. |
| `5`{:.mono} | `Compressed`{:.mono} | `0` the body is raw, `1` the body is compressed |
| `4`{:.mono} | `Layout`{:.mono} | `0` row-major, `1` columnar |
| `3-0`{:.mono} | `reserved`{:.mono} | always `0`, a decoder rejects anything else |

##### Byte 1 - integer widths

<div class="bit-diagram" role="img" aria-label="Byte 1: bits 7 and 6 BinDimensionsWidth, 5 and 4 ItemDimensionsWidth, 3 and 2 ItemCoordinatesWidth, 1 and 0 reserved." markdown="0">
{% include bit-field.html size="7-6" name="BinDimensionsWidth" note="bin L, W, H" span=2 %}
{% include bit-field.html size="5-4" name="ItemDimensionsWidth" note="item L, W, H" span=2 %}
{% include bit-field.html size="3-2" name="ItemCoordinatesWidth" note="item X, Y, Z" span=2 %}
{% include bit-field.html size="1-0" name="reserved" note="always 0" span=2 kind="reserved" %}
</div>

| Bits | Field |
|---|---|
| `7-6`{:.mono} | `BinDimensionsWidth`{:.mono} |
| `5-4`{:.mono} | `ItemDimensionsWidth`{:.mono} |
| `3-2`{:.mono} | `ItemCoordinatesWidth`{:.mono} |
| `1-0`{:.mono} | reserved, always `0` |

`Compressed` and `Layout` describe that one blob, not the format. An encoder picks them per blob and a decoder
obeys them, so two blobs from the same version can differ in both.

## 📦 Body

The body carries the item count, the bin dimensions, then the items:

<div class="bit-diagram" style="--cols:9" role="img" aria-label="The body: a two byte item count, the bin dimensions, then the items." markdown="0">
{% include bit-field.html size="2 B" name="Item count" note="uint16" span=2 %}
{% include bit-field.html size="3 or 6 B" name="Bin L, W, H" note="Bin width" span=3 %}
{% include bit-field.html size="varies" name="Items" note="Item widths" span=4 %}
</div>

- **Item count** is always a 2 byte `uint16`, whatever the width fields say.
- **Bin dimensions** are Length, Width, Height, in that order.
- Items come back in the order they were sent.

`Layout` decides only how the items are arranged. The count and the bin dimensions are the same either way.

##### Row-major (`Layout = 0`)

Each item is written whole - its three dimensions, then its three coordinates - before the next item.

<div class="bit-diagram" style="--cols:8" role="img" aria-label="Row-major: one item as L, W, H, X, Y, Z in turn, then the next item, and so on." markdown="0">
{% include bit-field.html size="1 or 2 B" name="L" note="dimension" %}
{% include bit-field.html size="1 or 2 B" name="W" note="dimension" %}
{% include bit-field.html size="1 or 2 B" name="H" note="dimension" %}
{% include bit-field.html size="1 or 2 B" name="X" note="coordinate" %}
{% include bit-field.html size="1 or 2 B" name="Y" note="coordinate" %}
{% include bit-field.html size="1 or 2 B" name="Z" note="coordinate" %}
{% include bit-field.html size="repeats" name="next item" note="same six fields" span=2 kind="more" %}
</div>

##### Columnar (`Layout = 1`)

Each field is written for every item before moving on to the next field. Six runs, each as long as the item count.

<div class="bit-diagram" style="--cols:6" role="img" aria-label="Columnar: six runs in turn, L, W, H, X, Y, Z, each as long as the item count." markdown="0">
{% include bit-field.html size="count values" name="L L L ..." note="dimension width" %}
{% include bit-field.html size="count values" name="W W W ..." note="dimension width" %}
{% include bit-field.html size="count values" name="H H H ..." note="dimension width" %}
{% include bit-field.html size="count values" name="X X X ..." note="coordinate width" %}
{% include bit-field.html size="count values" name="Y Y Y ..." note="coordinate width" %}
{% include bit-field.html size="count values" name="Z Z Z ..." note="coordinate width" %}
</div>

Columnar puts values of similar size next to each other, which usually compresses better. Row-major is easier to
read in a hex dump. Neither is required - the header bit says which was used.

## 🔢 Integer Widths

Each of the three sections - bin dimensions, item dimensions, item coordinates - is stored at one width, chosen
independently:

<table markdown="0">
  <thead><tr><th>Code</th><th class="table__number">Bytes per integer</th><th class="table__number">Largest value</th></tr></thead>
  <tbody>
    <tr><td class="mono">0</td><td class="mono table__number">1</td><td class="mono table__number">255</td></tr>
    <tr><td class="mono">1</td><td class="mono table__number">2</td><td class="mono table__number">65,535</td></tr>
    <tr><td class="mono">2, 3</td><td class="table__number table__off">reserved</td><td class="table__number table__off">a decoder rejects these</td></tr>
  </tbody>
</table>

Widths are fixed per section, not per value: every item shares one dimensions width and one coordinates width.
An encoder picks the smallest width that holds the largest value in the section.

The three sections are sized separately because real data needs them to be. A bin of `5000x2000x2000` holding
items no larger than `200` gives 16 bit bin dimensions, 8 bit item dimensions and 16 bit coordinates.

## 🗜️ Compression

The `Compressed` header bit records what the encoder did. It is not a hint to guess at.

- `Compressed = 0` - the bytes after the header are the body.
- `Compressed = 1` - the bytes after the header are a compressed stream holding the body.

The codec is **raw DEFLATE** ([RFC 1951](https://www.rfc-editor.org/rfc/rfc1951)), with **no** zlib or gzip
wrapper. The compressed stream is the DEFLATE bit stream and nothing else.

Compression is not applied by size. The encoder compresses the body, keeps whichever of the two is shorter, and
sets the bit to say which it kept - so a blob is never made larger by compressing it. A decoder accepts both.

## 🔤 Text Form

The stored and shared artifact is **base64**, and the format is optimized for it.

- Standard base64 ([RFC 4648 §4](https://www.rfc-editor.org/rfc/rfc4648#section-4)), alphabet `A-Z a-z 0-9 + /`,
  padded with `=`. Not the URL-safe alphabet.
- No line wrapping and no whitespace.

## 📏 Limits

| Field | Rule |
|---|---|
| Bin dimensions L, W, H | `1 to 65,535`{:.mono} |
| Item dimensions L, W, H | `1 to 65,535`{:.mono} |
| Item coordinates X, Y, Z | `0 to 65,535`{:.mono}. Zero is valid - an item flush to the bin origin. |
| Item count | `up to 65,535`{:.mono} |

A value outside these ranges is an error when encoding. Nothing is clamped or widened silently.

## 🧪 Example

A bin of `10x20x30` holding one item `1x2x3` at `(4,5,6)`.

Every value fits in a byte, so all three sections use the 1 byte width, and the encoder wrote it row-major and
uncompressed. Both header bytes are therefore `0x00`:

<div class="bit-diagram" style="--cols:13" role="img" aria-label="The thirteen bytes of the example, grouped: Byte 0, Byte 1, the count, the bin, the item's dimensions and its coordinates." markdown="0">
{% include bit-field.html size="00" name="Byte 0" note="1 B" %}
{% include bit-field.html size="00" name="Byte 1" note="1 B" %}
{% include bit-field.html size="01 00" name="Count" note="1 item" span=2 %}
{% include bit-field.html size="0A 14 1E" name="Bin" note="10 x 20 x 30" span=3 %}
{% include bit-field.html size="01 02 03" name="Item" note="1 x 2 x 3" span=3 %}
{% include bit-field.html size="04 05 06" name="At" note="4, 5, 6" span=3 %}
</div>

## 🖥️ Try it

Five known-good strings, ready to paste. Each is the `viPaqData` field of a `pack` response for one of
the demo's own worked examples, packed with First Fit Decreasing. Paste one into the
[ViPaq Decoder]({% vlink /configuration/ui-module/index.md %}) and it draws the bin with the items in it.

##### Five boxes, packed full

Bin `30x20x20`, holding one `20x20x20` and four `10x10x10`. 100% full.

```text
AAAFAB4UFBQUFAAAAAoKChQAAAoKChQKAAoKChQACgoKChQKCg==
```
{: data-name="viPaqData" data-wrap="" }

##### Ten boxes, two sizes

Bin `60x35x25`, holding six `25x8x25`, two `16x10x25` and two `10x16x25`. 88% full.

```text
AAAKADwjGRkIGQAAABkIGRkAABkIGQAIABkIGRkIABkIGQAQABkIGRkQABAKGQAYABAKGRkYAAoQGTIAAAoQGTIQAA==
```
{: data-name="viPaqData" data-wrap="" }

##### Twenty-four cubes

Bin `40x30x25`, holding twenty-four `10x10x10`. 80% full.

```text
AAAYACgeGQoKCgAAAAoKCgoAAAoKCgAKAAoKCgAACgoKChQAAAoKCgoKAAoKCgoACgoKCgAUAAoKCgAKCgoKCh4AAAoKChQKAAoKChQACgoKCgoUAAoKCgoKCgoKCgAUCgoKCh4KAAoKCh4ACgoKChQUAAoKChQKCgoKCgoUCgoKCh4UAAoKCh4KCgoKChQUCgoKCh4UCg==
```
{: data-name="viPaqData" data-wrap="" }

##### Thirteen boxes, mixed sizes

Bin `35x30x25`, holding one `20x18x16`, four `15x15x10` and eight `10x10x8`. 81% full.

```text
AAANACMeGRQSEAAAAA8PChQAAA8PChQPAA8PChQACg8PChQPCgoKCAASAAoKCAAAEAoKCAoSAAoKCAASCAoKCAoAEAoKCAoSCAoKCAASEAoKCAoSEA==
```
{: data-name="viPaqData" data-wrap="" }

##### Eight flat items

Bin `50x50x12`, holding eight `24x24x5`. 77% full.

```text
AAAIADIyDBgYBQAAABgYBRgAABgYBQAYABgYBQAABRgYBRgYABgYBRgABRgYBQAYBRgYBRgYBQ==
```
{: data-name="viPaqData" data-wrap="" }

## 📖 Full Specification

The normative wire specification, including the decoding order, everything a decoder must reject, and further
worked examples, lives with the source:
[ViPaq Protocol Specification](https://github.com/binacle-labs/Binacle.Net/blob/v3.0.0/vipaq/PROTOCOL.md).
