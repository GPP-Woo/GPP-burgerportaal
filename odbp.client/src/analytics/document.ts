export type DocumentEvent = {
  action: "download" | "view";
  titel: string;
  bestandsnaam: string;
  // Only paths, like every other tracked URL
  documentPath: string;
  publicatiePath: string;
};

export const getFileType = (bestandsnaam: string) =>
  bestandsnaam.includes(".") ? bestandsnaam.split(".").pop()!.toLowerCase() : "";
