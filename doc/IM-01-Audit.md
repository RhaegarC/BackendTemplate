# IM-01 — The audit feature cannot persist anything yet

- **Priority:** 🔴 Blocking
- **Area:** Audit
- **Status:** Open
- **Index:** [improve.md](improve.md)

## Problem

The scaffold is well-shaped, but it is non-functional end to end — and the compiler is
already reporting it. The 7 build warnings are all this bug:

- `src/content/Tmp.Api/UserContextService.cs:41-45` — `GetCurrentUserInfo()` is a TODO
  that returns `string.Empty` and never assigns `EntraObjectId`, `IpAddress`,
  `UserAgent`, or `CorrelationId`. `_currentUser.EntraObjectId` is therefore **always null**.
- `src/content/Tmp.Model/DatabaseEntity/AuditLog.cs:10,20,25,30` — `Actor`, `TableName`,
  `EntityId`, `Action` are non-nullable `string` with no `required` or initializer
  → 4 × **CS8618**.
- `src/content/Tmp.Repository/AuditSaveChangesInterceptor.cs:70,71,76` — assigns
  possibly-null values into those properties → 3 × **CS8601**.

Those three CS8601s say exactly one thing: an audit row will carry `null` into a
`NOT NULL` column. **As soon as a real database is attached, every audited
`SaveChanges` fails with a NOT NULL violation** — the audit interceptor blocks all
writes rather than recording them.

## Direction

1. Implement `UserContextService` against `HttpContext.User.Claims` (Entra object id,
   display name), plus client IP and user agent from `HttpContext.Request`.
2. Give `Actor` a non-null fallback (`"system"` for background work, `"anonymous"` for
   unauthenticated requests) so the column is always satisfiable.
3. Make the required fields honest — either `required` on `AuditLog` or nullable where a
   value may genuinely be absent. Do not leave the compiler warning in place; it is the
   only thing currently flagging this.

## Related

- [IM-08](IM-08-UserContext.md) — mutable ambient state on `IUserContextService`
- [IM-11](IM-11-AuditSyncPath.md) — audit only covers the async save path
