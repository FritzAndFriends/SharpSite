# Site Asset Management

> Reuse one Blazor component for logo/favicon maintenance across setup and admin.

## SCOPE

✅ THIS SKILL PRODUCES:
- A shared upload/preview/remove component for site identity assets
- Deterministic filenames for durable branding assets
- Layout bindings that resolve custom assets through the file API

❌ THIS SKILL DOES NOT PRODUCE:
- Image cropping, resizing, or optimization pipelines
- Media-library browsing
- Theme-specific asset variants

## Pattern

When SharpSite needs editable site branding:

1. Store only the selected filename in `ApplicationState`
2. Keep the binary in file storage, served from `/api/files/{filename}`
3. Use a shared interactive component to:
   - preview the current asset
   - validate allowed extensions for the asset kind
   - upload a replacement using a deterministic filename
   - remove the current custom asset
4. Let parent components persist the chosen filename so the same component works in setup and admin

## Current Conventions

- Logo filenames: `site-logo.{ext}`
- Favicon filenames: `site-favicon.{ext}`
- Logo types: PNG, JPG, JPEG, WebP, GIF, SVG
- Favicon types: PNG, ICO, SVG

## Why This Exists

Logo and favicon management were explicitly unified for v0.8. Reusing the component keeps the UX aligned between the setup wizard and the admin appearance screen while minimizing render and maintenance cost.
