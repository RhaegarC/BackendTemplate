# IM-07 — `Tmp.Repository` depends on `Tmp.Interface.Service`

- **Priority:** 🟠 Medium
- **Area:** Layering
- **Status:** Open
- **Index:** [improve.md](improve.md)

## Problem

`src/content/Tmp.Repository/AuditSaveChangesInterceptor.cs:8` imports
`Tmp.Interface.Service` to get `IUserContextService`. An ambient request-context
abstraction is infrastructure — it belongs *below* the repository, not in the service
layer.

## Direction

Move `IUserContextService` to something like `Tmp.Interface.Infrastructure` (or
`Context`) so the dependency points the right way.

## Related

- [IM-08](IM-08-UserContext.md) — the abstraction itself
