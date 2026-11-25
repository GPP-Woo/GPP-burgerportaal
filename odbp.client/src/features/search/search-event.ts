import type { SearchEvent } from "@/analytics/search";
import { formatIsoDate } from "@/helpers";
import type { Lijsten } from "@/stores/lijsten";
import type { Facets } from "./service";

type Query = {
  query: string;
  gepubliceerdOpVanaf: string;
  gepubliceerdOpTot: string;
  laatstGewijzigdDatumVanaf: string;
  laatstGewijzigdDatumTot: string;
} & Record<keyof Facets, readonly string[]>;

const FACET_KEYS: Array<keyof Facets> = [
  "resultTypes",
  "publishers",
  "informatieCategorieen",
  "onderwerpen"
];

const DATE_KEYS = [
  "gepubliceerdOpVanaf",
  "gepubliceerdOpTot",
  "laatstGewijzigdDatumVanaf",
  "laatstGewijzigdDatumTot"
] as const;

// Result types are filtered by name; the other filters by uuid, resolved via the waardelijsten
const LIJST_KEYS = {
  publishers: "organisaties",
  informatieCategorieen: "informatiecategorieen",
  onderwerpen: "onderwerpen"
} as const;

// Sorting and paging are not part of a search: they never change this event
export const toSearchEvent = (
  query: Query,
  lijsten: Lijsten,
  count: number
): SearchEvent | undefined => {
  const filters: SearchEvent["filters"] = {};

  FACET_KEYS.forEach((key) => {
    // Only names are reported, never raw ids: unresolvable values are left out
    const resolved = query[key].flatMap((value) => {
      const naam =
        key === "resultTypes"
          ? value
          : lijsten?.[LIJST_KEYS[key]].find((item) => item.uuid === value)?.naam;

      return naam ? [naam] : [];
    });

    if (resolved.length) filters[key] = resolved;
  });

  DATE_KEYS.forEach((key) => {
    // Normalized like the search request: arbitrary route strings are never forwarded
    const date = formatIsoDate(query[key]);

    if (date) filters[key] = [date];
  });

  const term = query.query.trim();

  // Opening the empty search page is a pageview, not a search
  const hasFilter =
    FACET_KEYS.some((key) => query[key].length) || DATE_KEYS.some((key) => query[key]);

  if (!term && !hasFilter) return;

  return { term, filters, count };
};
