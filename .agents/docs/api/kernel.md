---
id: api/kernel
description: Binacle.Net.Kernel — shared patterns used by all API projects and modules
verified: 2026-09-04
check: IApiMarker and the registration helpers match api/src/Binacle.Net.Kernel/; the endpoint interface and convention tables match Endpoints/EndpointDefinitions.cs, Endpoints/EndpointConventions.cs and the registrar in Endpoints/ExtensionMethods/; both AddHealthCheck overloads still live in HealthChecks/ExtensionMethods/HealthCheckServiceCollectionExtensions.cs and grep for `AddHealthChecks()` across api/src still hits only DiagnosticsModule/ModuleDefinition.cs; Serialization/ still holds JsonStringNullableEnumConverter.cs (the factory, the internal NullableEnumConverter<T>, the internal EnumValueReader) and JsonEnumValueException.cs, and OpenApi/Transformers/EnumStringsSchemaTransformer.cs still matches on JsonStringEnumConverter and JsonStringNullableEnumConverter and only lists member names for the second; every section here names a type that still exists under Kernel/, and every folder under Kernel/ has a section
also_update:
  - api/endpoints
paths:
  - "api/src/Binacle.Net.Kernel/**"

---

# Kernel

`api/src/Binacle.Net.Kernel` is the foundation every other project references.
It provides no business logic — only patterns, infrastructure, and helpers.

## BindingResult\<T\>

Replaces the default model binding for endpoint handlers. It does two things in one step: deserialise the
JSON body and run FluentValidation. Handlers always receive a `BindingResult<T>` and call `ValidateAsync()`:

```csharp
internal async Task<IResult> HandleAsync(
    BindingResult<MyRequest> bindingResult, ...)
{
    return await bindingResult.ValidateAsync(async request => {
        // request is the validated, typed model
    });
}
```

What `ValidateAsync()` returns before calling your handler:

| Condition | Response |
|---|---|
| JSON parse error | `400` with `"Invalid JSON Format"` problem details |
| Null body | `400` with `"Malformed Request"` problem details |
| Unknown enum value (`JsonEnumValueException`) | `422`, keyed and worded like a missing value — see Serialization below |
| Validation failure | `422` with FluentValidation field errors |
| Other exception | `500` (exception details exposed in Development only) |

`BindingProblem` is the one place that makes this mapping, so the same input cannot answer 422 on one route
and 400 on the next. `JsonEnumValueException` is a `JsonException`, so it is matched first.

`BindingResult<T>` is registered automatically via `BindAsync()` — no DI setup needed.

## Endpoint Interfaces

| Interface | Used for |
|---|---|
| `IEndpointGroup` | Defines a route group prefix and shared metadata |
| `IGroupedEndpoint<TGroup>` | One endpoint inside a group (most common) |
| `IEndpoint` | One standalone endpoint, not in a group |
| `IEndpointConvention` | A module's hook into endpoints defined by another assembly |

The first three are discovered and registered automatically via
`RegisterEndpointsFromAssemblyContaining<TMarker>()`. `IEndpointConvention` is resolved from DI instead: a module
registers one, and the registrar applies every registered convention to each group.

**The registrar applies conventions in one `Finally`, never an `Add`.** An `Add` convention on a group runs
before each endpoint's own, so a convention that reads endpoint metadata would read a list still being filled
and find nothing, silently. `Finally` runs after. The registrar owns this so no module has to know it. With no
convention registered the block is skipped and no endpoint gets anything it did not ask for itself.

`RateLimitedMetadata` (same file) is the one marker the Kernel ships, placed by the `.RateLimited()` builder
extension. It says an endpoint is user compute and names no policy — see `$api/endpoints` for what it means and
`$api/modules/service` for the convention that reads it.

## IOptionalDependency\<T\>

Defined in the Kernel project at `Dependencies/Services/IOptionalDependency.cs`, but its namespace is
**`Binacle.Net.Services`** (not `Binacle.Net.Kernel.*`) — that is the `using` you need. The interface and
`OptionalDependency<T>` class are both `public`.

Wraps a service that may not be registered (e.g., a channel from a module that might be off).

```csharp
public OptionalDependency(IServiceProvider serviceProvider) {
    Value = serviceProvider.GetService<T>(); // null if not registered
}
```

Registered as open generic in `Program.cs`:

```csharp
services.AddTransient(typeof(IOptionalDependency<>), typeof(OptionalDependency<>));
```

Used by `BinacleService` to optionally write to the packing log channel without failing if DiagnosticsModule
packing logs are disabled.

## Feature.Manager

Created **before the DI container** in `Program.cs`. Reads from `IConfiguration` and environment variables.

```csharp
Feature.Manager = new FeatureManagerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .ReadFrom.EnvironmentVariables()
    .CreateManager();
```

