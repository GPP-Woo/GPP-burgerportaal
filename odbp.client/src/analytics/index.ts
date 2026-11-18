import { nextTick } from "vue";
import { START_LOCATION, type Router } from "vue-router";
import type { Resources } from "@/resources";
import { loadMatomo } from "./matomo";
import { loadPlausible } from "./plausible";
import { maskSearchTerm, type SearchEvent } from "./search";
import { loadSiteimprove } from "./siteimprove";
import type { Tracker } from "./utils";

let tracker: Tracker | undefined;

// For views that track their own pageview, once their specific title is known
export const trackPageView = (path: string, title: string) => tracker?.track(path, title);

// The term is masked here, so no caller can forget it
export const trackSearch = (event: SearchEvent) =>
  tracker?.search?.({ ...event, term: maskSearchTerm(event.term) });

export const initAnalytics = (router: Router, analytics: Resources["analytics"]) => {
  if (!analytics?.identifier) return;

  try {
    // Siteimprove is a hosted service, so it has no URL
    if (analytics.tool === "Siteimprove") tracker = loadSiteimprove(analytics.identifier);
    else if (!analytics.url) return;
    else if (analytics.tool === "Matomo") tracker = loadMatomo(analytics.url, analytics.identifier);
    else if (analytics.tool === "Plausible")
      tracker = loadPlausible(analytics.url, analytics.identifier);
    else return;
  } catch {
    // An unusable URL must never break the portal
    return;
  }

  router.afterEach((to, from, failure) => {
    // Routes can opt out; meta is inherited by child routes
    if (!failure && to.meta.noAnalytics) tracker?.stop?.();
    if (failure || to.meta.noAnalytics || to.meta.titleFromView) return;

    // Only query or hash changes are no new pageview, but the first navigation (also path "/") is
    if (to.path === from.path && from !== START_LOCATION) return;

    // Wait for the view to set the document title
    nextTick(() => tracker?.track(to.path, document.title));
  });
};
