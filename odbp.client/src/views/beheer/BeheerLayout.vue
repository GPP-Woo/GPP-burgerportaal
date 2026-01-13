<template>
  <the-header>
    <template #nav-bar>
      <nav class="utrecht-nav-bar" aria-label="Beheermenu">
        <div class="utrecht-nav-bar__content">
          <ul v-if="!loading" role="list" class="utrecht-nav-list" id="menu">
            <li class="utrecht-nav-list__item">
              <span v-if="user" class="utrecht-nav-list__link">
                {{ user.fullName }}
              </span>
            </li>

            <li class="utrecht-nav-list__item">
              <a href="/api/logoff" class="utrecht-nav-list__link utrecht-link utrecht-link--html-a"
                >Uitloggen</a
              >
            </li>
          </ul>
        </div>
      </nav>
    </template>
  </the-header>

  <section class="utrecht-page utrecht-page-content">
    <router-view />
  </section>

  <the-footer />
</template>

<script setup lang="ts">
import TheHeader from "@/components/TheHeader.vue";
import TheFooter from "@/components/TheFooter.vue";
import { useLoader } from "@/composables/use-loader";
import { fetchAuthUser, type AuthUser } from "@/api/auth";

const { data: user, loading } = useLoader<AuthUser | null>(() => fetchAuthUser());
</script>

<style lang="scss" scoped></style>
