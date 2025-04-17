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

</style>
