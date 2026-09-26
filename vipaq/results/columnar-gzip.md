# Columnar layout, gzip

How much smaller is a columnar ViPaq token than protobuf and JSON, all compressed with gzip?

> Every number in this file is fake. The tables show the shape only.

## 📊 FFD by group

<!--
Table: columnar ViPaq size after gzip as × of protobuf and JSON after gzip, per group, FFD packs.
Reads: the "## Gzip" table of vipaq/results/measurements/encoded-size/ffd/columnar-<group>.md, one file per group.
Rows: thpack1 to thpack7, custom problems, demo samples, then All.
Columns: Group, Packs (count), × JSON, × protobuf, ViPaq larger than protobuf on (count of packs).
Notes: per pack, ViPaq's length divided by the other format's; then the mean per group. An average above 1.00× is
bold. Every length is base64 after gzip, JSON's too, so a ratio is the byte ratio. A "ViPaq larger than JSON on"
count is added only if some pack has ViPaq larger than JSON.
-->

| Group | Packs | × JSON | × protobuf | ViPaq larger than protobuf on |
|---|---|---|---|---|
| thpack1 | 666 | 0.77× | 0.88× | 9 |
| thpack2 | 111 | 0.22× | 0.33× | 4 |
| thpack3 | 555 | 0.66× | 0.77× | 8 |
| thpack4 | 999 | 0.11× | 0.22× | 3 |
| thpack5 | 444 | 0.55× | 0.66× | 7 |
| thpack6 | 888 | 0.99× | 0.11× | 2 |
| thpack7 | 333 | 0.44× | 0.55× | 6 |
| custom problems | 777 | 0.88× | 0.99× | 1 |
| demo samples | 222 | 0.33× | 0.44× | 5 |
| All | 666 | 0.77× | 0.88× | 9 |

## 📊 FFD by widths

<!--
Table: columnar ViPaq size after gzip as × of protobuf and JSON after gzip, per widths, FFD packs.
Reads: the "## Gzip" table of vipaq/results/measurements/encoded-size/ffd/columnar-<group>.md, every group.
Rows: one per Widths value found in the packs - bin / item / coordinate bits, e.g. 16/8/16.
Columns: Widths, Packs (count), × JSON, × protobuf, ViPaq larger than protobuf on (count of packs).
Notes: as for FFD by group.
-->

| Widths | Packs | × JSON | × protobuf | ViPaq larger than protobuf on |
|---|---|---|---|---|
| 8/8/8 | 111 | 0.22× | 0.33× | 4 |
| 16/8/16 | 555 | 0.66× | 0.77× | 8 |
| 16/16/16 | 999 | 0.11× | 0.22× | 3 |

## 📊 WFD by group

<!--
Table: columnar ViPaq size after gzip as × of protobuf and JSON after gzip, per group, WFD packs.
Reads: the "## Gzip" table of vipaq/results/measurements/encoded-size/wfd/columnar-<group>.md, one file per group.
Rows: thpack1 to thpack7, custom problems, demo samples, then All.
Columns: Group, Packs (count), × JSON, × protobuf, ViPaq larger than protobuf on (count of packs).
Notes: per pack, ViPaq's length divided by the other format's; then the mean per group. An average above 1.00× is
bold. Every length is base64 after gzip, JSON's too, so a ratio is the byte ratio. A "ViPaq larger than JSON on"
count is added only if some pack has ViPaq larger than JSON.
-->

| Group | Packs | × JSON | × protobuf | ViPaq larger than protobuf on |
|---|---|---|---|---|
| thpack1 | 444 | 0.55× | 0.66× | 7 |
| thpack2 | 888 | 0.99× | 0.11× | 2 |
| thpack3 | 333 | 0.44× | 0.55× | 6 |
| thpack4 | 777 | 0.88× | 0.99× | 1 |
| thpack5 | 222 | 0.33× | 0.44× | 5 |
| thpack6 | 666 | 0.77× | 0.88× | 9 |
| thpack7 | 111 | 0.22× | 0.33× | 4 |
| custom problems | 555 | 0.66× | 0.77× | 8 |
| demo samples | 999 | 0.11× | 0.22× | 3 |
| All | 444 | 0.55× | 0.66× | 7 |

## 📊 WFD by widths

<!--
Table: columnar ViPaq size after gzip as × of protobuf and JSON after gzip, per widths, WFD packs.
Reads: the "## Gzip" table of vipaq/results/measurements/encoded-size/wfd/columnar-<group>.md, every group.
Rows: one per Widths value found in the packs - bin / item / coordinate bits, e.g. 16/8/16.
Columns: Widths, Packs (count), × JSON, × protobuf, ViPaq larger than protobuf on (count of packs).
Notes: as for WFD by group.
-->

| Widths | Packs | × JSON | × protobuf | ViPaq larger than protobuf on |
|---|---|---|---|---|
| 8/8/8 | 888 | 0.99× | 0.11× | 2 |
| 16/8/16 | 333 | 0.44× | 0.55× | 6 |
| 16/16/16 | 777 | 0.88× | 0.99× | 1 |

## 📊 BFD by group

<!--
Table: columnar ViPaq size after gzip as × of protobuf and JSON after gzip, per group, BFD packs.
Reads: the "## Gzip" table of vipaq/results/measurements/encoded-size/bfd/columnar-<group>.md, one file per group.
Rows: thpack1 to thpack7, custom problems, demo samples, then All.
Columns: Group, Packs (count), × JSON, × protobuf, ViPaq larger than protobuf on (count of packs).
Notes: per pack, ViPaq's length divided by the other format's; then the mean per group. An average above 1.00× is
bold. Every length is base64 after gzip, JSON's too, so a ratio is the byte ratio. A "ViPaq larger than JSON on"
count is added only if some pack has ViPaq larger than JSON.
-->

| Group | Packs | × JSON | × protobuf | ViPaq larger than protobuf on |
|---|---|---|---|---|
| thpack1 | 222 | 0.33× | 0.44× | 5 |
| thpack2 | 666 | 0.77× | 0.88× | 9 |
| thpack3 | 111 | 0.22× | 0.33× | 4 |
| thpack4 | 555 | 0.66× | 0.77× | 8 |
| thpack5 | 999 | 0.11× | 0.22× | 3 |
| thpack6 | 444 | 0.55× | 0.66× | 7 |
| thpack7 | 888 | 0.99× | 0.11× | 2 |
| custom problems | 333 | 0.44× | 0.55× | 6 |
| demo samples | 777 | 0.88× | 0.99× | 1 |
| All | 222 | 0.33× | 0.44× | 5 |

## 📊 BFD by widths

<!--
Table: columnar ViPaq size after gzip as × of protobuf and JSON after gzip, per widths, BFD packs.
Reads: the "## Gzip" table of vipaq/results/measurements/encoded-size/bfd/columnar-<group>.md, every group.
Rows: one per Widths value found in the packs - bin / item / coordinate bits, e.g. 16/8/16.
Columns: Widths, Packs (count), × JSON, × protobuf, ViPaq larger than protobuf on (count of packs).
Notes: as for BFD by group.
-->

| Widths | Packs | × JSON | × protobuf | ViPaq larger than protobuf on |
|---|---|---|---|---|
| 8/8/8 | 666 | 0.77× | 0.88× | 9 |
| 16/8/16 | 111 | 0.22× | 0.33× | 4 |
| 16/16/8 | 555 | 0.66× | 0.77× | 8 |

## Gaps and open questions

<!--
Gap: protobuf is a row message only, so against columnar ViPaq part of the win is the layout. No columnar protobuf.
Gap: no MessagePack or CBOR measured.
-->
