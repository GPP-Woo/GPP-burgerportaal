import { loadScript, toTrackedUrl, withTrailingSlash, type Tracker } from "./utils";

declare global {
  interface Window {
    _paq?: unknown[][];
  }
}

const NO_TERM = "(geen zoekterm)";

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
    },
    // The custom URL of the last pageview stays set, so no query string is sent
    search: ({ term, filters, count }) => {
      // The filters are the category, so the result count belongs to the complete search
      const category = Object.entries(filters)
        .map(([key, values]) => `${key}: ${values.join(", ")}`)
        .join(" | ");

      // The keyword is required, so a search with only filters gets a placeholder
      window._paq!.push(["trackSiteSearch", term || NO_TERM, category || false, count]);
    }
  } satisfies Tracker;
};
