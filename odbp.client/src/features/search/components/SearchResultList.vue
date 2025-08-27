<template>
  <ol class="gpp-woo-search-result-list">
    <li
      v-for="{
        type,
        record: {
          uuid,
          officieleTitel,
          informatieCategorieen,
          publisher,
          publicatiedatum,
          laatstGewijzigdDatum,
          omschrijving
        }
      } in results"
      :key="uuid"
      class="gpp-woo-search-result-list__item"
    >
      <utrecht-article>
        <utrecht-heading :level="3">
          <router-link
            :to="{ name: resultOptions[type].name, params: { uuid } }"
            class="utrecht-link utrecht-link--html-a"
          >
            {{ officieleTitel }}
          </router-link>
        </utrecht-heading>

        <ul class="gpp-woo-meta-data-list">
          <li class="gpp-woo-meta-data-list__item">
            <strong>{{ resultOptions[type].label }}</strong>
          </li>

          <li v-if="publisher" class="gpp-woo-meta-data-list__item">
            {{ publisher.naam }}
          </li>

          <li
            class="gpp-woo-meta-data-list__item gpp-woo-meta-data-list__item--category"
            v-for="categorie in informatieCategorieen"
            :key="categorie.uuid"
          >
            {{ categorie.naam }}
          </li>
        </ul>

        <utrecht-paragraph>{{ truncate(omschrijving, 150) }}</utrecht-paragraph>

        <utrecht-paragraph>
          <time :datetime="publicatiedatum">{{ formatDate(publicatiedatum) }}</time>

          <template
            v-if="laatstGewijzigdDatum?.substring(0, 10) !== publicatiedatum?.substring(0, 10)"
          >
            <span>{{ ", gewijzigd op " }}</span>

            <time :datetime="laatstGewijzigdDatum">{{ formatDate(laatstGewijzigdDatum) }}</time>
          </template>
        </utrecht-paragraph>
      </utrecht-article>
    </li>
  </ol>
</template>

<script setup lang="ts">
import { formatDate, truncate } from "@/helpers";
import { resultOptions, type SearchResponseItem } from "@/features/search/service";

defineProps<{ results: Readonly<SearchResponseItem[]> }>();
</script>

<style lang="scss" scoped>
.gpp-woo-search-result-list {
  --utrecht-heading-3-margin-block-end: var(
    --gpp-woo-search-result-list-heading-3-margin-block-end
  );
  --utrecht-heading-3-margin-block-start: var(
    --gpp-woo-search-result-list-heading-3-margin-block-start
  );

  list-style: none;
  margin: 0;
  padding: 0;

  &__item {
    margin-block: var(--gpp-woo-search-result-list-item-margin-block);
  }
}

.utrecht-paragraph:has(time) {
  font-size: var(--gpp-woo-search-result-list-paragraph-has-time-font-size);
}

.gpp-woo-meta-data-list {
  list-style: none;
  padding: 0;
  margin-block: var(--gpp-woo-meta-data-list-margin-block);

  display: flex;
  align-items: center;
  flex-wrap: wrap;
  column-gap: var(--gpp-woo-meta-data-list-column-gap);
  row-gap: var(--gpp-woo-meta-data-list-row-gap);

  &__item {
    font-size: var(--gpp-woo-meta-data-list-item-font-size);

    &--category {
      border-bottom: var(--gpp-woo-meta-data-list-item-category-border-bottom);
    }
  }
}
</style>
