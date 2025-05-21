<template>
  <article class="utrecht-article">
    <video-embed
      v-if="resources?.videoUrl"
      :url="resources?.videoUrl"
      title="Burgerportaal instructievideo"
    />

    <div v-html="html"></div>
  </article>

  <utrecht-spotlight-section v-if="promoted?.length">
    <utrecht-heading :level="2">Onderwerpen</utrecht-heading>

    <gpp-woo-tile-carousel :tiles="promoted"> </gpp-woo-tile-carousel>
  </utrecht-spotlight-section>
</template>

<script lang="ts" setup>
import { computed } from "vue";
import { injectResources } from "@/resources";
import { sanitizeHtml } from "@/helpers";
import VideoEmbed from "@/components/VideoEmbed.vue";
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
@use "@/assets/variables";

.utrecht-spotlight-section {
  --utrecht-heading-2-margin-block-start: 0;
  --utrecht-heading-2-margin-block-end: calc(var(--gpp-woo-tile-grid-grid-gap) / 4);
}

iframe {
  float: right;
  margin-block: 0.5rem;
  margin-inline-start: 2rem;

  @media screen and (min-width: #{variables.$breakpoint-md}) {
    max-inline-size: 20rem;
  }
}
</style>
