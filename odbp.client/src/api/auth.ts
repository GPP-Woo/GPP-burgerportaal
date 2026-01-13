export type AuthUser = {
  isLoggedIn: boolean;
  id: string;
  fullName: string;
  roles: string[];
  isAdmin: boolean;
};

export const fetchAuthUser = (): Promise<AuthUser | null> =>
  fetch("/api/me", { headers: { "is-api": "true" } }).then((r) => (r.ok ? r.json() : null));
