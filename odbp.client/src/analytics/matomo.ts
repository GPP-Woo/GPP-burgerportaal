import { loadScript, toTrackedUrl, withTrailingSlash, type Tracker } from "./utils";

declare global {
  interface Window {
    _paq?: unknown[][];
  }
}

export const loadMatomo = (url: string, siteId: string) => {
  const baseUrl = withTrailingSlash(url);

  window._paq = window._paq || [];

  // Explicitly no cookies, so no consent is needed
  window._paq.push(["disableCookies"]);
  window._paq.push(["setTrackerUrl", `${baseUrl}matomo.php`]);
  window._paq.push(["setSiteId", siteId]);

  loadScript(`${baseUrl}matomo.js`);

  // Overrides the default referrer of the tracker, which could contain a query string after a full page load
  let previousUrl = toTrackedUrl(document.referrer);

  // Always push to window._paq, since Matomo replaces its push method once loaded
  return {
    track: (path: string, title: string) => {
      const currentUrl = toTrackedUrl(path);

      if (previousUrl) window._paq!.push(["setReferrerUrl", previousUrl]);
      window._paq!.push(["setCustomUrl", currentUrl]);
      window._paq!.push(["setDocumentTitle", title]);
      window._paq!.push(["trackPageView"]);

      previousUrl = currentUrl;
    }
  } satisfies Tracker;
};
