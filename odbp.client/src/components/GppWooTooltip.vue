<template>
  <div class="utrecht-tooltip-anchor" @click.prevent>
    <utrecht-button :appearance="'primary-action-button'" @click="isVisible = !isVisible">?</utrecht-button>

    <div role="tooltip" :class="tooltipClasses">
      <slot></slot>

      <span :class="arrowClasses"></span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from "vue";

const isVisible = ref(false);

const POSITION_VALUES = [
  "block-end",
  "block-start",
  "bottom",
  "inline-end",
  "inline-start",
  "left",
  "right",
  "top"
] as const;

type TooltipPosition = (typeof POSITION_VALUES)[number];

const isTooltipPosition = (x: unknown): x is TooltipPosition =>
  POSITION_VALUES.includes(x as TooltipPosition);

const props = defineProps<{ position: TooltipPosition }>();

const tooltipClasses = computed(() => {
  const positionClass = isTooltipPosition(props.position)
    ? `utrecht-tooltip--${props.position}`
    : null;

  const visibleClass = !isVisible.value
    ? "utrecht-tooltip--not-visible"
    : "utrecht-tooltip--visible";

  return ["utrecht-tooltip", positionClass, visibleClass].filter(Boolean);
});

const arrowClasses = computed(() => {
  const positionClass = isTooltipPosition(props.position)
    ? `utrecht-tooltip__arrow--${props.position}`
    : null;

  return ["utrecht-tooltip__arrow", positionClass].filter(Boolean);
});
</script>
