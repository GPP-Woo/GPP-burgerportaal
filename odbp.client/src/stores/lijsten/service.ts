import { ref } from "vue";
import { fetchAllPages } from "@/composables/use-all-pages";
import { promiseAll } from "@/utils";
import type { ListItem } from "./types";

const urls = {
  organisaties: "/api/v1/organisaties",
  informatiecategorieen: "/api/v1/informatiecategorieen",
  onderwerpen: "/api/v1/onderwerpen"
} as const;

export const lijsten = ref<Record<keyof typeof urls, ListItem[]>>();

const fetcher = async (url: string) =>
  fetchAllPages<ListItem | { uuid: string; officieleTitel: string }>(url).then((r) =>
    r.map(({ uuid, ...rest }) => ({
      uuid,
      naam: "naam" in rest ? rest.naam : rest.officieleTitel
    }))
  );

export const loadLijsten = async () => {
  try {
    const promises = Object.fromEntries(
      Object.entries(urls).map(([key, url]) => [key, fetcher(url)])
    ) as Record<keyof typeof urls, Promise<ListItem[]>>;

    lijsten.value = await promiseAll(promises);
  } catch {
    return;
  }
};
