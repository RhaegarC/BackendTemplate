# IM-13 — Soft delete is written but never enforced

- **Priority:** 🟡 Low
- **Area:** Data
- **Status:** Done
- **Index:** [improve.md](../improve.md)

## Problem

`DeleteAsync` in `src/content/Tmp.Repository/DatabaseRepository.cs` sets
`IsDeleted = true`, but `GetAsync` / `GetListAsync` apply no filter and there is no
global query filter, so reads still return deleted rows.

The interceptor also logs a soft delete as `Modified`, not `Deleted`.

## Direction

Add a global query filter in `TmpContext.OnModelCreating`
(`HasQueryFilter(e => e.IsDeleted != true)`), plus an explicit way to include deleted
rows where needed.

## Related

- [IM-09](IM-09-AuditLogIndexes.md) — same configuration site
