# Audit branch — design review and improvements

- **Branch:** `audit` (local, ahead of `origin/audit` by 3; head `fa87b9a`)
- **Reviewed:** 2026-09-13
- **Scope:** the whole `src/content` solution as it stands after merging
  `refactor/config-to-iconfiguration` (the `IConfiguration` refactor) into the
  audit-trail work.
- **Build state at review:** succeeds, 0 errors, **7 warnings** — all 7 are symptoms
  of [IM-01](archive/IM-01-Audit.md), not noise.

Each finding is broken out into its own file, numbered in priority order.

## Findings

| # | Priority | Area | Issue | File | Status |
| --- | --- | --- | --- | --- | --- |
| 1 | 🔴 Blocking | Audit | Audit trail cannot persist a row; will fail on `NOT NULL` | [IM-01-Audit.md](archive/IM-01-Audit.md) | Done |
| 2 | 🔴 Blocking | CORS | `AllowedOrigins` separator mismatch; all origins rejected | [IM-02-CORS.md](archive/IM-02-CORS.md) | Done |
| 3 | 🔴 High | Security | DB credentials committed in `launchSettings.json` | [IM-03-Credentials.md](archive/IM-03-Credentials.md) | Done |
| 4 | 🔴 High | Build | `Dockerfile` references a non-existent project | [IM-04-Dockerfile.md](archive/IM-04-Dockerfile.md) | Done |
| 5 | 🟠 High | Auth | `AuthMiddleWare` is a pass-through with dead error handling | [IM-05-AuthMiddleware.md](archive/IM-05-AuthMiddleware.md) | Done |
| 6 | 🟠 High | Data | `CreateAsync(List<T>)` is fire-and-forget | [IM-06-BatchCreate.md](archive/IM-06-BatchCreate.md) | Done |
| 7 | 🟠 Medium | Layering | `Tmp.Repository` depends on `Tmp.Interface.Service` | [IM-07-Layering.md](archive/IM-07-Layering.md) | Done |
| 8 | 🟠 Medium | Design | Mutable ambient state on `IUserContextService` | [IM-08-UserContext.md](archive/IM-08-UserContext.md) | Done |
| 9 | 🟠 Medium | Data | `AuditLog` has no indexes; value columns are plain `text` | [IM-09-AuditLogIndexes.md](archive/IM-09-AuditLogIndexes.md) | Done |
| 10 | 🟠 Medium | Security | Audit snapshots serialize every column, including secrets | [IM-10-AuditRedaction.md](archive/IM-10-AuditRedaction.md) | Done |
| 11 | 🟡 Low | Correctness | Audit only hooks the async save path | [IM-11-AuditSyncPath.md](archive/IM-11-AuditSyncPath.md) | Done |
| 12 | 🟡 Low | Model | `AuditLog` key strategy inconsistent with `EntityBase` | [IM-12-KeyStrategy.md](archive/IM-12-KeyStrategy.md) | Done |
| 13 | 🟡 Low | Data | Soft delete written but never enforced | [IM-13-SoftDelete.md](archive/IM-13-SoftDelete.md) | Done |
| 14 | 🟡 Low | Data | `EntityId` null for DB-generated keys on insert | [IM-14-EntityId.md](archive/IM-14-EntityId.md) | Done |
| 15 | 🟡 Low | Cleanup | Unused `Constant.App.SwaggerUrl`; no tests or CI | [IM-15-Cleanup.md](archive/IM-15-Cleanup.md) | Done |

**Status values:** `Open` → `In Progress` → `Done`. Set a finding to `Done` only once its
fix has merged into `develop` — not when it is merely written. Each finding's own file
carries a matching `**Status:**` line in its header; update that line and this table
together, or the index and the detail will disagree about what is finished. A `Done`
finding's file moves to [`archive/`](archive/), per [git-workflow.md](git-workflow.md).

## Suggested order of work

1. **[IM-01](archive/IM-01-Audit.md)** — make `UserContextService` real and the `AuditLog`
   fields honest. Until then the audit trail is decorative and database writes will fail.
2. **[IM-02](archive/IM-02-CORS.md) and [IM-03](archive/IM-03-Credentials.md)** — fix the CORS separator
   and ports, and move credentials out of `launchSettings.json`. Both are small, and both
   are template-hygiene issues that propagate to every project generated from it.
3. **[IM-05](archive/IM-05-AuthMiddleware.md) and [IM-06](archive/IM-06-BatchCreate.md)** — decide what
   authentication should be, and fix the fire-and-forget repository call.
4. **[IM-04](archive/IM-04-Dockerfile.md), [IM-07](archive/IM-07-Layering.md)–[IM-15](archive/IM-15-Cleanup.md)**
   — build correctness and design cleanups.

## Verification checklist

Any change here should be checked with the following. The CORS check covers **both** launch
profiles — a single-profile version is what let [IM-02](archive/IM-02-CORS.md) through:

```bash
# Build
dotnet build src/content/Tmp.slnx

# Run WITH the launch profile (the default path — do not use --no-launch-profile here).
# `dotnet run` picks the first profile, "http".
ASPNETCORE_ENVIRONMENT=Development dotnet run --project src/content/Tmp.Api

# Health (audit renamed this path from /api/health)
curl -i http://localhost:8080/health

# CORS preflight, default "http" profile — must return Access-Control-Allow-Origin.
curl -i -X OPTIONS http://localhost:8080/User/index \
  -H "Origin: http://localhost:8080" -H "Access-Control-Request-Method: GET"

# CORS preflight, "https" profile. This is a DIFFERENT configuration path: it takes
# AllowedOrigins from launchSettings.json (an env var) rather than appsettings, which is
# exactly what IM-02 broke. Checking only the http profile cannot detect that.
# Target 8086, not 8080: UseHttpsRedirection runs before UseCors, so an http request is
# answered with a 307 carrying no CORS headers and looks like a failure either way.
ASPNETCORE_ENVIRONMENT=Development dotnet run --project src/content/Tmp.Api --launch-profile https
curl -k -i -X OPTIONS https://localhost:8086/User/index \
  -H "Origin: https://localhost:8086" -H "Access-Control-Request-Method: GET"
# and an unlisted origin must NOT receive the header
curl -k -i -X OPTIONS https://localhost:8086/User/index \
  -H "Origin: https://evil.example" -H "Access-Control-Request-Method: GET"

# DI chain resolves end to end (IUserService -> IUserRepository -> TmpContext)
curl -i http://localhost:8080/User/index

# Graph checks (required before committing in this repo)
node .gitnexus/run.cjs analyze --index-only
node .gitnexus/run.cjs detect-changes --scope all --repo .
```
