<template>
  <utrecht-heading :level="1" :id="headingId">{{ uuid }}</utrecht-heading>

  <utrecht-table :aria-labelledby="headingId">
    <utrecht-table-header class="utrecht-table__header--hidden">
      <utrecht-table-row>
        <utrecht-table-header-cell scope="col">Onderwerpkenmerk</utrecht-table-header-cell>
        <utrecht-table-header-cell scope="col">Onderwerpkenmerkwaarde</utrecht-table-header-cell>
      </utrecht-table-row>
    </utrecht-table-header>

    <utrecht-table-body>
      <utrecht-table-row v-for="[key, value] in publicatieRows" :key="key">
        <template v-if="value?.length">
          <utrecht-table-header-cell scope="row">{{ key }}</utrecht-table-header-cell>
          <utrecht-table-cell>{{ value }}</utrecht-table-cell>
        </template>
      </utrecht-table-row>
    </utrecht-table-body>
  </utrecht-table>

  <search-grid :onderwerp="uuid" />
</template>

<script setup lang="ts">
import { computed, useId } from "vue";
import SearchGrid from "@/features/search/SearchGrid.vue";
import { formatDate } from "@/helpers";

defineProps<{ uuid: string }>();

const headingId = useId();

const publicatieRows = computed(
  () =>
    new Map<string, string | undefined>([
      [
        "Omschrijving",
        `Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. 
        Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. 
        Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. 
        Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.`
      ],
      ["Geregistreerd op", formatDate(`2025-03-12T16:23:57.37167+01:00`)],
      ["Laatst gewijzigd op", formatDate(`2025-03-12T16:23:57.371694+01:00`)]
    ])
);
</script>

<style lang="scss" scoped>
th[scope="row"] {
  inline-size: 20ch;
}

.gpp-woo-search {
  margin-block-start: 2rem;
}
</style>