After this point, `Feature.IsEnabled("FLAG_NAME")` is available anywhere.
Used in `Program.cs` to conditionally call `AddServiceModule()` and `AddUIModule()`.

Flags checked in `Program.cs`: `SERVICE_MODULE`, `UI_MODULE`, `SWAGGER_UI`, `SCALAR_UI`.
See `$api/modules` for what each flag enables and how modules use them.

## FeatureOptions

**`Feature.Manager` answers "is this switched on" before the container exists. `FeatureOptions` is the
in-container record of what actually got switched on**, and the two are separate on purpose: a flag can be set
and the feature still not registered.

```csharp
options.AddFeature("SwaggerUI", "/swagger");   // name, and where it answers
options.IsFeatureEnabled("SwaggerUI");
options.PathFor("SwaggerUI");                  // null for a feature with no URL
```

**The path is recorded by whoever switches the feature on**, because some of them are configurable and nothing
else can know where one ended up — the health check path comes from `HealthChecks.json`, so only the
DiagnosticsModule can supply it. `AddFeature` is a dictionary write, so registering the same name twice
replaces rather than duplicates.

The UI module's instance page reads it one feature at a time, through `IsFeatureEnabled` and `PathFor`.
`SystemHealthCheck` and `/_debug` list the lot: `EnabledFeatures` is the dictionary's key set, in no order, so
both sort it before printing.

## ReservedPathOptions

Path prefixes the API serves that **must never answer with a web page**.

```csharp
options.AddPrefix("/api");                     // a prefix with no leading slash gets one
reservedPaths.Covers(context.Request.Path);    // StartsWithSegments against every prefix
```

`Program.cs` reserves `/api`, `/openapi`, `/swagger` and `/scalar` whether or not the UI serving them is on;
each module adds its own. `AddPrefix` ignores an empty value and prepends the slash a `PathString` comparison
requires, so a prefix written `api` cannot fail every request. **Who declares what, and what the UI module does
with the answer, is in `$api/modules`** — nothing here decides policy.

Also surfaced by `SystemHealthCheck` and `/_debug`, beside the feature list.

## IModuleMarker / IApiMarker

Each module defines its own `IModuleMarker` — DiagnosticsModule, ServiceModule, and UIModule each have one.
The core API (`Binacle.Net`) uses `IApiMarker` instead (a separate interface, same pattern).
Neither interface has members — they exist only as assembly anchors for scanning:

```csharp
services.AddValidatorsFromAssemblyContaining<IModuleMarker>(...);
services.AddOpenApiDocumentsFromAssemblyContaining<IModuleMarker>();
app.RegisterEndpointsFromAssemblyContaining<IModuleMarker>();
```

This keeps each module's validators, OpenAPI docs, and endpoints isolated to its own assembly.
All three modules use this pattern: DiagnosticsModule (`$api/modules/diagnostics`), ServiceModule (`$api/modules/service`), UIModule (`$api/modules/ui`).

## IOpenApiDocument

Each module registers its own OpenAPI document by implementing `IOpenApiDocument`.
`Program.cs` scans for all registered documents and wires them into SwaggerUI / Scalar at startup.
The transformers, the group-level 500 wiring, and the external `OpenApiExamples` package are covered in
`$api/openapi`.

## IStartupTask

Post-build async initialization. Registered via `services.AddStartupTask<T>()`, run via `app.RunStartupTasksAsync()`.
Used by Infrastructure to create database schemas before the app starts serving requests — see ServiceModule (`$api/modules/service`).

## IConfigurationOptions

Base interface for strongly-typed config classes loaded from JSON files.
Provides: `FilePath`, `SectionName`, `Optional`, `ReloadOnChange`, and `GetEnvironmentFilePath(env)`.
Config is loaded relative to `Config_Files/` (set as base path in `Program.cs`).

Register a validated options class with `services.AddValidatableJsonConfigurationOptions<TOptions>()`
(`Configuration/ExtensionsMethods/ConfigurationExtensions.cs`): it adds the JSON file + env override + env vars,
binds the section, and runs FluentValidation at startup (`ValidateFluently().ValidateOnStart()`). Used in
`Program.cs` for `BinPresetOptions`, `CorsOptions`, and `ForwardedHeadersConfigurationOptions`, and by each
module's `ModuleDefinition`.

## Validation

`BindingResult<T>` handles **request-body** validation (above). The same FluentValidation machinery validates
**options** at startup and provides reusable helpers:

- `FluentValidationOptions<TOptions>` — an `IValidateOptions<TOptions>` that runs the registered
  `IValidator<TOptions>`; wired via the `ValidateFluently()` options-builder extension.
