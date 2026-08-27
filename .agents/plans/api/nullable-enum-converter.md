---
description: The Kernel's nullable-enum converter accepts any string and returns default rather than refusing it - a leniency nobody chose, with one dead throw sitting on top of it
state: proposed
waits-on: "a yes or a no on whether an unknown enum string is refused at deserialization or left to validation"
paths:
  - "api/src/Binacle.Net.Kernel/Serialization/**"
---

# The nullable-enum converter never rejects a bad value

**Found 2026-08-27.** `api/src/Binacle.Net.Kernel/Serialization/JsonStringNullableEnumConverter.cs:104` -
`TryParseEnumFromString` has five returns and every one of them is `return true`, including the fall-through
at `:129` where nothing matched. **An unknown enum string is read as `default!` rather than refused**, so
validation is the only thing that can catch it.

**The lenient parse is worth a deliberate decision rather than an accident**, and the dead throw says the
opposite of what the code does. That is the whole reason this is a plan and not a one-line fix.

## What follows from it

- **The `throw new JsonException(...)` at `:78` in `ReadAsPropertyName` is unreachable.** Nothing can make
  `TryParseEnumFromString` return `false`.
- **Not a live defect where the value is required.** `default!` on a nullable enum is null, and the
  validators reject null - `v3/Contracts/Algorithm.cs:26` and the v4 one both `NotNull()`, so a bogus
  algorithm is a 422 either way.
- **It does change behaviour on a patch.** `ServiceModule/v0/Contracts/Admin/SubscriptionPatchRequest.cs`
  only requires that one of `Type` and `Status` has a value, so a request naming a bogus `Type` and a good
  `Status` passes and **the bad field is silently dropped instead of refused.**

## Done when

- [ ] An unknown enum string has one stated behaviour, and the code does that.
      **By eye.** Either `TryParseEnumFromString` can return `false` and the caller refuses, or a comment at
      the fall-through says the leniency is deliberate and names what catches it.
- [ ] No unreachable throw is left claiming otherwise.
      **By eye.** `:78` is either reachable or gone.
- [ ] A patch naming a bogus `Type` and a good `Status` does not silently drop the bad field.
      Send that request to the subscription patch endpoint and read the response.
