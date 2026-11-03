# Analytics and the Content Security Policy (CSP)

## Context

An administrator can configure an analytics tool (currently Matomo) in the management screen. The tool loads a script, sends requests and loads a tracking image from the configured server, so that server has to be allowed by the CSP. Without analytics, the CSP is unchanged.

## How it works

- `AnalyticsOriginMiddleware` reads the configured tool and URL from the database and derives the **origin** (`https://host[:port]`, without path or wildcard). Only a valid `https` origin is accepted; anything else results in the strictest CSP (no analytics).
- The origin is added to `script-src`, `connect-src` and `img-src` only.
- The CSP is selected per request with `SetPolicySelector` (NetEscapades.AspNetCore.SecurityHeaders 1.x). A header policy is built once per origin and reused.
- COEP is omitted while analytics is configured. See [coep_analytics.md](coep_analytics.md).

## Cache duration

The analytics origin is cached in memory for **at most 5 minutes**. Saving the analytics configuration in the management screen clears the cache of the instance that handles the request.

- **Multiple instances (pods):** the other instances keep the old origin until their cache expires, so a change is active everywhere within 5 minutes at the latest. No restart is needed.
- **Database unavailable:** the strictest CSP (no analytics) is used, never a widened one. Nothing is cached in that case, so every request retries the read and the configured origin returns as soon as the database is available again.

## Known limitation: single page application

The portal is a single page application, so the CSP is only evaluated on a full page load. After a change, a visitor who already has the portal open keeps the old CSP, and an already loaded tracking script keeps running, until the next full page load (refresh or opening the portal again). This is accepted, but relevant for administrators who unlink the tool for privacy reasons: it can take until the next full page load before no more data is sent.

## Review

Revisit this when another analytics tool is added (for example Siteimprove or Plausible): each tool needs its own list of CSP sources, since the origin of the configured URL is not always sufficient.
