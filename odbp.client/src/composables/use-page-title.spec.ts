import { nextTick, ref } from "vue";
import { beforeEach, describe, expect, test, vi } from "vitest";
import { trackPageView } from "@/analytics";
import { initBaseTitle } from "@/title";
import { usePageTitle } from "./use-page-title";

vi.mock("@/analytics", () => ({ trackPageView: vi.fn() }));
vi.mock("vue-router", () => ({ useRoute: () => ({ path: "/publicaties/1" }) }));

describe("usePageTitle", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    document.title = "Portaal";
    initBaseTitle();
  });

  test("tracks the pageview with the specific title once it is available", async () => {
    const title = ref<string>();

    usePageTitle(title);
    await nextTick();

    expect(trackPageView).not.toHaveBeenCalled();

    title.value = "Besluit";
    await nextTick();

    expect(trackPageView).toHaveBeenCalledExactlyOnceWith("/publicaties/1", "Besluit | Portaal");
  });

  test("does not track again when the title changes on the same path", async () => {
    const title = ref<string>("Besluit");

    usePageTitle(title);
    title.value = "Ander besluit";
    await nextTick();

    expect(document.title).toBe("Ander besluit | Portaal");
    expect(trackPageView).toHaveBeenCalledTimes(1);
  });

  test("tracks with the generic title when loading failed", async () => {
    const title = ref<string>();
    const failed = ref<unknown>(null);

    usePageTitle(title, failed);
    await nextTick();

    expect(trackPageView).not.toHaveBeenCalled();

    failed.value = new Error("fetch failed");
    await nextTick();

    expect(trackPageView).toHaveBeenCalledExactlyOnceWith("/publicaties/1", "Portaal");
  });
});
