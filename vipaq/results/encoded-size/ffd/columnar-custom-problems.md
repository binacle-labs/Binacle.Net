# Encoded size - Custom problems, FFD, columnar layout

Written by `just measure vipaq` from `Binacle.ViPaq.EncodedSize` over 21 real packs (custom-problems), packed by FFD, in the columnar layout. One file per group per algorithm per layout; the others sit beside this one. Do not edit.

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
| Complex_FitsInSmall_1                   | 5     | 8/8/8    | 52    | 80    | 328   | 92      | 0.65        |
| Simple_15x15x15-16_FitIn_60x40x30       | 16    | 8/8/8    | 140   | 260   | 962   | 308     | 0.54        |
| Simple_15x15x15-8_FitIn_60x40x20        | 8     | 8/8/8    | 76    | 128   | 504   | 154     | 0.59        |
| Simple_16bit-4_FitIn_600x400x300        | 4     | 16/16/16 | 80    | 84    | 295   | 97      | 0.95        |
| Simple_2Same_1Rotated_FitIn_60x40x10    | 2     | 8/8/8    | 28    | 40    | 165   | 43      | 0.70        |
| Simple_2Same_1Rotated_FitIn_60x40x20    | 2     | 8/8/8    | 28    | 40    | 165   | 43      | 0.70        |
| Simple_2Same_FitIn_60x40x10             | 2     | 8/8/8    | 28    | 40    | 165   | 43      | 0.70        |
| Simple_2Same_FitIn_60x40x20             | 2     | 8/8/8    | 28    | 40    | 165   | 43      | 0.70        |
| Simple_30x30x30-2_FitIn_60x40x30        | 2     | 8/8/8    | 28    | 40    | 165   | 43      | 0.70        |
| Simple_30x30x30-3_DoesNotFitIn_60x40x30 | 2     | 8/8/8    | 28    | 40    | 165   | 43      | 0.70        |
| Simple_5x5x5-100_FitIn_60x40x10         | 100   | 8/8/8    | 812   | 1636  | 5383  | 1537    | 0.50        |
| Simple_5x5x5-13_FitIn_50x50x50          | 13    | 8/8/8    | 116   | 200   | 736   | 196     | 0.58        |
| Simple_5x5x5-200_FitIn_50x50x50         | 200   | 8/8/8    | 1612  | 3348  | 10782 | 3136    | 0.48        |
| Simple_5x5x5-50_FitIn_50x50x50          | 50    | 8/8/8    | 412   | 800   | 2706  | 760     | 0.52        |
| Simple_5x5x5-5_FitIn_50x50x50           | 5     | 8/8/8    | 52    | 80    | 315   | 79      | 0.65        |

## Deflate
Every column is the base64 length after deflate. Widths are the bin / item / coordinate bits ViPaq picked. ViPaq/Proto compares those two columns, in this codec.

| Scenario                                | Items | Widths   | ViPaq | Proto | JSON | Compact | ViPaq/Proto |
|-----------------------------------------|-------|----------|-------|-------|------|---------|-------------|
| Baseline_15x15x15-1_FitsIn_60x40x20     | 1     | 8/8/8    | 20    | 28    | 104  | 32      | 0.71        |
| Baseline_25x25x25-1_FitsIn_60x40x30     | 1     | 8/8/8    | 20    | 28    | 104  | 32      | 0.71        |
| Baseline_40x40x40-1_DoesNotFit_60x40x30 | 0     | 8/8/8    | 12    | 16    | 72   | 16      | 0.75        |
| Baseline_5x5x5-1_FitsIn_60x40x10        | 1     | 8/8/8    | 20    | 28    | 100  | 28      | 0.71        |
| Baseline_5x5x80_TooLong_60x40x30        | 0     | 8/8/8    | 12    | 16    | 72   | 16      | 0.75        |
| Complex_FitsInMedium_1                  | 16    | 8/8/8    | 72    | 120   | 204  | 112     | 0.60        |
| Complex_FitsInSmall_1                   | 5     | 8/8/8    | 44    | 68    | 152  | 72      | 0.65        |
| Simple_15x15x15-16_FitIn_60x40x30       | 16    | 8/8/8    | 52    | 100   | 188  | 104     | 0.52        |
| Simple_15x15x15-8_FitIn_60x40x20        | 8     | 8/8/8    | 36    | 64    | 144  | 64      | 0.56        |
| Simple_16bit-4_FitIn_600x400x300        | 4     | 16/16/16 | 40    | 60    | 128  | 60      | 0.67        |
| Simple_2Same_1Rotated_FitIn_60x40x10    | 2     | 8/8/8    | 24    | 32    | 112  | 40      | 0.75        |
| Simple_2Same_1Rotated_FitIn_60x40x20    | 2     | 8/8/8    | 28    | 36    | 112  | 44      | 0.78        |
| Simple_2Same_FitIn_60x40x10             | 2     | 8/8/8    | 24    | 32    | 112  | 40      | 0.75        |
| Simple_2Same_FitIn_60x40x20             | 2     | 8/8/8    | 28    | 36    | 112  | 44      | 0.78        |
| Simple_30x30x30-2_FitIn_60x40x30        | 2     | 8/8/8    | 20    | 32    | 108  | 36      | 0.62        |
| Simple_30x30x30-3_DoesNotFitIn_60x40x30 | 2     | 8/8/8    | 20    | 32    | 108  | 36      | 0.62        |
| Simple_5x5x5-100_FitIn_60x40x10         | 100   | 8/8/8    | 132   | 360   | 520  | 344     | 0.37        |
| Simple_5x5x5-13_FitIn_50x50x50          | 13    | 8/8/8    | 48    | 84    | 160  | 80      | 0.57        |
| Simple_5x5x5-200_FitIn_50x50x50         | 200   | 8/8/8    | 204   | 680   | 972  | 668     | 0.30        |
| Simple_5x5x5-50_FitIn_50x50x50          | 50    | 8/8/8    | 92    | 204   | 312  | 192     | 0.45        |
| Simple_5x5x5-5_FitIn_50x50x50           | 5     | 8/8/8    | 32    | 44    | 120  | 48      | 0.73        |

