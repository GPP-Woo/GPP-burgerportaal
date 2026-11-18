import { describe, expect, test } from "vitest";
import { toSearchEvent } from "./search-event";
import type { Facets } from "./service";

const empty = {
  query: "",
  gepubliceerdOpVanaf: "",
  gepubliceerdOpTot: "",
  laatstGewijzigdDatumVanaf: "",
  laatstGewijzigdDatumTot: "",
  resultTypes: [],
  publishers: [],
  informatieCategorieen: [],
  onderwerpen: []
};

const facets = {
  resultTypes: [],
  publishers: [{ uuid: "u-1", naam: "Gemeente", count: 1 }],
  informatieCategorieen: [],
  onderwerpen: []
} as unknown as Facets;

describe("toSearchEvent", () => {
  test("empty search page is no search", () =>
    expect(toSearchEvent(empty, facets, 5)).toBeUndefined());

  test("term only", () =>
    expect(toSearchEvent({ ...empty, query: " klimaat " }, facets, 5)).toEqual({
      term: "klimaat",
      filters: {},
      count: 5
    }));

  test("filters only, reported by name instead of id", () =>
    expect(toSearchEvent({ ...empty, publishers: ["u-1", "onbekend"] }, facets, 2)).toEqual({
      term: "",
      filters: { publishers: ["Gemeente"] },
      count: 2
    }));

  test("unknown filter values still count as a search", () =>
    expect(toSearchEvent({ ...empty, publishers: ["onbekend"] }, undefined, 0)).toEqual({
      term: "",
      filters: {},
      count: 0
    }));
});
