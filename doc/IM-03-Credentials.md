# IM-03 — Credentials committed to the repository

- **Priority:** 🔴 High
- **Area:** Security
- **Status:** Open
- **Index:** [improve.md](improve.md)

## Problem

`src/content/Tmp.Api/Properties/launchSettings.json:20-21` commits a `DbConnection`
containing `Uid=postgres;Pwd=postgres;` plus a `TenantId`. `launchSettings.json` is
version-controlled, so every clone of this template inherits them.

It is also present only in the `https` profile — the `http` profile silently has no
database configured.

## Direction

Move dev secrets into `dotnet user-secrets`; keep `launchSettings.json` to wiring
(`ASPNETCORE_ENVIRONMENT`) only. Remove the committed values from history if this
template has been shared.

## Related

- [IM-02](archive/IM-02-CORS.md) — separator mismatch in the same file
