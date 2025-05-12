<template>
  <div class="gpp-woo-tile-carousel">
    <p class="visually-hidden" aria-live="polite">
      {{ `Slide ${normalizedIndex + 1} van ${tiles.length}` }}
    </p>

    <ul ref="scrollContainer" class="gpp-woo-slides">
      <li
        v-for="(tile, index) in infiniteTiles"
        :key="`slide-${index}`"
        :ref="!index ? `firstTile` : undefined"
        class="gpp-woo-slides__slide"
      >
        <gpp-woo-tile v-bind="{ ...tile, maxDescriptionLength: tileDescriptionLength }" />
      </li>
    </ul>

    <menu>
      <li>
        <utrecht-button
          @click="scrollPrev"
          aria-label="Vorig item"
          appearance="primary-action-button"
          >⟨</utrecht-button
        >
      </li>

      <li>
        <utrecht-button
          @click="toggleAutoplay"
          :aria-label="autoplayEnabled ? `Pause` : `Start`"
          appearance="primary-action-button"
          :aria-pressed="autoplayEnabled"
        >
          {{ autoplayEnabled ? "⏸ Pause" : "▶ Start" }}
        </utrecht-button>
      </li>

      <li>
        <utrecht-button
          @click="scrollNext"
          aria-label="Volgend item"
          appearance="primary-action-button"
          >⟩</utrecht-button
        >
      </li>
    </menu>

    <div class="gpp-woo-indicators" role="tablist">
      <button
        v-for="index in tiles.length"
        :key="`indicator-${index}`"
        role="tab"
        class="gpp-woo-indicators__indicator"
        :class="{ 'gpp-woo-indicators__indicator--active': normalizedIndex === index - 1 }"
        :aria-selected="normalizedIndex === index - 1"
        :aria-controls="`slide-${index}`"
        @click="scrollToIndex(index - 1, false)"
      >
        <span class="visually-hidden">Ga naar item {{ index }}</span>
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from "vue";
import {
  useScroll,
  useElementSize,
  useResizeObserver,
  useEventListener,
  useTimeoutFn
} from "@vueuse/core";
import GppWooTile, { type Tile } from "@/components/GppWooTile.vue";

const {
  tiles,
  tileDescriptionLength = 150,
  autoplayInterval = 3000,
  autoplayInitialState = true
} = defineProps<{
  tiles: Tile[];
  tileDescriptionLength?: number;
  autoplayInterval?: number;
  autoplayInitialState?: boolean;
}>();

const infiniteTiles = computed(() => [...tiles, ...tiles, ...tiles]);

const scrollContainer = ref<HTMLElement | null>(null);
const { x: scrollLeft } = useScroll(scrollContainer);

const firstTile = ref<HTMLElement | null>(null);
const { width: tileWidth } = useElementSize(firstTile);

const slideWidth = computed(() => {
  if (!scrollContainer.value) return 1;

  return tileWidth.value + parseInt(getComputedStyle(scrollContainer.value).columnGap);
});

const currentIndex = computed(() => Math.round(scrollLeft.value / slideWidth.value));
const normalizedIndex = computed(() => currentIndex.value % tiles.length);

const handleInfiniteScroll = () => {
  if (!scrollContainer.value) return;

  let index = null;

  if (currentIndex.value <= 0) {
    index = currentIndex.value + tiles.length;
  } else if (currentIndex.value >= infiniteTiles.value.length - tiles.length) {
    index = currentIndex.value - tiles.length;
  }

  if (index !== null) {
    scrollContainer.value.style.scrollBehavior = "auto";
    scrollContainer.value.scrollLeft = index * slideWidth.value;
  }
};

useResizeObserver(scrollContainer, () => handleInfiniteScroll());

useEventListener(scrollContainer, "scrollend", handleInfiniteScroll, { passive: true });

const scrollPrev = () => scrollToIndex(currentIndex.value - 1);

const scrollNext = () => scrollToIndex(currentIndex.value + 1);

const scrollToIndex = (index: number, smooth = true) => {
  scrollContainer.value?.scrollTo({
    left: index * slideWidth.value,
    behavior: smooth ? "smooth" : "auto"
  });

  if (autoplayEnabled.value) {
    stopAutoplay();
    startAutoplay();
  }
};
const autoplayEnabled = ref(autoplayInitialState);

const { start: startAutoplay, stop: stopAutoplay } = useTimeoutFn(() => {
  if (!autoplayEnabled.value) return;

  scrollNext();
  startAutoplay();
}, autoplayInterval);

const toggleAutoplay = () => {
  autoplayEnabled.value = !autoplayEnabled.value;

  if (autoplayEnabled.value) {
    startAutoplay();
  } else {
    stopAutoplay();
  }
};

onMounted(() => startAutoplay());

useEventListener(scrollContainer, "mouseenter", stopAutoplay);
useEventListener(scrollContainer, "mouseleave", () => autoplayEnabled.value && startAutoplay());
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
  margin-block-start: 2rem;

  &__indicator {
    block-size: 1rem;
    inline-size: 1rem;
    background-color: #ccc;
    border: none;
    border-radius: 50%;
    cursor: pointer;

    &--active {
      background-color: #000;
    }

    &:focus-visible {
      outline-color: var(--utrecht-focus-outline-color);
      outline-offset: var(--utrecht-focus-outline-offset);
      outline-style: var(--utrecht-focus-outline-style);
      outline-width: var(--utrecht-focus-outline-width);
    }
  }
}
</style>
