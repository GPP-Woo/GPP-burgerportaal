import { loadScript, toTrackedUrl, withTrailingSlash, type Tracker } from "./utils";

type PlausibleOptions = Record<string, unknown> & {
  transformRequest?: (payload: Record<string, unknown>) => Record<string, unknown> | null;
};

type PlausibleEventOptions = { u: string; props?: Record<string, string> };

declare global {
  interface Window {
    plausible?: ((event: string, options?: PlausibleEventOptions) => void) & {
      q?: unknown[][];
      o?: PlausibleOptions;
    };
  }
}

export const loadPlausible = (url: string, scriptId: string) => {
  window.plausible =
    window.plausible ||
    Object.assign(
      (...args: unknown[]) => {
        (window.plausible!.q = window.plausible!.q || []).push(args);
      },
      { q: [] as unknown[][] }
    );

  // Tracking of navigations, links, downloads and forms is switched off
  // A pageview dropped by transformRequest makes the script flush and stop its engagement tracking
  const ignoredUrl = "about:blank";
  let tracked = false;

  window.plausible.o = {
    autoCapturePageviews: false,
    outboundLinks: false,
    fileDownloads: false,
    formSubmissions: false,
    transformRequest: (payload) =>
      payload.u === ignoredUrl
        ? null
        : {
            ...payload,
            r: toTrackedUrl(String(payload.r ?? "")) || null
          }
  };

  loadScript(`${withTrailingSlash(url)}js/${scriptId}.js`);

  return {
    track: (path: string, title: string) => {
      tracked = true;
      window.plausible!("pageview", { u: toTrackedUrl(path), props: { title } });
    },
    search: ({ term, filters, count }) =>
      window.plausible!("search", {
        // The location has a query string with the search: only the path is sent
        u: toTrackedUrl(window.location.href),
        props: {
          // Props are reported separately: this one keeps the complete search together
          query: [term, ...Object.entries(filters).map(([k, names]) => `${k}: ${names.join(", ")}`)]
            .filter(Boolean)
            .join(" | "),
          ...(term && { term }),
          results: String(count),
          ...Object.fromEntries(
            Object.entries(filters).map(([key, names]) => [`filter_${key}`, names.join(", ")])
          )
        }
      }),
    stop: () => {
      if (!tracked) return;

      tracked = false;
      window.plausible!("pageview", { u: ignoredUrl });
    }
  } satisfies Tracker;
};
