<template>
  <section class="gpp-woo-hero">
    <div class="utrecht-page utrecht-page-content">
      <img
        v-if="resources?.imageUrl"
        :src="resources.imageUrl"
        class="gpp-woo-hero-image"
        alt="Afbeelding gemeente"
        crossorigin="anonymous"
      />

      <div class="gpp-woo-card">
        <div class="gpp-woo-card-content">
          <span class="utrecht-heading-2"
            >{{ resources?.title ? `${resources.title}, ` : `` }}zoek hier in openbaar gemaakte
            informatie{{ resources?.name ? ` van ${resources.name}` : `` }}</span
          >

          <form v-if="route.name === 'home'" class="utrecht-form" @submit.prevent.stop="submit">
            <utrecht-fieldset class="zoeken">
              <utrecht-legend class="visually-hidden">Zoeken</utrecht-legend>

              <search-bar v-model="query" @submit="submit" />
            </utrecht-fieldset>
          </form>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref, watch } from "vue";
import { useRoute, useRouter } from "vue-router";
import { injectResources } from "@/resources";
import SearchBar from "./SearchBar.vue";

const resources = injectResources();

const route = useRoute();
const router = useRouter();

const query = ref("");

const submit = () => router.push({ name: "zoeken", query: { query: query.value } });

watch(
  () => route.path,
  () => (query.value = "")
);
</script>

<style lang="scss" scoped>
.gpp-woo-hero {
  position: relative;

  .gpp-woo-hero-image {
    position: absolute;
    inset: 0;
    width: 100%;
    height: 100%;
    object-fit: cover;
  }

  .gpp-woo-card {
    position: relative;
  }
}

.gpp-woo-card {
  inline-size: min(100%, var(--gpp-woo-card-max-inline-size));
  background-color: var(--gpp-woo-card-background-color);

  .gpp-woo-card-content {
    padding-block: var(--gpp-woo-card-content-padding-block);
    padding-inline: var(--gpp-woo-card-content-padding-inline);
  }
}
</style>
