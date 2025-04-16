<template>
  <utrecht-heading :level="1">Onderwerpen</utrecht-heading>

  <utrecht-heading :level="2">Gepromoot</utrecht-heading>

  <ul>
    <li v-for="{ uuid, officieleTitel, omschrijving } in promoted" :key="uuid">
      <gpp-woo-tile
        :link="`/onderwerpen/${uuid}`"
        :title="officieleTitel"
        :description="omschrijving"
      />
    </li>
  </ul>

  <utrecht-heading :level="2">Alle onderwerpen</utrecht-heading>

  <ul>
    <li v-for="{ uuid, officieleTitel, omschrijving } in lijsten?.onderwerpen" :key="uuid">
      <gpp-woo-tile
        :link="`/onderwerpen/${uuid}`"
        :title="officieleTitel"
        :description="omschrijving"
      />
    </li>
  </ul>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { lijsten } from "@/stores/lijsten";
import GppWooTile from "@/components/GppWooTile.vue";

const promoted = computed(() => lijsten.value?.onderwerpen.filter((o) => o.promoot));
</script>

<style lang="scss" scoped>
ul {
  --_max-columns: 3;
  --_grid-gap: 2rem;
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
