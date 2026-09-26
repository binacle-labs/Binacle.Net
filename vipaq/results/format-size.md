# Format size

How much smaller is a ViPaq token than JSON, compact notation and protobuf, stored as is?

> Every number in this file is fake. The tables show the shape only.

## 📊 FFD by group

<!--
Table: raw ViPaq size as × of each other format, per group, FFD packs.
Reads: the "## Raw" table of vipaq/results/measurements/encoded-size/ffd/row-<group>.md, one file per group.
Rows: thpack1 to thpack7, custom problems, demo samples, then All.
Columns: Group, Packs (count), × JSON, × compact, × protobuf, ViPaq larger than protobuf on (count of packs).
Notes: per pack, ViPaq's length divided by the other format's; then the mean per group. An average above 1.00× is
bold. Lengths are characters: ViPaq and protobuf as base64, JSON and compact notation as text. Compact notation joins
the bin and the items with ";" because it has no whole-pack form. A "ViPaq larger than JSON on" count is added only
if some pack has ViPaq larger than JSON.
-->

| Group | Packs | × JSON | × compact | × protobuf | ViPaq larger than protobuf on |
|---|---|---|---|---|---|
| thpack1 | 999 | 0.11× | 0.22× | 0.33× | 4 |
| thpack2 | 555 | 0.66× | 0.77× | 0.88× | 9 |
| thpack3 | 111 | 0.22× | 0.33× | 0.44× | 5 |
| thpack4 | 666 | 0.77× | 0.88× | 0.99× | 1 |
| thpack5 | 222 | 0.33× | 0.44× | 0.55× | 6 |
| thpack6 | 777 | 0.88× | 0.99× | 0.11× | 2 |
| thpack7 | 333 | 0.44× | 0.55× | 0.66× | 7 |
| custom problems | 888 | 0.99× | 0.11× | 0.22× | 3 |
| demo samples | 444 | 0.55× | 0.66× | 0.77× | 8 |
| All | 999 | 0.11× | 0.22× | 0.33× | 4 |

## 📊 FFD by widths

<!--
Table: raw ViPaq size as × of each other format, per widths, FFD packs.
Reads: the "## Raw" table of vipaq/results/measurements/encoded-size/ffd/row-<group>.md, every group.
Rows: one per Widths value found in the packs - bin / item / coordinate bits, e.g. 16/8/16.
Columns: Widths, Packs (count), × JSON, × compact, × protobuf, ViPaq larger than protobuf on (count of packs).
Notes: as for FFD by group.
-->

| Widths | Packs | × JSON | × compact | × protobuf | ViPaq larger than protobuf on |
|---|---|---|---|---|---|
| 8/8/8 | 555 | 0.66× | 0.77× | 0.88× | 9 |
| 16/8/16 | 111 | 0.22× | 0.33× | 0.44× | 5 |
| 16/16/16 | 666 | 0.77× | 0.88× | 0.99× | 1 |

## 📊 WFD by group

<!--
Table: raw ViPaq size as × of each other format, per group, WFD packs.
Reads: the "## Raw" table of vipaq/results/measurements/encoded-size/wfd/row-<group>.md, one file per group.
Rows: thpack1 to thpack7, custom problems, demo samples, then All.
Columns: Group, Packs (count), × JSON, × compact, × protobuf, ViPaq larger than protobuf on (count of packs).
Notes: per pack, ViPaq's length divided by the other format's; then the mean per group. An average above 1.00× is
bold. Lengths are characters: ViPaq and protobuf as base64, JSON and compact notation as text. Compact notation joins
the bin and the items with ";" because it has no whole-pack form. A "ViPaq larger than JSON on" count is added only
if some pack has ViPaq larger than JSON.
-->

| Group | Packs | × JSON | × compact | × protobuf | ViPaq larger than protobuf on |
|---|---|---|---|---|---|
| thpack1 | 222 | 0.33× | 0.44× | 0.55× | 6 |
| thpack2 | 777 | 0.88× | 0.99× | 0.11× | 2 |
| thpack3 | 333 | 0.44× | 0.55× | 0.66× | 7 |
| thpack4 | 888 | 0.99× | 0.11× | 0.22× | 3 |
| thpack5 | 444 | 0.55× | 0.66× | 0.77× | 8 |
| thpack6 | 999 | 0.11× | 0.22× | 0.33× | 4 |
| thpack7 | 555 | 0.66× | 0.77× | 0.88× | 9 |
| custom problems | 111 | 0.22× | 0.33× | 0.44× | 5 |
| demo samples | 666 | 0.77× | 0.88× | 0.99× | 1 |
| All | 222 | 0.33× | 0.44× | 0.55× | 6 |

