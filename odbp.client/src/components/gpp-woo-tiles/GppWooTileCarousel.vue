<template>
  <div class="gpp-woo-tile-carousel">
    <!-- This announces the current slide position to screen readers when it changes -->
    <p class="visually-hidden" aria-live="polite">
      {{ `Tegel ${normalizedIndex + 1} van ${tiles.length}` }}
    </p>

    <!-- Main carousel element, uses horizontal scroll with CSS scroll-snap for smooth navigation -->
    <ul ref="scrollContainer" class="gpp-woo-tiles" aria-label="Tegel carrousel">
      <!-- Conditionally render infinite tiles only when needed for scrolling -->
      <!-- When fewer tiles than visible count, render regular tiles to avoid unnecessary DOM elements -->
      <li
        v-for="(tile, index) in tiles.length <= visibleItemsCount ? tiles : infiniteItems"
        :key="`tile-${index}`"
        :id="`tile-${index}`"
        :ref="index === 0 ? `firstItem` : undefined"
        class="gpp-woo-tiles__tile"
        :aria-hidden="!isItemVisible(index)"
      >
        <gpp-woo-tile v-bind="{ ...tile, maxDescriptionLength: tileDescriptionLength }" />
      </li>
    </ul>

    <!-- No tiles available: clear feedback rather than showing an empty carousel -->
    <utrecht-paragraph v-if="tiles.length === 0">Geen onderwerpen gevonden.</utrecht-paragraph>

    <!-- This prevents showing navigation for carousels that don't need it -->
    <template v-if="tiles.length > visibleItemsCount">
      <!-- Indicator dots for direct navigation between tiles -->
      <div class="gpp-woo-indicators" role="tablist">
        <button
          v-for="index in tiles.length"
          :key="`indicator-${index}`"
          role="tab"
          class="gpp-woo-indicators__indicator"
          :class="{ 'gpp-woo-indicators__indicator--active': normalizedIndex === index - 1 }"
          :aria-selected="normalizedIndex === index - 1"
          :aria-controls="`tile-${tiles.length + index - 1}`"
          @click="scrollToIndex(index - 1, false)"
        >
          <span class="visually-hidden">Ga naar tegel {{ index }}</span>
        </button>
      </div>

      <!-- Main carousel navigation controls -->
      <menu class="gpp-woo-menu">
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
            {{ autoplayEnabled ? "⏸ Pauzeren" : "▶ Starten" }}
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
    </template>
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
  visibleItemsCount,
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

.gpp-woo-tiles {
  --_column-gap: var(--gpp-woo-tile-grid-grid-gap, 0rem);

  @include tile-config.reset-list();
  display: flex;
  column-gap: var(--_column-gap);
  overflow-x: scroll;
  scroll-snap-type: x mandatory;
  scrollbar-width: none;

  &:focus-visible {
    outline: none;
  }

  @include tile-config.tiles-per-row();

  &__tile {
    flex: 0 0
      calc((100% - (var(--tiles-per-row, 1) - 1) * var(--_column-gap)) / var(--tiles-per-row, 1));
    scroll-snap-align: start;
  }
}

.gpp-woo-indicators {
  display: flex;
  justify-content: center;
  column-gap: var(--gpp-woo-indicators-column-gap);
  margin-block-start: var(--gpp-woo-indicators-margin-block-start);
  margin-block-end: var(--gpp-woo-indicators-margin-block-end);

  &__indicator {
    block-size: var(--gpp-woo-indicator-block-size);
    inline-size: var(--gpp-woo-indicator-inline-size);
    background-color: var(--gpp-woo-indicator-background-color);
    border-radius: var(--gpp-woo-indicator-border-radius);
    border: none;
    cursor: pointer;

    &--active {
      background-color: var(--gpp-woo-indicator-background-color-active);
    }

    &:focus-visible {
      outline-color: var(--utrecht-focus-outline-color);
      outline-offset: var(--utrecht-focus-outline-offset);
      outline-style: var(--utrecht-focus-outline-style);
      outline-width: var(--utrecht-focus-outline-width);
    }
  }
}

.gpp-woo-menu {
  @include tile-config.reset-list();
  display: flex;
  justify-content: space-between;
  align-items: center;
}
</style>
