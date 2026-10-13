<template>
  <ul class="gpp-woo-link-list">
    <li v-for="{ naam, url } in links" :key="naam">
      <router-link :to="url" class="utrecht-link utrecht-link--html-a">{{ naam }}</router-link>
    </li>
  </ul>
</template>

<script setup lang="ts">
export type LinkListItem = { naam: string; url: string };

defineProps<{ links: LinkListItem[] }>();
</script>

<style lang="scss" scoped>
.gpp-woo-link-list {
  list-style: none;
  margin: 0;
  padding: 0;
  display: flex;
  flex-wrap: wrap;
  column-gap: var(--utrecht-space-inline-md);
  row-gap: var(--utrecht-space-block-xs);
}
</style>
