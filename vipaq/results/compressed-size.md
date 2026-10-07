# Compressed size

Which ViPaq layout and which codec to use, and how long a compressed ViPaq token is against protobuf, JSON
and compact notation.

Every size is a character count: ViPaq and protobuf as base64, JSON and compact notation as text, each
compressed one as base64. Each pack is the result of packing one problem with FFD, WFD or BFD; all three
algorithms' packs are pooled. Every ratio is worked out on each pack first, then summarised.

## 📊 Layout: columnar against row

### 📈 Deflate

On each pack, the columnar token's length divided by the row token's, both with deflate. Below 1 means
columnar is shorter.

| Group | Item types | Packs | Min | Mean | Median | Max | Packs where columnar is longer |
|---|---|---|---|---|---|---|---|
| thpack1 | 3 | 300 | 0.29 | 0.68 | 0.71 | 0.87 | 0 |
| thpack2 | 5 | 300 | 0.46 | 0.76 | 0.77 | 0.92 | 0 |
| thpack3 | 8 | 300 | 0.66 | 0.82 | 0.83 | 0.97 | 0 |
| thpack4 | 10 | 300 | 0.71 | 0.86 | 0.86 | 0.97 | 0 |
| thpack5 | 12 | 300 | 0.79 | 0.89 | 0.89 | 1.01 | 1 |
| thpack6 | 15 | 300 | 0.81 | 0.92 | 0.92 | 1.03 | 3 |
| thpack7 | 20 | 300 | 0.82 | 0.96 | 0.96 | 1.06 | 34 |
| demo-samples | - | 159 | 0.48 | 0.91 | 0.95 | 1.12 | 17 |
| custom-problems | - | 63 | 0.16 | 0.86 | 0.92 | 1.17 | 6 |
| **All** |  | 2322 | 0.16 | 0.85 | 0.86 | 1.17 | 61 |

### 📈 Gzip

On each pack, the columnar token's length divided by the row token's, both with gzip. Below 1 means
columnar is shorter.

| Group | Item types | Packs | Min | Mean | Median | Max | Packs where columnar is longer |
|---|---|---|---|---|---|---|---|
| thpack1 | 3 | 300 | 0.31 | 0.70 | 0.73 | 0.88 | 0 |
| thpack2 | 5 | 300 | 0.49 | 0.77 | 0.79 | 0.93 | 0 |
| thpack3 | 8 | 300 | 0.67 | 0.83 | 0.84 | 0.97 | 0 |
| thpack4 | 10 | 300 | 0.73 | 0.87 | 0.87 | 0.97 | 0 |
| thpack5 | 12 | 300 | 0.80 | 0.89 | 0.89 | 1.01 | 1 |
| thpack6 | 15 | 300 | 0.82 | 0.92 | 0.92 | 1.03 | 3 |
| thpack7 | 20 | 300 | 0.83 | 0.96 | 0.96 | 1.06 | 34 |
| demo-samples | - | 159 | 0.59 | 0.93 | 0.96 | 1.09 | 17 |
| custom-problems | - | 63 | 0.20 | 0.88 | 0.94 | 1.08 | 6 |
| **All** |  | 2322 | 0.20 | 0.86 | 0.87 | 1.09 | 61 |

## 📊 Codec: gzip against deflate

On each pack and layout, the ViPaq token's length with gzip divided by its length with deflate. Above 1 means
gzip is longer.

| Layout | Packs | Min | Mean | Median | Max | Packs where gzip is shorter |
|---|---|---|---|---|---|---|
| row | 2322 | 1.02 | 1.12 | 1.06 | 3.00 | 0 |
| columnar | 2322 | 1.03 | 1.14 | 1.08 | 3.00 | 0 |

## 📊 Compressed against uncompressed

On each pack, the compressed length divided by the uncompressed length of the same format. Above 1 means
compressing made it longer. The last column is the item count of those packs.

| Format | Codec | Min | Mean | Median | Max | Packs where it got longer | Their items |
|---|---|---|---|---|---|---|---|
| ViPaq, row | deflate | 0.22 | 0.42 | 0.40 | 1.00 | 0 | - |
| ViPaq, row | gzip | 0.23 | 0.49 | 0.42 | 3.00 | 119 | 0 to 8 |
| ViPaq, columnar | deflate | 0.05 | 0.36 | 0.34 | 1.00 | 0 | - |
| ViPaq, columnar | gzip | 0.06 | 0.44 | 0.37 | 3.00 | 109 | 0 to 13 |
| protobuf | deflate | 0.17 | 0.40 | 0.38 | 1.33 | 36 | 0 to 1 |
| protobuf | gzip | 0.18 | 0.45 | 0.40 | 3.33 | 84 | 0 to 6 |
| JSON | deflate | 0.07 | 0.17 | 0.14 | 1.31 | 9 | 0 |
| JSON | gzip | 0.08 | 0.18 | 0.15 | 1.75 | 39 | 0 to 1 |
| compact notation | deflate | 0.17 | 0.32 | 0.28 | 2.00 | 54 | 0 to 2 |
| compact notation | gzip | 0.18 | 0.37 | 0.30 | 5.00 | 79 | 0 to 6 |

