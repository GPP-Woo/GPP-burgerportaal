<template>
  <figure v-if="svg" v-html="svg" class="gpp-woo-logo"></figure>

  <figure v-else-if="logoUrl" class="gpp-woo-logo">
    <img :src="logoUrl" :alt="`Logo ${organisationName}`" crossorigin="anonymous" />
  </figure>
</template>

<script lang="ts" setup>
import { computed } from "vue";

const props = defineProps<{
  logoUrl?: string;
  organisationName?: string;
}>();

const svg = computed(() => {
  if (!props.logoUrl?.endsWith(`.svg`)) return;

  const svgTemplateId = btoa(props.logoUrl);

  return (document.getElementById(svgTemplateId) as HTMLTemplateElement)?.innerHTML;
});
</script>

<style lang="scss" scoped>
.gpp-woo-logo {
  margin: 0;
  max-inline-size: var(--utrecht-logo-max-inline-size);
  min-inline-size: var(--utrecht-logo-min-inline-size);

  > :deep(img),
  > :deep(svg) {
    display: block;
    max-block-size: var(--utrecht-logo-max-block-size);
    max-inline-size: 100%;
  }

  // override explicit width/height to respect aspect ratio from view-box
  > :deep(svg) {
    block-size: auto;
    inline-size: auto;
  }
}
</style>
