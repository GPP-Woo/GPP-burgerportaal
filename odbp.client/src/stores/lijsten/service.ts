import { ref } from "vue";
import { promiseAll } from "@/utils";
import { fetchAllPages } from "@/composables/use-all-pages";
import type { WaardelijstItem } from "./types";
import type { Onderwerp } from "@/features/onderwerp/types";

const endpoints = {
  organisaties: { url: "/api/v1/organisaties", type: [] as WaardelijstItem[] },
  informatiecategorieen: { url: "/api/v1/informatiecategorieen", type: [] as WaardelijstItem[] },
  onderwerpen: { url: "/api/v1/onderwerpen", type: [] as (Onderwerp & { naam: string })[] }
} as const;

type EndpointKey = keyof typeof endpoints;

type ListTypes = {
  [K in EndpointKey]: (typeof endpoints)[K]["type"];
};

export const lijsten = ref<ListTypes>();

const fetcher = <K extends EndpointKey>(key: K) =>
  fetchAllPages<WaardelijstItem | Onderwerp>(endpoints[key].url).then(
    (r) =>
      r.map((item) => ({
        ...item,
        naam: "officieleTitel" in item ? item.officieleTitel : item.naam
      })) as ListTypes[K]
  );

export const loadLijsten = async () => {
  try {
    const promises = Object.fromEntries(
      (Object.keys(endpoints) as Array<EndpointKey>).map((key) => [key, fetcher(key)])
    ) as { [K in EndpointKey]: Promise<ListTypes[K]> };

    lijsten.value = await promiseAll(promises);
  } catch {
    return;
  }
};
