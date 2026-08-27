# Writes and lints the OpenAPI documents.
#
#   just openapi generate          write artifacts/openapi/Binacle.Net_v3.json and _v4.json
#   just openapi generate <dir>    write them somewhere else
#   just openapi lint [<dir>]      generate, then lint them against tooling/openapi.spectral.yaml
#
# The documents come out of the build and not out of a running server, so nothing has to be brought up. The
# host it starts has no launch profile, so ServiceModule is off - which is the shape the committed specs
# assume, with no /api/auth/token path. The two generation settings are in
# api/src/Binacle.Net/Binacle.Net.csproj.

# Recipes in a module run from the folder the module is in. Every path below is written from the repo root.
set working-directory := '..'

# dotnet and spectral both print why they failed, so just's own "Recipe failed" line adds nothing. The exit
# code still gets through, so CI still fails.
set no-exit-message := true

# Absolute, because MSBuild resolves a relative OpenApiDir against the project folder - a plain
# `artifacts/openapi` would land in api/src/Binacle.Net/. justfile_directory() is the root justfile's folder,
# so it is the repo root.
openapi_dir := justfile_directory() / "artifacts" / "openapi"

# Named because the ruleset no longer sits at the repo root, where spectral finds one by itself. Absolute for
# the same reason as openapi_dir, since a caller can pass any dir.
ruleset := justfile_directory() / "tooling" / "openapi.spectral.yaml"

# List the recipes
default:
    @just --list openapi

# Write the v3 and v4 documents [dir]
generate dir=openapi_dir:
    dotnet build api/src/Binacle.Net -p:GenerateOpenApi=true -p:OpenApiDir='{{ dir }}'

# --fail-severity=warn because spectral's default exits 0 on a warning. Without it the step prints
# "No results with a severity of 'error' found!" and passes whatever the warnings say.

# Generate the documents, then lint them against tooling/openapi.spectral.yaml [dir]
lint dir=openapi_dir: (generate dir)
    npx --yes @stoplight/spectral-cli lint --ruleset '{{ ruleset }}' --fail-severity=warn '{{ dir }}'/*
