import { toValue, watch, type MaybeRefOrGetter } from "vue";
import { useRoute } from "vue-router";
import { trackPageView } from "@/analytics";
import { setPageTitle } from "@/title";

export const UNAVAILABLE_TITLE = "Niet (meer) beschikbaar";

// Replaces the generic route title with a specific one, as soon as it is available.
// Routes with meta.titleFromView are not tracked by the router, so the pageview is tracked here:
// once per page (the view is remounted per path, see App.vue), as soon as the title is known
// or loading failed (then with "Niet (meer) beschikbaar").
export const usePageTitle = (
  title: MaybeRefOrGetter<string | undefined>,
  failed?: MaybeRefOrGetter<unknown>
) => {
  const path = useRoute().path;
  let tracked = false;

  return watch(
    () => [toValue(title), !!toValue(failed)] as const,
    ([value, hasFailed]) => {
      if (value) setPageTitle(value);
      else if (hasFailed) setPageTitle(UNAVAILABLE_TITLE);

      if (tracked || !(value || hasFailed)) return;

      trackPageView(path, document.title);
      tracked = true;
    },
    { immediate: true }
  );
};
