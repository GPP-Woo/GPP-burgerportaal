<template>
  <div class="gpp-woo-search">
    <utrecht-heading :level="1">Zoeken</utrecht-heading>

    <form class="utrecht-form" @submit.prevent.stop="submit" ref="formElement">
      <search-bar v-model="formFields" @submit="trySubmit" class="gpp-woo-search-bar" />

      <section>
        <search-filters v-model="formFields" :facets="data?.facets" @submit="trySubmit" />
      </section>
    </form>

    <section class="gpp-woo-search-results" aria-live="polite" aria-atomic="true">
      <utrecht-heading :level="2" class="visually-hidden">Zoekresultaat</utrecht-heading>

      <simple-spinner v-if="showSpinner" />

      <utrecht-paragraph v-else-if="error"
        >Er ging iets mis. Probeer het opnieuw.</utrecht-paragraph
      >

      <template v-else-if="data">
        <div v-if="data.results.length">
          <utrecht-paragraph>{{ data.count }} resultaten gevonden</utrecht-paragraph>

          <search-result-list :results="data.results" />

          <utrecht-pagination v-if="pagination" v-bind="pagination" />
        </div>

        <utrecht-paragraph v-else>Geen resultaten gevonden.</utrecht-paragraph>
      </template>
    </section>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import { useRoute, useRouter, type RouteLocationRaw } from "vue-router";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import UtrechtPagination from "@/components/UtrechtPagination.vue";
import SearchBar from "@/features/search/components/SearchBar.vue";
import SearchFilters from "@/features/search/components/SearchFilters.vue";
import SearchResultList from "@/features/search/components/SearchResultList.vue";
import { useLoader } from "@/composables/use-loader";
import { useSpinner } from "@/composables/use-spinner";
import { sortOptions, search, type SearchFormFields } from "@/features/search/service";
import { mapPaginatedResultsToUtrechtPagination } from "@/helpers";

const route = useRoute();
const formElement = ref<HTMLFormElement>();

const first = <T,>(v: T | Array<T>) => (Array.isArray(v) ? v[0] : v);

const array = <T,>(v: T | Array<T> | null | undefined) => {
  if (!v) return;

  return Array.isArray(v)
    ? v.filter((val): val is Exclude<T, null> => val !== "" && val !== null)
    : [v];
};

const parsedQuery = computed(() => ({
  query: first(route.query.query) || "",
  page: +(first(route.query.page) || "1"),
  sort:
    first(route.query.sort) === sortOptions.chronological.value
      ? sortOptions.chronological.value
      : sortOptions.relevance.value,
  registratiedatumVanaf: first(route.query.registratiedatumVanaf) || "",
  registratiedatumTot: first(route.query.registratiedatumTot) || "",
  laatstGewijzigdDatumVanaf: first(route.query.laatstGewijzigdDatumVanaf) || "",
  laatstGewijzigdDatumTot: first(route.query.laatstGewijzigdDatumTot) || "",
  resultTypes: array(route.query.resultTypes) || [],
  publishers: array(route.query.publishers) || [],
  informatieCategorieen: array(route.query.informatieCategorieen) || []
}));

const formFields = ref<SearchFormFields>({
  query: "",
  sort: sortOptions.relevance.value,
  registratiedatumVanaf: "",
  registratiedatumTot: "",
  laatstGewijzigdDatumVanaf: "",
  laatstGewijzigdDatumTot: "",
  resultTypes: [],
  publishers: [],
  informatieCategorieen: []
});

onMounted(() => {
  const keys = Object.keys(formFields.value) as Array<keyof SearchFormFields>;
  const query = parsedQuery.value;

  keys.forEach((key) =>
    Array.isArray(formFields.value[key])
      ? ((formFields.value[key] as string[]) = query[key] as string[])
      : ((formFields.value[key] as string) = query[key] as string)
  );
});

const router = useRouter();

const submit = () =>
  router.push({
    path: route.path,
    query: {
      ...route.query,
      ...formFields.value,
      page: 1
    }
  });

const filterFacets = (
  key: keyof Pick<SearchFormFields, "publishers" | "informatieCategorieen" | "resultTypes">
) =>
  formFields.value[key].filter((nameOrUuid) =>
    data.value?.facets?.[key]?.some(
      (bucket) => ("uuid" in bucket && bucket.uuid === nameOrUuid) || bucket.naam === nameOrUuid
    )
  );

const trySubmit = () => {
  // Remove bucket entries from formField facets that are not present anymore in search response facets
  formFields.value = {
    ...formFields.value,
    publishers: filterFacets(`publishers`),
    informatieCategorieen: filterFacets(`informatieCategorieen`),
    resultTypes: filterFacets(`resultTypes`)
  };

  if (!formElement.value?.checkValidity()) return;

  const keys = Object.keys(formFields.value) as Array<keyof SearchFormFields>;
  const query = parsedQuery.value;

  if (keys.every((key) => query[key] === formFields.value[key])) return;

  submit();
};

const { error, loading, data } = useLoader((signal) =>
  search({
    ...parsedQuery.value,
    signal
  })
);

const showSpinner = useSpinner(loading);

const getRoute = (page: number): RouteLocationRaw => ({
  path: route.path,
  query: {
    ...route.query,
    page: page.toString()
  }
});

const pagination = computed(
  () =>
    data.value &&
    mapPaginatedResultsToUtrechtPagination({
      page: parsedQuery.value.page,
      pagination: data.value,
      getRoute
    })
);
</script>

<style lang="scss" scoped>
@use "@/assets/variables";

.gpp-woo-search {
  --utrecht-heading-1-margin-block-end: var(--gpp-woo-search-heading-1-margin-block-end);
  --utrecht-heading-1-margin-block-start: var(--gpp-woo-search-heading-1-margin-block-start);
  --utrecht-heading-2-margin-block-end: var(--gpp-woo-search-heading-2-margin-block-end);
  --utrecht-heading-2-margin-block-start: var(--gpp-woo-search-heading-2-margin-block-start);
  --utrecht-paragraph-margin-block-start: var(--gpp-woo-search-paragraph-margin-block-start);

  display: grid;
  grid-template-areas:
    "heading"
    "bar"
    "subheading"
    "filters"
    "results";

  @media screen and (min-width: #{variables.$breakpoint-md}) {
    grid-template-columns: minmax(auto, 18rem) 1fr;
    grid-template-rows: auto auto 1fr;
    grid-template-areas:
      "subheading heading"
      "filters bar"
      "filters results";
    column-gap: var(--gpp-woo-search-grid-column-gap);
  }

  .utrecht-heading-1 {
    grid-area: heading;
  }

  form,
  form > section {
    display: contents;
  }

  .gpp-woo-search-bar {
    grid-area: bar;
  }

  :deep(.utrecht-heading-2) {
    grid-area: subheading;
  }

  :deep(.gpp-woo-search-filters) {
    grid-area: filters;
  }

  .gpp-woo-search-results {
    grid-area: results;

    > * {
      display: flex;
      flex-direction: column;
    }

    .utrecht-pagination {
      margin-inline: auto;
    }
  }
}
</style>
