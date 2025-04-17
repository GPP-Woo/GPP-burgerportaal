<template>
  <article v-html="html" class="utrecht-article"></article>

  <section v-if="promoted?.length">
    <utrecht-heading :level="2">Onderwerpen</utrecht-heading>

    <gpp-woo-tile-carousel :tiles="promoted" />
  </section>
</template>

<script lang="ts" setup>
import { computed } from "vue";
import { injectResources } from "@/resources";
import { sanitizeHtml, truncate } from "@/helpers";
import GppWooTileCarousel from "@/components/GppWooTileCarousel.vue";
import type { Tile } from "@/components/GppWooTile.vue";
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
        description: truncate(o.omschrijving, 150),
        level: 3
      })
    )
);
</script>
