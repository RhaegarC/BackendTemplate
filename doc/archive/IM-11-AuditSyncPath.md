# IM-11 — Audit only covers the asynchronous save path

- **Priority:** 🟡 Low
- **Area:** Correctness
- **Status:** Done
- **Index:** [improve.md](../improve.md)

## Problem

Only `SavingChangesAsync` is overridden in
`src/content/Tmp.Repository/AuditSaveChangesInterceptor.cs`. Any synchronous
`SaveChanges()` bypasses auditing silently.

`DatabaseRepository` is async-only today, so this is latent — but it is a trap for the
next contributor.

## Direction

Override the synchronous `SavingChanges` as well, or centralise auditing so both paths
go through one implementation.

## Related

- [IM-01](IM-01-Audit.md)
