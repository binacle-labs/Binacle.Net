---
id: ci-cd/decisions/D9
description: the Postgres service in shared-image-tests.yml carries no password
status: pending
verified: 2026-09-29
paths:
  - ".github/workflows/shared-image-tests.yml"
---

# D9 — the Postgres service in `shared-image-tests.yml` carries no password

`POSTGRES_HOST_AUTH_METHOD: trust`, and no `POSTGRES_PASSWORD`.

**Why:** the container lives for one job, is reachable only from that job, and is thrown away after. There is
nothing for a password to protect. Under `trust` it accepts any password, so the integration tests connect
unchanged using the shared local-dev connection string.

**This removed the credential from that file, not from the repo.** The same local-dev password is still in
`tooling/serve.services.yml`, `tooling/image.full.yml`, the test default in `api/test/Binacle.Net.ServiceModule.IntegrationTests/BinacleApi.cs` and
`Config_Files/ServiceModule/ConnectionStrings.Development.json`, where it is load-bearing. It is the same value
everywhere on purpose. Change it in all of them or none.
