<template>
  <ul>
    <li v-for="(tile, index) in tiles" :key="index">
      <gpp-woo-tile v-bind="tile" />
    </li>
  </ul>
</template>

<script setup lang="ts">
import GppWooTile, { type Tile } from "@/components/GppWooTile.vue";

defineProps<{ tiles: Tile[] }>();
</script>

<style lang="scss" scoped>
ul {
  --_max-columns: var(--gpp-woo-tile-grid-max-columns);
  --_grid-gap: var(--gpp-woo-tile-grid-grid-gap);
  --_grid-column-width: calc(
    (var(--utrecht-page-max-inline-size-calc) - (var(--_max-columns) - 1) * var(--_grid-gap)) /
      var(--_max-columns)
  );

  list-style: none;
  padding: 0;
  margin: 0;
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(var(--_grid-column-width), 1fr));
  grid-gap: var(--_grid-gap);
}
</style>
