<template>
  <simple-spinner v-if="loading"></simple-spinner>

  <utrecht-alert v-else-if="error"
    >Helaas! Deze publicatie is niet (meer) beschikbaar! Mogelijk is deze verwijderd. Neem contact
    op met {{ resources?.organisationLabel ?? "de organisatie" }} voor nadere
    informatie.</utrecht-alert
  >

  <template v-else>
    <utrecht-heading :level="1">{{ publicatieData?.officieleTitel }}</utrecht-heading>

    <section>
      <gpp-woo-table-container>
        <utrecht-table>
          <utrecht-table-caption>Over deze publicatie</utrecht-table-caption>

          <utrecht-table-header class="utrecht-table__header--hidden">
            <utrecht-table-row>
              <utrecht-table-header-cell scope="col">Publicatiekenmerk</utrecht-table-header-cell>
              <utrecht-table-header-cell scope="col"
                >Publicatiekenmerkwaarde</utrecht-table-header-cell
              >
            </utrecht-table-row>
          </utrecht-table-header>

          <utrecht-table-body>
            <utrecht-table-row v-for="[key, value] in publicatieRows" :key="key">
              <template v-if="value?.length">
                <utrecht-table-header-cell scope="row">{{ key }}</utrecht-table-header-cell>
                <utrecht-table-cell class="gpp-woo-pre-wrap">
                  <utrecht-badge-list v-if="Array.isArray(value)" :badges="value" />

                  <template v-else>{{ value }}</template>
                </utrecht-table-cell>
              </template>
            </utrecht-table-row>
          </utrecht-table-body>
        </utrecht-table>
      </gpp-woo-table-container>

      <gpp-woo-table-container v-if="documenten.length">
        <utrecht-heading :level="2" :id="headingId"
          >Documenten bij deze publicatie <small-spinner v-if="loadingDocumenten"
        /></utrecht-heading>

        <utrecht-table
          :aria-labelledby="headingId"
          :aria-busy="loadingDocumenten"
          class="utrecht-table--alternate-row-color"
          :class="[
            'utrecht-table--alternate-row-color',
            loadingDocumenten && 'utrecht-table--loading'
          ]"
        >
          <utrecht-table-header>
            <utrecht-table-row>
              <utrecht-table-header-cell
                scope="col"
                :aria-sort="sortField === 'officiele_titel' ? sortDir : undefined"
              >
                <button
                  type="button"
                  class="utrecht-button utrecht-table__header-cell-button"
                  @click="toggleSort('officiele_titel')"
                >
                  <utrecht-icon :icon="sortField === 'officiele_titel' ? sortIcon : 'sort-none'" />
                  Officiële titel
                </button>
              </utrecht-table-header-cell>
              <utrecht-table-header-cell
                scope="col"
                class="gpp-woo-table-fixed-header"
                :aria-sort="sortField === 'creatiedatum' ? sortDir : undefined"
              >
                <button
                  class="utrecht-button utrecht-table__header-cell-button"
                  type="button"
                  @click="toggleSort('creatiedatum')"
                >
                  <utrecht-icon :icon="sortField === 'creatiedatum' ? sortIcon : 'sort-none'" />
                  Datum document
                </button>
              </utrecht-table-header-cell>
              <utrecht-table-header-cell scope="col" class="gpp-woo-table-fixed-header"
                >Bestand</utrecht-table-header-cell
              >
            </utrecht-table-row>
          </utrecht-table-header>

          <utrecht-table-body>
            <utrecht-table-row
              v-for="{
                uuid,
                officieleTitel,
                creatiedatum,
                bestandsnaam,
                bestandsomvang
              } in documenten"
              :key="uuid"
            >
              <utrecht-table-cell>
                <router-link
                  :to="{ name: 'document', params: { uuid } }"
                  class="utrecht-link utrecht-link--html-a"
                  >{{ officieleTitel }}</router-link
                >
              </utrecht-table-cell>
              <utrecht-table-cell>{{ formatDate(creatiedatum) }}</utrecht-table-cell>
              <utrecht-table-cell>
                <utrecht-link
                  :href="`${API_URL}/documenten/${uuid}/download`"
                  :download="bestandsnaam"
                  class="gpp-woo-link--icon"
                >
                  <utrecht-icon icon="download" />

                  Download ({{ bestandsnaam.split(".").pop()
                  }}{{ bestandsomvang ? `, ${Math.floor(bestandsomvang / 1024)}kb` : null }})
                </utrecht-link>
              </utrecht-table-cell>
            </utrecht-table-row>
          </utrecht-table-body>
        </utrecht-table>
      </gpp-woo-table-container>
    </section>
  </template>
