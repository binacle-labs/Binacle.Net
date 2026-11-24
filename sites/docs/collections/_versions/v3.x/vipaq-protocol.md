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

| Size | Name | Note | Width |
|---|---|---|---|
| 2 B | `Header` | Never compressed | 1 |
| varies | `Body` | Compressed if smaller | 3 |
{: .bit-diagram aria-label="A two byte header, never compressed, then the body, compressed if smaller." }

- The header is **never compressed**. The body is built first, compressed if that makes it smaller, and the
  header is put in front afterwards.
- Every blob is therefore self-describing: a decoder reads the two header bytes, then reads the body exactly as
  they say.
- All multi-byte integers are **unsigned** and **little-endian**.

## 🧾 Header

Two bytes, one job each. Byte 0 says **how to read** the body, byte 1 says **how wide** its integers are.

##### Byte 0 - form

| Size | Name | Note | Width |
|---|---|---|---|
| 7-6 | `Version` | 0 for this format | 2 |
| 5 | `Compressed` | raw or compressed | 1 |
| 4 | `Layout` | rows or columns | 1 |
| 3-0 | `reserved`{:.reserved} | always 0 | 4 |
{: .bit-diagram aria-label="Byte 0: bits 7 and 6 Version, bit 5 Compressed, bit 4 Layout, bits 3 to 0 reserved." }

| Bits | Field | Values |
|---|---|---|
| `7-6`{:.mono} | `Version`{:.mono} | `0` for this format. `1`-`3` are reserved. |
| `5`{:.mono} | `Compressed`{:.mono} | `0` the body is raw, `1` the body is compressed |
| `4`{:.mono} | `Layout`{:.mono} | `0` row-major, `1` columnar |
| `3-0`{:.mono} | `reserved`{:.mono} | always `0`, a decoder rejects anything else |

##### Byte 1 - integer widths

| Size | Name | Note | Width |
|---|---|---|---|
| 7-6 | `BinDimensionsWidth` | bin L, W, H | 2 |
| 5-4 | `ItemDimensionsWidth` | item L, W, H | 2 |
| 3-2 | `ItemCoordinatesWidth` | item X, Y, Z | 2 |
| 1-0 | `reserved`{:.reserved} | always 0 | 2 |
{: .bit-diagram aria-label="Byte 1: bits 7 and 6 BinDimensionsWidth, 5 and 4 ItemDimensionsWidth, 3 and 2 ItemCoordinatesWidth, 1 and 0 reserved." }

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

| Size | Name | Note | Width |
|---|---|---|---|
| 2 B | `Item count` | uint16 | 2 |
| 3 or 6 B | `Bin L, W, H` | Bin width | 3 |
| varies | `Items` | Item widths | 4 |
{: .bit-diagram aria-label="The body: a two byte item count, the bin dimensions, then the items." }

- **Item count** is always a 2 byte `uint16`, whatever the width fields say.
- **Bin dimensions** are Length, Width, Height, in that order.
- Items come back in the order they were sent.

`Layout` decides only how the items are arranged. The count and the bin dimensions are the same either way.

##### Row-major (`Layout = 0`)

Each item is written whole - its three dimensions, then its three coordinates - before the next item.

| Size | Name | Note | Width |
|---|---|---|---|
| 1 or 2 B | `L` | dimension | 1 |
| 1 or 2 B | `W` | dimension | 1 |
| 1 or 2 B | `H` | dimension | 1 |
| 1 or 2 B | `X` | coordinate | 1 |
| 1 or 2 B | `Y` | coordinate | 1 |
| 1 or 2 B | `Z` | coordinate | 1 |
| repeats | `next item`{:.more} | same six fields | 2 |
{: .bit-diagram aria-label="Row-major: one item as L, W, H, X, Y, Z in turn, then the next item, and so on." }

##### Columnar (`Layout = 1`)

Each field is written for every item before moving on to the next field. Six runs, each as long as the item count.

| Size | Name | Note | Width |
|---|---|---|---|
| count values | `L L L ...` | dimension width | 1 |
| count values | `W W W ...` | dimension width | 1 |
| count values | `H H H ...` | dimension width | 1 |
| count values | `X X X ...` | coordinate width | 1 |
| count values | `Y Y Y ...` | coordinate width | 1 |
| count values | `Z Z Z ...` | coordinate width | 1 |
{: .bit-diagram aria-label="Columnar: six runs in turn, L, W, H, X, Y, Z, each as long as the item count." }

Columnar puts values of similar size next to each other, which usually compresses better. Row-major is easier to
read in a hex dump. Neither is required - the header bit says which was used.

## 🔢 Integer Widths

Each of the three sections - bin dimensions, item dimensions, item coordinates - is stored at one width, chosen
independently:

| Code | Bytes per integer | Largest value |
|---|--:|--:|
| `0`{:.mono} | `1`{:.mono} | `255`{:.mono} |
| `1`{:.mono} | `2`{:.mono} | `65,535`{:.mono} |
| `2, 3`{:.mono} | *reserved*{:.table__off} | *a decoder rejects these*{:.table__off} |

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

| Size | Name | Note | Width |
|---|---|---|---|
| 00 | `Byte 0` | 1 B | 1 |
| 00 | `Byte 1` | 1 B | 1 |
| 01 00 | `Count` | 1 item | 2 |
| 0A 14 1E | `Bin` | 10 x 20 x 30 | 3 |
| 01 02 03 | `Item` | 1 x 2 x 3 | 3 |
| 04 05 06 | `At` | 4, 5, 6 | 3 |
{: .bit-diagram aria-label="The thirteen bytes of the example, grouped: Byte 0, Byte 1, the count, the bin, the item's dimensions and its coordinates." }

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
