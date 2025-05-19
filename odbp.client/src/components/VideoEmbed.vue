<template>
  <iframe
    :src="url"
    :title="title"
    allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
    referrerpolicy="strict-origin-when-cross-origin"
    allowfullscreen
  ></iframe>
</template>

<script setup lang="ts">
defineProps<{ title: string; url: string }>();
</script>

<style scoped>
iframe {
  inline-size: 100%;
  aspect-ratio: 16/9;
  border: none;
}
</style>
