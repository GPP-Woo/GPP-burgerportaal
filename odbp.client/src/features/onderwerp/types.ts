export const PublicatieStatus = Object.freeze({
  concept: "concept",
  gepubliceerd: "gepubliceerd",
  ingetrokken: "ingetrokken"
});

export type Onderwerp = {
  uuid: string;
  publicaties: string[];
  officieleTitel: string;
  omschrijving: string;
  afbeelding: string;
  publicatiestatus: keyof typeof PublicatieStatus;
  promoot: boolean;
  registratiedatum: string;
  laatstGewijzigdDatum: string;
};
