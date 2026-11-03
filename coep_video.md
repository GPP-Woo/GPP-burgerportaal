# Disable Cross-Origin Embedder Policy (COEP) for Video Embedding

## Context

Cross-Origin Embedder Policy (COEP) is a security header that requires all cross-origin resources to explicitly opt-in to being embedded. When COEP is enabled with `require-corp`, the browser enforces that:

1. All cross-origin subresources must include a `Cross-Origin-Resource-Policy` header or a `Access-Control-Allow-Origin` header for CORS
2. All cross-origin iframes must include a `Cross-Origin-Embedder-Policy` header

COEP provides protection against Spectre-style attacks by isolating the page in a secure context, preventing cross-origin information leaks.

Our application implements COEP by default to enhance security posture and meet modern web security standards.

## Problem

YouTube and Vimeo embedded players do not include the required `Cross-Origin-Embedder-Policy` header on their main player endpoints:

- `https://player.vimeo.com/video/*`
- `https://www.youtube.com/embed/*`

When COEP is enabled, browsers block the loading of these iframes entirely. This is a incompatibility between third-party video providers and COEP requirements.

## Decision

We will **conditionally disable COEP** when video embedding is configured in the application.

**Implementation:**

- Check if `VideoUrl` is configured in application settings
- If video embedding is enabled, omit the COEP header
- If no video embedding, apply COEP with `require-corp` policy

## Rationale

### Why This Solution

1. **Provider limitation**: YouTube and Vimeo control their header implementation, not us
2. **No viable alternatives**: Other approaches break essential video features or lack browser support
3. **Minimal security impact**: Other CSP directives still provide substantial protection

### Alternatives Considered

| Alternative               | Why Rejected                                              |
| ------------------------- | --------------------------------------------------------- |
| **iframe sandbox**        | Breaks essential video features                           |
| **iframe credentialless** | Limited browser support (~60%), no Safari/Firefox support |
| **Proxy video resources** | Technical complexity                                      |

## Monitoring and Review

This decision should be revisited when:

1. **Provider updates**: YouTube or Vimeo implement COEP headers
2. **Browser evolution**: `credentialless` attribute reaches >90% browser support
3. **Standards development**: New web standards for secure video embedding
4. **Security requirements change**: If COEP becomes mandatory regardless of functionality

## References

- [Cross-Origin Embedder Policy Specification](https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Headers/Cross-Origin-Embedder-Policy)
- [Browser Support for credentialless](https://caniuse.com/mdn-html_elements_iframe_credentialless)
- [issuetracker.google.com](https://issuetracker.google.com/issues/351843802)
