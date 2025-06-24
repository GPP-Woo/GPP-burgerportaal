<template>
  <div ref="anchorRef" class="utrecht-tooltip-anchor" @click.prevent>
    <button
      @click="toggle()"
      :aria-controls="tooltipId"
      :aria-describedby="tooltipId"
    >
      <utrecht-icon icon="question" />
    </button>

    <div role="tooltip" :id="tooltipId" :class="tooltipClasses">
      <slot></slot>

      <span :class="arrowClasses"></span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, useId } from "vue";
import { onClickOutside, onKeyStroke, useToggle } from "@vueuse/core";
import UtrechtIcon from "@/components/UtrechtIcon.vue";

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

const props = defineProps<{ position?: TooltipPosition }>();

const anchorRef = ref<HTMLElement>();

const tooltipId = useId();

const [isVisible, toggle] = useToggle(false);

onKeyStroke(["Escape", "Tab"], () => (isVisible.value = false));

onClickOutside(anchorRef, () => (isVisible.value = false));

const tooltipClasses = computed(() => {
  const positionClass = isTooltipPosition(props.position)
    ? `utrecht-tooltip--${props.position}`
    : null;

  const visibleClass = !isVisible.value ? "utrecht-tooltip--not-visible" : null;

  return ["utrecht-tooltip", positionClass, visibleClass, "gpp-woo-pre-wrap"].filter(Boolean);
});

const arrowClasses = computed(() => {
  const positionClass = isTooltipPosition(props.position)
    ? `utrecht-tooltip__arrow--${props.position}`
    : null;

  return ["utrecht-tooltip__arrow", positionClass].filter(Boolean);
});
</script>

<style lang="scss" scoped>
.utrecht-tooltip-anchor {
  --utrecht-icon-size: 1em;
}

.utrecht-tooltip {
  font-weight: var(--utrecht-paragraph-font-weight);
  cursor: text;
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
