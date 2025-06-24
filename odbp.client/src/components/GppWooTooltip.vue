<template>
  <div class="utrecht-tooltip-anchor">
    <button
      type="button"
      :popovertarget="tooltipId"
      :aria-controls="tooltipId"
      :aria-describedby="tooltipId"
    >
      <utrecht-icon icon="question" />
    </button>

    <div
      popover
      role="tooltip"
      :id="tooltipId"
      class="utrecht-tooltip gpp-woo-pre-wrap"
      @click.prevent
    >
      <slot></slot>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useId } from "vue";
import UtrechtIcon from "@/components/UtrechtIcon.vue";

const tooltipId = useId();
</script>

<style lang="scss" scoped>
.utrecht-tooltip-anchor {
  --utrecht-icon-size: 1em;
}

.utrecht-tooltip {
  top: 50dvh;
  left: 50vw;
  transform: translate(-50%, -50%);
  margin: 0;
  font-weight: var(--utrecht-paragraph-font-weight);
  cursor: text;

  &::backdrop {
    backdrop-filter: blur(1px);
  }
}

button {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  height: calc(var(--utrecht-document-line-height) * 1rem);
  width: calc(var(--utrecht-document-line-height) * 1rem);
  padding: 0;
  border: none;
  border-radius: 50%;
  color: var(--utrecht-button-primary-action-color);
  background-color: var(--utrecht-button-primary-action-background-color);
  cursor: pointer;
}
</style>
