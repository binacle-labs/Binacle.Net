# Compressed size

Compressed, how much smaller is a ViPaq token than JSON, compact notation and protobuf - and which layout and codec should it use?

## 📝 What was observed

- Columnar wins. It is 0.86× row at the same codec, and the gap widens the bigger the pack.
- Deflate wins. Gzip is never smaller than deflate on any pack.
- Best pairing is columnar with deflate: 0.47× JSON, 0.68× compact notation, 0.60× protobuf.
- Compressed, ViPaq is smaller than every format on every pack.
- Compressing beats sending raw from about 5 items up. Below that it can be larger.

## 📊 By layout and codec, FFD

<!--
Which layout and codec to use. Per pack, then the mean over every group.
Reads: the "## Deflate" and "## Gzip" tables of vipaq/results/measurements/encoded-size/ffd/<layout>-<group>.md
-->

| Layout + codec | Packs | × JSON | × compact | × protobuf |
|---|---|---|---|---|
| row + deflate | 774 | 0.55× | 0.79× | 0.70× |
| row + gzip | 774 | 0.57× | 0.80× | 0.72× |
| columnar + deflate | 774 | 0.47× | 0.68× | 0.60× |
| columnar + gzip | 774 | 0.50× | 0.70× | 0.63× |

## 📊 By group, columnar + deflate, FFD

<!--
The best pairing, broken out per group. Per pack, then the mean per group.
Reads: the "## Deflate" table of vipaq/results/measurements/encoded-size/ffd/columnar-<group>.md
-->

| Group | Packs | ViPaq chars (median) | × JSON | × compact | × protobuf |
|---|---|---|---|---|---|
| thpack1 | 100 | 244 | 0.40× | 0.56× | 0.50× |
| thpack2 | 100 | 288 | 0.45× | 0.62× | 0.55× |
| thpack3 | 100 | 332 | 0.49× | 0.68× | 0.59× |
| thpack4 | 100 | 356 | 0.50× | 0.70× | 0.61× |
| thpack5 | 100 | 378 | 0.52× | 0.72× | 0.63× |
| thpack6 | 100 | 418 | 0.54× | 0.74× | 0.65× |
| thpack7 | 100 | 438 | 0.56× | 0.77× | 0.67× |
| custom problems | 21 | 28 | 0.24× | 0.59× | 0.64× |
| demo samples | 53 | 52 | 0.29× | 0.61× | 0.65× |
| All | 774 | 344 | 0.47× | 0.68× | 0.60× |

## 📊 Where ViPaq loses

<!--
Every compressed pack where ViPaq is at least as long as another format. The catch a mean cannot give.
Reads: the "## Deflate" and "## Gzip" tables of every file under vipaq/results/measurements/encoded-size/
-->

None, in either layout, either codec, or any of the three algorithms.
