export type Tracker = {
  track: (path: string, title: string) => void;
  stop?: () => void;
};

// Endpoint names must be appended to the pathname, not to a query string or hash the URL may contain
export const withTrailingSlash = (url: string) => {
  const baseUrl = new URL(url);

  baseUrl.search = "";
  baseUrl.hash = "";
  if (!baseUrl.pathname.endsWith("/")) baseUrl.pathname += "/";

  return baseUrl.href;
};

// Only the path is ever sent: query strings and hashes can contain user input (e.g. search terms)
export const toTrackedUrl = (url: string) => {
  if (!url) return "";

  try {
    const { origin, pathname } = new URL(url, window.location.origin);

    return `${origin}${pathname}`;
  } catch {
    return "";
  }
};

export const loadScript = (src: string) =>
  document.head.appendChild(Object.assign(document.createElement("script"), { async: true, src }));
