export async function promiseAll<T extends Record<string, Promise<unknown>>>(
  promises: T
): Promise<{ [K in keyof T]: Awaited<T[K]> }> {
  const entries = await Promise.all(
    Object.entries(promises).map(([key, promise]) => promise.then((value) => [key, value]))
  );

  return Object.fromEntries(entries);
}
