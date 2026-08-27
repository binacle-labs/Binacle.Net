---
description: How far ServiceModule is taken - one decision, and the three pieces of work behind it - collapsing the layering, a schema-migration path, and refresh tokens
state: deferred
waits-on: "the maintainer. He said on 2026-08-27 that ServiceModule work is taken as one piece, not row by row"
paths:
  - "api/src/Binacle.Net.ServiceModule/**"
  - "api/src/Binacle.Net.ServiceModule.Domain/**"
  - "api/src/Binacle.Net.ServiceModule.Infrastructure/**"
---

# ServiceModule - one decision, three pieces

**How far the module is taken is one question, not three.** Collapsing its layering, giving its store a
migration path and growing its auth all move together, and the answer is the maintainer's. **Ask before
widening any of this into a redesign.**

**Nothing here has an external forcing function.** Nothing breaks if none of it happens, which is why it has
never been urgent and why it keeps being the thing everything else in the module waits on.

## What it is today

Three projects - `ServiceModule` / `ServiceModule.Domain` / `ServiceModule.Infrastructure` - with full DDD
layering, aggregate roots, value objects and an auditable-entity hierarchy, for a domain that is **two
entities**: `Account` and `Subscription`.

**Two different things hide under "clean architecture" here, and they deserve opposite verdicts.**

- **The provider seam** - `IAccountRepository` / `ISubscriptionRepository` in Domain, with SQLite, Postgres
  and Azure implementations in Infrastructure, chosen by connection string. **Keep it.** It is the DB-swap
  mechanism and it pays for itself.
- **The DDD ceremony** - three `.csproj`, `Entity -> AggregateRoot -> AuditableEntity`, `ValueObject` bases,
  an `IPasswordService` abstraction, for two CRUD entities. **That is the overkill.**

## Piece 1 - collapse the ceremony, keep the seam

- **Three projects to one.** Fold `Domain` and `Infrastructure` in as folders. The dependency direction was
  the only reason for three assemblies, and namespaces carry that.
- **Keep the repository interfaces and the per-provider implementations**, as folders. Only the project
  boundary goes.
- **Flatten the entity and value-object hierarchy** to plain records wherever the base class carries no real
  behaviour. Keep what does, such as password hashing.
- **Do not touch the packing core** (`Binacle.Lib`, `Binacle.Geometry`, `Binacle.Net.Kernel`). That is a
  reusable library, not ceremony.

**Open:** how much of the `Entity`/`AggregateRoot`/`AuditableEntity` chain carries behaviour rather than
pattern, and whether merging the projects breaks a test reference or a DI registration that assumed three
assemblies.

## Piece 2 - a schema-migration path

Each backend creates its schema idempotently at startup - `CREATE TABLE IF NOT EXISTS`, or the Azure-table
equivalent - **and nothing else.** That works exactly once, on a fresh database. The first time a column is
added on a store that already has rows, the create step sees the table, skips, and the column never appears.
Silently.

**A small ordered versioned runner, not EF Core.** EF is heavy for a two-entity domain and does nothing for
the schemaless Azure backend. Two right-sized shapes: **DbUp** (ordered `.sql` scripts plus an applied-version
table; one small dependency, relational only, one script folder per dialect), or **homegrown** (extend the
existing startup task into an ordered runner with a version marker; no new dependency, DbUp-lite).

**It has to run in-app at startup.** On an app-only-volume host there is no shell to run migrations by hand.

**The store choice narrows it.** One relational store means ordered SQL for one dialect. Two means two script
folders. Azure Tables is schemaless, so DDL migrations barely apply. Decide the store first and the shape
falls out.

**Open:** DbUp or homegrown, where the applied-version marker lives per backend, and whether this replaces
the `EnsureRequired*TablesExist` startup tasks or layers on top of them.

## Piece 3 - refresh tokens

The Alpine client calls the packing API with no token, so with ServiceModule on every call is anonymous and
bound by `ApiUsageAnonymous` - the site takes 429s under normal use. Authenticating fixes that, but an access
token expires (`JwtAuth.ExpirationInSeconds`, 3600) and **there is no way to renew it without re-sending
credentials.**

- **`/api/auth/refresh`** - takes a refresh token, returns a fresh access token, no credentials.
- **`/api/auth/token`** - extend it to return a refresh token as well.
- **A refresh-token store** - `IRefreshTokenRepository` in Domain with the three implementations, mirroring
  `IAccountRepository`. Row: token hash, `accountId`, `expiresAt`, `revoked`. Server-side, unlike the
  stateless access JWT, so tokens can be revoked.