## 📊 WFD by widths

<!--
Table: raw ViPaq size as × of each other format, per widths, WFD packs.
Reads: the "## Raw" table of vipaq/results/measurements/encoded-size/wfd/row-<group>.md, every group.
Rows: one per Widths value found in the packs - bin / item / coordinate bits, e.g. 16/8/16.
Columns: Widths, Packs (count), × JSON, × compact, × protobuf, ViPaq larger than protobuf on (count of packs).
Notes: as for WFD by group.
-->

| Widths | Packs | × JSON | × compact | × protobuf | ViPaq larger than protobuf on |
|---|---|---|---|---|---|
| 8/8/8 | 777 | 0.88× | 0.99× | 0.11× | 2 |
| 16/8/16 | 333 | 0.44× | 0.55× | 0.66× | 7 |
| 16/16/16 | 888 | 0.99× | 0.11× | 0.22× | 3 |

## 📊 BFD by group

<!--
Table: raw ViPaq size as × of each other format, per group, BFD packs.
Reads: the "## Raw" table of vipaq/results/measurements/encoded-size/bfd/row-<group>.md, one file per group.
Rows: thpack1 to thpack7, custom problems, demo samples, then All.
Columns: Group, Packs (count), × JSON, × compact, × protobuf, ViPaq larger than protobuf on (count of packs).
Notes: per pack, ViPaq's length divided by the other format's; then the mean per group. An average above 1.00× is
bold. Lengths are characters: ViPaq and protobuf as base64, JSON and compact notation as text. Compact notation joins
the bin and the items with ";" because it has no whole-pack form. A "ViPaq larger than JSON on" count is added only
if some pack has ViPaq larger than JSON.
-->

| Group | Packs | × JSON | × compact | × protobuf | ViPaq larger than protobuf on |
|---|---|---|---|---|---|
| thpack1 | 444 | 0.55× | 0.66× | 0.77× | 8 |
| thpack2 | 999 | 0.11× | 0.22× | 0.33× | 4 |
| thpack3 | 555 | 0.66× | 0.77× | 0.88× | 9 |
| thpack4 | 111 | 0.22× | 0.33× | 0.44× | 5 |
| thpack5 | 666 | 0.77× | 0.88× | 0.99× | 1 |
| thpack6 | 222 | 0.33× | 0.44× | 0.55× | 6 |
| thpack7 | 777 | 0.88× | 0.99× | 0.11× | 2 |
| custom problems | 333 | 0.44× | 0.55× | 0.66× | 7 |
| demo samples | 888 | 0.99× | 0.11× | 0.22× | 3 |
| All | 444 | 0.55× | 0.66× | 0.77× | 8 |

## 📊 BFD by widths

<!--
Table: raw ViPaq size as × of each other format, per widths, BFD packs.
Reads: the "## Raw" table of vipaq/results/measurements/encoded-size/bfd/row-<group>.md, every group.
Rows: one per Widths value found in the packs - bin / item / coordinate bits, e.g. 16/8/16.
Columns: Widths, Packs (count), × JSON, × compact, × protobuf, ViPaq larger than protobuf on (count of packs).
Notes: as for BFD by group.
-->

| Widths | Packs | × JSON | × compact | × protobuf | ViPaq larger than protobuf on |
|---|---|---|---|---|---|
| 8/8/8 | 999 | 0.11× | 0.22× | 0.33× | 4 |
| 16/8/16 | 555 | 0.66× | 0.77× | 0.88× | 9 |
| 16/16/8 | 111 | 0.22× | 0.33× | 0.44× | 5 |

## Gaps and open questions

<!--
Question: are raw row and raw columnar the same length? This file reads the row files only. If they differ, ask.
Gap: no MessagePack, CBOR or columnar protobuf measured.
Question: ViPaq has no earlier version to show its direction is sound, the way v1 against v2 does for the packing
library. Is there an earlier format worth measuring, or is "against protobuf" the whole story?
-->
