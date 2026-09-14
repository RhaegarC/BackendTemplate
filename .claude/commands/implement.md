---
description: Implement one task from a markdown file, following the repo's branch workflow
argument-hint: <path/to/task.md>
---

# /implement — run one task file through the branch workflow

`$ARGUMENTS` is the path to a markdown file describing exactly one task — for example
`doc/IM-02-CORS.md`. If `$ARGUMENTS` is empty, or does not resolve to an existing file,
**stop and ask which task file to use.** Do not guess, and do not pick one off the findings
list yourself.

The full rules live in [doc/git-workflow.md](../../doc/git-workflow.md); this command is
that lifecycle applied to one task file.

## 1. Branch from `develop`

```bash
git checkout develop
git pull --ff-only
git checkout -b <type>/<short-description>
```

Never work on `develop` or `master` directly. Name the branch after the task, not the file:
`fix/cors-separator`, `feat/user-search`, `docs/improve-status-column`. `<type>` is one of
`feat`, `fix`, `docs`, `refactor`, `chore`.

## 2. Make the change

Read the task file and implement what it describes. For a `doc/IM-*.md` finding, its own
`## Direction` section is the specification.

Repo rules that apply before you edit any code:

- Run `impact` on every function, class, or method you touch, and report callers and risk.
- **Treat `risk: UNKNOWN` as unresolved, not as low** — an empty caller set can mean the
  callers are not resolvable, not that there are none. Confirm by text search first.
- Never rename symbols with find-and-replace.

## 3. Commit locally, then stop

Run the graph change analysis the repo requires, then commit:

```bash
node .gitnexus/run.cjs detect-changes --scope all --repo .
git add <paths>
git commit -m "<message>"
```

`partial: true` or `truncated: true` is not a clean check — re-run it.

Then **stop and wait for approval.** Do not push. Do not open a PR. The local commit is the
reviewable unit, and pushing before approval turns the review into a moving target.

## 4. After approval — archive the task file and mark it done

Only once the user has approved:

**4a. Move the file** into an `archive/` folder beside its original location. Use `git mv` so
history follows it:

```bash
mkdir -p doc/archive
git mv doc/IM-02-CORS.md doc/archive/IM-02-CORS.md
```

**4b. Repair the relative links the move breaks.** A move changes every relative path into
and out of the file, so fix them in the same commit:

- **Parent list** — the row's link gains a segment:
  `[IM-02-CORS.md](IM-02-CORS.md)` → `[IM-02-CORS.md](archive/IM-02-CORS.md)`
- **Archived file** — links back up to `doc/` gain a level:
  `[improve.md](improve.md)` → `[improve.md](../improve.md)`; links to siblings still in
  `doc/` become `../<sibling>.md`
- **Every other file** that links to the moved file needs the `archive/` segment too.

Find them before you claim it is done:

```bash
grep -rn "IM-02-CORS" doc/ .claude/ --include="*.md"
```

**4c. Mark it done in the parent list.** Take the parent list from the task file's own
`**Index:**` header line (for the `IM-*.md` findings that is `doc/improve.md`). Then set
`Status` to `Done` in *both* places: the parent list's findings table, and the archived
file's own `**Status:**` line. Doing only one leaves the index and the detail disagreeing
about what is finished.

**4d. Commit the bookkeeping** as its own commit, separate from the implementation:

```bash
git add -A
git commit -m "Archive IM-02 and mark it Done"
```

## 5. Push and open the PR

```bash
git push -u origin <type>/<short-description>
gh pr create --base develop --head <type>/<short-description>
```

Target `develop`. Never `master`.

## 6. After the PR is merged, sync `develop`

```bash
git checkout develop
git pull --ff-only
```

## 7. Delete the local branch

```bash
git branch -d <type>/<short-description>
```

`-d` refuses to delete a branch carrying unmerged commits, and that refusal is the point.
Reach for `-D` only when you deliberately mean to discard work.

---

## Resuming mid-cycle

If the branch already exists with the work committed — the usual case when approval arrives
in a later turn — **do not redo steps 1–3.** Check `git branch --show-current` and
`git log` first, then continue at step 4.

If the PR is already open and merged, continue at step 6.

## Applies to which task files

Any task markdown file. The archive and parent-list steps assume the file has an `**Index:**`
header pointing at a parent list with a `Status` column; if it does not, say so and ask
where the status should be recorded rather than inventing a location.
