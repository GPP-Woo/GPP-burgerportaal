import { loadScript, toTrackedUrl, type Tracker } from "./utils";

declare global {
  interface Window {
    _sz?: unknown[][];
  }
}

export const loadSiteimprove = (accountId: string) => {
  window._sz = window._sz || [];

  loadScript(`https://siteimproveanalytics.com/js/siteanalyze_${accountId}.js`);

  let previousUrl = toTrackedUrl(document.referrer);
  // The script registers the first pageview itself, which can't be suppressed or overridden
  let isFirst = true;

  // Always push to window._sz, since the script replaces its push method once loaded
  return {
    track: (path: string, title: string) => {
      const currentUrl = toTrackedUrl(path);

      if (!isFirst)
        window._sz!.push(["trackdynamic", { url: currentUrl, ref: previousUrl, title }]);

      isFirst = false;
      previousUrl = currentUrl;
    }
  } satisfies Tracker;
};
