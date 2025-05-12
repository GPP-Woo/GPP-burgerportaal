<template>
  <div class="gpp-woo-tile-carousel">
    <p class="visually-hidden" aria-live="polite">
      {{ `Slide ${normalizedIndex + 1} van ${tiles.length}` }}
    </p>

    <ul ref="scrollContainer" class="gpp-woo-slides">
      <li
        v-for="(tile, index) in infiniteItems"
        :key="`slide-${index}`"
        :ref="index === 0 ? `firstItem` : undefined"
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
import GppWooTile, { type Tile } from "@/components/gpp-woo-tiles/GppWooTile.vue";
import { useCarousel } from "@/components/gpp-woo-tiles/use-carousel";

const { tiles, tileDescriptionLength = 150 } = defineProps<{
  tiles: Tile[];
  tileDescriptionLength?: number;
}>();

const {
  scrollContainer,
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  firstItem,
  infiniteItems,
  normalizedIndex,
  autoplayEnabled,
  scrollToIndex,
  scrollPrev,
  scrollNext,
  toggleAutoplay
} = useCarousel(tiles);
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
