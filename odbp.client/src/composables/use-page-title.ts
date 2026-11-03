import { toValue, watch, type MaybeRefOrGetter } from "vue";
import { useRoute } from "vue-router";
import { trackPageView } from "@/analytics";
import { setPageTitle } from "@/title";

// Replaces the generic route title with a specific one, as soon as it is available.
// Routes with meta.titleFromView are not tracked by the router, so the pageview is tracked here:
// once per path, as soon as the title is known or loading failed (then with the generic route title).
export const usePageTitle = (
  title: MaybeRefOrGetter<string | undefined>,
  failed?: MaybeRefOrGetter<unknown>
) => {
  const route = useRoute();
  let trackedPath: string | undefined;

  return watch(
    () => [toValue(title), !!toValue(failed)] as const,
    ([value, hasFailed]) => {
      if (value) setPageTitle(value);

      if (trackedPath === route.path || !(value || hasFailed)) return;

      trackedPath = route.path;
      trackPageView(route.path, document.title);
    },
    { immediate: true }
  );
};
