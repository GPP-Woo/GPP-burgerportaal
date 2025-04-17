<template>
  <utrecht-heading :level="level ?? 3">
    <router-link :to="link" :title="title" class="utrecht-link utrecht-link--html-a">
      {{ title }}
    </router-link>
  </utrecht-heading>

  <utrecht-paragraph v-if="description" class="gpp-woo-pre-wrap">{{
    description
  }}</utrecht-paragraph>
</template>

<script setup lang="ts">
export type Tile = {
  link: string;
  title: string;
  description?: string;
  level?: 2 | 3 | 4;
};

defineProps<Tile>();
</script>

<style lang="scss" scoped>
[class^="utrecht-heading"] {
  --utrecht-space-around: 1;
}

.utrecht-paragraph {
  --utrecht-space-around: 1;
}
</style>
