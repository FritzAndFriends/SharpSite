# Decision: Pin Aspire Port for E2E Test Mode

**Date:** 2026-03-26  
**Author:** Zoe (CI/DevOps)  
**Status:** Implemented  
**PR:** #352

## Context

After upgrading to Aspire 13.2, the Playwright CI workflow hangs because Aspire assigns dynamic ports to the web frontend. The `build-and-test.ps1` script and E2E tests both expect port 5020.

## Decision

- Added `WithHttpEndpoint(port: 5020, name: "http")` in the AppHost when `testOnly` is true
- Improved `build-and-test.ps1` with stderr capture, progress logging, and diagnostic dump on failure
- Reduced HTTP health-check timeout from 5s to 2s (effective max wait drops from ~10.5 min to ~6 min)

## Impact

- CI pipeline should no longer hang waiting for an unreachable port
- Failures will now produce diagnostic output (AppHost stdout/stderr) for faster debugging
- No impact on non-test (production) Aspire configuration