</template>

<script setup lang="ts">
import { computed, ref, useId } from "vue";
import { injectResources } from "@/resources";
import { useFetchApi } from "@/api/use-fetch-api";
import { useAllPages } from "@/composables/use-all-pages";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import SmallSpinner from "@/components/SmallSpinner.vue";
import UtrechtAlert from "@/components/UtrechtAlert.vue";
import UtrechtBadgeList, { type BadgeListItem } from "@/components/UtrechtBadgeList.vue";
import UtrechtIcon from "@/components/UtrechtIcon.vue";
import GppWooTableContainer from "@/components/GppWooTableContainer.vue";
import { formatDate } from "@/helpers";
import type { Publicatie, PublicatieDocument } from "./types";
import { lijsten } from "@/stores/lijsten";

const API_URL = `/api/v2`;

const props = defineProps<{ uuid: string }>();

const headingId = useId();

const resources = injectResources();

const loading = computed(
  () => loadingPublicatie.value || (loadingDocumenten.value && !documenten.value.length)
);

const error = computed(() => !!publicatieError.value || !!documentenError.value);

type SortField = "officiele_titel" | "creatiedatum";
type SortDirection = "ascending" | "descending";

const sortField = ref<SortField>("creatiedatum");
const sortDir = ref<SortDirection>("descending");
const sortIcon = computed(() => `sort-${sortDir.value}`);
const sortParam = computed(() => `${sortDir.value === "descending" ? "-" : ""}${sortField.value}`);

function toggleSort(field: SortField) {
  if (sortField.value === field) {
    sortDir.value = sortDir.value === "ascending" ? "descending" : "ascending";
  } else {
    sortField.value = field;
    sortDir.value = field === "creatiedatum" ? "descending" : "ascending";
  }
}

const {
  data: publicatieData,
  isFetching: loadingPublicatie,
  error: publicatieError
} = useFetchApi(() => `${API_URL}/publicaties/${props.uuid}`).json<Publicatie>();

const {
  data: documenten,
  loading: loadingDocumenten,
  error: documentenError
} = useAllPages<PublicatieDocument>(
  computed(() => `${API_URL}/documenten/?publicatie=${props.uuid}&sorteer=${sortParam.value}`)
);

const publicatieRows = computed(
  () =>
    new Map<string, string | BadgeListItem[] | undefined>([
      ["Officiële titel", publicatieData.value?.officieleTitel],
      ["Verkorte titel", publicatieData.value?.verkorteTitel],
      ["Omschrijving", publicatieData.value?.omschrijving],
      [
        "Organisatie",
        lijsten.value?.organisaties.find((o) => o.uuid === publicatieData.value?.publisher)?.naam ||
          "onbekend"
      ],
      [
        "Onderwerpen",
        publicatieData.value?.onderwerpen.map((uuid) => ({
          naam: lijsten.value?.onderwerpen.find((o) => o.uuid === uuid)?.naam || "onbekend",
          url: `/onderwerpen/${uuid}`
        }))
      ],
      [
        "Informatiecategorieën",
        publicatieData.value?.informatieCategorieen.map((uuid) => ({
          naam:
            lijsten.value?.informatiecategorieen.find((c) => c.uuid === uuid)?.naam || "onbekend"
        }))
      ],
      ["Datum in werking", formatDate(publicatieData.value?.datumBeginGeldigheid)],
      ["Datum buiten werking", formatDate(publicatieData.value?.datumEindeGeldigheid)],
      ["Kenmerken", publicatieData.value?.kenmerken.map((i) => i.kenmerk).join(", ")],
      ["Gepubliceerd op", formatDate(publicatieData.value?.gepubliceerdOp)],
      ["Laatst gewijzigd op", formatDate(publicatieData.value?.laatstGewijzigdDatum)]
    ])
);
</script>

<style lang="scss" scoped>
.utrecht-heading-2 {
  display: flex;
  align-items: center;
  gap: 1ch;
}

.utrecht-table {
  --utrecht-button-color: var(--utrecht-document-color);

  &--loading {
    opacity: 0.5;
    pointer-events: none;
  }
}
</style>
