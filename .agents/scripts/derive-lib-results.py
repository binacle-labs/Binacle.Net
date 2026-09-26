"""Writes the derived lib result files from the raw files. Run from the repo root.

The files it writes are drafts under review; nothing in the build runs this."""
import re
import textwrap
import statistics as st

RES = 'lib/results'
ALGS = ['FFD', 'BFD', 'WFD']
SETS = [str(i) for i in range(1, 8)]
UNIT = {'ns': 1e-3, 'μs': 1, 'ms': 1e3}
MEM = {'B': 1 / 1024, 'KB': 1, 'MB': 1024}

# fill, from the measure file
fill = {}   # (alg, problem) -> fill
types = {}  # set -> item types
for line in open(f'{RES}/measurements/packing-efficiency.md'):
    if not line.startswith('| OrLibrary'):
        continue
    c = [x.strip() for x in line.strip().strip('|').split('|')]
    s = re.search(r'thpack(\d)_', c[0]).group(1)
    types[s] = c[1]
    for i, a in [(4, 'FFD'), (5, 'WFD'), (6, 'BFD')]:
        fill[(a, c[0])] = float(c[i])
problems = sorted({p for _, p in fill})


def bench(alg, op):
    d = {}
    for line in open(f'{RES}/benchmarks/baseline/algorithms/Full_{alg}_{op}.md'):
        c = [x.strip().strip('*').strip() for x in line.strip().strip('|').split('|')]
        if len(c) < 11 or c[0] not in ('v1', 'v2'):
            continue
        v, u = c[2].split()
        a, au = c[9].split()
        d[(c[0], c[1])] = (float(v.replace(',', '')) * UNIT[u], float(a.replace(',', '')) * MEM[au])
    return d


B = {(a, op): bench(a, op) for a in ALGS for op in ('Packing', 'Fitting')}


def in_set(s):
    return [p for p in problems if f'thpack{s}_' in p]


ROWS = [(f'thpack{s}', types[s], in_set(s)) for s in SETS] + [('**All 700**', '3 to 20', problems)]


def means_table(cols, value, fmt):
    """cols: [(header, key)]; value(key, problem) -> number."""
    out = ['| Set | Item types | ' + ' | '.join(h for h, _ in cols) + ' |',
           '|---|---|' + '---|' * len(cols)]
    for name, t, ps in ROWS:
        cells = [fmt(st.mean(value(k, p) for p in ps)) for _, k in cols]
        if name.startswith('**'):
            cells = [f'**{x}**' for x in cells]
        out.append(f'| {name} | {t} | ' + ' | '.join(cells) + ' |')
    return '\n'.join(out)


def spread_table(value, fmt):
    out = ['| Set | Item types | Min | Mean | Median | Max |', '|---|---|---|---|---|---|']
    for name, t, ps in ROWS:
        v = [value(p) for p in ps]
        cells = [fmt(x) for x in (min(v), st.mean(v), st.median(v), max(v))]
        if name.startswith('**'):
            cells = [f'**{x}**' for x in cells]
        out.append(f'| {name} | {t} | ' + ' | '.join(cells) + ' |')
    return '\n'.join(out)


def grid(cols):
    """cols: [(header, value(problem), aggregate over a set, fmt)]."""
    out = ['| Set | Item types | ' + ' | '.join(h for h, _, _, _ in cols) + ' |', '|---|---|' + '---|' * len(cols)]
    for name, t, ps in ROWS:
        cells = [fmt(agg([f(p) for p in ps])) for _, f, agg, fmt in cols]
        if name.startswith('**'):
            cells = [f'**{x}**' for x in cells]
        out.append(f'| {name} | {t} | ' + ' | '.join(cells) + ' |')
    return '\n'.join(out)


SPREAD = [('min', min), ('mean', st.mean), ('median', st.median), ('max', max)]


def write(name, doc):
    wrapped = [x if x.startswith(('|', '#')) or not x else textwrap.fill(x, 118, break_on_hyphens=False) for x in doc]
    open(f'{RES}/{name}.md', 'w').write('\n'.join(wrapped) + '\n')


