# IM-06 — `CreateAsync<T>(List<T>)` is fire-and-forget

- **Priority:** 🟠 High
- **Area:** Data
- **Status:** Done
- **Index:** [improve.md](../improve.md)

## Problem

`src/content/Tmp.Repository/DatabaseRepository.cs:36-41`:

```csharp
items.ForEach(async item => await _context.AddAsync(item ?? throw new Exception()));
int count = await _context.SaveChangesAsync();
```

`List<T>.ForEach` takes `Action<T>`, so the lambda is `async void` — nothing awaits it,
and `SaveChangesAsync()` can run before the adds complete, silently persisting a partial
batch.

## Direction

Use a real `foreach`. Also `throw new Exception()` with no message; prefer
`ArgumentNullException.ThrowIfNull`, matching the existing style in the single-item
overload.
