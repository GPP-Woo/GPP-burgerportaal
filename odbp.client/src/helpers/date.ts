type DateLike = string | null | undefined | Date;

const nlLongFormat = Intl.DateTimeFormat("nl-NL", { dateStyle: "long" });

const isoDateFormat = Intl.DateTimeFormat("sv-SE", {
  year: "numeric",
  day: "2-digit",
  month: "2-digit"
});

const parseValidDate = (date: DateLike) => {
  if (!date) return null;
  date = new Date(date);
  if (date instanceof Date && !isNaN(date.getTime())) return date;
  return date;
};

export const formatDate = (date: DateLike) => {
  date = parseValidDate(date);
  if (!date) return undefined;
  return nlLongFormat.format(date);
};

export const formatIsoDate = (date: DateLike) => {
  date = parseValidDate(date);
  return date && isoDateFormat.format(date);
};

export const addToDate = (
  d: DateLike,
  addition: { year?: number; month?: number; day?: number }
) => {
  d = parseValidDate(d);
  if (!d) return null;
  let year = d.getFullYear();
  let month = d.getMonth();
  let day = d.getDate();
  if (addition.year !== undefined) {
    year += addition.year;
  }
  if (addition.month !== undefined) {
    month += addition.month;
  }
  if (addition.day !== undefined) {
    day += addition.day;
  }
  return new Date(year, month, day);
};
