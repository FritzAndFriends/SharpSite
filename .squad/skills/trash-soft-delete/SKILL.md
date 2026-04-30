---
name: trash-soft-delete
description: Use a shared trash contract for recoverable content deletion
domain: backend, data
confidence: high
source: earned (issues #309 and #299)
---

## Context

SharpSite's content delete semantics for pages and posts are recycle-bin based. "Delete" should hide content from normal reads immediately, preserve enough metadata for recovery, and keep permanent removal as a separate explicit action.

## Patterns

- Add a shared contract in abstractions (`ITrashableContent`) instead of duplicating `IsDeleted` / `DeletedAt` logic per model.
- Put state transitions in small shared helpers (`MoveToTrash`, `RestoreFromTrash`) so repositories only handle persistence concerns.
- Keep repository semantics explicit:
  - `Delete*` => move to trash
  - `GetDeleted*` => list trash
  - `Restore*` => recover from trash
  - `PermanentlyDelete*` => hard delete
- Make all normal read paths exclude trashed content by default so UI components don't need to remember the filter.
- Mirror schema and repository changes in both `src\SharpSite.Data.Postgres` and `plugins\SharpSite.Plugins.Data.Postgres` while both data stacks exist.

## Examples

- `PgPostRepository.DeletePost(slug)` marks `IsDeleted` and `DeletedAt`, then saves.
- `PgPageRepository.GetPages()` returns only `!IsDeleted` rows and invalidates the cached active-page list on trash/restore/purge.
- EF migrations add nullable `DeletedAt` plus non-null `IsDeleted` with a default of `false`.

## Testing Notes

- Repository tests can use EF Core InMemory safely when `PgContext` is registered with a shared `InMemoryDatabaseRoot`; otherwise separate scopes may not observe the same data.
- Validate the contract end-to-end for each content type:
  1. `Delete*` marks the entity deleted and removes it from normal reads
  2. `GetDeleted*` exposes the trashed entity
  3. `Restore*` clears trash metadata and restores normal visibility
  4. `PermanentlyDelete*` removes the row entirely
- For pages, re-read the saved entity before asserting by `Id`, because `PgPageRepository.AddPage` currently returns the original abstraction object rather than the tracked database instance.

## Anti-Patterns

- Treating soft delete like a UI concern and filtering only in Razor components
- Reusing `Delete*` for permanent removal
- Adding trash fields to one data stack but not the mirrored plugin-backed data stack
