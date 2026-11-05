# Disable Cross-Origin Embedder Policy (COEP) when Analytics is Configured

## Context

An administrator can configure an analytics tool (currently Matomo) in the management screen. See [coep_video.md](coep_video.md) for what COEP is and for the same decision for video embedding. For the CSP side of analytics, see [analytics_csp.md](analytics_csp.md).

With COEP `require-corp`, every cross-origin resource must opt in explicitly:

- resources loaded without CORS (for example a plain `<script>` or `<img>`) need a `Cross-Origin-Resource-Policy: cross-origin` header (or `same-site` when the analytics server is on the same site as the portal);
- resources loaded with CORS (`fetch`, XHR, or `crossorigin="anonymous"`) need a valid `Access-Control-Allow-Origin` header.

## Problem

An analytics tool loads a script, sends tracking requests and may load a tracking image from a different origin, the server of the organisation or supplier. For Matomo:

- `matomo.js` is loaded as a plain script (no CORS). By default it might have no `Access-Control-Allow-Origin` header, in which case loading it with `crossorigin="anonymous"` would be blocked as well.
- `matomo.php` receives the tracking requests. Depending on the browser and the tracker configuration this is a beacon, an XHR/fetch request (CORS) or an image request (no CORS).

Different installations configure different tools and servers, and we do not know whether each of them sends the headers that `require-corp` demands. If they do not, the tool is blocked by the browser and nothing is tracked.

## Decision

We **omit the COEP header while an analytics tool is configured**, the same rule as for video.

**Implementation:**

- `AnalyticsOriginMiddleware` determines the configured analytics origin (see [analytics_csp.md](analytics_csp.md)).
- `CoepMiddleware` omits the COEP header if a video URL or an analytics origin is configured, and otherwise sets `Cross-Origin-Embedder-Policy: require-corp`.
- COEP is owned exclusively by `CoepMiddleware`. The security headers policy does not set it, since it would otherwise be added or removed per request in a second place.

## Alternatives Considered

| Alternative                                           | Why Rejected                                                                    |
| ----------------------------------------------------- | ------------------------------------------------------------------------------- |
| **Load `matomo.js` with `crossorigin`**               | Requires `Access-Control-Allow-Origin` on `matomo.js`, which is not sent by default |
| **Require CORP/CORS headers on the analytics server** | An extra requirement for every installation; we cannot verify it for the server |
| **Separate setting to keep COEP**                     | Larger scope; a product decision                                                |

Organisations that run their own Matomo can still make it COEP-compatible, but the portal does not assume this.

## Consequences

- With analytics configured, the portal does not have the isolation that COEP provides. The other security headers (CSP, COOP, CORP and so on) are unchanged. The CSP only gains the analytics origin, without path or wildcard.
- Choosing "Geen" (none) restores COEP. The change follows the cache duration of the analytics origin (at most 5 minutes by default, see `ANALYTICS_CACHE_DURATION_MINUTES` in [analytics_csp.md](analytics_csp.md)) and, for a visitor who already has the portal open, applies on the next full page load. See [analytics_csp.md](analytics_csp.md).

## Monitoring and Review

This decision should be revisited when:

1. **Analytics servers**: the supported tools reliably send `Cross-Origin-Resource-Policy` or CORS headers for all resources the portal uses.
2. **Additional tools**: a new analytics tool is added; it must be evaluated separately.
3. **Security requirements change**: if COEP becomes mandatory regardless of functionality.
