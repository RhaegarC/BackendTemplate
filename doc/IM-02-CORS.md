# IM-02 — CORS is broken under the launch profile

- **Priority:** 🔴 Blocking
- **Area:** CORS
- **Status:** Open
- **Index:** [improve.md](improve.md)

## Problem

`src/content/Tmp.Api/Properties/launchSettings.json:22` sets `AllowedOrigins` with a
**semicolon** separator, and it is an *environment variable* — so in the default
configuration order it overrides `appsettings.Development.json`, which uses a **comma**.
`AllowCORS` splits on comma, so the entire string becomes a single bogus origin and
every real origin is rejected:

```
origin: http://localhost:8080  ->  NO Access-Control-Allow-Origin header (rejected)
```

The ports have also drifted: `appsettings.Development.json` lists `7054/5015`,
`launchSettings.json` lists `8086/8080`.

> **How this was missed:** the earlier verification ran with `--no-launch-profile`, which
> skips `launchSettings.json` entirely and falls through to `appsettings`. That path
> works. The broken path is the default `dotnet run` / F5 experience.
> **Any CORS change must be verified with the launch profile active.**

## Direction

Choose one separator and use it everywhere. Comma is the natural fit for
`IConfiguration` lists and matches the existing `appsettings` entries; update
`launchSettings.json` and align the ports.

## Verification

```bash
# Run WITH the launch profile — do not use --no-launch-profile here
ASPNETCORE_ENVIRONMENT=Development dotnet run --project src/content/Tmp.Api

curl -i -X OPTIONS http://localhost:8080/User/index \
  -H "Origin: http://localhost:8080" -H "Access-Control-Request-Method: GET"
# must return Access-Control-Allow-Origin
```

## Related

- [IM-03](IM-03-Credentials.md) — credentials committed in the same file
