# IM-08 — Mutable ambient state on `IUserContextService`

- **Priority:** 🟠 Medium
- **Area:** Design
- **Status:** Done
- **Index:** [improve.md](../improve.md)

## Problem

The interface exposes settable properties (`ActorName`, `IpAddress`, `UserAgent`,
`CorrelationId`) and `src/content/Tmp.Api/UserContextService.cs:9-13` does work in the
constructor, discarding the result of `GetCurrentUserInfo()`.

## Direction

Make the members read-only, populate them once, and compute them from `HttpContext`
rather than in a constructor side effect. `EntraObjectId` is already get-only but never
assigned — the same fix covers it.

## Related

- [IM-01](IM-01-Audit.md) — the audit trail depends on this
- [IM-07](IM-07-Layering.md) — where the interface should live