- `RuleBuilderValidationExtensions.MustNotThrow(...)` — a custom rule that passes unless the given action throws.
  Use it to assert "this construction/conversion succeeds" (e.g. a volume calc that could overflow).
- `ValidationExtensions.GetValidationSummary()` — groups a `ValidationResult` into `Dictionary<string, string[]>`,
  the shape fed to `HttpValidationProblemDetails` (the 422 body).
- `ValidationMessage` — the shared message text, so a value rejected at bind time and the same value rejected
  by a rule read the same. `JsonEnumValueException` uses `ValidationMessage.RequiredEnumValues`.

## Network

`Kernel/Network/IPEntry` reads an IP entry as an operator writes it in configuration: **a single address, or
CIDR notation**. Anywhere a module takes a list of addresses from a config file, it parses them through here, so
one spelling means one thing across the app.

```csharp
if (!IPEntry.TryParse(entry, out var network)) { /* refuse it */ }
var caller = IPEntry.Normalize(context.Connection.RemoteIpAddress);
```

The slash picks the form; nothing else is attempted. Two behaviours are worth knowing before you use it:

- **An entry must read as the host it admits.** `IPAddress.TryParse` still accepts the inet_aton forms -
  `010.10.10.10` is octal and lands on `8.10.10.10`, `0x0A.10.10.10` is hex, `10.1` and `167772161` both become
  `10.0.0.1` - and it drops an IPv6 scope id. `IPEntry` refuses anything that does not survive a round trip
  through `IPAddress.ToString()`, which is one rule instead of a table. IPv6 is held to the same rule, so
  `2001:0db8::1` must be written `2001:db8::1`.
- **Host bits are masked off**, as they are everywhere else in .NET: `192.168.1.1/24` is the whole
  `192.168.1.0/24`. The BCL does this silently and the docs claim it throws - it does not. **This is a caller
  obligation, not just a fact about this type:** a caller that hands the list to an operator has to say what
  each entry resolved to, because 256 addresses is not what the operator wrote.
  `HealthChecksProtectionMiddleware` is the one caller that honours it today.

`TryParse` never throws, including on an `AddressFamily` it does not know - it refuses instead. That is
deliberate: the input comes from a config file, and a config file must not be able to crash startup.

`Normalize` unmaps an IPv4-mapped IPv6 address, which a dual-mode socket produces for every IPv4 caller. The
entry side is normalised during parse, so an IPv4-mapped CIDR entry (`::ffff:192.168.1.0/120`) is refused rather
than parsed into something that matches no caller. That refusal exists because `IPNetwork.Contains` is not
symmetric: an unmapped network contains a mapped caller, but a mapped network does not contain an unmapped one,
so such an entry would match a container's caller and not a real IPv4 one. Carrying the prefix over instead
(taking 96 off it, so `/120` becomes `/24`) works and was measured, if a reason to accept the form ever appears.

Tested in `api/test/Binacle.Net.Kernel.UnitTests/Network/`.

## Logging

Timed operations (in `Kernel/Logging`):

- `logger.BeginTimedOperation("template", args)` → returns an `IDisposable`; on dispose it logs
  `"{template} completed in {OperationDurationMs}ms"`. Default level Information.
- `logger.BeginTimedActivityOperation("message")` → same, and also starts an `Activity` (tracing span) named
  `message` on `Binacle.Net.Diagnostics.ActivitySource`. One `using` gives both a timed log and a span.
- `logger.EnrichState(...)` — wraps a dictionary / string set into a `BeginScope` for structured-log enrichment.

## Logs (generic pipeline)

`Binacle.Net.Kernel/Logs/` holds a **generic, feature-agnostic** log pipeline. It has no packing types and no
reference to `Binacle.CompactNotation` — a feature plugs in its own request and entry types.

- `ILogEntryConvertible<TLogEntry>` (`Logs/Models`) — a channel request implements it:
  `TLogEntry ToLogEntry(DateTimeOffset timestamp)`.
- `LogsProcessor<TRequest, TLog>` (`Logs/Services`, `where TRequest : ILogEntryConvertible<TLog>`) — a generic
  `BackgroundService`. Drains a `Channel<TRequest>`, calls `request.ToLogEntry(timeProvider.GetUtcNow())`,
  JSON-serialises the entry, and appends one line to a dated file. Knows nothing about any feature's types.
- `LogsProcessorOptions<TChannelRequest>` (`Logs/Models`) — `Path` / `FileNameFormat` / `DateFormat` +
  `MaxConsecutiveAllowedExceptions` (default 10) and `RetentionDays` (nullable, default null). The type param
  only keys the DI registration.
- `LogsRetentionProcessor<TRequest>` (`Logs/Services`) — a second `BackgroundService` that deletes files past
  `RetentionDays`. It runs its own loop and never touches the write channel, so a slow sweep cannot stall
  logging. `RetentionDays` null means it does nothing: these logs are an archive, so deletion is opt-in.
