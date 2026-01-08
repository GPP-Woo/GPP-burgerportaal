<template>
  <template v-if="isBeheer">
    <router-view />
  </template>

  <template v-else>
    <the-header />

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
import TheHeader from "./components/TheHeader.vue";
import GppWooHero from "./components/GppWooHero.vue";
import TheFooter from "./components/TheFooter.vue";

const isBeheer = window.location.pathname.startsWith("/beheer");
</script>
