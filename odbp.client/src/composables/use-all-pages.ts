import { ref, toValue, type MaybeRefOrGetter } from "vue";
import { asyncComputed } from "@vueuse/core";
import { type PagedResult } from "@/api";

const fetchPage = <T>(url: string, signal?: AbortSignal | undefined) =>
  fetch(url, { headers: { "is-api": "true" }, signal })
    .then((r) => (r.ok ? r : Promise.reject(r)))
    .then((r) => r.json() as Promise<PagedResult<T> | T[]>)
    .then((r) => (Array.isArray(r) ? { results: r, next: undefined } : r));

export const fetchAllPages = async <T>(
  url: string,
  signal?: AbortSignal | undefined
): Promise<T[]> => {
  const { results, next } = await fetchPage<T>(url, signal);

  if (next) {
    const { pathname, search } = new URL(next);

    return [...results, ...(await fetchAllPages<T>(pathname + search, signal))];
  }

  return results;
};

export const useAllPages = <T>(url: MaybeRefOrGetter<string | null>) => {
  const loading = ref(true);
  const error = ref(false);

  const currentUrl = toValue(url);

  const data = asyncComputed(
    async (onCancel) => {
      if (!currentUrl) return [] as T[];

      const abortController = new AbortController();

      onCancel(() => abortController.abort());

      return await fetchAllPages<T>(currentUrl, abortController.signal).catch(() => {
        error.value = true;

        return [] as T[];
      });
    },
    [] as T[],
    loading
  );

  return {
    data,
    loading,
    error
  };
};
