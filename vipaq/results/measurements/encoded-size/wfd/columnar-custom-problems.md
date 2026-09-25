# Encoded size - Custom problems, WFD, columnar layout

Written by `just measure vipaq` from `Binacle.ViPaq.EncodedSize` over 21 real packs (custom-problems), packed by WFD, in the columnar layout. One file per group per algorithm per layout; the others sit beside this one. Do not edit.

## Raw
ViPaq and protobuf are base64 lengths; JSON and compact are text lengths, because text is its own stored form. Widths are the bin / item / coordinate bits ViPaq picked. ViPaq/Proto compares those two columns, in this codec.

| Scenario                                | Items | Widths   | ViPaq | Proto | JSON  | Compact | ViPaq/Proto |
|-----------------------------------------|-------|----------|-------|-------|-------|---------|-------------|
| Baseline_15x15x15-1_FitsIn_60x40x20     | 1     | 8/8/8    | 20    | 24    | 109   | 25      | 0.83        |
| Baseline_25x25x25-1_FitsIn_60x40x30     | 1     | 8/8/8    | 20    | 24    | 109   | 25      | 0.83        |
| Baseline_40x40x40-1_DoesNotFit_60x40x30 | 0     | 8/8/8    | 12    | 12    | 55    | 8       | 1.00        |
| Baseline_5x5x5-1_FitsIn_60x40x10        | 1     | 8/8/8    | 20    | 24    | 106   | 22      | 0.83        |
| Baseline_5x5x80_TooLong_60x40x30        | 0     | 8/8/8    | 12    | 12    | 55    | 8       | 1.00        |
| Complex_FitsInMedium_1                  | 16    | 8/8/8    | 140   | 256   | 954   | 300     | 0.55        |
| Complex_FitsInSmall_1                   | 5     | 8/8/8    | 52    | 84    | 329   | 93      | 0.62        |
| Simple_15x15x15-16_FitIn_60x40x30       | 16    | 8/8/8    | 140   | 260   | 962   | 308     | 0.54        |
| Simple_15x15x15-8_FitIn_60x40x20        | 8     | 8/8/8    | 76    | 128   | 504   | 154     | 0.59        |
| Simple_16bit-4_FitIn_600x400x300        | 4     | 16/16/16 | 80    | 88    | 297   | 99      | 0.91        |
| Simple_2Same_1Rotated_FitIn_60x40x10    | 2     | 8/8/8    | 28    | 40    | 165   | 43      | 0.70        |
| Simple_2Same_1Rotated_FitIn_60x40x20    | 2     | 8/8/8    | 28    | 40    | 165   | 43      | 0.70        |
| Simple_2Same_FitIn_60x40x10             | 2     | 8/8/8    | 28    | 40    | 165   | 43      | 0.70        |
| Simple_2Same_FitIn_60x40x20             | 2     | 8/8/8    | 28    | 40    | 165   | 43      | 0.70        |
| Simple_30x30x30-2_FitIn_60x40x30        | 2     | 8/8/8    | 28    | 40    | 165   | 43      | 0.70        |
| Simple_30x30x30-3_DoesNotFitIn_60x40x30 | 2     | 8/8/8    | 28    | 40    | 165   | 43      | 0.70        |
| Simple_5x5x5-100_FitIn_60x40x10         | 100   | 8/8/8    | 812   | 1568  | 5413  | 1567    | 0.52        |
| Simple_5x5x5-13_FitIn_50x50x50          | 13    | 8/8/8    | 116   | 192   | 739   | 199     | 0.60        |
| Simple_5x5x5-200_FitIn_50x50x50         | 200   | 8/8/8    | 1612  | 3376  | 10774 | 3128    | 0.48        |
| Simple_5x5x5-50_FitIn_50x50x50          | 50    | 8/8/8    | 412   | 776   | 2724  | 778     | 0.53        |
| Simple_5x5x5-5_FitIn_50x50x50           | 5     | 8/8/8    | 52    | 80    | 317   | 81      | 0.65        |

## Deflate
Every column is the base64 length after deflate. Widths are the bin / item / coordinate bits ViPaq picked. ViPaq/Proto compares those two columns, in this codec.

