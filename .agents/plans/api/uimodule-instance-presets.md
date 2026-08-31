---
description: The instance page reads its presets over HTTP from the browser - move it to server-side state
state: idea
waits-on: "the maintainer expanding it"
paths:
  - "api/src/Binacle.Net.UIModule/**"
  - "api/src/Binacle.Net.Kernel/**"
---

# The instance page's presets, without a call

`_js/instance.js` calls `GET /api/v4/presets` from the browser and renders the reply. The server is holding
`BinPresetOptions` when it renders that page - it is going out to the network to ask itself a question it can
answer. It also breaks behind an auth layer, a proxy or a CORS rule, which is exactly what an operator is
looking at when they open that page. Version, environment and the switch list are already rendered
server-side.

**Settled: no browser fetch and no `HttpClient`.** The module makes no server-side HTTP calls and has one
project reference. Neither changes.

**The closed route, measured 22 Aug 2026.** `BinPresetOptions` lives in the entry project, which references
the UI module, so a project reference back is a cycle. The pattern to copy is `FeatureOptions` and
`ReservedPathOptions`: a plain options type in `Kernel`, filled in `Program.cs`, read through `IOptions`.
`Instance.cshtml.cs` already injects one. Keep `Bin` types out of it - `Kernel` references neither
`Binacle.Packing` nor `Binacle.Geometry`, and pulling one in would be the largest cost here by far.

**The wrinkle to settle deliberately:** `BinPresetOptions.ReloadOnChange` is `true`, so a bag filled once at
startup goes stale the moment an operator edits `Presets.json` - and that operator is who this is for. Bind
through `IOptionsMonitor`, or accept the staleness and say so where the decision is taken.

**When it lands, `_js/instance.js` is dead** along with its webpack entry, and the UI module doc's section
explaining why the page fetches over HTTP gets replaced, not edited.

**It also closes a watched gap.** That fetch is the only v4 call shipped inside the image, and v4 is
documented as able to change in a patch release. `plans/unwatched-gaps.md` holds the gap and defers the fix
here, so nobody edits the call to v3 in a file this plan removes.
