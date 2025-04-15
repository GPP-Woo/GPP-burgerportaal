<template>
  <onderwerp-details :uuid="uuid" />

  <search-grid :onderwerp="uuid" />
</template>

<script setup lang="ts">
import OnderwerpDetails from "@/features/onderwerp/OnderwerpDetails.vue";
import SearchGrid from "@/features/search/SearchGrid.vue";

defineProps<{ uuid: string }>();
</script>

<style lang="scss" scoped>
.gpp-woo-search {
  margin-block: 2rem;
}
</style>
