---
description: The ten gems now report coverage locally. What is left is confirming SonarCloud imports the simplecov json at all, which no run has yet shown.
state: blocked
waits-on: "a Sonar run - none has happened since the gems landed"
paths:
  - "ruby/**"
  - "tooling/**"
---

# Coverage for the Ruby gems

**Built.** All ten gems report coverage. `tooling/tests.ruby-coverage.rb` holds the setup and the rspec recipe loads
it through `RUBYOPT`, so nothing under `ruby/` reaches above its own gem folder and no gemspec gained a
development dependency. `cobertura` feeds the local table, `sonar` writes simplecov json.

How it works is in the Ruby doc. Do not restate it here.

## What is left

**Nobody has seen SonarCloud read the json.** `sonar.ruby.coverage.reportPaths` is set and the gems are in
scope - nothing in the analysis settings excludes `ruby/` - so the expectation is that the numbers go from
"every gem line uncovered" to the local figures, which are 96% and up for nine of the ten.

**If Sonar imports nothing, the local table is the only consumer.** The work still stands; what changes is
one line in the analysis settings saying so, and the ledger entry that goes with it.

## Done when

- [ ] Sonar imports them, or the analysis settings say in one line why it does not.
      **By eye.** Run the Sonar workflow, then read the gems in SonarCloud. They must not sit at zero.
