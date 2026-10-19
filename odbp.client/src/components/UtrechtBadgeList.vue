<template>
  <ul class="utrecht-badge-list">
    <li v-for="({ naam }, index) in badges" :key="index">
      <span class="utrecht-data-badge">{{ naam }}</span>
    </li>
  </ul>
</template>

<script setup lang="ts">
export type BadgeListItem = { naam: string };

defineProps<{ badges: BadgeListItem[] }>();
</script>

<style lang="scss" scoped>
.utrecht-badge-list {
  list-style: none;
  margin: 0;
  padding: 0;

  .utrecht-data-badge {
    display: block;
  }
}
</style>
