<template>
  <small-spinner v-if="isFetching" />

  <template v-else-if="statistics && !error">
    <utrecht-heading :level="2">Cijfers over deze website</utrecht-heading>

    <dl>
      <template v-for="{ label, count, link } in statistics" :key="label">
        <dt>{{ label }}:</dt>
        <dd>
          <router-link :to="link" class="utrecht-link utrecht-link--html-a">{{
            count
          }}</router-link>
        </dd>
      </template>
    </dl>
  </template>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useFetchApi } from "@/api";
import SmallSpinner from "@/components/SmallSpinner.vue";
import { resultOptions, type SearchResponse } from "@/features/search/service";

const { data, isFetching, error } = useFetchApi(() => "/api/zoeken")
  .json<SearchResponse>()
  .post({});

const statistics = computed(() => {
  const facets = data.value?.facets;

  if (!facets) return null;

  const getCount = (name: string) => facets.resultTypes.find((r) => r.naam === name)?.count ?? 0;

  return [
    {
      label: "Publicaties",
      count: getCount(resultOptions.publication.value),
      link: { name: "zoeken", query: { resultTypes: [resultOptions.publication.value] } }
    },
    {
      label: "Documenten",
      count: getCount(resultOptions.document.value),
      link: { name: "zoeken", query: { resultTypes: [resultOptions.document.value] } }
    },
    {
      label: "Onderwerpen",
      count: getCount(resultOptions.topic.value),
      link: { name: "onderwerpen" }
    }
  ];
});
</script>

<style lang="scss" scoped>
dl {
  display: grid;
  grid-template-columns: max-content 1fr;
  gap: 0.5rem;
  margin-block: 0;

  dt {
    grid-column: 1;
  }

  dd {
    grid-column: 2;
    margin-inline: 0;
  }
}
</style>
