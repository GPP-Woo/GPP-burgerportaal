import { nextTick } from "vue";
import { START_LOCATION, type Router } from "vue-router";
import type { Resources } from "./resources";

declare global {
  interface Window {
    _paq?: unknown[][];
    plausible?: ((event: string, options?: { u: string }) => void) & { q?: unknown[][] };
  }
}

// Endpoint names must be appended to the pathname, not to a query string or hash the URL may contain
const withTrailingSlash = (url: string) => {
  const baseUrl = new URL(url);

  baseUrl.search = "";
  baseUrl.hash = "";
  if (!baseUrl.pathname.endsWith("/")) baseUrl.pathname += "/";

  return baseUrl.href;
};

// Only the path is ever sent: query strings and hashes can contain user input (e.g. search terms)
const toTrackedUrl = (url: string) => {
  if (!url) return "";

  try {
    const { origin, pathname } = new URL(url, window.location.origin);

    return `${origin}${pathname}`;
  } catch {
    return "";
  }
};

const loadMatomo = (url: string, siteId: string) => {
  const baseUrl = withTrailingSlash(url);

  window._paq = window._paq || [];

  // Explicitly no cookies, so no consent is needed
  window._paq.push(["disableCookies"]);
  window._paq.push(["setTrackerUrl", `${baseUrl}matomo.php`]);
  window._paq.push(["setSiteId", siteId]);

  const script = Object.assign(document.createElement("script"), {
    async: true,
    src: `${baseUrl}matomo.js`
  });

  document.head.appendChild(script);

  // Overrides the default referrer of the tracker, which could contain a query string after a full page load
  let previousUrl = toTrackedUrl(document.referrer);

  // Always push to window._paq, since Matomo replaces its push method once loaded
  return (path: string, title: string) => {
    const currentUrl = toTrackedUrl(path);

    if (previousUrl) window._paq!.push(["setReferrerUrl", previousUrl]);
    window._paq!.push(["setCustomUrl", currentUrl]);
    window._paq!.push(["setDocumentTitle", title]);
    window._paq!.push(["trackPageView"]);

    previousUrl = currentUrl;
  };
};

// Manual mode: the script does not track navigations itself, so the portal sends one pageview per navigation.
// The script always sends document.referrer, which cannot be overridden (the Referrer-Policy keeps it to the origin)
const loadPlausible = (url: string, domain: string) => {
  window.plausible =
    window.plausible ||
    Object.assign(
      (...args: unknown[]) => {
        (window.plausible!.q = window.plausible!.q || []).push(args);
      },
      { q: [] as unknown[][] }
    );

  const script = Object.assign(document.createElement("script"), {
    async: true,
    src: `${withTrailingSlash(url)}js/script.manual.js`
  });

  script.dataset.domain = domain;
  document.head.appendChild(script);

  return (path: string) => window.plausible!("pageview", { u: toTrackedUrl(path) });
};

let tracker: ((path: string, title: string) => void) | undefined;

// For views that track their own pageview, once their specific title is known
export const trackPageView = (path: string, title: string) => tracker?.(path, title);

export const initAnalytics = (router: Router, analytics: Resources["analytics"]) => {
  if (!analytics?.url || !analytics.identifier) return;

  try {
    if (analytics.tool === "Matomo") tracker = loadMatomo(analytics.url, analytics.identifier);
    else if (analytics.tool === "Plausible")
      tracker = loadPlausible(analytics.url, analytics.identifier);
    else return;
  } catch {
    // An unusable URL must never break the portal
    return;
  }

  router.afterEach((to, from, failure) => {
    // Routes can opt out (the management screens); meta is inherited by child routes
    if (failure || to.meta.noAnalytics || to.meta.titleFromView) return;

    // Only query or hash changes are no new pageview, but the first navigation (also path "/") is
    if (to.path === from.path && from !== START_LOCATION) return;

    // Wait for the view to set the document title
    nextTick(() => tracker?.(to.path, document.title));
  });
};
