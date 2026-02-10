<template>
  <small-spinner v-if="isFetching" />

  <template v-else-if="statistics && !error">
    <utrecht-heading :level="2">Cijfers over deze website</utrecht-heading>

    <utrecht-data-list>
      <utrecht-data-list-item v-for="{ label, count, link } in statistics" :key="label">
        <utrecht-data-list-key>{{ label }}:</utrecht-data-list-key>
        <utrecht-data-list-value :value="count">
          <router-link :to="link" class="utrecht-link utrecht-link--html-a">{{
            count
          }}</router-link>
        </utrecht-data-list-value>
      </utrecht-data-list-item>
    </utrecht-data-list>
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
