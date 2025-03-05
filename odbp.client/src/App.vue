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
import { RouterView } from "vue-router";
import TheHeader from "./components/TheHeader.vue";
import GppWooHero from "./components/GppWooHero.vue";
import TheFooter from "./components/TheFooter.vue";
</script>
