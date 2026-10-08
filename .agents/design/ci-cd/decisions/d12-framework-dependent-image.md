---
id: ci-cd/decisions/D12
description: the image is framework-dependent, and the publish flag is spelled out
status: pending
verified: 2026-09-29
check: D12 against build.just's publish recipe
paths:
  - "tooling/build.just"
  - "Dockerfile"
---

# D12 — the image is framework-dependent, and the publish flag is spelled out

`--no-self-contained --runtime linux-x64`, written explicitly rather than left to the default of a bare
`--runtime`.

**Why:** the runtime comes from the `aspnet` base image, which is the whole point, and that has to be readable
on the line. The image was self-contained until 2026-08-10 while basing on `aspnet:10.0`, so it carried two
copies of .NET — the bundled one the app ran on, and the base image's, which nothing loaded.

**Measured before the change was kept:** image 150.2 MB to 103.2 MB, publish output 123 MB to 18 MB,
`System.*.dll` count 172 to 4. All structure assertions, all five smoke profiles and every test green on
the rebuilt image. The entrypoint did not change — `dotnet Binacle.Net.dll` was always the framework-dependent
idiom, which is what made the old pairing wrong in the first place.

The second reason is durability: framework-dependent means a .NET security fix reaches users by rebasing on a
newer `aspnet` tag rather than by republishing the app, which matters for a project that ships months apart.

**One home, as of 2026-09-04.** The flags are in the `publish` recipe in `tooling/build.just` and nowhere else; the workflow calls
`just build publish`. This entry used to warn that they were written twice and had to be kept in step - that
warning describes a shape the repository no longer has, and following it would mean editing a line that does
not exist.
