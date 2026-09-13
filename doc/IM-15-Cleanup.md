# IM-15 — Cleanup

- **Priority:** 🟡 Low
- **Area:** Cleanup
- **Status:** Open
- **Index:** [improve.md](improve.md)

## Problem

- `Constant.App.SwaggerUrl` is unused, and no Swagger UI is mapped.
- No test project and no CI. The audit interceptor, the CORS guard, and the
  configuration precedence are all currently unverified by anything automatic.

## Direction

- Remove the unused constant, or map a Swagger UI if it is intended.
- Add a test project and a CI workflow; the verification checklist in
  [improve.md](improve.md) is a reasonable starting point for both.
