# Audit branch — design review and improvements

- **Branch:** `audit` (local, ahead of `origin/audit` by 3; head `fa87b9a`)
- **Reviewed:** 2026-09-13
- **Scope:** the whole `src/content` solution as it stands after merging
  `refactor/config-to-iconfiguration` (the `IConfiguration` refactor) into the
  audit-trail work.
- **Build state at review:** succeeds, 0 errors, **7 warnings** — all 7 are symptoms
  of [IM-01](IM-01-Audit.md), not noise.

Each finding is broken out into its own file, numbered in priority order.

## Findings

| # | Priority | Area | Issue | File |
| --- | --- | --- | --- | --- |
| 1 | 🔴 Blocking | Audit | Audit trail cannot persist a row; will fail on `NOT NULL` | [IM-01-Audit.md](IM-01-Audit.md) |
| 2 | 🔴 Blocking | CORS | `AllowedOrigins` separator mismatch; all origins rejected | [IM-02-CORS.md](IM-02-CORS.md) |
| 3 | 🔴 High | Security | DB credentials committed in `launchSettings.json` | [IM-03-Credentials.md](IM-03-Credentials.md) |
| 4 | 🔴 High | Build | `Dockerfile` references a non-existent project | [IM-04-Dockerfile.md](IM-04-Dockerfile.md) |
| 5 | 🟠 High | Auth | `AuthMiddleWare` is a pass-through with dead error handling | [IM-05-AuthMiddleware.md](IM-05-AuthMiddleware.md) |
| 6 | 🟠 High | Data | `CreateAsync(List<T>)` is fire-and-forget | [IM-06-BatchCreate.md](IM-06-BatchCreate.md) |
| 7 | 🟠 Medium | Layering | `Tmp.Repository` depends on `Tmp.Interface.Service` | [IM-07-Layering.md](IM-07-Layering.md) |
| 8 | 🟠 Medium | Design | Mutable ambient state on `IUserContextService` | [IM-08-UserContext.md](IM-08-UserContext.md) |
| 9 | 🟠 Medium | Data | `AuditLog` has no indexes; value columns are plain `text` | [IM-09-AuditLogIndexes.md](IM-09-AuditLogIndexes.md) |
| 10 | 🟠 Medium | Security | Audit snapshots serialize every column, including secrets | [IM-10-AuditRedaction.md](IM-10-AuditRedaction.md) |
| 11 | 🟡 Low | Correctness | Audit only hooks the async save path | [IM-11-AuditSyncPath.md](IM-11-AuditSyncPath.md) |
| 12 | 🟡 Low | Model | `AuditLog` key strategy inconsistent with `EntityBase` | [IM-12-KeyStrategy.md](IM-12-KeyStrategy.md) |
| 13 | 🟡 Low | Data | Soft delete written but never enforced | [IM-13-SoftDelete.md](IM-13-SoftDelete.md) |
| 14 | 🟡 Low | Data | `EntityId` null for DB-generated keys on insert | [IM-14-EntityId.md](IM-14-EntityId.md) |
| 15 | 🟡 Low | Cleanup | Unused `Constant.App.SwaggerUrl`; no tests or CI | [IM-15-Cleanup.md](IM-15-Cleanup.md) |

## Suggested order of work

1. **[IM-01](IM-01-Audit.md)** — make `UserContextService` real and the `AuditLog`
   fields honest. Until then the audit trail is decorative and database writes will fail.
2. **[IM-02](IM-02-CORS.md) and [IM-03](IM-03-Credentials.md)** — fix the CORS separator
   and ports, and move credentials out of `launchSettings.json`. Both are small, and both
   are template-hygiene issues that propagate to every project generated from it.
3. **[IM-05](IM-05-AuthMiddleware.md) and [IM-06](IM-06-BatchCreate.md)** — decide what
   authentication should be, and fix the fire-and-forget repository call.
4. **[IM-04](IM-04-Dockerfile.md), [IM-07](IM-07-Layering.md)–[IM-15](IM-15-Cleanup.md)**
   — build correctness and design cleanups.

## Verification checklist

Any change here should be checked with the following, which is what caught
[IM-02](IM-02-CORS.md):

```bash
# Build
dotnet build src/content/Tmp.slnx

# Run WITH the launch profile (the default path — do not use --no-launch-profile here)
ASPNETCORE_ENVIRONMENT=Development dotnet run --project src/content/Tmp.Api

# Health (audit renamed this path from /api/health)
curl -i http://localhost:8080/health

# CORS preflight from a configured origin — must return Access-Control-Allow-Origin
curl -i -X OPTIONS http://localhost:8080/User/index \
  -H "Origin: http://localhost:8080" -H "Access-Control-Request-Method: GET"

# DI chain resolves end to end (IUserService -> IUserRepository -> TmpContext)
curl -i http://localhost:8080/User/index

# Graph checks (required before committing in this repo)
node .gitnexus/run.cjs analyze --index-only
node .gitnexus/run.cjs detect-changes --scope all --repo .
```
