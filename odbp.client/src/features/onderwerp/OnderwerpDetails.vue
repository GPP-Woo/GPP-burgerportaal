<template>
  <simple-spinner v-if="isFetching"></simple-spinner>

  <utrecht-alert v-else-if="error"
    >Er is iets misgegaan bij het ophalen van de publicatie...</utrecht-alert
  >

  <article v-else class="utrecht-article">
    <utrecht-heading :level="1" :id="headingId">{{ data?.officieleTitel }}</utrecht-heading>

    <utrecht-paragraph
      class="utrecht-paragraph--lead utrecht-spotlight-section utrecht-spotlight-section--info"
      ><b class="utrecht-paragraph__b">{{ data?.omschrijving }}</b></utrecht-paragraph
    >

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
  </article>
</template>

<script setup lang="ts">
import { computed, useId } from "vue";
import { useFetchApi } from "@/api";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import UtrechtAlert from "@/components/UtrechtAlert.vue";
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

<style lang="scss" scoped></style>
