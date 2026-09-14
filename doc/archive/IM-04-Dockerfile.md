# IM-04 — The Dockerfile cannot build

- **Priority:** 🔴 High
- **Area:** Build
- **Status:** Done
- **Index:** [improve.md](../improve.md)

## Problem

`src/content/Dockerfile` still references `Targaryen.Api/Targaryen.Api.csproj` (the
project is `Tmp.Api`) and uses `aspnet:8.0` / `sdk:8.0` base images against a `net10.0`
target. Left over from a rename and never updated.

## Direction

Update the project paths and base-image tags, and add a CI step that builds the image so
it cannot rot again.
