import { nextTick } from "vue";
import { createMemoryHistory, createRouter, type RouteRecordRaw } from "vue-router";
import { beforeEach, describe, expect, test, vi } from "vitest";

const component = { render: () => null };

const routes: RouteRecordRaw[] = [
  { path: "/", component, meta: { title: "Homepage" } },
  { path: "/zoeken", component, meta: { title: "Zoeken" } },
  { path: "/oud", redirect: "/zoeken" },
  {
    path: "/beheer",
    component,
    meta: { noAnalytics: true },
    children: [
      { path: "", component },
      { path: "/forbidden", component }
    ]
  }
];

const matomo = { tool: "Matomo", url: "https://matomo.example.com", identifier: "7" } as const;

const setup = async (analytics: unknown = matomo, referrer = "") => {
  vi.resetModules();
  window._paq = undefined;
  document.head.innerHTML = "";
  Object.defineProperty(document, "referrer", { value: referrer, configurable: true });

  const { initAnalytics } = await import("./analytics");
  const router = createRouter({ history: createMemoryHistory(), routes });

  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  initAnalytics(router, analytics as any);

  const go = async (path: string) => {
    await router.push(path);
    await nextTick();
  };

  return { go };
};

const commands = (name: string) => (window._paq ?? []).filter(([command]) => command === name);
const trackedUrls = () => commands("setCustomUrl").map(([, url]) => url);

describe("analytics (Matomo)", () => {
  beforeEach(() => {
    document.title = "Portaal";
  });

  test("loads the script asynchronously and disables cookies before the first pageview", async () => {
    const { go } = await setup();
    await go("/");

    const script = document.head.querySelector("script");

    expect(script?.src).toBe("https://matomo.example.com/matomo.js");
    expect(script?.async).toBe(true);
    expect(window._paq?.[0]).toEqual(["disableCookies"]);
    expect(window._paq).toContainEqual(["setSiteId", "7"]);
    expect(commands("enableLinkTracking")).toHaveLength(0);
  });

  test("tracks the first visit of the homepage exactly once", async () => {
    const { go } = await setup();
    await go("/");

    expect(commands("trackPageView")).toHaveLength(1);
    expect(trackedUrls()).toEqual([`${window.location.origin}/`]);
  });

  test("only tracks the path, also for the referrer", async () => {
    const { go } = await setup();
    await go("/");
    await go("/zoeken?query=jansen#resultaten");

    expect(trackedUrls()).toEqual([
      `${window.location.origin}/`,
      `${window.location.origin}/zoeken`
    ]);
    expect(commands("setReferrerUrl")).toEqual([["setReferrerUrl", `${window.location.origin}/`]]);
  });

  test("replaces a referrer with a query string after a full page load", async () => {
    const { go } = await setup(matomo, `${window.location.origin}/zoeken?query=jansen`);
    await go("/");

    expect(commands("setReferrerUrl")).toEqual([
      ["setReferrerUrl", `${window.location.origin}/zoeken`]
    ]);
  });

  test("does not track navigations that only change the query or hash", async () => {
    const { go } = await setup();
    await go("/zoeken");
    await go("/zoeken?query=a");
    await go("/zoeken?query=a#anker");

    expect(commands("trackPageView")).toHaveLength(1);
  });

  test("only tracks the final destination of a redirect", async () => {
    const { go } = await setup();
    await go("/oud");

    expect(trackedUrls()).toEqual([`${window.location.origin}/zoeken`]);
  });

  test("does not track routes that opt out, including child routes outside /beheer", async () => {
    const { go } = await setup();
    await go("/beheer");
    await go("/forbidden");

    expect(commands("trackPageView")).toHaveLength(0);
  });

  test.each([{ tool: "None", url: "", identifier: "" }, null])(
    "loads nothing without an analytics tool (%o)",
    async (analytics) => {
      const { go } = await setup(analytics);
      await go("/");

      expect(window._paq).toBeUndefined();
      expect(document.head.querySelector("script")).toBeNull();
    }
  );
});
