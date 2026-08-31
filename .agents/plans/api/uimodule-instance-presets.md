---
description: The instance page reads its presets over HTTP from the browser - move it to server-side state
state: idea
waits-on: "nobody - it is an idea"
horizon: next-release
paths:
  - "api/src/Binacle.Net.UIModule/**"
  - "api/src/Binacle.Net.Kernel/**"
---

# The instance page's presets, without a call

`_js/instance.js` calls `GET /api/v4/presets` from the browser and renders the reply. The server is holding
`BinPresetOptions` when it renders that page, so it is going out to the network to ask itself a question it can
answer. The fetch also breaks behind an auth layer, a proxy or a CORS rule, which is exactly what an operator
is looking at when they open that page, and version, environment and the switch list are already rendered
server-side. Render the presets the same way.

## Research

### Settled - no browser fetch and no `HttpClient`

The module makes no server-side HTTP calls and has one project reference. Neither changes.

### 2026-08-22 - the closed route, measured

`BinPresetOptions` lives in the entry project, which references the UI module, so a project reference back is a
cycle. The pattern to copy is `FeatureOptions` and `ReservedPathOptions`: a plain options type in `Kernel`,
filled in `Program.cs`, read through `IOptions`. `Instance.cshtml.cs` already injects one.

**Keep `Bin` types out of it.** `Kernel` references neither `Binacle.Packing` nor `Binacle.Geometry`, and
pulling one in would be the largest cost here by far.

### Date not recorded - the wrinkle to settle deliberately

`BinPresetOptions.ReloadOnChange` is `true`, so a bag filled once at startup goes stale the moment an operator
edits `Presets.json` - and that operator is who this is for. Bind through `IOptionsMonitor`, or accept the
staleness and say so where the decision is taken.

### Date not recorded - what it takes with it

When it lands, `_js/instance.js` is dead along with its webpack entry, and the UI module doc's section
explaining why the page fetches over HTTP gets replaced, not edited.

It also closes a watched gap: that fetch is the only v4 call shipped inside the image, and v4 is documented as
able to change in a patch release. Nobody should edit the call to v3 in the meantime, because this removes the
file.
