type DateLike = string | null | undefined | Date;

const nlLongFormat = Intl.DateTimeFormat("nl-NL", { dateStyle: "long" });

const parseValidDate = (date: DateLike) => {
  if (!date) return undefined;
  date = new Date(date);

  if (date instanceof Date && !isNaN(date.getTime())) return date;
  return undefined;
};

export const formatDate = (date: DateLike) => {
  date = parseValidDate(date);
  if (!date) return undefined;
  return nlLongFormat.format(date);
};

export const formatIsoDate = (date: DateLike, timeZone?: string) => {
  date = parseValidDate(date);
  if (!date) return undefined;

  const parts = new Intl.DateTimeFormat("nl-NL", {
    timeZone,
    year: "numeric",
    month: "2-digit",
    day: "2-digit"
  }).formatToParts(date);

  const get = (type: "year" | "month" | "day") => parts.find((p) => p.type === type)?.value;

  return [get("year"), get("month"), get("day")].join("-");
};

export const todayIsoDate = () => formatIsoDate(new Date(), "Europe/Amsterdam")!;

export const addToDate = (
  d: DateLike,
  addition: { year?: number; month?: number; day?: number }
) => {
  d = parseValidDate(d);
  if (!d) return undefined;
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
