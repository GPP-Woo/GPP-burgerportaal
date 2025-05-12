<template>
  <article v-html="html" class="utrecht-article"></article>

  <utrecht-spotlight-section v-if="promoted?.length">
    <utrecht-heading :level="2">Onderwerpen</utrecht-heading>

    <gpp-woo-tile-carousel :tiles="promoted"> </gpp-woo-tile-carousel>
  </utrecht-spotlight-section>
</template>

<script lang="ts" setup>
import { computed } from "vue";
import { injectResources } from "@/resources";
import { sanitizeHtml } from "@/helpers";
import UtrechtSpotlightSection from "@/components/UtrechtSpotlightSection.vue";
import GppWooTileCarousel from "@/components/gpp-woo-tiles/GppWooTileCarousel.vue";
import type { Tile } from "@/components/gpp-woo-tiles/GppWooTile.vue";
import { lijsten } from "@/stores/lijsten";

const resources = injectResources();

const html = computed(() => (resources?.welcome ? sanitizeHtml(resources.welcome) : ""));

const promoted = computed(() =>
  lijsten.value?.onderwerpen
    .filter((o) => o.promoot)
    .map(
      (o): Tile => ({
        link: `/onderwerpen/${o.uuid}`,
        title: o.officieleTitel,
        description: o.omschrijving,
        image: o.afbeelding,
        level: 3
      })
    )
);
</script>

<style lang="scss" scoped>
.utrecht-spotlight-section {
  --utrecht-heading-2-margin-block-start: 0;
  --utrecht-heading-2-margin-block-end: calc(var(--gpp-woo-tile-grid-grid-gap) / 4);
}
</style>
