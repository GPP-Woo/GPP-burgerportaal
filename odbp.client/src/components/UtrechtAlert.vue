<template>
  <div
    class="utrecht-alert"
    :class="{
      'utrecht-alert--ok': type === 'ok',
      'utrecht-alert--error': type === 'error'
    }"
  >
    <div class="utrecht-alert__content">
      <div class="utrecht-alert__message" role="alert">
        <slot></slot>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
defineProps<{ type?: "ok" | "error" }>();
</script>
