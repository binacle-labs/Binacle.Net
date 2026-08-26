---
description: The ten Ruby gems produce no coverage, so they are absent from the coverage table and from Sonar. Add simplecov per gem, without breaking the rule that a gem must be droppable into an unrelated site.
state: ready
waits-on: "nothing"
paths:
  - "ruby/**"
  - "tooling/**"
---

# Coverage for the Ruby gems

**Decided 27 Aug 2026: yes, the gems get coverage.** The alternative was writing down that Ruby coverage is
deliberately absent. That was weighed and rejected — this is not a question any more, and it should not be
reopened.

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

So simplecov goes in as a development dependency of **each gem**, required from **each**
`spec/spec_helper.rb`. That is:

- ten gemspecs,
- ten spec helpers,
- the lock file,
- the rspec leaf learning the coverage format it currently ignores,
- one Sonar property, plus confirming Sonar imports the format at all.

## Start with the unverified part

**Nobody has checked what SonarCloud does with Ruby coverage on the current plan.** Check that first, before
the ten gemspecs. If Sonar imports nothing, the coverage table is the only consumer — the work still gets
done, but it is worth knowing which of the two outcomes was bought before touching ten published gemspecs.

## What will bite

**simplecov must start before the code it measures is loaded**, or it reports near-zero on files that were
already required. In a gem's spec helper that means it goes above the `require` of the gem itself.

**Each gem writes its own report**, and the coverage folder holds one flat file per suite named after the
project. Ruby's output is a folder by default, the same trap the TS leaf already works around - lift the file
out and name it after the gem, or the folder shape stops being one file per suite.

**The rubocop rule `Gemspec/DevelopmentDependencies` already fires on every gemspec.** Adding another
development dependency makes that finding larger, on a config nobody has run clean yet.

**A gemspec is read by strangers.** Ten published gemspecs each gain a development dependency, so keep the
addition to one line per file and nothing else.

## Done when

- [ ] Every gem writes a coverage report, and the table has a row for all ten.
      `just coverage all` then count the rows; ten gem names must appear.
- [ ] Sonar imports them, or the analysis settings say in one line why it does not.
      **By eye.** Read the analysis settings, then read the project in SonarCloud.
- [ ] No gem requires anything above its own folder.
      `grep -rn "require_relative \"\.\./\.\./" ruby/*/spec/` returns nothing.
