# Encoded size - Demo samples, BFD, row layout

Written by `just measure vipaq` from `Binacle.ViPaq.EncodedSize` over 53 real packs (demo-samples), packed by BFD, in the row layout. One file per group per algorithm per layout; the others sit beside this one. Do not edit.

## Raw
ViPaq and protobuf are base64 lengths; JSON and compact are text lengths, because text is its own stored form. Widths are the bin / item / coordinate bits ViPaq picked. ViPaq/Proto compares those two columns, in this codec.

| Scenario                                         | Items | Widths  | ViPaq | Proto | JSON | Compact | ViPaq/Proto |
|--------------------------------------------------|-------|---------|-------|-------|------|---------|-------------|
| DemoSample_00_TwoWinners_40x30x30                | 13    | 8/8/8   | 116   | 208   | 785  | 245     | 0.56        |
| DemoSample_00_TwoWinners_45x30x25                | 11    | 8/8/8   | 100   | 176   | 673  | 209     | 0.57        |
| DemoSample_01_OpeningSet_30x20x20                | 5     | 8/8/8   | 52    | 88    | 337  | 101     | 0.59        |
| DemoSample_01_OpeningSet_40x30x30                | 6     | 8/8/8   | 60    | 104   | 393  | 119     | 0.58        |
| DemoSample_01_OpeningSet_50x40x40                | 6     | 8/8/8   | 60    | 100   | 392  | 118     | 0.60        |
| DemoSample_02_PacksNowhere_30x30x30              | 1     | 8/8/8   | 20    | 24    | 109  | 25      | 0.83        |
| DemoSample_03_ThreeAnswers_60x40x20              | 13    | 8/8/8   | 116   | 200   | 783  | 243     | 0.58        |
| DemoSample_04_BfdLoses_60x35x25                  | 9     | 8/8/8   | 84    | 140   | 552  | 164     | 0.60        |
| DemoSample_05_OneOfEach_25x20x20                 | 1     | 8/8/8   | 20    | 24    | 109  | 25      | 0.83        |
| DemoSample_05_OneOfEach_40x30x25                 | 2     | 8/8/8   | 28    | 40    | 165  | 43      | 0.70        |
| DemoSample_06_LongItems_30x30x30                 | 2     | 8/8/8   | 28    | 40    | 165  | 43      | 0.70        |
| DemoSample_06_LongItems_50x30x20                 | 5     | 8/8/8   | 52    | 88    | 336  | 100     | 0.59        |
| DemoSample_07_TallItems_20x20x60                 | 4     | 8/8/8   | 44    | 68    | 266  | 68      | 0.65        |
| DemoSample_07_TallItems_40x40x30                 | 0     | 8/8/8   | 12    | 12    | 55   | 8       | 1.00        |
| DemoSample_08_CubeBin_34x34x34                   | 1     | 8/8/8   | 20    | 24    | 109  | 25      | 0.83        |
| DemoSample_08_CubeBin_60x40x40                   | 6     | 8/8/8   | 60    | 96    | 391  | 117     | 0.62        |
| DemoSample_09_BfdFitsMore_40x30x30               | 11    | 8/8/8   | 100   | 184   | 674  | 210     | 0.54        |
| DemoSample_09_BfdFitsMore_50x40x35               | 12    | 8/8/8   | 108   | 188   | 726  | 224     | 0.57        |
| DemoSample_10_SixTypes_22x20x16                  | 2     | 8/8/8   | 28    | 40    | 163  | 41      | 0.70        |
| DemoSample_10_SixTypes_35x30x25                  | 6     | 8/8/8   | 60    | 96    | 385  | 111     | 0.62        |
| DemoSample_10_SixTypes_45x35x30                  | 6     | 8/8/8   | 60    | 96    | 385  | 111     | 0.62        |
| DemoSample_11_SevenTypes_30x25x20                | 2     | 8/8/8   | 28    | 40    | 164  | 42      | 0.70        |
| DemoSample_11_SevenTypes_40x30x30                | 6     | 8/8/8   | 60    | 96    | 388  | 114     | 0.62        |
| DemoSample_11_SevenTypes_65x50x40                | 13    | 8/8/8   | 116   | 212   | 782  | 242     | 0.55        |
| DemoSample_12_MiddleBinWins_30x25x20             | 6     | 8/8/8   | 60    | 104   | 393  | 119     | 0.58        |
| DemoSample_12_MiddleBinWins_40x35x30             | 10    | 8/8/8   | 92    | 168   | 621  | 195     | 0.55        |
| DemoSample_12_MiddleBinWins_60x50x40             | 10    | 8/8/8   | 92    | 160   | 618  | 192     | 0.57        |
| DemoSample_13_TwentyFourCubes_30x25x20           | 12    | 8/8/8   | 108   | 196   | 734  | 232     | 0.55        |
| DemoSample_13_TwentyFourCubes_40x30x25           | 24    | 8/8/8   | 204   | 392   | 1420 | 462     | 0.52        |
| DemoSample_13_TwentyFourCubes_50x40x30           | 24    | 8/8/8   | 204   | 392   | 1420 | 462     | 0.52        |
| DemoSample_14_FlatItems_30x30x20                 | 4     | 8/8/8   | 44    | 64    | 272  | 74      | 0.69        |
| DemoSample_14_FlatItems_40x30x30                 | 8     | 8/8/8   | 76    | 120   | 492  | 142     | 0.63        |
| DemoSample_14_FlatItems_50x50x12                 | 8     | 8/8/8   | 76    | 132   | 494  | 144     | 0.58        |
| DemoSample_15_SameVolumeDifferentShape_100x20x20 | 8     | 8/8/8   | 76    | 120   | 502  | 152     | 0.63        |
| DemoSample_15_SameVolumeDifferentShape_40x40x25  | 16    | 8/8/8   | 140   | 260   | 962  | 308     | 0.54        |
| DemoSample_15_SameVolumeDifferentShape_50x25x32  | 16    | 8/8/8   | 140   | 260   | 962  | 308     | 0.54        |
| DemoSample_16_OnlyBfdFullyPacks_30x25x25         | 1     | 8/8/8   | 20    | 24    | 109  | 25      | 0.83        |
| DemoSample_16_OnlyBfdFullyPacks_35x30x30         | 1     | 8/8/8   | 20    | 24    | 109  | 25      | 0.83        |
| DemoSample_16_OnlyBfdFullyPacks_60x55x50         | 14    | 8/8/8   | 124   | 240   | 852  | 274     | 0.52        |
| DemoSample_17_FourBinsBfdAhead_30x30x25          | 7     | 8/8/8   | 68    | 120   | 447  | 135     | 0.57        |
| DemoSample_17_FourBinsBfdAhead_40x30x30          | 13    | 8/8/8   | 116   | 220   | 774  | 234     | 0.53        |
| DemoSample_17_FourBinsBfdAhead_45x40x35          | 14    | 8/8/8   | 124   | 232   | 830  | 252     | 0.53        |
| DemoSample_17_FourBinsBfdAhead_55x45x40          | 14    | 8/8/8   | 124   | 224   | 826  | 248     | 0.55        |
| DemoSample_18_FourBinsBfdFullyPacks_25x25x20     | 1     | 8/8/8   | 20    | 24    | 109  | 25      | 0.83        |
| DemoSample_18_FourBinsBfdFullyPacks_35x30x25     | 8     | 8/8/8   | 76    | 132   | 506  | 156     | 0.58        |
| DemoSample_18_FourBinsBfdFullyPacks_45x35x30     | 20    | 8/8/8   | 172   | 336   | 1158 | 352     | 0.51        |
| DemoSample_18_FourBinsBfdFullyPacks_50x40x40     | 20    | 8/8/8   | 172   | 336   | 1162 | 356     | 0.51        |
| DemoSample_19_FiveBins_25x20x20                  | 1     | 8/8/8   | 20    | 24    | 109  | 25      | 0.83        |
| DemoSample_19_FiveBins_35x30x25                  | 13    | 8/8/8   | 116   | 216   | 783  | 243     | 0.54        |
| DemoSample_19_FiveBins_45x35x30                  | 8     | 8/8/8   | 76    | 132   | 506  | 156     | 0.58        |
| DemoSample_19_FiveBins_50x45x40                  | 16    | 8/8/8   | 140   | 264   | 955  | 301     | 0.53        |
| DemoSample_19_FiveBins_60x60x50                  | 16    | 8/8/8   | 140   | 252   | 950  | 296     | 0.56        |
| DemoSample_20_OrLibraryThpack1_58_587x233x220    | 75    | 16/8/16 | 916   | 1424  | 4530 | 1634    | 0.64        |

