<template>
  <utrecht-heading :level="3">
    <router-link :to="link" :title="title" class="utrecht-link utrecht-link--html-a">
      {{ title }}
    </router-link>
  </utrecht-heading>

  <utrecht-paragraph v-if="description">{{ description }}</utrecht-paragraph>
</template>

<script setup lang="ts">
withDefaults(
  defineProps<{
    link: string;
    title: string;
    level?: 2 | 3 | 4;
    description?: string;
  }>(),
  {
    level: 3
  }
);
</script>

<style lang="scss" scoped>
[class^="utrecht-heading"] {
  --utrecht-space-around: 1;
}

.utrecht-paragraph {
  --utrecht-space-around: 1;
}
</style>
