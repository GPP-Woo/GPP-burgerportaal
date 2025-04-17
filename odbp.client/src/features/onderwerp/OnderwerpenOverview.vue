<template>
  <utrecht-heading :level="1">Onderwerpen</utrecht-heading>

  <utrecht-paragraph
    >Bekijk alle gepubliceerde documenten die horen bij een belangrijk onderwerp dat speelt in
    {{ resources?.name }}.</utrecht-paragraph
  >

  <section
    v-if="promoted?.length"
    class="utrecht-spotlight-section utrecht-spotlight-section--info"
  >
    <utrecht-heading :level="2">Gepromoot</utrecht-heading>

    <gpp-woo-tile-grid :tiles="promoted" />
  </section>

  <section v-if="onderwerpen?.length" aria-live="polite">
    <utrecht-heading :level="2"
      >Alle onderwerpen ({{ lijsten?.onderwerpen.length }})</utrecht-heading
    >

    <gpp-woo-tile-grid :tiles="onderwerpen" />

    <utrecht-paragraph>
      <span>{{ onderwerpen.length }} van {{ lijsten?.onderwerpen.length }} onderwerpen</span>

      <utrecht-button
        v-if="showButton"
        @click="showMore"
        type="submit"
        :appearance="'primary-action-button'"
        >Toon meer onderwerpen</utrecht-button
      >
    </utrecht-paragraph>
  </section>

  <utrecht-paragraph v-else>Er zijn geen onderwerpen gevonden...</utrecht-paragraph>
</template>

<script setup lang="ts">
import { computed, ref } from "vue";
import { injectResources } from "@/resources";
import GppWooTileGrid from "@/components/GppWooTileGrid.vue";
import type { Tile } from "@/components/GppWooTile.vue";
import { truncate } from "@/helpers";
import { lijsten } from "@/stores/lijsten";
import type { Onderwerp } from "./types";

const resources = injectResources();

const mapOnderwerpToTile = (o: Onderwerp): Tile => ({
  link: `/onderwerpen/${o.uuid}`,
  title: o.officieleTitel,
  description: truncate(o.omschrijving, 300),
  level: 3
});

const promoted = computed(() =>
  lijsten.value?.onderwerpen.filter((o) => o.promoot).map(mapOnderwerpToTile)
);

const currentPage = ref(1);
const onderwerpenPerPage = 9;

const onderwerpen = computed(() =>
  lijsten.value?.onderwerpen
    .slice(0, currentPage.value * onderwerpenPerPage)
    .map(mapOnderwerpToTile)
);

const showMore = () => currentPage.value++;

const showButton = computed(() => onderwerpen.value?.length !== lijsten.value?.onderwerpen.length);
</script>

<style lang="scss" scoped>
.utrecht-spotlight-section {
  --utrecht-spotlight-section-padding-inline-start: var(--utrecht-page-margin-inline-start);
  --utrecht-spotlight-section-padding-inline-end: var(--utrecht-page-margin-inline-end);
  --utrecht-spotlight-section-padding-block-start: var(--utrecht-page-padding-block-start);
  --utrecht-spotlight-section-padding-block-end: var(
    --utrecht-spotlight-section-padding-block-start
  );

  margin-inline-end: calc(-1 * var(--utrecht-page-margin-inline-end));
  margin-inline-start: calc(-1 * var(--utrecht-page-margin-inline-start));

  .utrecht-heading-2 {
    --utrecht-heading-2-margin-block-start: 0;
  }
}

.utrecht-paragraph {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
</style>
