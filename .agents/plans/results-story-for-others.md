---
description: A results story for people outside the project, told from the same lib and ViPaq results files the maintainer's own story uses
state: idea
waits-on: "nobody - it is an idea"
horizon: undecided
paths:
  - "lib/results/**"
  - "vipaq/results/**"
---

# A results story for others

The results files under `lib/results/` and `vipaq/results/` are written for the maintainer: they show him how
the work is going and what to decide. A version for others - a reader who wants to know whether the packer
fills well, how fast it is, how small a ViPaq token is - could be told from the same files, with different
words and fewer tables. What it must never do: compare a time across machines, or claim ViPaq is the smallest
format while MessagePack and a columnar protobuf are unmeasured. Where it would live (a README, the docs site,
www) is open.