| Scenario                                | Items | Widths   | ViPaq | Proto | JSON | Compact | ViPaq/Proto |
|-----------------------------------------|-------|----------|-------|-------|------|---------|-------------|
| Baseline_15x15x15-1_FitsIn_60x40x20     | 1     | 8/8/8    | 20    | 28    | 104  | 32      | 0.71        |
| Baseline_25x25x25-1_FitsIn_60x40x30     | 1     | 8/8/8    | 20    | 28    | 104  | 32      | 0.71        |
| Baseline_40x40x40-1_DoesNotFit_60x40x30 | 0     | 8/8/8    | 12    | 16    | 72   | 16      | 0.75        |
| Baseline_5x5x5-1_FitsIn_60x40x10        | 1     | 8/8/8    | 20    | 28    | 100  | 28      | 0.71        |
| Baseline_5x5x80_TooLong_60x40x30        | 0     | 8/8/8    | 12    | 16    | 72   | 16      | 0.75        |
| Complex_FitsInMedium_1                  | 16    | 8/8/8    | 72    | 116   | 196  | 108     | 0.62        |
| Complex_FitsInSmall_1                   | 5     | 8/8/8    | 44    | 68    | 148  | 68      | 0.65        |
| Simple_15x15x15-16_FitIn_60x40x30       | 16    | 8/8/8    | 36    | 96    | 184  | 100     | 0.38        |
| Simple_15x15x15-8_FitIn_60x40x20        | 8     | 8/8/8    | 32    | 60    | 144  | 64      | 0.53        |
| Simple_16bit-4_FitIn_600x400x300        | 4     | 16/16/16 | 40    | 60    | 124  | 56      | 0.67        |
| Simple_2Same_1Rotated_FitIn_60x40x10    | 2     | 8/8/8    | 24    | 32    | 112  | 40      | 0.75        |
| Simple_2Same_1Rotated_FitIn_60x40x20    | 2     | 8/8/8    | 28    | 36    | 112  | 44      | 0.78        |
| Simple_2Same_FitIn_60x40x10             | 2     | 8/8/8    | 24    | 32    | 112  | 40      | 0.75        |
| Simple_2Same_FitIn_60x40x20             | 2     | 8/8/8    | 28    | 36    | 112  | 44      | 0.78        |
| Simple_30x30x30-2_FitIn_60x40x30        | 2     | 8/8/8    | 20    | 32    | 108  | 36      | 0.62        |
| Simple_30x30x30-3_DoesNotFitIn_60x40x30 | 2     | 8/8/8    | 20    | 32    | 108  | 36      | 0.62        |
| Simple_5x5x5-100_FitIn_60x40x10         | 100   | 8/8/8    | 140   | 316   | 432  | 308     | 0.44        |
| Simple_5x5x5-13_FitIn_50x50x50          | 13    | 8/8/8    | 40    | 80    | 152  | 76      | 0.50        |
| Simple_5x5x5-200_FitIn_50x50x50         | 200   | 8/8/8    | 268   | 600   | 792  | 584     | 0.45        |
| Simple_5x5x5-50_FitIn_50x50x50          | 50    | 8/8/8    | 76    | 168   | 264  | 176     | 0.45        |
| Simple_5x5x5-5_FitIn_50x50x50           | 5     | 8/8/8    | 28    | 44    | 116  | 44      | 0.64        |

## Gzip
Every column is the base64 length after gzip. Widths are the bin / item / coordinate bits ViPaq picked. ViPaq/Proto compares those two columns, in this codec.

| Scenario                                | Items | Widths   | ViPaq | Proto | JSON | Compact | ViPaq/Proto |
|-----------------------------------------|-------|----------|-------|-------|------|---------|-------------|
| Baseline_15x15x15-1_FitsIn_60x40x20     | 1     | 8/8/8    | 44    | 52    | 128  | 56      | 0.85        |
| Baseline_25x25x25-1_FitsIn_60x40x30     | 1     | 8/8/8    | 44    | 52    | 128  | 56      | 0.85        |
| Baseline_40x40x40-1_DoesNotFit_60x40x30 | 0     | 8/8/8    | 36    | 40    | 96   | 40      | 0.90        |
| Baseline_5x5x5-1_FitsIn_60x40x10        | 1     | 8/8/8    | 44    | 52    | 124  | 52      | 0.85        |
| Baseline_5x5x80_TooLong_60x40x30        | 0     | 8/8/8    | 36    | 40    | 96   | 40      | 0.90        |
| Complex_FitsInMedium_1                  | 16    | 8/8/8    | 96    | 140   | 220  | 132     | 0.69        |
| Complex_FitsInSmall_1                   | 5     | 8/8/8    | 68    | 92    | 172  | 92      | 0.74        |
| Simple_15x15x15-16_FitIn_60x40x30       | 16    | 8/8/8    | 60    | 120   | 208  | 124     | 0.50        |
| Simple_15x15x15-8_FitIn_60x40x20        | 8     | 8/8/8    | 56    | 84    | 168  | 88      | 0.67        |
| Simple_16bit-4_FitIn_600x400x300        | 4     | 16/16/16 | 64    | 84    | 148  | 80      | 0.76        |
| Simple_2Same_1Rotated_FitIn_60x40x10    | 2     | 8/8/8    | 48    | 56    | 136  | 64      | 0.86        |
| Simple_2Same_1Rotated_FitIn_60x40x20    | 2     | 8/8/8    | 52    | 60    | 136  | 68      | 0.87        |
| Simple_2Same_FitIn_60x40x10             | 2     | 8/8/8    | 48    | 56    | 136  | 64      | 0.86        |
| Simple_2Same_FitIn_60x40x20             | 2     | 8/8/8    | 52    | 60    | 136  | 68      | 0.87        |
| Simple_30x30x30-2_FitIn_60x40x30        | 2     | 8/8/8    | 44    | 56    | 132  | 60      | 0.79        |
| Simple_30x30x30-3_DoesNotFitIn_60x40x30 | 2     | 8/8/8    | 44    | 56    | 132  | 60      | 0.79        |
| Simple_5x5x5-100_FitIn_60x40x10         | 100   | 8/8/8    | 164   | 340   | 456  | 332     | 0.48        |
| Simple_5x5x5-13_FitIn_50x50x50          | 13    | 8/8/8    | 64    | 104   | 176  | 100     | 0.62        |
| Simple_5x5x5-200_FitIn_50x50x50         | 200   | 8/8/8    | 292   | 624   | 816  | 608     | 0.47        |
| Simple_5x5x5-50_FitIn_50x50x50          | 50    | 8/8/8    | 100   | 192   | 288  | 200     | 0.52        |
| Simple_5x5x5-5_FitIn_50x50x50           | 5     | 8/8/8    | 52    | 68    | 140  | 68      | 0.76        |

