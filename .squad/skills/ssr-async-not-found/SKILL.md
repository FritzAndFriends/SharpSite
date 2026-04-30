---
name: ssr-async-not-found
description: Prevent premature 404s on SSR Razor components that load content asynchronously
domain: frontend, blazor
confidence: high
source: earned (public post route blocker)
---

## Context

In SharpSite SSR pages, a component can render once before an awaited repository call completes. If the page treats `null` model state as "not found" immediately, the first render can set HTTP 404 even though the content exists and arrives moments later.

## Pattern

- Separate route-only components from shared render/load components when a page needs multiple public routes.
- Track load completion explicitly with a flag such as `_IsLoaded`.
- Render a neutral loading state while the async fetch is in flight.
- Only render `PageNotFound` after the fetch completes and the model is still null.
- Keep the actual content rendering path unchanged once the model is present.

## Example Shape

1. Route component maps `/post/{slug}` and passes parameters into a shared page component.
2. Shared page component:
   - sets `_IsLoaded = false`
   - awaits repository lookup
   - sets `_IsLoaded = true`
   - renders content when found
   - renders `PageNotFound` only when `_IsLoaded` is true and the model is null

## Anti-Patterns

- Using `model is null` as both "loading" and "missing"
- Setting 404 response codes from the first render of an async SSR page
- Duplicating the full page markup just to support an alternate route when a shared content component will do
