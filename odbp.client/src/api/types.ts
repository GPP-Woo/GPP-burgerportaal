export type PagedResult<T> = Readonly<{
  count: number;
  next?: string;
  previous?: string;
  results: T[];
}>;

export const PublicatieStatus = Object.freeze({
  concept: "concept",
  gepubliceerd: "gepubliceerd",
  ingetrokken: "ingetrokken"
});
