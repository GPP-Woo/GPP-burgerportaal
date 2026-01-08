<template>
  <div class="beheer-layout">
    <header>
      <h1>GPP-Woo Burgerportaal (beheer)</h1>

      <nav>
        <ul role="list">
          <li>
            <span v-if="user?.fullName">
              {{ user.fullName }}
            </span>
          </li>

          <li>
            <a href="/api/logoff" title="Uitloggen">Uitloggen</a>
          </li>
        </ul>
      </nav>
    </header>

    <main>
      <router-view />
    </main>

    <footer>
      <div v-if="versionInfo">
        <span v-if="versionInfo.semanticVersion">Versie: {{ versionInfo.semanticVersion }}</span>
        <span v-if="versionInfo.gitSha">Commit: {{ versionInfo.gitSha.substring(0, 7) }}</span>
      </div>
    </footer>
  </div>
</template>

<script setup lang="ts">
import { useLoader } from "@/composables/use-loader";

type AuthUser = {
  isLoggedIn: boolean;
  id: string;
  fullName: string;
  roles: string[];
  isAdmin: boolean;
};

const { data: user } = useLoader<AuthUser>(() =>
  fetch("/api/me", { headers: { "is-api": "true" } }).then((r) => (r.ok ? r.json() : null))
);

const { data: versionInfo } = useLoader<{ semanticVersion?: string; gitSha?: string }>(() =>
  fetch("/api/environment/version", { headers: { "is-api": "true" } }).then((r) =>
    r.ok ? r.json() : undefined
  )
);
</script>

<style lang="scss" scoped></style>
