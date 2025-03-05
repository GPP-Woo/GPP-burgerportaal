<template>
  <div class="search-page">
    <utrecht-heading :level="1">Zoeken</utrecht-heading>

    <form class="utrecht-form" @submit.prevent.stop="submit" ref="formElement">
      <utrecht-fieldset class="search">
        <utrecht-legend class="visually-hidden">Zoeken en sorteren</utrecht-legend>

        <search-bar v-model="formFields.query" @submit="trySubmit" />

        <utrecht-form-field>
          <utrecht-form-label for="sort-select">Sorteren</utrecht-form-label>

          <div>
            <utrecht-select
              id="sort-select"
              v-model="formFields.sort"
              :options="Object.values(sortOptions)"
              @change="trySubmit"
            />

            <gpp-woo-icon icon="sort" />
          </div>
        </utrecht-form-field>
      </utrecht-fieldset>

      <section class="filter-section">
        <utrecht-heading :level="2">Filters</utrecht-heading>

        <div class="filters">
          <utrecht-fieldset>
            <utrecht-legend class="visually-hidden">Registratiedatum</utrecht-legend>

            <utrecht-form-field>
              <utrecht-form-label for="registration-date-from"
                >Registratiedatum vanaf</utrecht-form-label
              >

              <utrecht-textbox
                id="registration-date-from"
                v-model="formFields.registratiedatumVanaf"
                type="date"
                @blur="trySubmit"
                @change="trySubmit"
              />
            </utrecht-form-field>

            <utrecht-form-field>
              <utrecht-form-label for="registration-date-until"
                >Registratiedatum tot en met</utrecht-form-label
              >

              <utrecht-textbox
                id="registration-date-until"
                v-model="formFields.registratiedatumTot"
                type="date"
                @blur="trySubmit"
                @change="trySubmit"
              />
            </utrecht-form-field>
          </utrecht-fieldset>

          <utrecht-fieldset>
            <utrecht-legend class="visually-hidden">Wijzigingsdatum</utrecht-legend>

            <utrecht-form-field>
              <utrecht-form-label for="update-date-from">Wijzigingsdatum vanaf</utrecht-form-label>

              <utrecht-textbox
                id="updated-date-from"
                v-model="formFields.laatstGewijzigdDatumVanaf"
                type="date"
                @blur="trySubmit"
                @change="trySubmit"
              />
            </utrecht-form-field>

            <utrecht-form-field>
              <utrecht-form-label for="updated-date-until"
                >Wijzigingsdatum tot en met</utrecht-form-label
              >

              <utrecht-textbox
                id="updated-date-until"
                v-model="formFields.laatstGewijzigdDatumTot"
                type="date"
                @blur="trySubmit"
                @change="trySubmit"
              />
            </utrecht-form-field>
          </utrecht-fieldset>

          <bucket-group
            legend="Type informatie"
            :buckets="data?.facets?.resultTypes"
            v-model="formFields.resultType"
            @change="trySubmit"
          />

          <bucket-group
            legend="Organisaties"
            :buckets="data?.facets?.publishers"
            v-model="formFields.publishers"
            @change="trySubmit"
          />

          <bucket-group
            legend="Informatiecategorieën"
            :buckets="data?.facets?.informatieCategorieen"
            v-model="formFields.informatieCategorieen"
            @change="trySubmit"
          />
        </div>
      </section>
    </form>

    <section class="results" aria-live="polite" aria-atomic="true">
      <utrecht-heading :level="2" class="visually-hidden">Zoekresultaat</utrecht-heading>

      <simple-spinner v-if="showSpinner" />

      <utrecht-paragraph v-else-if="error"
        >Er ging iets mis. Probeer het opnieuw.</utrecht-paragraph
      >

      <template v-else-if="data">
        <div v-if="data.results.length">
          <utrecht-paragraph>{{ data.count }} resultaten gevonden</utrecht-paragraph>

          <ol>
            <li
              v-for="{
                type,
                record: {
                  uuid,
                  officieleTitel,
                  informatieCategorieen,
                  publisher,
                  registratiedatum,
                  laatstGewijzigdDatum,
                  omschrijving
                }
              } in data.results"
              :key="uuid"
            >
              <utrecht-article class="search-result">
                <utrecht-heading :level="3">
                  <router-link
                    :to="`/${type === resultOptions.document.value ? 'documenten' : 'publicaties'}/${uuid}`"
                    class="utrecht-link utrecht-link--html-a"
                  >
                    {{ officieleTitel }}
                  </router-link>
                </utrecht-heading>

                <ul>
                  <li class="result-type">
                    <strong>{{ resultOptions[type].label }}</strong>
                  </li>

                  <li class="publisher">{{ publisher.naam }}</li>

                  <li
                    class="category"
                    v-for="categorie in informatieCategorieen"
                    :key="categorie.uuid"
                  >
                    {{ categorie.naam }}
                  </li>
                </ul>

                <utrecht-paragraph>{{ truncate(omschrijving, 150) }}</utrecht-paragraph>

                <utrecht-paragraph>
                  <time :datetime="registratiedatum">{{ formatDate(registratiedatum) }}</time>

                  <template
                    v-if="
                      laatstGewijzigdDatum?.substring(0, 10) !== registratiedatum?.substring(0, 10)
                    "
                  >
                    <span>{{ ", gewijzigd op " }}</span>
                    <time :datetime="laatstGewijzigdDatum">{{
                      formatDate(laatstGewijzigdDatum)
                    }}</time>
                  </template>
                </utrecht-paragraph>
              </utrecht-article>
            </li>
          </ol>

          <utrecht-pagination v-if="pagination" v-bind="pagination" class="pagination" />
        </div>

        <utrecht-paragraph v-else>Geen resultaten gevonden.</utrecht-paragraph>
      </template>
    </section>
  </div>
