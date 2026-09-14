export const formatFileSize = (bytes?: number) => {
  if (!bytes) return;

  const megabytes = bytes / 1024 / 1024;
  if (megabytes >= 1)
    return `${Intl.NumberFormat("nl-NL", { maximumFractionDigits: 1 }).format(megabytes)} MB`;

  return `${Math.floor(bytes / 1024)} kB`;
};
