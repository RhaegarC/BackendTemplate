# IM-10 — Audit snapshots serialize every column

- **Priority:** 🟠 Medium
- **Area:** Security
- **Status:** Done
- **Index:** [improve.md](../improve.md)

## Problem

`src/content/Tmp.Repository/AuditSaveChangesInterceptor.cs:102-119` serializes every
property of every changed entity. The commented-out `properties.Remove("PasswordHash")`
shows the intent, but there is no mechanism.

Audit tables usually have looser access control than the tables they describe, so this
is a data-leak vector for credentials, tokens, and PII.

## Direction

Add a `[NotAudited]` attribute and filter on it in `SerializeEntity`, with the sensitive
columns opted out explicitly.
