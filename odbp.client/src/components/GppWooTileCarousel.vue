<template>
  <div class="gpp-woo-tile-carousel">
    <ul
      ref="scrollContainer"
      class="gpp-woo-slides"
      :style="{ transform: `translateX(-${currentIndex * 100}%)` }"
      @keydown="handleKeydown"
      tabindex="0"
    >
      <li
        v-for="(tile, index) in tiles"
        class="gpp-woo-slides__slide"
        role="group"
        :key="index"
        :id="`slide-${index}`"
        :aria-hidden="index !== currentIndex"
        :aria-label="`Item ${index + 1} van ${tiles.length}`"
      >
        <gpp-woo-tile v-bind="tile" />
      </li>
    </ul>

    <menu>
      <li>
        <utrecht-button
          @click="prevSlide"
          aria-label="Volgend item"
          appearance="primary-action-button"
          >⟨</utrecht-button
        >
      </li>
      <li>
        <utrecht-button
          @click="toggleAutoPlay"
          :aria-label="autoPlay ? 'Pause' : 'Start'"
          appearance="primary-action-button"
        >
          {{ autoPlay ? "Pause" : "Start" }}
        </utrecht-button>
      </li>
      <li>
        <utrecht-button
          @click="nextSlide"
          aria-label="Vorig item"
          appearance="primary-action-button"
          >⟩</utrecht-button
        >
      </li>
    </menu>

    <div class="gpp-woo-indicators" role="tablist">
      <button
        v-for="(_, index) in tiles"
        :key="index"
        role="tab"
        class="gpp-woo-indicators__indicator"
        :class="{ 'gpp-woo-indicators__indicator--active': index === currentIndex }"
        :aria-selected="index === currentIndex"
        :aria-controls="`slide-${index}`"
        @click="goToSlide(index)"
      >
        <span class="visually-hidden">Ga naar item {{ index + 1 }}</span>
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onBeforeUnmount, onMounted, ref } from "vue";
// import { useElementSize, useEventListener, useScroll, useSwipe } from "@vueuse/core";
import GppWooTile, { type Tile } from "@/components/GppWooTile.vue";

const props = defineProps<{ tiles: Tile[] }>();

const scrollContainer = ref<HTMLElement | null>(null);
const currentIndex = ref(0);
const autoPlay = ref(true);

const nextSlide = () => {
  currentIndex.value = (currentIndex.value + 1) % props.tiles.length;
};

const prevSlide = () => {
  currentIndex.value = (currentIndex.value - 1 + props.tiles.length) % props.tiles.length;
};

const handleKeydown = (event: KeyboardEvent) => {
  switch (event.key) {
    case "ArrowRight":
      nextSlide();
      break;
    case "ArrowLeft":
      prevSlide();
      break;
  }
};

const toggleAutoPlay = () => {
  autoPlay.value = !autoPlay.value;

  manageAutoPlay();
};

let autoPlayInterval: number | undefined;

const manageAutoPlay = () => {
  clearInterval(autoPlayInterval);

  if (autoPlay.value) {
    autoPlayInterval = window.setInterval(nextSlide, 5000);
  }
};

const goToSlide = (index: number) => {
  clearInterval(autoPlayInterval);

  manageAutoPlay();

  currentIndex.value = index;
};

onMounted(() => manageAutoPlay());

onBeforeUnmount(() => clearInterval(autoPlayInterval));

// const { width: containerWidth } = useElementSize(scrollContainer);
// const { x: scrollLeft } = useScroll(scrollContainer);
// const { isSwiping, direction } = useSwipe(scrollContainer);
// useEventListener(scrollContainer, "scroll", updateCurrentIndex, { passive: true });
</script>

<style lang="scss" scoped>
ul,
menu {
  list-style: none;
  padding: 0;
  margin: 0;
}

menu {
  display: flex;
  justify-content: space-between;
  margin-block-start: 1rem;
}

.gpp-woo-tile-carousel {
  overflow: hidden;
}

.gpp-woo-slides {
  display: flex;
  transition: transform 0.5s ease-in-out;

  &__slide {
    min-width: 100%;
  }
}

.gpp-woo-indicators {
  display: flex;
  justify-content: center;
  column-gap: 1rem;
  margin-block-start: 1rem;

  &__indicator {
    background: none;
    border: none;
    cursor: pointer;
    border-bottom: 2px solid #ccc;

    &--active {
      border-bottom: 2px solid #000;
    }
  }
}
</style>
