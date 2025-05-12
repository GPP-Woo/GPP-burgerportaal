<template>
  <div class="gpp-woo-tile-carousel">
    <p class="visually-hidden" aria-live="polite">
      {{ `Slide ${normalizedIndex + 1} van ${tiles.length}` }}
    </p>

    <ul ref="scrollContainer" class="gpp-woo-slides">
      <li
        v-for="(item, index) in infiniteItems"
        :key="`slide-${index}`"
        :ref="index === 0 ? `firstItem` : undefined"
        class="gpp-woo-slides__slide"
        :aria-hidden="!isItemVisible(index)"
      >
        <gpp-woo-tile v-bind="{ ...item, maxDescriptionLength: tileDescriptionLength }" />
      </li>
    </ul>

    <menu class="gpp-woo-slide-menu">
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
          :aria-pressed="autoplayEnabled"
          appearance="primary-action-button"
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

    <div class="gpp-woo-slide-indicators" role="tablist">
      <button
        v-for="index in tiles.length"
        :key="`indicator-${index}`"
        role="tab"
        class="gpp-woo-slide-indicators__indicator"
        :class="{ 'gpp-woo-slide-indicators__indicator--active': normalizedIndex === index - 1 }"
        :aria-selected="normalizedIndex === index - 1"
        :aria-controls="`slide-${tiles.length + index - 1}`"
        @click="scrollToIndex(index - 1, false)"
      >
        <span class="visually-hidden">Ga naar slide {{ index }}</span>
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useCarousel } from "@/composables/use-carousel";
import GppWooTile, { type Tile } from "@/components/gpp-woo-tiles/GppWooTile.vue";

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
  isItemVisible,
  scrollToIndex,
  scrollPrev,
  scrollNext,
  toggleAutoplay
} = useCarousel(tiles);
</script>

<style lang="scss" scoped>
@use "./tile-config";

.gpp-woo-tile-carousel {
  overflow: hidden;
}

.gpp-woo-slides {
  @include tile-config.reset-list();
  display: flex;
  column-gap: var(--gpp-woo-tile-grid-grid-gap);
  overflow-x: scroll;
  scroll-snap-type: x mandatory;
  scrollbar-width: none;

  @include tile-config.tiles-per-row();

  &__slide {
    flex: 0 0
      calc(
        (100% - (var(--tiles-per-row, 1) - 1) * var(--gpp-woo-tile-grid-grid-gap)) /
          var(--tiles-per-row, 1)
      );
    scroll-snap-align: start;
  }
}

.gpp-woo-slide-menu {
  @include tile-config.reset-list();
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-block-start: 1rem;
}

.gpp-woo-slide-indicators {
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
