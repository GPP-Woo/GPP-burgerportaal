let baseTitle = "";

// Call once the portal title is known
export const initBaseTitle = () => {
  baseTitle = document.title;
};

export const setPageTitle = (pageTitle?: string) => {
  document.title = pageTitle ? `${pageTitle} | ${baseTitle}` : baseTitle;
};
