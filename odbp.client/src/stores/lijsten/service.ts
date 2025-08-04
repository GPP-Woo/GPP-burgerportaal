import { ref } from "vue";
import { promiseAll } from "@/utils";
import { fetchAllPages } from "@/composables/use-all-pages";
import type { WaardelijstItem } from "./types";
import type { Onderwerp } from "@/features/onderwerp/types";

const fetchLijsten = () =>
  promiseAll({
    organisaties: fetchAllPages<WaardelijstItem>("/api/v2/organisaties"),
    informatiecategorieen: fetchAllPages<WaardelijstItem>("/api/v2/informatiecategorieen"),
    onderwerpen: fetchAllPages<Onderwerp>("/api/v2/onderwerpen").then((r) =>
      r.map((item) => ({
        ...item,
        naam: item.officieleTitel
      }))
    ) as Promise<(Onderwerp & { naam: string })[]>
  });

export const lijsten = ref<Awaited<ReturnType<typeof fetchLijsten>>>();

export const loadLijsten = async () => {
  try {
    lijsten.value = await fetchLijsten();
  } catch {
    return;
  }
};
