---
description: Two android icons are copied into all three sites and the UI module and referenced by nothing. Either a web app manifest names them, or they stop being copied.
state: proposed
waits-on: "a call - write the manifest, or drop the two files"
---

# Two icons nothing points at

`assets/android-chrome-192x192.png` and `assets/android-chrome-512x512.png` are copied by `gulpfile.js`
into `sites/www`, `sites/demo`, `sites/docs` and the UI module. **Nothing anywhere references either
file.**

`grep -rn "android-chrome\|webmanifest\|manifest.json"` over the sites, the gulpfile and the UI module
finds nothing. They exist for a web app manifest, and **there is no manifest on any site.**

## Two ways out

**Write the manifest.** Each site gets a small `site.webmanifest` at its root naming the two icons, and one
`<link rel="manifest">` in the head. This is what the files are for, and it is what makes a site installable
on Android.

**Stop copying them.** One line in the ignore list in `gulpfile.js`, and the two files leave every site.

**Recommendation: write the manifest.** The files exist, they are already shipped, and they are already paid
for in image size. Deleting them throws away work that is done.

## What will bite

**A manifest needs strings that are not in any config today** - `name`, `short_name`, `theme_color`,
`background_color`. Two of the three sites already carry `title` and `default_theme`; the colours are a
choice nobody has made. **Do not invent them.** Read them off the built stylesheet or ask.

**A manifest link is a new element in the head of every page of every site.** It is the only change, and it
is on all 127 pages.

**The UI module is copied separately from the three sites.** Whichever way this goes, it is four targets in
`gulpfile.js`, not three.

**A manifest with a wrong icon path fails silently.** Nothing renders it, no console error on a normal page
load. Check it in a browser's application panel, not by reading the file.

## Done when

- [ ] The two icons are either referenced by a manifest or no longer copied.
      `grep -rn "android-chrome" sites/ gulpfile.js` finds one or the other, not neither
- [ ] If the manifest wins, every value in it traces to config or a measured colour.
      **By eye.** A colour nobody chose is a bug
- [ ] If the manifest wins, all three sites serve it and link it.
      `test -f artifacts/www/site.webmanifest` and the same for demo and docs, and
      `grep -c 'rel="manifest"' artifacts/*/index.html` is 1 each
