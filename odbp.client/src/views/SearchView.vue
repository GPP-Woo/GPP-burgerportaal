<template>
  <div class="zoeken-page">
    <utrecht-heading :level="1">Zoeken</utrecht-heading>

    <form class="utrecht-form" @submit.prevent.stop="submit" ref="formElement">
      <utrecht-fieldset class="zoeken">
        <utrecht-legend class="visually-hidden">Zoeken</utrecht-legend>

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

      <utrecht-fieldset class="filters">
        <utrecht-legend class="visually-hidden">Filters</utrecht-legend>

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
    </form>

    <div class="results" aria-live="polite" aria-atomic="true">
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
                <utrecht-heading :level="2">
                  <router-link
                    :to="`/${type === resultOptions.document.value ? 'documenten' : 'publicaties'}/${uuid}`"
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
    </div>
  </div>
</template>

<script setup lang="ts">
import GppWooIcon from "@/components/GppWooIcon.vue";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import UtrechtPagination from "@/components/UtrechtPagination.vue";
import SearchBar from "@/components/SearchBar.vue";
import { useLoader } from "@/composables/use-loader";
import { useSpinner } from "@/composables/use-spinner";
import { sortOptions, search, resultOptions } from "@/features/search/service";
import { formatDate, mapPaginatedResultsToUtrechtPagination, truncate } from "@/helpers";
import { computed, onMounted, reactive, ref } from "vue";
import { useRoute, useRouter, type RouteLocationRaw } from "vue-router";

const route = useRoute();
const formElement = ref<HTMLFormElement>();

const first = <T,>(v: T | Array<T>) => (Array.isArray(v) ? v[0] : v);

const parsedQuery = computed(() => ({
  query: first(route.query.query) || "",
  registratiedatumVanaf: first(route.query.registratiedatumVanaf) || "",
  registratiedatumTot: first(route.query.registratiedatumTot) || "",
  laatstGewijzigdDatumVanaf: first(route.query.laatstGewijzigdDatumVanaf) || "",
  laatstGewijzigdDatumTot: first(route.query.laatstGewijzigdDatumTot) || "",
  page: +(first(route.query.page) || "1"),
  sort:
    first(route.query.sort) === sortOptions.chronological.value
      ? sortOptions.chronological.value
      : sortOptions.relevance.value
}));

const formFields = reactive({
  query: "",
  registratiedatumVanaf: "",
  registratiedatumTot: "",
  laatstGewijzigdDatumVanaf: "",
  laatstGewijzigdDatumTot: "",
  sort: sortOptions.relevance.value as string
});

onMounted(() => {
  const keys = Object.keys(formFields) as Array<keyof typeof formFields>;
  const query = parsedQuery.value;
  keys.forEach((key) => (formFields[key] = query[key]));
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

.zoeken-page {
  --utrecht-paragraph-margin-block-start: 0;

  display: grid;
  grid-template-areas:
    "heading"
    "search"
    "filters"
    "results";

  @media screen and (min-width: variables.$breakpoint-md) {
    grid-template-columns: minmax(auto, 15rem) 1fr;
    grid-template-rows: auto auto 1fr;
    grid-template-areas:
      ". heading"
      "filters search"
      "filters results";
    column-gap: 3vw;
  }

  .utrecht-heading-1 {
    grid-area: heading;
  }

  form {
    display: contents;
  }

  .zoeken {
    grid-area: search;

    > :first-child {
      display: flex;
      flex-wrap: wrap;
      column-gap: calc(var(--utrecht-space-inline-md) * 2);

      > * {
        flex: 1 0 auto;
      }
    }
  }

  .filters {
    grid-area: filters;

    > :first-child {
      display: grid;
      gap: var(--utrecht-space-inline-xs);
    }
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
  --utrecht-heading-2-font-weight: var(--utrecht-heading-3-font-weight);
  --utrecht-heading-2-margin-block-start: var(--utrecht-heading-3-margin-block-start);

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
