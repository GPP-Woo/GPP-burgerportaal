<template>
  <progress class="gpp-woo-progress" :value="loaded" :max="total"> Bezig met laden... </progress>
</template>

<script setup lang="ts">
defineProps<{ loaded?: number; total?: number }>();
</script>

<style lang="scss" scoped>
.gpp-woo-progress {
  block-size: var(--gpp-woo-progress-block-size);
  inline-size: var(--gpp-woo-progress-inline-size);
  border-radius: var(--gpp-woo-progress-border-radius);
  appearance: none;
  border: none;
  overflow: hidden;

  &::-webkit-progress-bar {
    background-color: var(--utrecht-button-disabled-background-color);
  }

  &::-webkit-progress-value {
    background-color: var(--utrecht-button-primary-action-background-color);
  }

  &::-moz-progress-bar {
    background-color: var(--utrecht-button-primary-action-background-color);
  }
}
</style>
