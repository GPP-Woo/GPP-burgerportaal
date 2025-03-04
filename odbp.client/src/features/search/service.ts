import { addToDate, formatIsoDate } from "@/helpers";

type SearchResponse = {
  count: number;
  previous: boolean;
  next: boolean;
  results: SearchResponseItem[];
  facets?: Facets;
};

type SearchResponseItem = {
  type: ResultType;
  record: {
    uuid: string;
    officieleTitel: string;
    informatieCategorieen: WaardelijstItem[];
    publisher: WaardelijstItem;
    publicatie: string;
    creatiedatum: string;
    registratiedatum: string;
    laatstGewijzigdDatum: string;
    omschrijving: string;
  };
};

type WaardelijstItem = {
  uuid: string;
  naam: string;
};

type Facets = {
  publishers: Bucket[];
  informatieCategorieen: Bucket[];
};

export type Bucket = {
  uuid: string;
  naam: string;
  count: number;
};

export const sortOptions = {
  relevance: { label: "Relevantie", value: "relevance" },
  chronological: { label: "Chronologisch", value: "chronological" }
} as const;

export const resultOptions = {
  publication: { label: "Publicatie", value: "publication" },
  document: { label: "Document", value: "document" }
} as const;

type ValueOf<T> = T[keyof T];
export type Sort = ValueOf<typeof sortOptions>["value"];
export type ResultType = ValueOf<typeof resultOptions>["value"];

export async function search({
  signal,
  registratiedatumVanaf,
  registratiedatumTot,
  laatstGewijzigdDatumVanaf,
  laatstGewijzigdDatumTot,
  ...body
}: {
  query: string;
  page: number;
  sort: Sort;
  registratiedatumVanaf?: string | null;
  registratiedatumTot?: string | null;
  laatstGewijzigdDatumVanaf?: string | null;
  laatstGewijzigdDatumTot?: string | null;
  publishers?: string[];
  informatieCategorieen?: string[];
  signal?: AbortSignal;
}): Promise<SearchResponse> {
  return fetch("/api/zoeken", {
    body: JSON.stringify({
      ...body,
      registratiedatumVanaf: formatIsoDate(registratiedatumVanaf),
      registratiedatumTot: formatIsoDate(addToDate(registratiedatumTot, { day: 1 })),
      laatstGewijzigdDatumVanaf: formatIsoDate(laatstGewijzigdDatumVanaf),
      laatstGewijzigdDatumTot: formatIsoDate(addToDate(laatstGewijzigdDatumTot, { day: 1 }))
    }),
    method: "POST",
    headers: {
      "content-type": "application/json"
    },
    signal
  }).then((r) => (r.ok ? r.json() : Promise.reject(r.status)));
}
