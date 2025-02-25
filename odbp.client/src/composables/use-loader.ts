import { onWatcherCleanup, readonly, ref, watchEffect } from "vue";

export function useLoader<T>(fetcher: (signal: AbortSignal) => Promise<T> | undefined) {
  const error = ref(false);
  const loading = ref(true);
  const data = ref<T>();

  watchEffect(() => {
    const controller = new AbortController();

    onWatcherCleanup(() => controller.abort());
    error.value = false;
    loading.value = true;

    const promise = fetcher(controller.signal);
    if (!promise) return;

    promise
      .then((v) => {
        data.value = v;
        loading.value = false;
      })
      .catch(() => {
        if (!controller.signal.aborted) {
          error.value = true;
          loading.value = false;
        }
      });
  });

  return {
    error: readonly(error),
    loading: readonly(loading),
    data: readonly(data)
  };
}
