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
      ref="tooltipRef"
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
import { ref, useId } from "vue";
import { onKeyStroke } from "@vueuse/core";
import UtrechtIcon from "@/components/UtrechtIcon.vue";

const tooltipId = useId();

const tooltipRef = ref<HTMLElement>();

onKeyStroke("Tab", () => tooltipRef.value?.hidePopover());
</script>

<style lang="scss" scoped>
.utrecht-tooltip-anchor {
  --utrecht-icon-size: 1em;
}

.utrecht-tooltip {
  --utrecht-tooltip-background-color: var(--utrecht-spotlight-section-info-background-color);
  --utrecht-tooltip-max-inline-size: 40rem;

  position: fixed;
  inset-block-start: 50dvh;
  inset-inline-start: 50vw;
  transform: translate(-50%, -50%);
  inline-size: 90%;
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
  block-size: calc(var(--utrecht-document-line-height) * 1rem);
  inline-size: calc(var(--utrecht-document-line-height) * 1rem);
  padding: 0;
  border: none;
  border-radius: 50%;
  color: var(--utrecht-button-primary-action-color);
  background-color: var(--utrecht-button-primary-action-background-color);
  cursor: pointer;

  &:focus-visible {
    outline-color: var(--utrecht-focus-outline-color);
    outline-offset: var(--utrecht-focus-outline-offset);
    outline-style: var(--utrecht-focus-outline-style);
    outline-width: var(--utrecht-focus-outline-width);
  }
}
</style>