</template>

<script setup lang="ts">
import GppWooIcon from "@/components/GppWooIcon.vue";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import UtrechtPagination from "@/components/UtrechtPagination.vue";
import SearchBar from "@/components/SearchBar.vue";
import BucketGroup from "@/features/search/components/BucketGroup.vue";
import { useLoader } from "@/composables/use-loader";
import { useSpinner } from "@/composables/use-spinner";
import { sortOptions, resultOptions, search, type ResultType } from "@/features/search/service";
import { formatDate, mapPaginatedResultsToUtrechtPagination, truncate } from "@/helpers";
import { computed, onMounted, reactive, ref } from "vue";
import { useRoute, useRouter, type RouteLocationRaw } from "vue-router";

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
  resultType: array(route.query.resultType) || [],
  publishers: array(route.query.publishers) || [],
  informatieCategorieen: array(route.query.informatieCategorieen) || []
}));

const formFields = reactive({
  query: "",
  sort: sortOptions.relevance.value as string,
  registratiedatumVanaf: "",
  registratiedatumTot: "",
  laatstGewijzigdDatumVanaf: "",
  laatstGewijzigdDatumTot: "",
  resultType: [] as string[],
  publishers: [] as string[],
  informatieCategorieen: [] as string[]
});

onMounted(() => {
  const keys = Object.keys(formFields) as Array<keyof typeof formFields>;
  const query = parsedQuery.value;

  keys.forEach((key) =>
    Array.isArray(formFields[key])
      ? ((formFields[key] as string[]) = query[key] as string[])
      : ((formFields[key] as string) = query[key] as string)
  );
});

const router = useRouter();

const submit = () =>
  router.push({
    path: route.path,
    query: {
      ...route.query,
      ...formFields,
      page: 1
    }
  });

const trySubmit = () => {
  if (!formElement.value?.checkValidity()) return;
  const keys = Object.keys(formFields) as Array<keyof typeof formFields>;
  const query = parsedQuery.value;
  if (keys.every((key) => query[key] === formFields[key])) return;
  submit();
};

const { error, loading, data } = useLoader((signal) =>
  search({
    ...parsedQuery.value,
    // resultTypes is offered as a facet so it's handled as an array in this component
    // and then here it's passed as enum, as expected by search
    ...{ resultType: first(parsedQuery.value.resultType) as ResultType },
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

.search-page {
  --utrecht-paragraph-margin-block-start: 0;

  display: grid;
  grid-template-areas:
    "heading"
    "search"
    "subheading"
    "filters"
    "results";

  @media screen and (min-width: variables.$breakpoint-md) {
    grid-template-columns: minmax(auto, 18rem) 1fr;
    grid-template-rows: auto auto 1fr;
    grid-template-areas:
      "subheading heading"
      "filters search"
      "filters results";
    column-gap: 3vw;
  }

  .utrecht-heading-1 {
    grid-area: heading;
    margin-block: var(--utrecht-space-block-xs);
  }

  .utrecht-heading-2 {
    grid-area: subheading;
    align-self: center;
    margin-block: var(--utrecht-space-block-xs);
  }

  form,
  .filter-section {
    display: contents;
  }

  .search {
    grid-area: search;

    > :first-child {
      display: flex;
      flex-wrap: wrap;
      column-gap: var(--utrecht-space-inline-3xl);

      > * {
        flex: 1 0 auto;
      }
    }
  }

  .filters {
    grid-area: filters;
    display: flex;
    flex-direction: column;
  }

  .results {
    grid-area: results;

    > * {
      display: flex;
      flex-direction: column;
    }
  }
}

:has(> #sort-select) {
  position: relative;
  max-inline-size: var(--utrecht-form-control-max-inline-size);

  > :last-child {
    position: absolute;
    inset-inline-end: 0;
    block-size: 100%;
    inline-size: 0.5rem;
    display: flex;
    padding-inline-end: var(
      --utrecht-select-padding-inline-end,
      var(--utrecht-form-control-padding-inline-end)
    );
    pointer-events: none;
  }
}

ol,
ul {
  list-style: none;
  padding: 0;
  margin: 0;
}

.utrecht-form-label {
  display: block;
}

.search-result {
  ul {
    display: flex;
    column-gap: var(--utrecht-space-inline-xs);
    row-gap: var(--utrecht-space-inline-2xs);
    align-items: center;
    flex-wrap: wrap;
  }

  li,
  :has(time) {
    font-size: var(--utrecht-typography-scale-xs-font-size);
  }

  .category {
    border-bottom: 1px dotted lightgray;
  }
}

.pagination {
  margin-inline: auto;
  margin-block-start: var(--utrecht-space-inline-md);
}
</style>