- `ILogParametersProvider` (`Logs/Models`) — `IReadOnlyList<string> ToLogParameters()`. A request's parameter type
  implements it so the background converter can project loose parameter strings without seeing the API's enums.
- `AddLogProcessor<TChannelRequest, TLog>(optionsFactory, channelFactory)` (`Logs/ExtensionMethods`, namespace
  `Binacle.Net`) — registers the channel, the options, and **both** hosted services (processor and retention).
  The owning feature supplies the types + factories.

The concrete packing feature (the request/entry types and their registration) lives in DiagnosticsModule — see
`$api/modules/diagnostics`.

`BinacleService` is the live producer: it injects `IOptionalDependency<Channel<AlgorithmOperationLogChannelRequest>>`
and writes via `WriteToChannelAsync(...)`, which **no-ops when the channel isn't registered** (i.e. when the
DiagnosticsModule packing-log feature is off).

## HealthChecks

`Kernel/HealthChecks/ExtensionMethods/HealthCheckServiceCollectionExtensions.cs` holds two `AddHealthCheck`
extensions on `IServiceCollection`. Its namespace is **`Binacle.Net`** (not `Binacle.Net.Kernel.*`) — that is the
`using` you need.

```csharp
services.AddHealthCheck<SqliteHealthCheck>("Database", HealthStatus.Unhealthy, ServiceTags);
services.AddHealthCheck("Database", sp => new SqliteHealthCheck(...), HealthStatus.Unhealthy, ServiceTags);
```

Both append a `HealthCheckRegistration` to `HealthCheckServiceOptions` through `services.Configure(...)`. The
generic overload builds the check with `ActivatorUtilities.CreateInstance<T>(sp)`, so **the check class itself
does not have to be registered** — only its constructor arguments have to resolve from the container. The
overload taking a `Func<IServiceProvider, IHealthCheck>` is for a check the container cannot build that way.
`failureStatus` and `tags` have no defaults, so every caller states them; only `timeout` is optional.

**Adding a check does not switch health checks on.** Nothing here registers `HealthCheckService`:
`AddHealthChecks()` is called once, by DiagnosticsModule's `ModuleDefinition`, which also maps the endpoint. A
registration added without that module sits in options with nothing to run it.

The `name` is the handle everything downstream uses — the endpoint's allow-list filters on it, not on the class
name. DiagnosticsModule registers `SystemHealthCheck`; each ServiceModule infrastructure provider (Azure Tables,
Sqlite, Npgsql) registers its own. The registered names, tags and the endpoint are in
`$api/modules/diagnostics`; the providers are in `$api/modules/service`.

## Serialization

`Kernel/Serialization/` holds two files — `JsonStringNullableEnumConverter.cs` and `JsonEnumValueException.cs`
— for a **nullable enum** written as a string. Namespace `Binacle.Net.Kernel.Serialization`. Nothing here is
registered in DI or added to `JsonSerializerOptions`; the converter is placed per property with
`[JsonConverter]`.

```csharp
[JsonConverter(typeof(JsonStringNullableEnumConverter))]
public required Algorithm? Algorithm { get; set; }
```

`JsonStringNullableEnumConverter.cs` holds three types, and the names do not say which is which:

| Type | What it is |
|---|---|
| `JsonStringNullableEnumConverter` | `public`, a `JsonConverterFactory`. `CanConvert` accepts only `Nullable<TEnum>`. This is the one contracts place. |
| `NullableEnumConverter<T>` | `internal`, and **what the factory actually creates**. Also handles the enum as a property name (`ReadAsPropertyName` / `WriteAsPropertyName`). |
| `EnumValueReader` | `internal static`, the shared read. `Enum.TryParse` case-sensitive first, then case-insensitive. |

**A JSON null and an empty string read as `default`, i.e. null. Any other string that matches no member
throws `JsonEnumValueException`** — a `JsonException` carrying the enum type. `BindingProblem` matches it
before the plain `JsonException` branch and answers `422`, keyed and worded like a missing value, so the field
reports one error either way. Reading an unknown value as null would accept the request with the field the
client actually sent silently dropped.

`EnumStringsSchemaTransformer` (`Kernel/OpenApi/Transformers`) matches on the converter type placed on the
property, and only for an enum or nullable-enum property. `JsonStringEnumConverter` (the BCL one) sets the
schema to `string`; `JsonStringNullableEnumConverter` sets `string` **and** lists the member names. Any other
converter leaves the schema alone. See `$api/openapi`.

Placed on v3 `PackRequestParameters` and `FitRequestParameters`, v4 `OperationParameters`, and the ServiceModule
admin account and subscription contracts.