## Deflate
Every column is the base64 length after deflate. Widths are the bin / item / coordinate bits ViPaq picked. ViPaq/Proto compares those two columns, in this codec.

| Scenario                                         | Items | Widths  | ViPaq | Proto | JSON | Compact | ViPaq/Proto |
|--------------------------------------------------|-------|---------|-------|-------|------|---------|-------------|
| DemoSample_00_TwoWinners_40x30x30                | 13    | 8/8/8   | 68    | 104   | 192  | 108     | 0.65        |
| DemoSample_00_TwoWinners_45x30x25                | 11    | 8/8/8   | 68    | 92    | 184  | 100     | 0.74        |
| DemoSample_01_OpeningSet_30x20x20                | 5     | 8/8/8   | 36    | 56    | 128  | 52      | 0.64        |
| DemoSample_01_OpeningSet_40x30x30                | 6     | 8/8/8   | 44    | 64    | 136  | 60      | 0.69        |
| DemoSample_01_OpeningSet_50x40x40                | 6     | 8/8/8   | 40    | 60    | 136  | 60      | 0.67        |
| DemoSample_02_PacksNowhere_30x30x30              | 1     | 8/8/8   | 20    | 28    | 92   | 28      | 0.71        |
| DemoSample_03_ThreeAnswers_60x40x20              | 13    | 8/8/8   | 72    | 100   | 196  | 104     | 0.72        |
| DemoSample_04_BfdLoses_60x35x25                  | 9     | 8/8/8   | 60    | 80    | 172  | 88      | 0.75        |
| DemoSample_05_OneOfEach_25x20x20                 | 1     | 8/8/8   | 20    | 28    | 100  | 32      | 0.71        |
| DemoSample_05_OneOfEach_40x30x25                 | 2     | 8/8/8   | 28    | 40    | 128  | 52      | 0.70        |
| DemoSample_06_LongItems_30x30x30                 | 2     | 8/8/8   | 24    | 36    | 100  | 36      | 0.67        |
| DemoSample_06_LongItems_50x30x20                 | 5     | 8/8/8   | 36    | 56    | 124  | 56      | 0.64        |
| DemoSample_07_TallItems_20x20x60                 | 4     | 8/8/8   | 36    | 48    | 116  | 48      | 0.75        |
| DemoSample_07_TallItems_40x40x30                 | 0     | 8/8/8   | 12    | 16    | 68   | 16      | 0.75        |
| DemoSample_08_CubeBin_34x34x34                   | 1     | 8/8/8   | 20    | 28    | 96   | 28      | 0.71        |
| DemoSample_08_CubeBin_60x40x40                   | 6     | 8/8/8   | 44    | 64    | 144  | 64      | 0.69        |
| DemoSample_09_BfdFitsMore_40x30x30               | 11    | 8/8/8   | 64    | 100   | 200  | 108     | 0.64        |
| DemoSample_09_BfdFitsMore_50x40x35               | 12    | 8/8/8   | 76    | 108   | 212  | 116     | 0.70        |
| DemoSample_10_SixTypes_22x20x16                  | 2     | 8/8/8   | 28    | 40    | 124  | 48      | 0.70        |
| DemoSample_10_SixTypes_35x30x25                  | 6     | 8/8/8   | 60    | 84    | 184  | 92      | 0.71        |
| DemoSample_10_SixTypes_45x35x30                  | 6     | 8/8/8   | 60    | 84    | 184  | 92      | 0.71        |
| DemoSample_11_SevenTypes_30x25x20                | 2     | 8/8/8   | 28    | 40    | 124  | 48      | 0.70        |
| DemoSample_11_SevenTypes_40x30x30                | 6     | 8/8/8   | 52    | 80    | 172  | 88      | 0.65        |
| DemoSample_11_SevenTypes_65x50x40                | 13    | 8/8/8   | 88    | 124   | 232  | 136     | 0.71        |
| DemoSample_12_MiddleBinWins_30x25x20             | 6     | 8/8/8   | 44    | 68    | 148  | 68      | 0.65        |
| DemoSample_12_MiddleBinWins_40x35x30             | 10    | 8/8/8   | 56    | 80    | 168  | 84      | 0.70        |
| DemoSample_12_MiddleBinWins_60x50x40             | 10    | 8/8/8   | 56    | 84    | 168  | 88      | 0.67        |
| DemoSample_13_TwentyFourCubes_30x25x20           | 12    | 8/8/8   | 56    | 80    | 152  | 80      | 0.70        |
| DemoSample_13_TwentyFourCubes_40x30x25           | 24    | 8/8/8   | 84    | 120   | 196  | 108     | 0.70        |
| DemoSample_13_TwentyFourCubes_50x40x30           | 24    | 8/8/8   | 84    | 120   | 200  | 112     | 0.70        |
| DemoSample_14_FlatItems_30x30x20                 | 4     | 8/8/8   | 32    | 40    | 120  | 48      | 0.80        |
| DemoSample_14_FlatItems_40x30x30                 | 8     | 8/8/8   | 52    | 68    | 156  | 72      | 0.76        |
| DemoSample_14_FlatItems_50x50x12                 | 8     | 8/8/8   | 48    | 64    | 144  | 68      | 0.75        |
| DemoSample_15_SameVolumeDifferentShape_100x20x20 | 8     | 8/8/8   | 48    | 56    | 140  | 64      | 0.86        |
| DemoSample_15_SameVolumeDifferentShape_40x40x25  | 16    | 8/8/8   | 64    | 96    | 180  | 100     | 0.67        |
| DemoSample_15_SameVolumeDifferentShape_50x25x32  | 16    | 8/8/8   | 72    | 96    | 188  | 100     | 0.75        |
| DemoSample_16_OnlyBfdFullyPacks_30x25x25         | 1     | 8/8/8   | 16    | 24    | 96   | 28      | 0.67        |
| DemoSample_16_OnlyBfdFullyPacks_35x30x30         | 1     | 8/8/8   | 20    | 28    | 100  | 28      | 0.71        |
| DemoSample_16_OnlyBfdFullyPacks_60x55x50         | 14    | 8/8/8   | 76    | 112   | 204  | 108     | 0.68        |
| DemoSample_17_FourBinsBfdAhead_30x30x25          | 7     | 8/8/8   | 52    | 84    | 168  | 84      | 0.62        |
| DemoSample_17_FourBinsBfdAhead_40x30x30          | 13    | 8/8/8   | 68    | 104   | 200  | 108     | 0.65        |
| DemoSample_17_FourBinsBfdAhead_45x40x35          | 14    | 8/8/8   | 72    | 112   | 204  | 112     | 0.64        |
| DemoSample_17_FourBinsBfdAhead_55x45x40          | 14    | 8/8/8   | 76    | 112   | 204  | 112     | 0.68        |
| DemoSample_18_FourBinsBfdFullyPacks_25x25x20     | 1     | 8/8/8   | 20    | 28    | 96   | 32      | 0.71        |
| DemoSample_18_FourBinsBfdFullyPacks_35x30x25     | 8     | 8/8/8   | 56    | 80    | 168  | 84      | 0.70        |
| DemoSample_18_FourBinsBfdFullyPacks_45x35x30     | 20    | 8/8/8   | 80    | 132   | 232  | 128     | 0.61        |
| DemoSample_18_FourBinsBfdFullyPacks_50x40x40     | 20    | 8/8/8   | 84    | 120   | 228  | 128     | 0.70        |
| DemoSample_19_FiveBins_25x20x20                  | 1     | 8/8/8   | 20    | 28    | 100  | 32      | 0.71        |
| DemoSample_19_FiveBins_35x30x25                  | 13    | 8/8/8   | 68    | 104   | 196  | 104     | 0.65        |
| DemoSample_19_FiveBins_45x35x30                  | 8     | 8/8/8   | 56    | 88    | 168  | 84      | 0.64        |
| DemoSample_19_FiveBins_50x45x40                  | 16    | 8/8/8   | 76    | 116   | 212  | 120     | 0.66        |
| DemoSample_19_FiveBins_60x60x50                  | 16    | 8/8/8   | 76    | 112   | 208  | 116     | 0.68        |
| DemoSample_20_OrLibraryThpack1_58_587x233x220    | 75    | 16/8/16 | 284   | 440   | 520  | 372     | 0.65        |

