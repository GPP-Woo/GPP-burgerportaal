<template>
  <ul class="utrecht-badge-list">
    <li v-for="(badge, index) in badges" class="utrecht-data-badge" :key="index">{{ badge }}</li>
  </ul>
</template>

<script setup lang="ts">
defineProps<{ badges: string[] }>();
</script>

<style lang="scss" scoped>
.utrecht-badge-list {
  list-style: none;
  margin: 0;
  padding: 0;
}
</style>
