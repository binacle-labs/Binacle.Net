# Binacle.FluxResults

Result and union types for the API - `FluxUnion<T0, T1>` and the typed results (`Success`, `NotFound`,
`Conflict`, and the rest) that a repository or handler returns instead of throwing.

This is the code from the FluxResults NuGet package, v1.0.0. That package is retired and the project now
carries the types itself. Only what this repo uses came over - the three- and four-arm unions and the
`FluxResult<T>` wrapper were dropped. The namespace became `Binacle.FluxResults`; the type names did not
change.

| File | What is in it |
|---|---|
| `TypedResults.cs` | The `ITypedResult` interfaces, the ten result structs, and the `TypedResult` factory |
| `FluxUnion.cs` | `FluxUnion<T0, T1>` - a two-arm union with implicit conversion from either arm and `Match` |
| `IFluxUnion.cs` | `IFluxUnion` and its `Is<T>` / `As<T>` / `Unwrap<T>` / `TryGetValue<T>` extensions |