## Gzip
Every column is the base64 length after gzip. Widths are the bin / item / coordinate bits ViPaq picked. ViPaq/Proto compares those two columns, in this codec.

| Scenario                                         | Items | Widths  | ViPaq | Proto | JSON | Compact | ViPaq/Proto |
|--------------------------------------------------|-------|---------|-------|-------|------|---------|-------------|
| DemoSample_00_TwoWinners_40x30x30                | 13    | 8/8/8   | 92    | 128   | 216  | 132     | 0.72        |
| DemoSample_00_TwoWinners_45x30x25                | 11    | 8/8/8   | 92    | 116   | 208  | 124     | 0.79        |
| DemoSample_01_OpeningSet_30x20x20                | 5     | 8/8/8   | 60    | 80    | 152  | 76      | 0.75        |
| DemoSample_01_OpeningSet_40x30x30                | 6     | 8/8/8   | 68    | 88    | 160  | 84      | 0.77        |
| DemoSample_01_OpeningSet_50x40x40                | 6     | 8/8/8   | 64    | 84    | 160  | 84      | 0.76        |
| DemoSample_02_PacksNowhere_30x30x30              | 1     | 8/8/8   | 44    | 52    | 116  | 52      | 0.85        |
| DemoSample_03_ThreeAnswers_60x40x20              | 13    | 8/8/8   | 96    | 124   | 220  | 128     | 0.77        |
| DemoSample_04_BfdLoses_60x35x25                  | 9     | 8/8/8   | 84    | 104   | 196  | 112     | 0.81        |
| DemoSample_05_OneOfEach_25x20x20                 | 1     | 8/8/8   | 44    | 52    | 124  | 56      | 0.85        |
| DemoSample_05_OneOfEach_40x30x25                 | 2     | 8/8/8   | 52    | 64    | 152  | 76      | 0.81        |
| DemoSample_06_LongItems_30x30x30                 | 2     | 8/8/8   | 48    | 60    | 124  | 60      | 0.80        |
| DemoSample_06_LongItems_50x30x20                 | 5     | 8/8/8   | 60    | 80    | 148  | 80      | 0.75        |
| DemoSample_07_TallItems_20x20x60                 | 4     | 8/8/8   | 60    | 72    | 140  | 72      | 0.83        |
| DemoSample_07_TallItems_40x40x30                 | 0     | 8/8/8   | 36    | 40    | 92   | 40      | 0.90        |
| DemoSample_08_CubeBin_34x34x34                   | 1     | 8/8/8   | 44    | 52    | 120  | 52      | 0.85        |
| DemoSample_08_CubeBin_60x40x40                   | 6     | 8/8/8   | 68    | 88    | 168  | 88      | 0.77        |
| DemoSample_09_BfdFitsMore_40x30x30               | 11    | 8/8/8   | 88    | 124   | 224  | 132     | 0.71        |
| DemoSample_09_BfdFitsMore_50x40x35               | 12    | 8/8/8   | 100   | 132   | 236  | 140     | 0.76        |
| DemoSample_10_SixTypes_22x20x16                  | 2     | 8/8/8   | 52    | 64    | 148  | 72      | 0.81        |
| DemoSample_10_SixTypes_35x30x25                  | 6     | 8/8/8   | 84    | 108   | 208  | 116     | 0.78        |
| DemoSample_10_SixTypes_45x35x30                  | 6     | 8/8/8   | 84    | 108   | 208  | 116     | 0.78        |
| DemoSample_11_SevenTypes_30x25x20                | 2     | 8/8/8   | 52    | 64    | 148  | 72      | 0.81        |
| DemoSample_11_SevenTypes_40x30x30                | 6     | 8/8/8   | 76    | 104   | 196  | 112     | 0.73        |
| DemoSample_11_SevenTypes_65x50x40                | 13    | 8/8/8   | 112   | 148   | 256  | 160     | 0.76        |
| DemoSample_12_MiddleBinWins_30x25x20             | 6     | 8/8/8   | 68    | 92    | 172  | 92      | 0.74        |
| DemoSample_12_MiddleBinWins_40x35x30             | 10    | 8/8/8   | 80    | 104   | 192  | 108     | 0.77        |
| DemoSample_12_MiddleBinWins_60x50x40             | 10    | 8/8/8   | 80    | 108   | 192  | 112     | 0.74        |
| DemoSample_13_TwentyFourCubes_30x25x20           | 12    | 8/8/8   | 80    | 104   | 176  | 104     | 0.77        |
| DemoSample_13_TwentyFourCubes_40x30x25           | 24    | 8/8/8   | 108   | 144   | 220  | 132     | 0.75        |
| DemoSample_13_TwentyFourCubes_50x40x30           | 24    | 8/8/8   | 108   | 144   | 224  | 136     | 0.75        |
| DemoSample_14_FlatItems_30x30x20                 | 4     | 8/8/8   | 56    | 64    | 144  | 72      | 0.88        |
| DemoSample_14_FlatItems_40x30x30                 | 8     | 8/8/8   | 76    | 92    | 180  | 96      | 0.83        |
| DemoSample_14_FlatItems_50x50x12                 | 8     | 8/8/8   | 72    | 88    | 168  | 92      | 0.82        |
| DemoSample_15_SameVolumeDifferentShape_100x20x20 | 8     | 8/8/8   | 72    | 80    | 164  | 88      | 0.90        |
| DemoSample_15_SameVolumeDifferentShape_40x40x25  | 16    | 8/8/8   | 88    | 120   | 204  | 124     | 0.73        |
| DemoSample_15_SameVolumeDifferentShape_50x25x32  | 16    | 8/8/8   | 96    | 120   | 212  | 124     | 0.80        |
| DemoSample_16_OnlyBfdFullyPacks_30x25x25         | 1     | 8/8/8   | 40    | 48    | 120  | 52      | 0.83        |
| DemoSample_16_OnlyBfdFullyPacks_35x30x30         | 1     | 8/8/8   | 44    | 52    | 124  | 52      | 0.85        |
| DemoSample_16_OnlyBfdFullyPacks_60x55x50         | 14    | 8/8/8   | 100   | 136   | 228  | 132     | 0.74        |
| DemoSample_17_FourBinsBfdAhead_30x30x25          | 7     | 8/8/8   | 76    | 108   | 192  | 108     | 0.70        |
| DemoSample_17_FourBinsBfdAhead_40x30x30          | 13    | 8/8/8   | 92    | 128   | 224  | 132     | 0.72        |
| DemoSample_17_FourBinsBfdAhead_45x40x35          | 14    | 8/8/8   | 96    | 136   | 228  | 136     | 0.71        |
| DemoSample_17_FourBinsBfdAhead_55x45x40          | 14    | 8/8/8   | 100   | 136   | 228  | 136     | 0.74        |
| DemoSample_18_FourBinsBfdFullyPacks_25x25x20     | 1     | 8/8/8   | 44    | 52    | 120  | 56      | 0.85        |
| DemoSample_18_FourBinsBfdFullyPacks_35x30x25     | 8     | 8/8/8   | 80    | 104   | 192  | 108     | 0.77        |
| DemoSample_18_FourBinsBfdFullyPacks_45x35x30     | 20    | 8/8/8   | 104   | 156   | 256  | 152     | 0.67        |
| DemoSample_18_FourBinsBfdFullyPacks_50x40x40     | 20    | 8/8/8   | 108   | 144   | 252  | 152     | 0.75        |
| DemoSample_19_FiveBins_25x20x20                  | 1     | 8/8/8   | 44    | 52    | 124  | 56      | 0.85        |
| DemoSample_19_FiveBins_35x30x25                  | 13    | 8/8/8   | 92    | 128   | 220  | 128     | 0.72        |
| DemoSample_19_FiveBins_45x35x30                  | 8     | 8/8/8   | 80    | 112   | 192  | 108     | 0.71        |
| DemoSample_19_FiveBins_50x45x40                  | 16    | 8/8/8   | 100   | 140   | 236  | 144     | 0.71        |
| DemoSample_19_FiveBins_60x60x50                  | 16    | 8/8/8   | 100   | 136   | 232  | 140     | 0.74        |
| DemoSample_20_OrLibraryThpack1_58_587x233x220    | 75    | 16/8/16 | 308   | 464   | 544  | 396     | 0.66        |

