<template>
  <slot v-if="isPopoverSupported" name="trigger" :trigger-props="triggerProps">
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
    <utrecht-icon icon="info" />

    <slot></slot>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, useId } from "vue";
import { onKeyStroke, useSupported } from "@vueuse/core";
import UtrechtIcon from "@/components/UtrechtIcon.vue";

const tooltipId = useId();

const tooltipRef = ref<HTMLElement>();

const triggerProps = computed(() => ({
  popovertarget: tooltipId,
  "aria-describedby": tooltipId
}));

onKeyStroke("Tab", () => tooltipRef.value?.hidePopover());

// 2025-06-26: popover feature has 'baseline 2024 newly available'
// for now, as this is a non critical feature and not supporting visiters are limited for the site,
// when popover is not supported we hide popover trigger instead of complete fallback
const isPopoverSupported = useSupported(() => "popover" in HTMLElement.prototype);
</script>

<style lang="scss" scoped>
.gpp-woo-info-popover {
  --utrecht-tooltip-background-color: var(--gpp-woo-info-popover-background-color);
  --utrecht-tooltip-max-inline-size: var(--gpp-woo-info-popover-max-inline-size);
  --utrecht-tooltip-padding-inline: var(--gpp-woo-info-popover-padding-inline);

  position: fixed;
  inset-block: 50%;
  inset-inline: 50%;
  transform: translate(-50%, -50%);
  margin: 0;
  box-shadow: var(--gpp-woo-info-popover-box-shadow);

  .utrecht-icon {
    --utrecht-icon-size: var(--gpp-woo-info-popover-icon-size);

    display: block;
    float: left;
    margin-inline-end: var(--utrecht-tooltip-padding-inline);
    margin-block-end: var(--utrecht-tooltip-padding-inline);
  }
}
</style>
