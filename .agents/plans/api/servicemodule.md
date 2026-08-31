---
description: How far ServiceModule is taken - answered. One store, one project, refresh tokens
state: proposed
waits-on: "the v3.0.0 tag - it goes with the image split"
paths:
  - "api/src/Binacle.Net.ServiceModule/**"
  - "api/src/Binacle.Net.ServiceModule.Domain/**"
  - "api/src/Binacle.Net.ServiceModule.Infrastructure/**"
---

# ServiceModule - answered 2026-08-31

**The module is for the maintainer's own instance.** That answers the question this file used to ask and it
collapses the scope: one deployment, one database, no public documentation, no migration story owed to anyone.

**Drop Azure Table Storage.** It is the one backend that cannot serve an admin screen - no secondary indexes,
so it cannot sort by anything but `RowKey`, cannot skip, and cannot count without reading every row. The line
is secondary indexes, not relational versus document, so keeping the document door open costs nothing and
dropping this store does not close it.

**Collapse three projects into one.** Full DDD layering for two entities, `Account` and `Subscription`. Fold
`Domain` and `Infrastructure` in as folders; namespaces carry the dependency direction that three assemblies
were carrying. **Keep the provider seam** - the repository interfaces with per-provider implementations chosen
by connection string. That is the DB-swap mechanism and it pays for itself. Flatten
`Entity -> AggregateRoot -> AuditableEntity` to plain records wherever the base carries no behaviour, keeping
password hashing. Do not touch the packing core.

**Migrations: homegrown, not EF Core and not DbUp.** One store means one dialect, so an ordered runner with a
version marker, extending the existing startup task. It has to run in-app at startup - on an app-only-volume
host there is no shell to run migrations by hand. Today each backend creates its schema with
`CREATE TABLE IF NOT EXISTS` and nothing else, which works exactly once: the first column added to a table
that already has rows never appears, silently.

**Refresh tokens.** The Alpine client calls with no token, so with the module on every call is anonymous and
the site takes 429s. `/api/auth/refresh`, a refresh token store mirroring the account one, rotation, revoke on
logout. Cheaper during the collapse than before it.

**Fix while the data is small.** `Username` carries no index and no unique constraint, so every login is a
full table scan and two concurrent creates both pass the check-then-write. It has to be a **partial** unique
index - deletion is soft and a username is reusable - and `CREATE INDEX IF NOT EXISTS` is idempotent, so this
is the one item that works without the migration runner. Also open: the account/subscription link is stored on
both sides with no transaction, and one-subscription-per-account is enforced in code only.

**Two `// TODO`s ride with this:** `ApiUsageRateLimitingPolicy.cs:32` and `AccountBindingResult.cs:57`.

**Goes with the image split** - `plans/api/packing-only-image.md`.
