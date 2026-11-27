const MAX_TERM_LENGTH = 200;

// Order matters: the more specific patterns must run before the 9 digits one
const MASKS: Array<[RegExp, string]> = [
  [/[\w.+-]+@[\w-]+(?:\.[\w-]+)+/g, "[e-mail]"],
  [/(?<![\w])[1-9]\d{3}\s?[a-z]{2}\s*\d+\s?[a-z]?(?![\w])/gi, "[adres]"],
  [
    /(?<![\d+])(?:(?:\+31|0031)[\s-]?(?:\(0\)[\s-]?)?\d(?:[\s-]?\d){8}|0[\s-]?\d(?:[\s-]?\d){8}|\(0\d{2}\)(?:[\s-]?\d){7}|\(0\d{3}\)(?:[\s-]?\d){6})(?!\d)/g,
    "[telefoon]"
  ],
  [/(?<!\d)\d{3}[\s.-]\d{3}[\s.-]\d{3}(?!\d)/g, "[nummer]"],
  [/(?<!\d)\d{9}(?!\d)/g, "[nummer]"]
];

// A term may contain personal data: only the recognised part is replaced
export const maskSearchTerm = (term: string) => {
  const masked = MASKS.reduce((t, [pattern, replacement]) => t.replace(pattern, replacement), term)
    .replace(/\s+/g, " ")
    .trim();

  return masked.slice(0, MAX_TERM_LENGTH);
};

export type SearchEvent = {
  term: string;
  // Names, never internal ids
  filters: Record<string, string[]>;
  count: number;
};
