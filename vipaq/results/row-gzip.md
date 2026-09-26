# Row layout, gzip

How much smaller is a row ViPaq token than protobuf and JSON, all compressed with gzip?

> Every number in this file is fake. The tables show the shape only.

## 📊 FFD by group

<!--
Table: row ViPaq size after gzip as × of protobuf and JSON after gzip, per group, FFD packs.
Reads: the "## Gzip" table of vipaq/results/measurements/encoded-size/ffd/row-<group>.md, one file per group.
Rows: thpack1 to thpack7, custom problems, demo samples, then All.
Columns: Group, Packs (count), × JSON, × protobuf, ViPaq larger than protobuf on (count of packs).
Notes: per pack, ViPaq's length divided by the other format's; then the mean per group. An average above 1.00× is
bold. Every length is base64 after gzip, JSON's too, so a ratio is the byte ratio. A "ViPaq larger than JSON on"
count is added only if some pack has ViPaq larger than JSON.
-->

| Group | Packs | × JSON | × protobuf | ViPaq larger than protobuf on |
|---|---|---|---|---|
| thpack1 | 999 | 0.11× | 0.22× | 3 |
| thpack2 | 444 | 0.55× | 0.66× | 7 |
| thpack3 | 888 | 0.99× | 0.11× | 2 |
| thpack4 | 333 | 0.44× | 0.55× | 6 |
| thpack5 | 777 | 0.88× | 0.99× | 1 |
| thpack6 | 222 | 0.33× | 0.44× | 5 |
| thpack7 | 666 | 0.77× | 0.88× | 9 |
| custom problems | 111 | 0.22× | 0.33× | 4 |
| demo samples | 555 | 0.66× | 0.77× | 8 |
| All | 999 | 0.11× | 0.22× | 3 |

## 📊 FFD by widths

<!--
Table: row ViPaq size after gzip as × of protobuf and JSON after gzip, per widths, FFD packs.
Reads: the "## Gzip" table of vipaq/results/measurements/encoded-size/ffd/row-<group>.md, every group.
Rows: one per Widths value found in the packs - bin / item / coordinate bits, e.g. 16/8/16.
Columns: Widths, Packs (count), × JSON, × protobuf, ViPaq larger than protobuf on (count of packs).
Notes: as for FFD by group.
-->

| Widths | Packs | × JSON | × protobuf | ViPaq larger than protobuf on |
|---|---|---|---|---|
| 8/8/8 | 444 | 0.55× | 0.66× | 7 |
| 16/8/16 | 888 | 0.99× | 0.11× | 2 |
| 16/16/16 | 333 | 0.44× | 0.55× | 6 |

## 📊 WFD by group

<!--
Table: row ViPaq size after gzip as × of protobuf and JSON after gzip, per group, WFD packs.
Reads: the "## Gzip" table of vipaq/results/measurements/encoded-size/wfd/row-<group>.md, one file per group.
Rows: thpack1 to thpack7, custom problems, demo samples, then All.
Columns: Group, Packs (count), × JSON, × protobuf, ViPaq larger than protobuf on (count of packs).
Notes: per pack, ViPaq's length divided by the other format's; then the mean per group. An average above 1.00× is
bold. Every length is base64 after gzip, JSON's too, so a ratio is the byte ratio. A "ViPaq larger than JSON on"
count is added only if some pack has ViPaq larger than JSON.
-->

| Group | Packs | × JSON | × protobuf | ViPaq larger than protobuf on |
|---|---|---|---|---|
| thpack1 | 777 | 0.88× | 0.99× | 1 |
| thpack2 | 222 | 0.33× | 0.44× | 5 |
| thpack3 | 666 | 0.77× | 0.88× | 9 |
| thpack4 | 111 | 0.22× | 0.33× | 4 |
| thpack5 | 555 | 0.66× | 0.77× | 8 |
| thpack6 | 999 | 0.11× | 0.22× | 3 |
| thpack7 | 444 | 0.55× | 0.66× | 7 |
| custom problems | 888 | 0.99× | 0.11× | 2 |
| demo samples | 333 | 0.44× | 0.55× | 6 |
| All | 777 | 0.88× | 0.99× | 1 |

## 📊 WFD by widths

<!--
Table: row ViPaq size after gzip as × of protobuf and JSON after gzip, per widths, WFD packs.
Reads: the "## Gzip" table of vipaq/results/measurements/encoded-size/wfd/row-<group>.md, every group.
Rows: one per Widths value found in the packs - bin / item / coordinate bits, e.g. 16/8/16.
Columns: Widths, Packs (count), × JSON, × protobuf, ViPaq larger than protobuf on (count of packs).
Notes: as for WFD by group.
-->

| Widths | Packs | × JSON | × protobuf | ViPaq larger than protobuf on |
|---|---|---|---|---|
| 8/8/8 | 222 | 0.33× | 0.44× | 5 |
| 16/8/16 | 666 | 0.77× | 0.88× | 9 |
| 16/16/16 | 111 | 0.22× | 0.33× | 4 |

## 📊 BFD by group

<!--
Table: row ViPaq size after gzip as × of protobuf and JSON after gzip, per group, BFD packs.
Reads: the "## Gzip" table of vipaq/results/measurements/encoded-size/bfd/row-<group>.md, one file per group.
Rows: thpack1 to thpack7, custom problems, demo samples, then All.
Columns: Group, Packs (count), × JSON, × protobuf, ViPaq larger than protobuf on (count of packs).
Notes: per pack, ViPaq's length divided by the other format's; then the mean per group. An average above 1.00× is
bold. Every length is base64 after gzip, JSON's too, so a ratio is the byte ratio. A "ViPaq larger than JSON on"
count is added only if some pack has ViPaq larger than JSON.
-->

| Group | Packs | × JSON | × protobuf | ViPaq larger than protobuf on |
|---|---|---|---|---|
| thpack1 | 555 | 0.66× | 0.77× | 8 |
| thpack2 | 999 | 0.11× | 0.22× | 3 |
| thpack3 | 444 | 0.55× | 0.66× | 7 |
| thpack4 | 888 | 0.99× | 0.11× | 2 |
| thpack5 | 333 | 0.44× | 0.55× | 6 |
| thpack6 | 777 | 0.88× | 0.99× | 1 |
| thpack7 | 222 | 0.33× | 0.44× | 5 |
| custom problems | 666 | 0.77× | 0.88× | 9 |
| demo samples | 111 | 0.22× | 0.33× | 4 |
| All | 555 | 0.66× | 0.77× | 8 |

## 📊 BFD by widths

<!--
Table: row ViPaq size after gzip as × of protobuf and JSON after gzip, per widths, BFD packs.
Reads: the "## Gzip" table of vipaq/results/measurements/encoded-size/bfd/row-<group>.md, every group.
Rows: one per Widths value found in the packs - bin / item / coordinate bits, e.g. 16/8/16.
Columns: Widths, Packs (count), × JSON, × protobuf, ViPaq larger than protobuf on (count of packs).
Notes: as for BFD by group.
-->

| Widths | Packs | × JSON | × protobuf | ViPaq larger than protobuf on |
|---|---|---|---|---|
| 8/8/8 | 999 | 0.11× | 0.22× | 3 |
| 16/8/16 | 444 | 0.55× | 0.66× | 7 |
| 16/16/8 | 888 | 0.99× | 0.11× | 2 |

## Gaps and open questions

<!--
Gap: no MessagePack, CBOR or columnar protobuf measured.
-->
