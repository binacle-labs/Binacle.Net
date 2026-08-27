---
name: test-naming
description: How a test is named - <lang>_<project>_<kind>, derived from the assembly or package, and the tests are private
type: convention
when: adding or renaming a test
paths:
  - "tooling/tests.just"
  - "**/test/**"
---

A test is one project and one run, so it is also one CI step. Its recipe name is **derived, never
chosen**:

```
<lang>_<project>_<kind>
```

- **`lang`** is `cs`, `ts` or `rb`.
- **`project`** is the assembly or package name, lowercased, dots turned to dashes, with `UnitTests` /
  `IntegrationTests` stripped. `Binacle.Net.Kernel.UnitTests` gives `binacle-net-kernel`. Nothing is
  shortened - the segment has to name a real assembly, package or gem.
- **`kind`** is `unit` or `integration`, spelled out.
- **`_` separates the three segments, `-` belongs inside a name.** That is what makes
  `rb_jekyll-webmanifest_unit` readable: `jekyll-webmanifest` is the gem's own name.

Nothing here is a judgement call, so two names can never collide or sit a letter apart. The C# and TS twins
line up on purpose - `cs_binacle-vipaq_unit` beside `ts_binacle-vipaq_unit`.

**The tests are `[private]`.** Shell completion offers `all`, `image` and `sites`, not twenty-six tests. A
private recipe still runs by name, which is how CI and `just test <name>` reach it. `just test` with no
argument prints the test list.

There is **one recipe per test**, listed by hand rather than generated. A CI step names the recipe it runs,
so a red check names the suite.

**Why:** these names are read off a red CI step and half-typed with tab. Deriving them means the step name
and the test name cannot drift, and there is no decision to get wrong per test.

**How to apply:** derive the name from the project, do not invent one. A CI step's `name:` is the assembly,
package or gem in full; the `run:` is the derived recipe name. Nothing checks that the lists and the steps agree.
Adding a test to `all` is a separate judgement - `all` is the infra-free set, so a test joins it only once
someone confirms it passes with nothing brought up.