pct = lambda x: f'{x:.2f}'
ratio = lambda x: f'{x:.2f}×'

MACHINE = ('Times come from one machine - an AMD Ryzen 9 9900X, 12 cores, .NET 10.0.12 - at the short job, three '
           'iterations per problem. One problem\'s time is rough; an average over 100 is not.')

# 1. packing efficiency stats
doc = ['# Packing efficiency stats', '',
       'How full each algorithm packs the 700 Bischoff problems, per set. Fill is the packed volume as a percentage '
       'of the bin, version 2. Computed from [measurements/packing-efficiency.md](measurements/packing-efficiency.md).', '',
       '## 📊 Mean fill per set', '',
       means_table([(a, a) for a in ALGS], lambda a, p: fill[(a, p)], pct)]
for a in ALGS:
    doc += ['', f'## 📊 {a} fill per set', '', spread_table(lambda p, a=a: fill[(a, p)], pct)]
write('packing-efficiency-stats', doc)

# 2. algorithm performance
def vs_ffd(a, p, i):
    return B[(a, 'Packing')][('v2', p)][i] / B[('FFD', 'Packing')][('v2', p)][i]


def fill_gain(a, p):
    return fill[(a, p)] - fill[('FFD', p)]


gain = lambda x: f'{x:+.2f}'
cols = [(f'{a} {h}', lambda p, f=f, a=a: f(a, p), st.mean, fmt)
        for a in ('BFD', 'WFD')
        for h, f, fmt in [('fill gain', fill_gain, gain), ('time', lambda a, p: vs_ffd(a, p, 0), ratio)]]
memory = {a: ratio(st.mean(vs_ffd(a, p, 1) for p in problems)) for a in ('BFD', 'WFD')}

doc = ['# Algorithm performance', '',
       'What BFD and WFD give and cost against FFD, packing, version 2. Fill gain is how many points fuller they '
       'pack than FFD. Time is how many times as long they take: each problem is divided by FFD on that same '
       'problem, then those are averaged, so FFD is 1.00×. Fill comes from '
       '[measurements/packing-efficiency.md](measurements/packing-efficiency.md), time from the '
       '`Full_*_Packing.md` reports in [benchmarks/baseline/algorithms/](benchmarks/baseline/algorithms).', '',
       MACHINE + ' The three algorithms ran as separate classes in one run of `just bench lib-algorithms-full`, '
       'which is why their times can be divided.', '',
       '## 📊 Fill gained and time paid, mean per set', '',
       grid(cols), '',
       f'Memory is about the same for all three: over all 700, BFD uses {memory["BFD"]} of FFD\'s memory and '
       f'WFD {memory["WFD"]}.']
for a in ('BFD', 'WFD'):
    doc += ['', f'## 📊 {a} packing time against FFD, per set', '',
            spread_table(lambda p, a=a: vs_ffd(a, p, 0), ratio)]
write('algorithm-performance', doc)

# 3. version differences, one file for packing and one for fitting
def v2_v1(a, op, p, i):
    d = B[(a, op)]
    return d[('v2', p)][i] / d[('v1', p)][i]


METRICS = [(0, 'time'), (1, 'memory')]
for op in ('Packing', 'Fitting'):
    value = lambda a, i, op=op: lambda p: v2_v1(a, op, p, i)
    overview = [(f'{a} {what}', value(a, i), st.mean, ratio) for a in ALGS for i, what in METRICS]
    doc = [f'# Version differences - {op.lower()}', '',
           f'Version 2 against version 1, {op.lower()}, per algorithm. Each problem is divided by v1 on that same '
           'problem, then those are averaged, so v1 is 1.00× and under it v2 is faster or allocates less. Computed '
           f'from the `Full_*_{op}.md` reports in [benchmarks/baseline/algorithms/](benchmarks/baseline/algorithms).', '',
           MACHINE + ' v1 and v2 of an algorithm ran side by side in one class, so the ratio compares anywhere.', '',
           f'## 📊 v2 against v1, mean per set', '',
           grid(overview)]
    for a in ALGS:
        spread = [(f'{what.capitalize()} {h}', value(a, i), agg, ratio) for i, what in METRICS for h, agg in SPREAD]
        doc += ['', f'## 📊 {a} v2 against v1, per set', '', grid(spread)]
    write(f'version-differences-{op.lower()}', doc)

