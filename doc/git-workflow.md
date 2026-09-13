# Git workflow

One cycle per implementation or bug fix, seven steps, in order. No step is optional.

Run the whole cycle against a single task file with
[`/implement`](../.claude/commands/implement.md): `/implement doc/IM-02-CORS.md`.

## The rules

| # | Rule |
| --- | --- |
| 1 | **Check out a new branch from `develop`.** |
| 2 | **Make the changes on that branch.** |
| 3 | **Commit locally and wait for review and approval** — do not push yet. |
| 4 | **Once approved, archive the task file into an `archive/` folder beside its original location, and mark it `Done` in its parent list.** |
| 5 | **Push the branch to the remote and open a PR into `develop`.** |
| 6 | **After the PR is merged, sync local `develop` with the remote.** |
| 7 | **Delete the local branch created in step 1.** |

## Commands

```bash
# 1. Branch from develop — never work on develop or master directly
git checkout develop
git pull --ff-only
git checkout -b <type>/<short-description>

# 2. ... make the changes ...

# 3. Commit locally, then stop for review
git add <paths>
git commit -m "<message>"

# 4. Once approved: archive the task file and mark it done
mkdir -p doc/archive
git mv doc/IM-02-CORS.md doc/archive/IM-02-CORS.md
# ... repair the relative links this breaks, and set Status: Done in both the
# ... parent list and the archived file's own header ...
git add -A
git commit -m "Archive IM-02 and mark it Done"

# 5. Push and open the PR
git push -u origin <type>/<short-description>
gh pr create --base develop --head <type>/<short-description>

# 6. After the PR is merged, sync develop
git checkout develop
git pull --ff-only

# 7. Delete the local branch
git branch -d <type>/<short-description>
```

`git branch -d` refuses to delete a branch carrying unmerged commits. That refusal is the
point of using `-d`; reach for `-D` only when you deliberately mean to discard work.

## Branch names

`<type>/<short-description>` — lower case, hyphen-separated. Types in use: `feat`, `fix`,
`docs`, `refactor`, `chore`. Examples: `feat/user-search`, `fix/cors-separator`,
`docs/git-workflow`.

## Notes

- **`develop` is the integration branch.** New branches start from it and PRs target it.
- **`master` is outside this cycle.** It is the repository's default branch, but it takes no
  part in the seven steps: nothing branches from it and no PR targets it.
- **Step 3 means what it says.** The local commit is the reviewable unit. Pushing before
  approval puts unreviewed work on the remote and turns a review into a moving target.
- **Step 4 is bookkeeping, not part of the fix.** Archive and mark `Done` only after
  approval, and in a commit of its own. `git mv` changes every relative path into and out of
  the file, so repair the links in that same commit — the full list is in
  [`.claude/commands/implement.md`](../.claude/commands/implement.md) — or the links rot
  silently while each one still looks fine in isolation.
- **Steps 6 and 7 are load-bearing, not tidiness.** A stale local `develop` means the next
  branch starts from the wrong base, and a branch left lying around is one you will
  eventually force-delete past the `-d` guard.
