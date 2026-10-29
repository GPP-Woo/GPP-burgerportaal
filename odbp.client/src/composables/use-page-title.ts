import { toValue, watch, type MaybeRefOrGetter } from "vue";
import { useRoute } from "vue-router";
import { trackPageView } from "@/analytics";
import { setPageTitle } from "@/title";

// Replaces the generic route title with a specific one, as soon as it is available.
// Routes with meta.titleFromView are not tracked by the router, so the pageview carries this title.
export const usePageTitle = (title: MaybeRefOrGetter<string | undefined>) => {
  const route = useRoute();

  return watch(
    () => toValue(title),
    (value) => {
      if (!value) return;

      setPageTitle(value);
      trackPageView(route.path, document.title);
    },
    { immediate: true }
  );
};
