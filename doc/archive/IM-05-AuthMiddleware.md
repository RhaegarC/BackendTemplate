# IM-05 — `AuthMiddleWare` is a pass-through

- **Priority:** 🟠 High
- **Area:** Auth
- **Status:** Done
- **Index:** [improve.md](../improve.md)

## Problem

`src/content/Tmp.Api/MiddleWare/AuthMiddleWare.cs:10-23` — the `try` body is a TODO, so:

- the `catch` block is **unreachable dead code**;
- `await next(context)` runs once, *outside* the `try`, so nothing is guarded;
- the net effect is no authentication anywhere in the pipeline.

Combined with the stub `UserContextService`, the current posture is "every request is
anonymous and no actor is recorded."

## Direction

Either implement authentication with the framework
(`AddAuthentication` / `UseAuthentication` / `UseAuthorization`) and delete this
middleware, or remove it entirely.

Error handling belongs in `UseExceptionHandler` / `IExceptionHandler`, not a catch-all
that converts every failure into 401.

## Related

- [IM-01](IM-01-Audit.md) — the stub `UserContextService`
