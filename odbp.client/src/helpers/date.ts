type DateLike = string | null | undefined | Date;

const nlLongFormat = Intl.DateTimeFormat("nl-NL", { dateStyle: "long" });

const padZero = (v: string | number, count: number) => {
  v = v.toString();
  while (v.length < count) {
    v = "0" + v;
  }
  return v;
};

const parseValidDate = (date: DateLike) => {
  if (!date) return undefined;
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
  if (!date) return undefined;
  const year = padZero(date.getFullYear(), 4),
    month = padZero(date.getMonth() + 1, 2),
    day = padZero(date.getDate(), 2);
  return [year, month, day].join("-");
};

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
