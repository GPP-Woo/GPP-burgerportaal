<template>
  <section
    class="utrecht-spotlight-section"
    :class="{
      'utrecht-spotlight-section--info': type === 'info'
    }"
  >
    <slot></slot>
  </section>
</template>

<style lang="scss" scoped>
.utrecht-spotlight-section {
  --utrecht-spotlight-section-padding-inline-start: var(--utrecht-page-margin-inline-start);
  --utrecht-spotlight-section-padding-inline-end: var(--utrecht-page-margin-inline-end);
  --utrecht-spotlight-section-padding-block-start: var(--utrecht-page-padding-block-start);
  --utrecht-spotlight-section-padding-block-end: var(
    --utrecht-spotlight-section-padding-block-start
  );

  margin-inline-end: calc(-1 * var(--utrecht-page-margin-inline-end));
  margin-inline-start: calc(-1 * var(--utrecht-page-margin-inline-start));
}
</style>

<script setup lang="ts">
defineProps<{ type?: "info" }>();
</script>
