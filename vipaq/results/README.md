# Encoded size results

Written by `just measure vipaq` from `Binacle.ViPaq.EncodedSize` over 2,322 real packs (bischoff-suite, custom-problems, demo-samples). Do not edit.

## 📊 ViPaq to protobuf
ViPaq base64 length over protobuf base64 length, both under the same codec, per pack; 0.65 means ViPaq is 65% the size. Mean of the per-pack ratios.

| Codec   | Layout   | Mean | Min  | Max  |
|---------|----------|------|------|------|
| Raw     | Row      | 0.65 | 0.48 | 1.00 |
| Raw     | Columnar | 0.65 | 0.48 | 1.00 |
| Deflate | Row      | 0.68 | 0.57 | 0.88 |
| Deflate | Columnar | 0.58 | 0.13 | 0.78 |
| Gzip    | Row      | 0.70 | 0.61 | 0.93 |
| Gzip    | Columnar | 0.60 | 0.17 | 0.90 |

## 📊 ViPaq to protobuf per algorithm
The same under deflate, split by the algorithm that placed the items.

| Algorithm | Layout   | Mean | Min  | Max  |
|-----------|----------|------|------|------|
| BFD       | Row      | 0.68 | 0.59 | 0.88 |
| BFD       | Columnar | 0.56 | 0.13 | 0.78 |
| FFD       | Row      | 0.70 | 0.57 | 0.88 |
| FFD       | Columnar | 0.60 | 0.30 | 0.78 |
| WFD       | Row      | 0.67 | 0.57 | 0.88 |
| WFD       | Columnar | 0.57 | 0.33 | 0.78 |

## 📊 Stored length per format
Characters per pack. JSON and compact notation are text; the rest are base64, of which the raw bytes are three quarters. JSON and compact carry the bin and the placed items only, no IDs.

| Format                  | Mean   | Min | Max   |
|-------------------------|--------|-----|-------|
| JSON                    | 4702.0 | 55  | 22048 |
| Compact notation        | 1669.1 | 8   | 7904  |
| Protobuf raw            | 1472.2 | 12  | 7168  |
| Protobuf deflate        | 529.4  | 16  | 1656  |
| ViPaq raw, Row          | 954.4  | 12  | 4468  |
| ViPaq deflate, Row      | 361.7  | 12  | 1248  |
| ViPaq raw, Columnar     | 954.4  | 12  | 4468  |
| ViPaq deflate, Columnar | 303.9  | 12  | 704   |

## 📊 The user's number
A ViPaq token under deflate, columnar layout - the smallest mode - as a share of each other format, per pack.

| Against          | ViPaq is (mean) | Min | Max  |
|------------------|-----------------|-----|------|
| JSON             | 7%              | 1%  | 22%  |
| Compact notation | 21%             | 2%  | 150% |
| Protobuf raw     | 24%             | 2%  | 100% |
| Protobuf deflate | 58%             | 13% | 78%  |

## 🔢 Crossover
The smallest pack, by item count, where deflate or gzip beats raw.

| Layout   | First pack where compression pays              | Items | Packs where it pays |
|----------|------------------------------------------------|-------|---------------------|
| Row      | DemoSample_16_OnlyBfdFullyPacks_30x25x25 (BFD) | 1     | 2265 of 2322        |
| Columnar | DemoSample_16_OnlyBfdFullyPacks_30x25x25 (BFD) | 1     | 2275 of 2322        |

## 🔢 Codec wins
How many of the 2322 packs each codec stored smallest. Raw wins a tie, then deflate.

| Layout   | Raw | Deflate | Gzip |
|----------|-----|---------|------|
| Row      | 57  | 2265    | 0    |
| Columnar | 47  | 2275    | 0    |

## 📏 The largest token
Every real pack deflates to at most 1248 base64 characters, in either layout. That is the size to plan a URL or a field around.

| Layout   | Largest deflate token | Pack                       | Items |
|----------|-----------------------|----------------------------|-------|
| Row      | 1248                  | OrLibrary_thpack1_65 (FFD) | 365   |
| Columnar | 704                   | OrLibrary_thpack1_65 (WFD) | 316   |

## 📂 Files

| File                               | What it is                                                                  |
|------------------------------------|-----------------------------------------------------------------------------|
| [encoded-size.md](encoded-size.md) | One row per pack per layout: every format's size, the ratio, the best codec |

