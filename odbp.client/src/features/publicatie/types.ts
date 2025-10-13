export const PublicatieStatus = Object.freeze({
  concept: "concept",
  gepubliceerd: "gepubliceerd",
  ingetrokken: "ingetrokken"
});

export type Publicatie = {
  uuid?: string;
  publisher: string;
  officieleTitel: string;
  verkorteTitel: string;
  omschrijving: string;
  registratiedatum?: string;
  laatstGewijzigdDatum?: string;
  informatieCategorieen: string[];
};

export type PublicatieDocument = {
  uuid: string;
  identifier: string;
  publicatie: string;
  officieleTitel: string;
  verkorteTitel?: string;
  omschrijving?: string;
  registratiedatum?: string;
  laatstGewijzigdDatum?: string;
  bestandsnaam: string;
  bestandsomvang: number;
};
