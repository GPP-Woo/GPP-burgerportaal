export const truncate = (s?: string, ch?: number) => {
  if (!s) return;
  if (!ch || s.length <= ch) return s;
  return s.substring(0, ch) + "...";
};
