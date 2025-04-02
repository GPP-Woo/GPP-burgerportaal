import type { PublicatieStatus } from "@/api";

export type Onderwerp = {
  uuid: string;
  publicaties: string[];
  officieleTitel: string;
  omschrijving: string;
  publicatiestatus: keyof typeof PublicatieStatus;
  promoot: boolean;
  registratiedatum: string;
  laatstGewijzigdDatum: string;
};
