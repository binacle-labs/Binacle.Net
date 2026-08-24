---
description: The ten Ruby gems produce no coverage, so they are absent from the coverage table and from Sonar. Adding simplecov collides with the rule that a gem must be droppable into an unrelated site.
state: proposed
waits-on: "a yes or a no - it is ten gems' published surface, and the alternative is writing down that Ruby coverage is deliberately absent"
paths:
  - "ruby/**"
  - "tooling/**"
---

# Coverage for the Ruby gems, or a decision that there is none

**Every other suite in the repo reports coverage. The ten gems report nothing.** The rspec leaf ignores the
coverage format it is handed, because Ruby's collector is simplecov and simplecov is not in the bundle.

**The failure mode is silence, not zero.** The coverage table builds its rows from the reports that exist, so
a suite that writes none has **no row** - it is not listed at zero, it is not listed at all. Same in Sonar:
there is no Ruby coverage path in the analysis settings, so those gems are outside the numbers entirely while
looking like part of the project.

**Right now that is ten gems, and the sites are built on them.** Nothing about the trend is good: the count
went from six to ten in one day.

## Why this is not a small change

**The gems are meant to be portable.** The rule is that a gem can be dropped into an unrelated Jekyll site
and work, and nothing in one refers to this product. **A shared spec helper above the gem folders breaks
that** - a gem installed from a package index has no parent directory to require.

So simplecov has to be a development dependency of **each gem**, required from **each** `spec/spec_helper.rb`.
That is:

- ten gemspecs,
- ten spec helpers,
- the lock file,
- the rspec leaf learning the coverage format it currently ignores,
- one Sonar property, plus confirming Sonar imports the format at all.

**The last one is unverified.** Nobody has checked what SonarCloud does with Ruby coverage on the current
plan. Check it before the ten gemspecs, not after - if it imports nothing, the table is the only consumer and
the case is much weaker.

## The case against, said properly

**These are small gems with tight suites and no branching logic worth the name.** A coverage number on a
Liquid tag that renders one string tells you very little that the spec count does not. The cost is a
development dependency added to ten published gemspecs, and every one of those is a file a stranger reads
when deciding whether to trust the gem.

**A number nobody acts on is worse than no number**, because it makes the table look complete.

## The other way out, and it is cheap

**Write down that Ruby coverage is deliberately absent**, in the design record that owns the coverage
pipeline, and say so in the tooling doc so the empty rows are expected rather than a bug someone re-discovers.
**That closes the question at the cost of one paragraph.**

**Leaving it open is the one outcome with no value** - the next person to read the coverage table finds ten
suites missing and has to work out from scratch whether that is deliberate.

## What will bite, if the answer is yes

**simplecov must start before the code it measures is loaded**, or it reports near-zero on files that were
already required. In a gem's spec helper that means it goes above the `require` of the gem itself.

**Each gem writes its own report**, and the coverage folder holds one flat file per suite named after the
project. Ruby's output is a folder by default, the same trap the TS leaf already works around - lift the file
out and name it after the gem, or the folder shape stops being one file per suite.

**The rubocop rule `Gemspec/DevelopmentDependencies` already fires on every gemspec.** Adding another
development dependency makes that finding larger, on a config nobody has run clean yet.

## Done when

Either:

- [ ] Every gem writes a coverage report, and the table has a row for all ten.
      `just coverage all` then count the rows; ten gem names must appear.
- [ ] Sonar imports them, or the settings say in one line why it does not.
      **By eye.** Read the analysis settings, then read the project in SonarCloud.

Or:

- [ ] The decision that there is no Ruby coverage is written where the coverage pipeline's reasoning lives,
      and the tooling doc says the empty rows are expected.
      **By eye.** Find the answer in the design record. If it is only in this plan, the box is open.
