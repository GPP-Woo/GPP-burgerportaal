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
        <router-view />
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
const isBeheer = computed(() => route.matched.some((r) => r.meta.isBeheer));
</script>
