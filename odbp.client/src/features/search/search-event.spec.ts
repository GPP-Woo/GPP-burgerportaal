import { describe, expect, test } from "vitest";
import { toSearchEvent } from "./search-event";
import type { Lijsten } from "@/stores/lijsten";

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

const lists = {
  organisaties: [{ uuid: "u-1", naam: "Gemeente" }],
  informatiecategorieen: [],
  onderwerpen: []
} as unknown as Lijsten;

describe("toSearchEvent", () => {
  test("empty search page is no search", () =>
    expect(toSearchEvent(empty, lists, 5)).toBeUndefined());

  test("term only", () =>
    expect(toSearchEvent({ ...empty, query: " klimaat " }, lists, 5)).toEqual({
      term: "klimaat",
      filters: {},
      count: 5
    }));

  test("filters reported by name instead of id, unresolvable ones left out", () =>
    expect(
      toSearchEvent(
        { ...empty, resultTypes: ["topic", "onbekend"], publishers: ["u-1", "x-1"] },
        lists,
        2
      )
    ).toEqual({
      term: "",
      filters: { resultTypes: ["Onderwerp"], publishers: ["Gemeente"] },
      count: 2
    }));

  test("filter-only search with unresolvable filters is still a search", () =>
    expect(toSearchEvent({ ...empty, publishers: ["u-1"] }, undefined, 0)).toEqual({
      term: "",
      filters: {},
      count: 0
    }));

  test("date filters are normalized, invalid values are omitted", () =>
    expect(
      toSearchEvent(
        {
          ...empty,
          query: "beleid",
          gepubliceerdOpVanaf: "2024-03-05",
          gepubliceerdOpTot: "jan@example.com"
        },
        lists,
        1
      )
    ).toEqual({
      term: "beleid",
      filters: { gepubliceerdOpVanaf: ["2024-03-05"] },
      count: 1
    }));
});