# 4. result selection, v2 against v1, read by column name because the reports differ in columns
def bdn_rows(path):
    head, rows = None, []
    for line in open(path):
        if not line.startswith('|') or line.startswith('|-'):
            continue
        c = [x.strip().strip('*').strip() for x in line.strip().strip('|').split('|')]
        if c[0] == 'Method':
            head = c
        elif c[0]:
            rows.append(dict(zip(head, c)))
    return rows


SELECTION = [('Best bin', 'BestBin'), ('Smallest bin', 'SmallestBin'), ('Best algorithm', 'BestAlgorithm')]


def to_us(text):
    v, u = text.split()
    return float(v.replace(',', '')) * UNIT[u]


def bdn_ratio(text):
    return float(text.rstrip('x'))


# The fastest pack of the 700, any algorithm, v2: the hardest thing to call a pick small against.
fastest = min((B[(a, 'Packing')][('v2', p)][0], a, p) for a in ALGS for p in problems)
picks = {n: bdn_rows(f'{RES}/benchmarks/baseline/result-selection/{n}.md') for _, n in SELECTION}
slowest_v2 = {n: max(to_us(r['Mean']) for r in rows if r['Method'] == 'v2') for n, rows in picks.items()}
share = lambda x: f'{x:.2f}%'

out = ['| Selector | Scenarios | Slowest v2 pick | Share of the fastest pack | Time, v2 against v1 | '
       'Memory, v2 against v1 |', '|---|---|---|---|---|---|']
for label, n in SELECTION:
    v2 = [r for r in picks[n] if r['Method'] == 'v2']
    out.append(f"| {label} | {len(v2)} | {slowest_v2[n] * 1e3:.1f} ns | {share(slowest_v2[n] / fastest[0] * 100)} | "
               f"{ratio(st.mean(bdn_ratio(r['Ratio']) for r in v2))} | "
               f"{ratio(st.mean(bdn_ratio(r['Alloc Ratio']) for r in v2))} |")

worst = max(slowest_v2.values())
doc = ['# Result selection', '',
       f'**Not worth optimizing.** After the algorithms run, one result is picked to return. The slowest v2 pick '
       f'takes {share(worst / fastest[0] * 100)} of the time of the fastest pack of the 700 Bischoff problems '
       f'({fastest[1]} v2 on {fastest[2].replace("OrLibrary_", "")}, {fastest[0]:.1f} μs). Every request packs at '
       'least once before it picks, so making the pick faster cannot make a request noticeably faster.', '',
       'The selectors: the best algorithm\'s result, the best bin, the smallest bin. Each ran on three results, 20 '
       'in the "20 bins" scenarios. Time and memory against v1 are BenchmarkDotNet\'s own ratios per scenario, '
       'averaged, so v1 is 1.00×. From the reports in '
       '[benchmarks/baseline/result-selection/](benchmarks/baseline/result-selection); the pack time from the '
       '`Full_*_Packing.md` reports in [benchmarks/baseline/algorithms/](benchmarks/baseline/algorithms).', '',
       'Times come from one machine - an AMD Ryzen 9 9900X, 12 cores, .NET 10.0.12 - at the short job, three '
       'iterations per case, so a time is rough. At this size that does not change the answer.', '',
       '## 📊 What a pick costs', '', '\n'.join(out), '',
       'Best algorithm is slower in v2 where a full result exists: v1 stops at the first fully packed result, v2 '
       'scores every candidate. It is still a few nanoseconds.']
write('result-selection', doc)
print('written')
