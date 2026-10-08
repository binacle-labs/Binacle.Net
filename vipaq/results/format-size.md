# Format size

How long a ViPaq token is, stored as is, against protobuf, JSON and compact notation.

Every size is a character count: ViPaq and protobuf as base64, JSON and compact notation as text, because text
is their own stored form. Each pack is the result of packing one problem with FFD, WFD or BFD. Uncompressed,
the row and columnar layouts are the same length on every pack, so layout does not appear here.

## 📊 ViPaq against protobuf

On each pack, ViPaq's length divided by protobuf's. Below 1 means ViPaq is shorter. All three algorithms'
packs are pooled.

| Group | Packs | Min | Mean | Median | Max | Packs where ViPaq is longer | Packs where they are equal |
|---|---|---|---|---|---|---|---|
| thpack1 | 300 | 0.62 | 0.65 | 0.65 | 0.70 | 0 | 0 |
| thpack2 | 300 | 0.62 | 0.65 | 0.65 | 0.69 | 0 | 0 |
| thpack3 | 300 | 0.62 | 0.65 | 0.65 | 0.70 | 0 | 0 |
| thpack4 | 300 | 0.63 | 0.65 | 0.65 | 0.69 | 0 | 0 |
| thpack5 | 300 | 0.62 | 0.65 | 0.65 | 0.69 | 0 | 0 |
| thpack6 | 300 | 0.63 | 0.65 | 0.65 | 0.69 | 0 | 0 |
| thpack7 | 300 | 0.63 | 0.65 | 0.65 | 0.68 | 0 | 0 |
| demo-samples | 159 | 0.51 | 0.63 | 0.58 | 1.00 | 0 | 3 |
| custom-problems | 63 | 0.48 | 0.70 | 0.70 | 1.00 | 0 | 6 |
| **All** | 2322 | 0.48 | 0.65 | 0.65 | 1.00 | 0 | 9 |

Every pack where ViPaq is not shorter holds 0 items: 12 characters against 12.

## 📊 ViPaq against JSON

On each pack, ViPaq's length divided by JSON's. Below 1 means ViPaq is shorter. All three algorithms'
packs are pooled.

| Group | Packs | Min | Mean | Median | Max | Packs where ViPaq is longer | Packs where they are equal |
|---|---|---|---|---|---|---|---|
| thpack1 | 300 | 0.20 | 0.20 | 0.20 | 0.21 | 0 | 0 |
| thpack2 | 300 | 0.20 | 0.20 | 0.20 | 0.21 | 0 | 0 |
| thpack3 | 300 | 0.20 | 0.20 | 0.20 | 0.21 | 0 | 0 |
| thpack4 | 300 | 0.20 | 0.20 | 0.20 | 0.21 | 0 | 0 |
| thpack5 | 300 | 0.20 | 0.20 | 0.20 | 0.21 | 0 | 0 |
| thpack6 | 300 | 0.20 | 0.20 | 0.20 | 0.21 | 0 | 0 |
| thpack7 | 300 | 0.20 | 0.20 | 0.20 | 0.21 | 0 | 0 |
| demo-samples | 159 | 0.14 | 0.16 | 0.15 | 0.22 | 0 | 0 |
| custom-problems | 63 | 0.15 | 0.17 | 0.17 | 0.27 | 0 | 0 |
| **All** | 2322 | 0.14 | 0.20 | 0.20 | 0.27 | 0 | 0 |

## 📊 ViPaq against compact notation

On each pack, ViPaq's length divided by compact notation's. Below 1 means ViPaq is shorter. All three algorithms'
packs are pooled.

| Group | Packs | Min | Mean | Median | Max | Packs where ViPaq is longer | Packs where they are equal |
|---|---|---|---|---|---|---|---|
| thpack1 | 300 | 0.55 | 0.58 | 0.58 | 0.60 | 0 | 0 |
| thpack2 | 300 | 0.55 | 0.57 | 0.57 | 0.60 | 0 | 0 |
| thpack3 | 300 | 0.55 | 0.57 | 0.57 | 0.60 | 0 | 0 |
| thpack4 | 300 | 0.55 | 0.57 | 0.57 | 0.60 | 0 | 0 |
| thpack5 | 300 | 0.56 | 0.57 | 0.57 | 0.60 | 0 | 0 |
| thpack6 | 300 | 0.56 | 0.57 | 0.57 | 0.60 | 0 | 0 |
| thpack7 | 300 | 0.56 | 0.57 | 0.57 | 0.59 | 0 | 0 |
| demo-samples | 159 | 0.44 | 0.57 | 0.50 | 1.50 | 3 | 0 |
| custom-problems | 63 | 0.45 | 0.71 | 0.65 | 1.50 | 6 | 0 |
| **All** | 2322 | 0.44 | 0.58 | 0.57 | 1.50 | 9 | 0 |

Every pack where ViPaq is not shorter holds 0 items: 12 characters against 8.

## 📊 By algorithm

The median of the per-pack ratios over every pack, per algorithm that packed it.

| Algorithm | ViPaq ÷ protobuf | ViPaq ÷ JSON | ViPaq ÷ compact notation |
|---|---|---|---|
| FFD | 0.65 | 0.20 | 0.57 |
| WFD | 0.66 | 0.20 | 0.57 |
| BFD | 0.65 | 0.20 | 0.57 |

## 📂 Where the numbers come from

The "Raw" tables in [measurements/encoded-size/](measurements/encoded-size), written by `just measure vipaq`.
