<template>
  <the-header />

  <main id="main">
    <gpp-woo-hero />

    <section class="utrecht-page utrecht-page-content">
      <router-view />
    </section>
  </main>

  <the-footer />
</template>

<script setup lang="ts">
import { watch } from "vue";
import { RouterView, useRoute } from "vue-router";
import { injectResources } from "./resources";
import TheHeader from "./components/TheHeader.vue";
import GppWooHero from "./components/GppWooHero.vue";
import TheFooter from "./components/TheFooter.vue";

const route = useRoute();
const resources = injectResources();

watch(
  route,
  async (to) => (document.title = `${to.meta?.title || document.title} | ${resources?.name || ""}`),
  { immediate: true }
);
</script>
