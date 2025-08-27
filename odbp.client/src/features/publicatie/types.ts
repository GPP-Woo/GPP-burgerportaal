export const PublicatieStatus = Object.freeze({
  concept: "concept",
  gepubliceerd: "gepubliceerd",
  ingetrokken: "ingetrokken"
});

export type Publicatie = {
  uuid?: string;
  publisher: string;
  verantwoordelijke: string;
  officieleTitel: string;
  verkorteTitel: string;
  omschrijving: string;
  eigenaar?: Eigenaar;
  publicatiestatus: keyof typeof PublicatieStatus;
  gepubliceerdOp?: string;
  laatstGewijzigdDatum?: string;
  datumBeginGeldigheid?: string | null;
  datumEindeGeldigheid?: string | null;
  informatieCategorieen: string[];
  onderwerpen: string[];
  kenmerken: Kenmerk[];
};

export type PublicatieDocument = {
  uuid: string;
  publicatie: string;
  officieleTitel: string;
  verkorteTitel?: string;
  omschrijving?: string;
  publicatiestatus: keyof typeof PublicatieStatus;
  creatiedatum: string;
  gepubliceerdOp?: string;
  laatstGewijzigdDatum?: string;
  ontvangstdatum?: string | null;
  datumOndertekend?: string | null;
  bestandsnaam: string;
  bestandsformaat: string;
  bestandsomvang: number;
  kenmerken: Kenmerk[];
};

type Eigenaar = {
  identifier: string;
  weergaveNaam: string;
};

type Kenmerk = {
  kenmerk: string;
  bron: string;
};
