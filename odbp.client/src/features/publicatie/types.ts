export type Publicatie = {
  uuid?: string;
  publisher: string;
  officieleTitel: string;
  verkorteTitel: string;
  omschrijving: string;
  gepubliceerdOp?: string;
  laatstGewijzigdDatum?: string;
  datumBeginGeldigheid?: string | null;
  datumEindeGeldigheid?: string | null;
  informatieCategorieen: string[];
  onderwerpen: string[];
  kenmerken: Kenmerk[];
  inzageProcedure?: InzageProcedure | null;
};

export type InzageProcedure = {
  uuid: string;
  urlBekendmaking?: string | null;
  toelichting: string;
  beschikbaarRechtsmiddel: string;
  urlReactieformulier?: string | null;
  datumBeginInzagetermijn: string;
  datumEindeInzagetermijn: string;
};

export type PublicatieDocument = {
  uuid: string;
  publicatie: string;
  officieleTitel: string;
  verkorteTitel?: string;
  omschrijving?: string;
  creatiedatum: string;
  gepubliceerdOp?: string;
  laatstGewijzigdDatum?: string;
  ontvangstdatum?: string | null;
  datumOndertekend?: string | null;
  bestandsnaam: string;
  bestandsomvang: number;
  kenmerken: Kenmerk[];
};

type Kenmerk = {
  kenmerk: string;
  bron: string;
};
