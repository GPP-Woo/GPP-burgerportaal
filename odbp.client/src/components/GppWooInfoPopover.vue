<template>
  <div v-if="isPopoverSupported" class="utrecht-tooltip-anchor">
    <slot name="trigger" :trigger-props="triggerProps">
      <button type="button" v-bind="triggerProps">?</button>
    </slot>

    <div
      ref="tooltipRef"
      popover
      role="tooltip"
      :id="tooltipId"
      class="utrecht-tooltip gpp-woo-info-popover"
      @click.prevent
    >
      <slot></slot>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, useId } from "vue";
import { onKeyStroke, useSupported } from "@vueuse/core";

const tooltipId = useId();

const tooltipRef = ref<HTMLElement>();

const triggerProps = computed(() => ({
  popovertarget: tooltipId,
  "aria-describedby": tooltipId
}));

onKeyStroke("Tab", () => tooltipRef.value?.hidePopover());

// 2025-06-26: popover feature has 'baseline 2024 newly available'
// for now, as this is a non critical feature and not supporting visiters are limited for the site,
// when popover is not supported we hide popover instead of complete fallback
const isPopoverSupported = useSupported(() => "popover" in HTMLElement.prototype);
</script>

<style lang="scss" scoped>
@use "@/assets/variables";

.gpp-woo-info-popover {
  --utrecht-tooltip-background-color: var(--gpp-woo-info-popover-background-color);
  --utrecht-tooltip-max-inline-size: var(--gpp-woo-info-popover-max-inline-size);
  --utrecht-tooltip-padding-inline: var(--gpp-woo-info-popover-padding-inline);

  position: fixed;
  inset-block: 50%;
  inset-inline: 50%;
  transform: translate(-50%, -50%);
  margin: var(--gpp-woo-info-popover-margin);
  box-shadow: var(--gpp-woo-info-popover-box-shadow);

  @media (min-width: variables.$breakpoint-md) {
    @supports (position-area: center) {
      inset: auto;
      transform: none;
      inline-size: auto;
      position-area: var(--gpp-woo-info-popover-position-area);
      position-try: flip-block;
    }
  }
}
</style>
