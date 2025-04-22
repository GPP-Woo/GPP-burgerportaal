<template>
  <simple-spinner v-if="isFetching"></simple-spinner>

  <utrecht-alert v-else-if="error"
    >Er is iets misgegaan bij het ophalen van de publicatie...</utrecht-alert
  >

  <utrecht-heading :level="1" :id="headingId">{{ data?.officieleTitel }}</utrecht-heading>

  <utrecht-spotlight-section :aria-labelledby="headingId">
    <img
      v-if="data?.afbeelding"
      :src="data.afbeelding"
      :alt="`Afbeelding ${data.officieleTitel}`"
      crossorigin="anonymous"
    />

    <utrecht-paragraph class="utrecht-paragraph--lead gpp-woo-pre-wrap">
      {{ data?.omschrijving }}
    </utrecht-paragraph>
  </utrecht-spotlight-section>

  <utrecht-table-container>
    <utrecht-table :aria-labelledby="headingId">
      <utrecht-table-header class="utrecht-table__header--hidden">
        <utrecht-table-row>
          <utrecht-table-header-cell scope="col">Kenmerk</utrecht-table-header-cell>
          <utrecht-table-header-cell scope="col">Kenmerkwaarde</utrecht-table-header-cell>
        </utrecht-table-row>
      </utrecht-table-header>

      <utrecht-table-body>
        <utrecht-table-row v-for="[key, value] in rows" :key="key">
          <template v-if="value?.length">
            <utrecht-table-header-cell scope="row">{{ key }}</utrecht-table-header-cell>
            <utrecht-table-cell>{{ value }}</utrecht-table-cell>
          </template>
        </utrecht-table-row>
      </utrecht-table-body>
    </utrecht-table>
  </utrecht-table-container>
</template>

<script setup lang="ts">
import { computed, useId } from "vue";
import { useFetchApi } from "@/api";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import UtrechtAlert from "@/components/UtrechtAlert.vue";
import UtrechtSpotlightSection from "@/components/UtrechtSpotlightSection.vue";
import UtrechtTableContainer from "@/components/UtrechtTableContainer.vue";
import { formatDate } from "@/helpers";
import type { Onderwerp } from "./types";

const API_URL = `/api/v1`;

const props = defineProps<{ uuid: string }>();

const headingId = useId();

const { data, isFetching, error } = useFetchApi(
  () => `${API_URL}/onderwerpen/${props.uuid}`
).json<Onderwerp>();

const rows = computed(
  () =>
    new Map<string, string | undefined>([
      ["Geregistreerd op", formatDate(data.value?.registratiedatum)],
      ["Laatst gewijzigd op", formatDate(data.value?.laatstGewijzigdDatum)]
    ])
);
</script>

<style lang="scss" scoped>
.utrecht-spotlight-section {
  --utrecht-paragraph-margin-block-start: 0;
  overflow: hidden;

  img {
    display: block;
    float: left;
    inline-size: 100%;
    max-inline-size: var(--gpp-woo-topic-detail-max-image-size);
    aspect-ratio: 16/9;
    object-fit: cover;
    margin-block-end: 0.5rem;
    margin-inline-end: 2rem;
    shape-outside: margin-box;
    border: 1px solid var(--utrecht-color-grey-80);
    box-sizing: border-box;
  }
}
</style>