- **Rotation** - each refresh issues a new token and invalidates the old one. A reused old token means theft,
  so revoke the chain.
- **Revoke on logout**, and optionally revoke-all-for-an-account.

Then the client authenticates once, holds the access token, and on a `401` calls refresh and retries once.
Refresh token ideally in an httpOnly cookie so XSS cannot read it; access token in memory. The UIModule is
out of scope.

**This is why piece 3 is cheaper during piece 1 than before it:** a new repository drops cleanly into
whatever shape the rework settles on.

**Open:** httpOnly cookie (needs CSRF handling) or localStorage; concrete access and refresh lifetimes; how
aggressively to revoke on reuse detection.

## Four storage facts worth fixing while the data is small

**The shape is portable and no migration is needed to add a provider.** The entities are flat scalars with no
nesting and no collections, ids are version 7 Guids so they are portable and already in creation order, enums
are stored as strings, deletion is soft and modelled in the data, and the password is one `type::hash::salt`
string so a hasher change can be a lazy per-row upgrade.

Four things get harder as real accounts accumulate:

- **No index on `Username`, and no unique constraint.** The primary key is the only index on any of the four
  tables. Every login is a full table scan on both SQL backends, and two concurrent creates of the same
  username both pass the check-then-write. **Adding a unique index later fails if duplicates exist by then.**
  It has to be a *partial* index - unique on `Username` where the row is not deleted - because deletion is
  soft and a username is reusable today. SQLite and Postgres both do partial indexes; Azure Tables has no
  indexes to add. **Indexes are the one thing that works without piece 2**: `CREATE INDEX IF NOT EXISTS` is
  idempotent on an existing table.
- **The account/subscription link is stored on both sides** - `Account.SubscriptionId` and
  `Subscription.AccountId` - kept in step by hand across two writes with no transaction. A crash between them
  leaves an orphan either way. Choosing one side means a reconciliation pass over whatever data exists.
- **One subscription per account is enforced in code only.** Nothing in any schema stops a second. If
  subscriptions ever become a history rather than one current row, `Account.SubscriptionId` is the wrong
  column and that is a real data migration.
- **There is no mechanism to apply a schema change at all** - piece 2. It is what turns any of the three
  above from a small fix into a blocked one.

## Azure Table Storage is the weak provider, and it may go

**It is the one backend that cannot serve an admin screen.** No secondary indexes, so it cannot sort by any
field but `RowKey`, cannot skip, and cannot count without reading every row. Filters are the one thing it
pushes down; everything else happens in memory in the provider.

**That is not what "NoSQL" costs - it is what this store costs.** A document store with a query engine
sorts, pages and counts natively. The line is secondary indexes, not relational versus document. So keeping
the document door open costs nothing, and **dropping Table Storage does not close it.** It also has no
dedicated sample and no smoke profile of its own, though it does run on the PR gate against Azurite.

If it stays, its admin list paths read every matching row on every request. That is bounded by the account
count and never touches the auth path, where get-by-id and get-by-username are point lookups - the one thing
the store is genuinely good at.

## Two `// TODO` comments in the code ride with this

- `api/src/Binacle.Net.ServiceModule/Services/ApiUsageRateLimitingPolicy.cs:32` - **"Review json config for
  default policies"**, at the top of `GetPartition`, which reads `ApiUsageAnonymous` and the tier
  configurations out of `RateLimiterConfiguration`.
- `api/src/Binacle.Net.ServiceModule/v0/Endpoints/AccountBindingResult.cs:57` - **"Make Response"**, on the
  `this.request is null` path, which returns a bare `ProblemDetails` ("Malformed Request", 400) where every
  other path in the file returns a typed result.

## Done when

- [ ] How far ServiceModule is taken is answered, in one place, by the maintainer.
      **By eye.** If the answer is only in someone's head, every box below is unopenable.
- [ ] The module is one project, or a line here says why three stayed.
      `ls -d api/src/Binacle.Net.ServiceModule*` lists one directory, or this plan says why not.
- [ ] The provider seam survived the collapse.
      `grep -rn "IAccountRepository\|ISubscriptionRepository" api/src/Binacle.Net.ServiceModule*` still finds
      an interface and three implementations.
- [ ] A column added to an already-created table reaches a database that already has rows.
      **By eye.** Start against an existing SQLite file after adding a column, and the column is there.
- [ ] `Username` carries a partial unique index on both SQL backends.
      `grep -rin "unique" api/src/Binacle.Net.ServiceModule*` finds it for SQLite and for Postgres.
- [ ] A client can stay authenticated without re-sending credentials.
      `grep -rn "auth/refresh" api/src` finds the route, and the token endpoint returns both tokens.
