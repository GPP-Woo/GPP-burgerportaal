import type { SearchEvent } from "@/analytics/search";
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

// Sorting and paging are not part of a search: they never change this event
export const toSearchEvent = (
  query: Query,
  facets: Facets | undefined,
  count: number
): SearchEvent | undefined => {
  const filters: SearchEvent["filters"] = {};

  FACET_KEYS.forEach((key) => {
    // Values can be ids: only names are reported, unknown values are dropped
    const names = query[key]
      .map((value) =>
        facets?.[key]?.find((b) => ("uuid" in b && b.uuid === value) || b.naam === value)
      )
      .flatMap((bucket) => (bucket ? [bucket.naam] : []));

    if (names.length) filters[key] = names;
  });

  DATE_KEYS.forEach((key) => {
    if (query[key]) filters[key] = [query[key]];
  });

  const term = query.query.trim();

  // Opening the empty search page is a pageview, not a search
  const hasFilter =
    FACET_KEYS.some((key) => query[key].length) || DATE_KEYS.some((key) => query[key]);

  if (!term && !hasFilter) return;

  return { term, filters, count };
};
