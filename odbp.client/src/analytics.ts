import { nextTick } from "vue";
import type { Router } from "vue-router";
import type { Resources } from "./resources";

declare global {
  interface Window {
    _paq?: unknown[][];
  }
}

const withTrailingSlash = (url: string) => (url.endsWith("/") ? url : `${url}/`);

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

  // Always push to window._paq, since Matomo replaces its push method once loaded
  return (path: string, title: string) => {
    window._paq!.push(["setCustomUrl", `${window.location.origin}${path}`]);
    window._paq!.push(["setDocumentTitle", title]);
    window._paq!.push(["trackPageView"]);
  };
};

let tracker: ReturnType<typeof loadMatomo> | undefined;

// For views that track their own pageview, once their specific title is known
export const trackPageView = (path: string, title: string) => tracker?.(path, title);

export const initAnalytics = (router: Router, analytics: Resources["analytics"]) => {
  if (analytics?.tool !== "Matomo" || !analytics.url || !analytics.identifier) return;

  tracker = loadMatomo(analytics.url, analytics.identifier);

  router.afterEach((to, from, failure) => {
    // Only the portal itself is tracked, not the management screens
    if (failure || to.path.startsWith("/beheer")) return;
    if (to.meta.titleFromView) return;

    // Query string is left out, as it can contain user input (e.g. search terms)
    // Wait for the view to set the document title
    nextTick(() => tracker?.(to.path, document.title));
  });
};
