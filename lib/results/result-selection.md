# Result selection

**Not worth optimizing.** After the algorithms run, one result is picked to return. The slowest v2 pick
takes 0.44% of the time of the fastest pack of the 700 Bischoff problems (FFD v2 on thpack1_72, 7.0 μs).
Every request packs at least once before it picks, so making the pick faster cannot make a request
noticeably faster.

...

## 📊 What a pick costs

| Selector       | Scenarios | Slowest v2 pick | Share of the fastest pack | Time, v2 against v1 | Memory, v2 against v1 |
  |----------------|-----------|-----------------|---------------------------|---------------------|-----------------------|
| Best bin       | 4         | 25.3 ns         | 0.36%                     | 0.18×               | 0.11×                 |
| Smallest bin   | 4         | 30.6 ns         | 0.44%                     | 0.24×               | 0.10×                 |
| Best algorithm | 3         | 6.2 ns          | 0.09%                     | 1.14×               | 0.72×                 |

Best algorithm is slower in v2 where a full result exists: v1 stops at the first fully packed result, v2
scores every candidate. It is still a few nanoseconds.
