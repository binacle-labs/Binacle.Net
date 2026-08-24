---
description: robots.txt is three byte-identical copies of a legal text. Replace the body with a {% robots %} tag in ruby/.
state: ready
waits-on: "nothing"
---

# A {% robots %} tag

`robots.txt` tells crawlers what they may do. **All three sites carry the same 33 lines, byte for byte.**
Only the settings at the top of the file differ, and only because demo is missing a `nav:` line the other two
have.

**This is worth doing for what the text is, not for its size.** The body reserves rights under Article 4 of the
EU copyright directive and states that AI training is refused. **Three hand-kept copies of a legal paragraph is
the thing that goes out of step without anyone noticing** - and the copy that drifts is the one that stops
saying what you meant.

The tag also writes the `Sitemap:` lines, which each site already builds by looping its own sitemap
collection.

## The gem

**`ruby/jekyll-binacle-robots`.** One gem per tag, matching `jekyll-gtm` and `jekyll-filters`, which are one
purpose each. **The gem does not exist yet.** This plan builds it, then the site half.

## What changes, site by site

| Site | Change |
|---|---|
| **all three** | one line in `Gemfile`, one in `_config.yml` under `plugins:` - **only if the gem is new** |
| **www** | `pages/robots.txt` drops to its settings plus `{% robots %}` |
| **demo** | same. **Also gains the `nav: exclude: true` line** the other two have, so the three files match |
| **docs** | same |

**The `nav: exclude: true` line does nothing on www or demo.** Only docs reads `nav.exclude`, and only when
filtering its sidebar, which reads a different collection. It is added to demo so the three files are the same
file, not because it has an effect.

## What will bite

**Call it as `{% robots %}` on its own line, with nothing after it.** The tag emits no trailing newline;
the file's own newline supplies it. The whitespace-trimming form `{%- robots -%}` breaks the byte match.


**Do not reword the body while moving it.** It is a rights reservation. The check is a byte comparison of the
built file, not a read.

**The content signal lines are commented out today.** `# Content-Signals: ai-train=no, search=yes,
ai-input=no` sits behind a `#` on all three sites. **Move it exactly as it is.** Whether it should be live is
a separate question and not this plan's to answer.

## Done when

- [ ] The tag exists in `ruby/jekyll-binacle-robots` with a spec.
- [ ] No site holds the body.
      `grep -rln "Article 4" sites/` finds nothing. **Not a line count** - the front matter alone is
      eight lines on www and docs.
- [ ] The three built files are byte identical to each other apart from the host in the `Sitemap:` lines.
      `diff` the three `artifacts/*/robots.txt`
- [ ] Each built file is byte identical to what it was before, apart from nothing.
      Build from the previous commit and `diff`
