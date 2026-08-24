---
description: The three footers each replace {now} with the build year by hand. jekyll-filters now has expand_year; move the footers onto it.
state: ready
waits-on: "nothing - site session work"
---

# The three footers use the year filter

Every footer assigns `now_year` from `site.time` and replaces `{now}` with it by hand. Three footers, four
replaces - `sites/docs/_includes/footer.html` does it twice, once for `copyright` and again for `license`.

**The filter exists.** `ruby/jekyll-filters` gained `expand_year` on 24 Aug 2026, with a spec. **No new gem
and no new line in any `Gemfile` or `plugins:` list** - all three sites already load `jekyll-filters`.

What is left is the site half: drop the `now_year` assign and the replace, and pipe the value through the
filter instead.

```liquid
{{ site.data.footer.copyright | expand_year }}
```

## What will bite

**One `now_year` assign feeds both docs lines.** Convert `copyright` alone and the licence line loses the
value it was reading, so it ships `{now}` as text.

**The filter reads `site.time`**, which is what the footers read today. The built strings do not change.

**www trims its whitespace and the others do not.** www writes `{%- assign -%}`; docs and demo write
`{% assign %}`. Removing those lines changes the whitespace around the value on all three, which is invisible
in the rendered page and visible in a byte diff of the built file.

## Done when

- [ ] No site does the replace by hand.
      `grep -rn 'replace: "{now}"' sites/` finds nothing
- [ ] `{now}` survives only in the data files.
      `grep -rln "{now}" sites/` lists `_data/footer.yml` and nothing else
- [ ] All three sites build and the footer year is the current one.

## The other half is done

**`{% vlink %}` is a gem.** `ruby/binacle-docs-versions` holds it along with the version stamps, and
`sites/docs/_plugins/` is empty. It went in as a one-site gem that hardcodes the docs vocabulary rather than
as a portable one, which is what kept it out of the generic set.

Nothing about that half is left. The footers are all this plan still holds.
