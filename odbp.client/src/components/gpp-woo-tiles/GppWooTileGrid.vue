<template>
  <ul class="gpp-woo-tile-grid">
    <li v-for="(tile, index) in tiles" :key="index">
      <gpp-woo-tile v-bind="{ ...tile, maxDescriptionLength }" />
    </li>
  </ul>
</template>

<script setup lang="ts">
import GppWooTile, { type Tile } from "@/components/gpp-woo-tiles/GppWooTile.vue";

defineProps<{ tiles: Tile[]; maxDescriptionLength?: number }>();
</script>

<style lang="scss" scoped>
@use "./tile-config";

.gpp-woo-tile-grid {
  @include tile-config.reset-list();
  display: grid;
  grid-template-columns: repeat(var(--tiles-per-row, 1), 1fr);
  grid-gap: var(--gpp-woo-tile-grid-grid-gap);

  @include tile-config.tiles-per-row();
}
</style>
