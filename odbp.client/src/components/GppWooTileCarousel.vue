<template>
  <div class="gpp-woo-tile-carousel">
    <ul ref="scrollContainer" class="gpp-woo-slides">
      <li v-for="(tile, index) in tiles" :key="`slide-${index}`" class="gpp-woo-slides__slide">
        <gpp-woo-tile v-bind="{ ...tile, maxDescriptionLength: 200 }" />
      </li>
    </ul>

    <menu>
      <li>
        <utrecht-button
          @click="scrollPrev"
          aria-label="Volgend item"
          appearance="primary-action-button"
          :disabled="isStart"
          >⟨</utrecht-button
        >
      </li>

      <li class="gpp-woo-indicators" role="tablist">
        <button
          v-for="index in totalSets"
          :key="`indicator-${index}`"
          role="tab"
          class="gpp-woo-indicators__indicator"
          :class="{ 'gpp-woo-indicators__indicator--active': currentSet === index - 1 }"
          :aria-selected="currentSet === index - 1"
          :aria-controls="`slide-${index}`"
          @click="scrollToIndex(index - 1)"
        >
          <span class="visually-hidden">Ga naar item {{ index }}</span>
        </button>
      </li>

      <li>
        <utrecht-button
          @click="scrollNext"
          aria-label="Vorig item"
          appearance="primary-action-button"
          :disabled="isEnd"
          >⟩</utrecht-button
        >
      </li>
    </menu>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from "vue";
import { useElementSize, useScroll, useEventListener, useResizeObserver } from "@vueuse/core";
import GppWooTile, { type Tile } from "@/components/GppWooTile.vue";

const { tiles } = defineProps<{ tiles: Tile[] }>();

const scrollContainer = ref<HTMLElement | null>(null);
const { width: containerWidth } = useElementSize(scrollContainer);
const { x: scrollLeft } = useScroll(scrollContainer);

const slidesPerView = ref(1);
const slideWidth = computed(() => containerWidth.value / slidesPerView.value);

const currentIndex = ref(0);

const updateSlidesPerView = () => {
  if (!scrollContainer.value) return;

  slidesPerView.value = +getComputedStyle(scrollContainer.value).getPropertyValue(
    "--_slides-per-view"
  );
};

useResizeObserver(scrollContainer, () => updateSlidesPerView());

const updateCurrentIndex = () => {
  currentIndex.value = Math.round(scrollLeft.value / slideWidth.value);
};

useEventListener(scrollContainer, "scroll", updateCurrentIndex, { passive: true });

const totalSets = computed(() =>
  tiles.length > slidesPerView.value ? tiles.length - slidesPerView.value + 1 : 1
);

const currentSet = computed(() => Math.round(scrollLeft.value / slideWidth.value));

const isStart = computed(() => currentIndex.value <= 0);

const isEnd = computed(() => currentIndex.value >= tiles.length - slidesPerView.value);

const scrollToIndex = (index: number) => {
  scrollContainer.value?.scrollTo({
    left: index * slideWidth.value,
    behavior: "smooth"
  });
};

const scrollPrev = () => scrollToIndex(currentIndex.value - 1);

const scrollNext = () => scrollToIndex(currentIndex.value + 1);
</script>

<style lang="scss" scoped>
@use "@/assets/variables";
ul,
menu {
  list-style: none;
  padding: 0;
  margin: 0;
}

.gpp-woo-tile-carousel {
  overflow: hidden;
}

.gpp-woo-slides {
  display: flex;
  column-gap: var(--gpp-woo-tile-grid-grid-gap);
  overflow-x: scroll;
  scroll-snap-type: x mandatory;
  scrollbar-width: none;

  --_slides-per-view: 1;

  @media screen and (min-width: #{variables.$breakpoint-md}) {
    --_slides-per-view: 2;
  }

  @media screen and (min-width: #{variables.$breakpoint-lg}) {
    --_slides-per-view: 3;
  }

  &__slide {
    flex: 0 0
      calc(
        (100% - (var(--_slides-per-view) - 1) * var(--gpp-woo-tile-grid-grid-gap)) /
          var(--_slides-per-view)
      );
    scroll-snap-align: start;
  }
}

menu {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-block-start: 1rem;
}

.gpp-woo-indicators {
  display: flex;
  justify-content: center;
  column-gap: 1rem;

  &__indicator {
    block-size: 1rem;
    inline-size: 1rem;
    background-color: #ccc;
    border: none;
    cursor: pointer;

    &--active {
      background-color: #000;
    }
  }
}
</style>
