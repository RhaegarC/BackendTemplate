# IM-09 — `AuditLog` has no indexes and no queryable value columns

- **Priority:** 🟠 Medium
- **Area:** Data
- **Status:** Done
- **Index:** [improve.md](../improve.md)

## Problem

`TmpContext` now has no `OnModelCreating` at all, so `AuditLog` gets default mapping
only: no indexes, and `OldValues` / `NewValues` as plain `text`.

An audit table is typically the fastest-growing table in a system. It needs at minimum
indexes on `(TableName, EntityId)` and on `Timestamp`, and — with Npgsql — `jsonb` for
the value columns if they are ever to be queried.

## Direction

Reintroduce `OnModelCreating` in `TmpContext` for this configuration.

## Related

- [IM-13](../IM-13-SoftDelete.md) — also configured there
