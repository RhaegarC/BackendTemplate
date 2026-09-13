# IM-12 — `AuditLog` key strategy is inconsistent with `EntityBase`

- **Priority:** 🟡 Low
- **Area:** Model
- **Status:** Open
- **Index:** [improve.md](improve.md)

## Problem

`src/content/Tmp.Model/DatabaseEntity/AuditLog.cs` uses `int Id` (database-generated)
while `src/content/Tmp.Model/DatabaseEntity/EntityBase.cs` uses `string? Id` with no
generation strategy. `AuditLog` also has no soft-delete or audit columns and is the only
non-sealed entity.

`EntityBase.Id` currently has nothing that assigns it, so inserts will fail on a null
primary key.

## Direction

Settle on one key strategy across the model.

## Related

- [IM-14](IM-14-EntityId.md) — the same root cause affects the audit `EntityId`