## Gzip
Every column is the base64 length after gzip. Widths are the bin / item / coordinate bits ViPaq picked. ViPaq/Proto compares those two columns, in this codec.

| Scenario                                | Items | Widths   | ViPaq | Proto | JSON | Compact | ViPaq/Proto |
|-----------------------------------------|-------|----------|-------|-------|------|---------|-------------|
| Baseline_15x15x15-1_FitsIn_60x40x20     | 1     | 8/8/8    | 44    | 52    | 128  | 56      | 0.85        |
| Baseline_25x25x25-1_FitsIn_60x40x30     | 1     | 8/8/8    | 44    | 52    | 128  | 56      | 0.85        |
| Baseline_40x40x40-1_DoesNotFit_60x40x30 | 0     | 8/8/8    | 36    | 40    | 96   | 40      | 0.90        |
| Baseline_5x5x5-1_FitsIn_60x40x10        | 1     | 8/8/8    | 44    | 52    | 124  | 52      | 0.85        |
| Baseline_5x5x80_TooLong_60x40x30        | 0     | 8/8/8    | 36    | 40    | 96   | 40      | 0.90        |
| Complex_FitsInMedium_1                  | 16    | 8/8/8    | 96    | 144   | 228  | 136     | 0.67        |
| Complex_FitsInSmall_1                   | 5     | 8/8/8    | 68    | 92    | 176  | 96      | 0.74        |
| Simple_15x15x15-16_FitIn_60x40x30       | 16    | 8/8/8    | 76    | 124   | 212  | 128     | 0.61        |
| Simple_15x15x15-8_FitIn_60x40x20        | 8     | 8/8/8    | 60    | 88    | 168  | 88      | 0.68        |
| Simple_16bit-4_FitIn_600x400x300        | 4     | 16/16/16 | 64    | 84    | 152  | 84      | 0.76        |
| Simple_2Same_1Rotated_FitIn_60x40x10    | 2     | 8/8/8    | 48    | 56    | 136  | 64      | 0.86        |
| Simple_2Same_1Rotated_FitIn_60x40x20    | 2     | 8/8/8    | 52    | 60    | 136  | 68      | 0.87        |
| Simple_2Same_FitIn_60x40x10             | 2     | 8/8/8    | 48    | 56    | 136  | 64      | 0.86        |
| Simple_2Same_FitIn_60x40x20             | 2     | 8/8/8    | 52    | 60    | 136  | 68      | 0.87        |
| Simple_30x30x30-2_FitIn_60x40x30        | 2     | 8/8/8    | 44    | 56    | 132  | 60      | 0.79        |
| Simple_30x30x30-3_DoesNotFitIn_60x40x30 | 2     | 8/8/8    | 44    | 56    | 132  | 60      | 0.79        |
| Simple_5x5x5-100_FitIn_60x40x10         | 100   | 8/8/8    | 156   | 384   | 544  | 368     | 0.41        |
| Simple_5x5x5-13_FitIn_50x50x50          | 13    | 8/8/8    | 72    | 108   | 184  | 104     | 0.67        |
| Simple_5x5x5-200_FitIn_50x50x50         | 200   | 8/8/8    | 228   | 704   | 996  | 692     | 0.32        |
| Simple_5x5x5-50_FitIn_50x50x50          | 50    | 8/8/8    | 116   | 228   | 336  | 216     | 0.51        |
| Simple_5x5x5-5_FitIn_50x50x50           | 5     | 8/8/8    | 56    | 68    | 144  | 72      | 0.82        |

