<template>
  <div class="gpp-woo-tile-carousel">
    <ul
      ref="scrollContainer"
      class="gpp-woo-slides"
      :style="`--_slides-per-view: ${slidesPerView}`"
    >
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
          v-for="index in totalVisibleSets"
          :key="`indicator-${index}`"
          role="tab"
          class="gpp-woo-indicators__indicator"
          :class="{ 'gpp-woo-indicators__indicator--active': currentSet === index - 1 }"
          :aria-selected="currentSet === index - 1"
          :aria-controls="`slide-${index}`"
          @click="scrollToSet(index - 1)"
        >
          <span class="visually-hidden">Ga naar item {{ index + 1 }}</span>
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
import { useElementSize, useScroll, useEventListener } from "@vueuse/core";
import GppWooTile, { type Tile } from "@/components/GppWooTile.vue";

const { tiles } = defineProps<{ tiles: Tile[] }>();

const slidesToScroll = 1;
const currentIndex = ref(0);

const scrollContainer = ref<HTMLElement | null>(null);
const { width: containerWidth } = useElementSize(scrollContainer);
const { x: scrollLeft } = useScroll(scrollContainer);

const slidesPerView = computed(() => {
  if (!scrollContainer.value) return 1;

  const firstSlide = scrollContainer.value.querySelector("li");

  if (!firstSlide) return 1;

  scrollContainer.value.classList.add("gpp-woo-slides--grid");

  const slideWidth = firstSlide.getBoundingClientRect().width;

  scrollContainer.value.classList.remove("gpp-woo-slides--grid");

  return Math.floor(containerWidth.value / slideWidth);
});

const slideWidth = computed(() => containerWidth.value / slidesPerView.value);

const totalVisibleSets = computed(() => {
  if (tiles.length <= slidesPerView.value) return 1;

  return Math.ceil((tiles.length - slidesPerView.value) / slidesToScroll) + 1;
});

const currentSet = computed(() =>
  Math.round(scrollLeft.value / (slideWidth.value * slidesToScroll))
);

const isStart = computed(() => currentIndex.value <= 0);

const isEnd = computed(() => currentIndex.value >= tiles.length - slidesPerView.value);

const updateCurrentIndex = () => {
  const index = Math.round(scrollLeft.value / slideWidth.value);

  if (index >= 0 && index <= tiles.length - slidesPerView.value) {
    currentIndex.value = index;
  }
};

useEventListener(scrollContainer, "scroll", updateCurrentIndex, { passive: true });

const scrollToIndex = (index: number) => {
  if (!scrollContainer.value) return;

  const safeIndex = Math.max(0, Math.min(index, tiles.length - slidesPerView.value));

  const targetPosition = safeIndex * slideWidth.value;

  scrollContainer.value.scrollTo({
    left: targetPosition,
    behavior: "smooth"
  });

  currentIndex.value = safeIndex;
};

const scrollToSet = (setIndex: number) => {
  const targetIndex = setIndex * slidesToScroll;

  scrollToIndex(targetIndex);
};

const scrollPrev = () => scrollToIndex(currentIndex.value - slidesToScroll);

const scrollNext = () => scrollToIndex(currentIndex.value + slidesToScroll);
</script>

<style lang="scss" scoped>
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
  --_slides-per-view: 1;
  --_grid-column-width: calc(
    (
        var(--utrecht-page-max-inline-size-calc) - (var(--gpp-woo-tile-grid-max-columns) - 1) *
          var(--gpp-woo-tile-grid-grid-gap)
      ) /
      var(--gpp-woo-tile-grid-max-columns)
  );

  display: flex;
  column-gap: var(--gpp-woo-tile-grid-grid-gap);
  overflow-x: scroll;
  scroll-snap-type: x mandatory;
  scrollbar-width: none;

  &--grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(var(--_grid-column-width), 1fr));
    grid-gap: var(--gpp-woo-tile-grid-grid-gap);
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
