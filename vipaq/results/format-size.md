# Format size

How much smaller is a ViPaq token than JSON, compact notation and protobuf, stored as is?

## 📝 What was observed

- 0.20× JSON, 0.58× compact notation, 0.65× protobuf.
- Never larger than JSON or protobuf. Larger than compact notation only on empty packs.
- The algorithm does not change it. The Bischoff set does not change it.
- Row and columnar are the same length uncompressed, character for character.

## 📊 By group, FFD

<!--
Raw ViPaq size as × of each other format, per group. Per pack, then the mean per group.
Reads: the "## Raw" table of vipaq/results/measurements/encoded-size/ffd/row-<group>.md
-->

| Group | Packs | ViPaq chars (median) | × JSON | × compact | × protobuf |
|---|---|---|---|---|---|
| thpack1 | 100 | 1036 | 0.20× | 0.58× | 0.65× |
| thpack2 | 100 | 1018 | 0.20× | 0.57× | 0.65× |
| thpack3 | 100 | 964 | 0.20× | 0.57× | 0.65× |
| thpack4 | 100 | 946 | 0.20× | 0.57× | 0.65× |
| thpack5 | 100 | 952 | 0.20× | 0.57× | 0.65× |
| thpack6 | 100 | 976 | 0.20× | 0.57× | 0.65× |
| thpack7 | 100 | 928 | 0.20× | 0.57× | 0.65× |
| custom problems | 21 | 28 | 0.17× | 0.72× | 0.70× |
| demo samples | 53 | 76 | 0.16× | 0.57× | 0.63× |
| All | 774 | 940 | 0.20× | 0.58× | 0.65× |

## 📊 By algorithm

<!--
Does the algorithm change the size ratio.
Reads: the "## Raw" table of vipaq/results/measurements/encoded-size/<alg>/row-<group>.md, every group
-->

| Algorithm | Packs | × JSON | × compact | × protobuf |
|---|---|---|---|---|
| FFD | 774 | 0.20× | 0.58× | 0.65× |
| WFD | 774 | 0.20× | 0.58× | 0.66× |
| BFD | 774 | 0.20× | 0.58× | 0.65× |

## 📊 Where ViPaq loses

<!--
Every pack where ViPaq is at least as long as another format. The catch a mean cannot give.
Reads: the "## Raw" table of every file under vipaq/results/measurements/encoded-size/, both layouts, all algorithms
-->

| Pack | Items | Widths | ViPaq | Compact | × compact | × protobuf |
|---|---|---|---|---|---|---|
| `Baseline_40x40x40-1_DoesNotFit_60x40x30` | 0 | 8/8/8 | 12 | 8 | 1.50× | 1.00× |
| `Baseline_5x5x80_TooLong_60x40x30` | 0 | 8/8/8 | 12 | 8 | 1.50× | 1.00× |
| `DemoSample_07_TallItems_40x40x30` | 0 | 8/8/8 | 12 | 8 | 1.50× | 1.00× |
