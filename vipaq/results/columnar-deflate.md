# Columnar layout, deflate

How much smaller is a columnar ViPaq token than protobuf and JSON, all compressed with deflate?

> Every number in this file is fake. The tables show the shape only.

## 📊 FFD by group

<!--
Table: columnar ViPaq size after deflate as × of protobuf and JSON after deflate, per group, FFD packs.
Reads: the "## Deflate" table of vipaq/results/measurements/encoded-size/ffd/columnar-<group>.md, one file per group.
Rows: thpack1 to thpack7, custom problems, demo samples, then All.
Columns: Group, Packs (count), × JSON, × protobuf, ViPaq larger than protobuf on (count of packs).
Notes: per pack, ViPaq's length divided by the other format's; then the mean per group. An average above 1.00× is
bold. Every length is base64 after deflate, JSON's too, so a ratio is the byte ratio. A "ViPaq larger than JSON on"
count is added only if some pack has ViPaq larger than JSON.
-->

| Group | Packs | × JSON | × protobuf | ViPaq larger than protobuf on |
|---|---|---|---|---|
| thpack1 | 333 | 0.44× | 0.55× | 6 |
| thpack2 | 777 | 0.88× | 0.99× | 1 |
| thpack3 | 222 | 0.33× | 0.44× | 5 |
| thpack4 | 666 | 0.77× | 0.88× | 9 |
| thpack5 | 111 | 0.22× | 0.33× | 4 |
| thpack6 | 555 | 0.66× | 0.77× | 8 |
| thpack7 | 999 | 0.11× | 0.22× | 3 |
| custom problems | 444 | 0.55× | 0.66× | 7 |
| demo samples | 888 | 0.99× | 0.11× | 2 |
| All | 333 | 0.44× | 0.55× | 6 |

## 📊 FFD by widths

<!--
Table: columnar ViPaq size after deflate as × of protobuf and JSON after deflate, per widths, FFD packs.
Reads: the "## Deflate" table of vipaq/results/measurements/encoded-size/ffd/columnar-<group>.md, every group.
Rows: one per Widths value found in the packs - bin / item / coordinate bits, e.g. 16/8/16.
Columns: Widths, Packs (count), × JSON, × protobuf, ViPaq larger than protobuf on (count of packs).
Notes: as for FFD by group.
-->

| Widths | Packs | × JSON | × protobuf | ViPaq larger than protobuf on |
|---|---|---|---|---|
| 8/8/8 | 777 | 0.88× | 0.99× | 1 |
| 16/8/16 | 222 | 0.33× | 0.44× | 5 |
| 16/16/16 | 666 | 0.77× | 0.88× | 9 |

## 📊 WFD by group

<!--
Table: columnar ViPaq size after deflate as × of protobuf and JSON after deflate, per group, WFD packs.
Reads: the "## Deflate" table of vipaq/results/measurements/encoded-size/wfd/columnar-<group>.md, one file per group.
Rows: thpack1 to thpack7, custom problems, demo samples, then All.
Columns: Group, Packs (count), × JSON, × protobuf, ViPaq larger than protobuf on (count of packs).
Notes: per pack, ViPaq's length divided by the other format's; then the mean per group. An average above 1.00× is
bold. Every length is base64 after deflate, JSON's too, so a ratio is the byte ratio. A "ViPaq larger than JSON on"
count is added only if some pack has ViPaq larger than JSON.
-->

| Group | Packs | × JSON | × protobuf | ViPaq larger than protobuf on |
|---|---|---|---|---|
| thpack1 | 111 | 0.22× | 0.33× | 4 |
| thpack2 | 555 | 0.66× | 0.77× | 8 |
| thpack3 | 999 | 0.11× | 0.22× | 3 |
| thpack4 | 444 | 0.55× | 0.66× | 7 |
| thpack5 | 888 | 0.99× | 0.11× | 2 |
| thpack6 | 333 | 0.44× | 0.55× | 6 |
| thpack7 | 777 | 0.88× | 0.99× | 1 |
| custom problems | 222 | 0.33× | 0.44× | 5 |
| demo samples | 666 | 0.77× | 0.88× | 9 |
| All | 111 | 0.22× | 0.33× | 4 |

## 📊 WFD by widths

<!--
Table: columnar ViPaq size after deflate as × of protobuf and JSON after deflate, per widths, WFD packs.
Reads: the "## Deflate" table of vipaq/results/measurements/encoded-size/wfd/columnar-<group>.md, every group.
Rows: one per Widths value found in the packs - bin / item / coordinate bits, e.g. 16/8/16.
Columns: Widths, Packs (count), × JSON, × protobuf, ViPaq larger than protobuf on (count of packs).
Notes: as for WFD by group.
-->

| Widths | Packs | × JSON | × protobuf | ViPaq larger than protobuf on |
|---|---|---|---|---|
| 8/8/8 | 555 | 0.66× | 0.77× | 8 |
| 16/8/16 | 999 | 0.11× | 0.22× | 3 |
| 16/16/16 | 444 | 0.55× | 0.66× | 7 |

## 📊 BFD by group

<!--
Table: columnar ViPaq size after deflate as × of protobuf and JSON after deflate, per group, BFD packs.
Reads: the "## Deflate" table of vipaq/results/measurements/encoded-size/bfd/columnar-<group>.md, one file per group.
Rows: thpack1 to thpack7, custom problems, demo samples, then All.
Columns: Group, Packs (count), × JSON, × protobuf, ViPaq larger than protobuf on (count of packs).
Notes: per pack, ViPaq's length divided by the other format's; then the mean per group. An average above 1.00× is
bold. Every length is base64 after deflate, JSON's too, so a ratio is the byte ratio. A "ViPaq larger than JSON on"
count is added only if some pack has ViPaq larger than JSON.
-->

| Group | Packs | × JSON | × protobuf | ViPaq larger than protobuf on |
|---|---|---|---|---|
| thpack1 | 888 | 0.99× | 0.11× | 2 |
| thpack2 | 333 | 0.44× | 0.55× | 6 |
| thpack3 | 777 | 0.88× | 0.99× | 1 |
| thpack4 | 222 | 0.33× | 0.44× | 5 |
| thpack5 | 666 | 0.77× | 0.88× | 9 |
| thpack6 | 111 | 0.22× | 0.33× | 4 |
| thpack7 | 555 | 0.66× | 0.77× | 8 |
| custom problems | 999 | 0.11× | 0.22× | 3 |
| demo samples | 444 | 0.55× | 0.66× | 7 |
| All | 888 | 0.99× | 0.11× | 2 |

## 📊 BFD by widths

<!--
Table: columnar ViPaq size after deflate as × of protobuf and JSON after deflate, per widths, BFD packs.
Reads: the "## Deflate" table of vipaq/results/measurements/encoded-size/bfd/columnar-<group>.md, every group.
Rows: one per Widths value found in the packs - bin / item / coordinate bits, e.g. 16/8/16.
Columns: Widths, Packs (count), × JSON, × protobuf, ViPaq larger than protobuf on (count of packs).
Notes: as for BFD by group.
-->

| Widths | Packs | × JSON | × protobuf | ViPaq larger than protobuf on |
|---|---|---|---|---|
| 8/8/8 | 333 | 0.44× | 0.55× | 6 |
| 16/8/16 | 777 | 0.88× | 0.99× | 1 |
| 16/16/8 | 222 | 0.33× | 0.44× | 5 |

## Gaps and open questions

<!--
Gap: protobuf is a row message only, so against columnar ViPaq part of the win is the layout. No columnar protobuf.
Gap: no MessagePack or CBOR measured.
-->
