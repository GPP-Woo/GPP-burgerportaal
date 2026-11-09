<template>
  <template v-if="isBeheer">
    <router-view />
  </template>

  <template v-else>
    <the-header>
      <template #nav-bar>
        <utrecht-nav-bar />
      </template>
    </the-header>

    <main id="main">
      <gpp-woo-hero />

      <section class="utrecht-page utrecht-page-content">
        <!-- key on path: fresh component state per page, so each page is tracked separately -->
        <router-view v-slot="{ Component, route: viewRoute }">
          <component :is="Component" :key="viewRoute.path" />
        </router-view>
      </section>
    </main>

    <the-footer />
  </template>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useRoute } from "vue-router";
import TheHeader from "./components/TheHeader.vue";
import UtrechtNavBar from "./components/UtrechtNavBar.vue";
import GppWooHero from "./components/GppWooHero.vue";
import TheFooter from "./components/TheFooter.vue";

const route = useRoute();
const isBeheer = computed(() => route.matched.some((r) => r.name === "beheer"));
</script>