## 📊 ViPaq, columnar and deflate, against the others

### 📈 Against protobuf

On each pack, the columnar deflate ViPaq token's length divided by protobuf's with deflate. Below 1
means ViPaq is shorter.

| Group | Packs | Min | Mean | Median | Max | Packs where ViPaq is longer |
|---|---|---|---|---|---|---|
| thpack1 | 300 | 0.19 | 0.46 | 0.49 | 0.60 | 0 |
| thpack2 | 300 | 0.31 | 0.51 | 0.52 | 0.62 | 0 |
| thpack3 | 300 | 0.44 | 0.56 | 0.56 | 0.66 | 0 |
| thpack4 | 300 | 0.48 | 0.59 | 0.59 | 0.67 | 0 |
| thpack5 | 300 | 0.52 | 0.61 | 0.61 | 0.70 | 0 |
| thpack6 | 300 | 0.54 | 0.63 | 0.63 | 0.73 | 0 |
| thpack7 | 300 | 0.56 | 0.66 | 0.66 | 0.75 | 0 |
| demo-samples | 159 | 0.33 | 0.63 | 0.65 | 0.77 | 0 |
| custom-problems | 63 | 0.13 | 0.62 | 0.65 | 0.78 | 0 |
| **All** | 2322 | 0.13 | 0.58 | 0.59 | 0.78 | 0 |

### 📈 Against JSON

On each pack, the columnar deflate ViPaq token's length divided by JSON's with deflate. Below 1
means ViPaq is shorter.

| Group | Packs | Min | Mean | Median | Max | Packs where ViPaq is longer |
|---|---|---|---|---|---|---|
| thpack1 | 300 | 0.16 | 0.38 | 0.40 | 0.50 | 0 |
| thpack2 | 300 | 0.27 | 0.43 | 0.44 | 0.52 | 0 |
| thpack3 | 300 | 0.37 | 0.47 | 0.47 | 0.54 | 0 |
| thpack4 | 300 | 0.41 | 0.50 | 0.50 | 0.57 | 0 |
| thpack5 | 300 | 0.44 | 0.51 | 0.51 | 0.60 | 0 |
| thpack6 | 300 | 0.46 | 0.53 | 0.53 | 0.59 | 0 |
| thpack7 | 300 | 0.48 | 0.56 | 0.56 | 0.62 | 0 |
| demo-samples | 159 | 0.17 | 0.28 | 0.29 | 0.41 | 0 |
| custom-problems | 63 | 0.09 | 0.23 | 0.21 | 0.37 | 0 |
| **All** | 2322 | 0.09 | 0.46 | 0.48 | 0.62 | 0 |

### 📈 Against compact notation

On each pack, the columnar deflate ViPaq token's length divided by compact notation's with deflate. Below 1
means ViPaq is shorter.

| Group | Packs | Min | Mean | Median | Max | Packs where ViPaq is longer |
|---|---|---|---|---|---|---|
| thpack1 | 300 | 0.20 | 0.53 | 0.55 | 0.68 | 0 |
| thpack2 | 300 | 0.35 | 0.60 | 0.61 | 0.75 | 0 |
| thpack3 | 300 | 0.50 | 0.66 | 0.66 | 0.78 | 0 |
| thpack4 | 300 | 0.56 | 0.69 | 0.69 | 0.81 | 0 |
| thpack5 | 300 | 0.62 | 0.72 | 0.72 | 0.85 | 0 |
| thpack6 | 300 | 0.65 | 0.74 | 0.74 | 0.86 | 0 |
| thpack7 | 300 | 0.68 | 0.77 | 0.78 | 0.87 | 0 |
| demo-samples | 159 | 0.32 | 0.60 | 0.62 | 0.75 | 0 |
| custom-problems | 63 | 0.14 | 0.58 | 0.60 | 0.75 | 0 |
| **All** | 2322 | 0.14 | 0.66 | 0.68 | 0.87 | 0 |

## 📂 Where the numbers come from

The "Raw", "Deflate" and "Gzip" tables in [measurements/encoded-size/](measurements/encoded-size), written by
`just measure vipaq`. Item types per Bischoff set come from `lib/results/measurements/packing-efficiency.md`.
