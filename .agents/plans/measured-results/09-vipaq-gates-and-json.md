---
description: Step 9 - the two round-trip gates become unit tests over ViPaq.Data and run under just test; the curated-picks check stays in the measure project
state: ready
waits-on: "step 8's gate"
horizon: next-release
paths: ["vipaq/**", ".agents/docs/vipaq/**"]
---

# Step 9 - the ViPaq gates

Shape: [results.md](results.md), the `Binacle.ViPaq.EncodedSize` bullets. Protocol: the orchestrator.

## The step

- The three pre-report checks in `Binacle.ViPaq.EncodedSize/PreReportChecks/` are of two kinds.
  `ReportPathRoundTripCheck` and `ForcedWidthRoundTripCheck` round-trip every real pack in every codec and
  layout, one at a forced 16-bit width - the only round trip of every real pack, and today it runs only
  when someone runs the report. They become xunit theories in `Binacle.ViPaq.UnitTests`, which references
  `Binacle.ViPaq.Data` for the packs - the support-projects shape's folder table - and never
  `Binacle.ViPaq.Testing`. The theories drive the real serializer, or the internal `ProtocolEncoder` the
  unit tests can already see, not the report's own `ViPaqEncoder`. That is a change in what is checked.
- `CuratedPicksCheck` asserts the curated picks name real scenarios. The picks live in `Testing`, which the
  unit tests never reference, so it stays where it is: the measure project's startup, one sentence before
  the report.
- The vipaq dependency doc's "UnitTests never references the kernel" becomes "references `ViPaq.Data`,
  never `ViPaq.Testing`", with the reason: the spec gate must not lean on the harness's rival encoder. The
  memory about Sonar issue ignores repeats the old sentence; same fix.

## Open before starting

- Whether the two theories are one class with the pack set as `MemberData`, or one per check. Read how the
  existing vector theories are shaped and match them.
- The forced-width case needs a header the public serializer will not produce; find the narrowest internal
  seam and name it in the test.

## Done when

- [x] `ls vipaq/measure/Binacle.ViPaq.EncodedSize/PreReportChecks/` lists the curated check only.
- [x] `grep -o 'Include="[^"]*csproj"' vipaq/test/Binacle.ViPaq.UnitTests/*.csproj` names `Binacle.ViPaq.Data`
      and not `Binacle.ViPaq.Testing`.
- [x] `grep -rln "Binacle.ViPaq.Data" vipaq/test/Binacle.ViPaq.UnitTests --include=*.cs` names the theory file.
- [x] `grep -rn "but does not yet\|does not reference it today\|UnitTests never touches" .agents/docs .agents/design .agents/memory` is empty;
      the wall says data yes, Testing no.
- [x] `just test cs_binacle-vipaq_unit` passes and its output counts the pack theories.
