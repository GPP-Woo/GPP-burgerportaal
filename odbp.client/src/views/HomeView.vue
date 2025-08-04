<template>
  <div v-if="resources?.videoUrl" class="gpp-woo-home-video-section">
    <article v-html="html" class="utrecht-article"></article>

    <section>
      <utrecht-heading :level="2">Uitleg Burgerportaal</utrecht-heading>

      <video-embed :url="resources.videoUrl" title="Uitleg Burgerportaal" />
    </section>
  </div>

  <article v-else v-html="html" class="utrecht-article"></article>

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

.gpp-woo-home-video-section {
  --utrecht-heading-2-margin-block-start: calc(
    var(--utrecht-space-around, 0) * var(--utrecht-heading-1-margin-block-start, 0)
  );

  display: grid;
  column-gap: var(--gpp-woo-home-video-section-column-gap);

  @media screen and (min-width: #{variables.$breakpoint-lg}) {
    grid-template-columns: repeat(2, 1fr);
  }

  iframe {
    max-inline-size: var(--utrecht-article-max-inline-size);
    margin-block-start: calc(
      var(--utrecht-space-around, 0) * var(--utrecht-paragraph-margin-block-start, 0)
    );
  }
}

.utrecht-spotlight-section {
  --utrecht-heading-2-margin-block-start: 0;
  --utrecht-heading-2-margin-block-end: calc(var(--gpp-woo-tile-grid-grid-gap) / 4);
}
</style>
