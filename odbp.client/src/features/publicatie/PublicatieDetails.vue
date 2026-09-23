<template>
  <simple-spinner v-if="loading"></simple-spinner>

  <utrecht-alert v-else-if="error"
    >Helaas! Deze publicatie is niet (meer) beschikbaar! Mogelijk is deze verwijderd. Neem contact
    op met {{ resources?.organisationLabel ?? "de organisatie" }} voor nadere
    informatie.</utrecht-alert
  >

  <template v-else>
    <utrecht-heading :level="1">{{ publicatieData?.officieleTitel }}</utrecht-heading>

    <inzage-procedure
      v-if="publicatieData?.inzageProcedure"
      :inzage-procedure="publicatieData.inzageProcedure"
    />

    <section>
      <utrecht-heading :level="2">
        <utrecht-button
          :id="publicatieGegevensToggleId"
          type="button"
          appearance="secondary-action-button"
          class="gpp-woo-toggle-button"
          :aria-controls="publicatieGegevensId"
          :aria-expanded="isPublicatieGegevensExpanded"
          @click="isPublicatieGegevensExpanded = !isPublicatieGegevensExpanded"
        >
          Over deze publicatie

          <utrecht-icon :icon="isPublicatieGegevensExpanded ? 'chevron-up' : 'chevron-down'" />
        </utrecht-button>
      </utrecht-heading>

      <gpp-woo-table-container :id="publicatieGegevensId" :hidden="!isPublicatieGegevensExpanded">
        <utrecht-table :aria-labelledby="publicatieGegevensToggleId">
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
        <utrecht-heading :level="2" :id="documentenHeadingId"
          >Documenten bij deze publicatie <small-spinner v-if="loadingDocumenten"
        /></utrecht-heading>

        <utrecht-table
          :aria-labelledby="documentenHeadingId"
          :aria-busy="loadingDocumenten"
          :class="[
            'utrecht-table--alternate-row-color',
            loadingDocumenten && 'utrecht-table--loading'
          ]"
        >
          <utrecht-table-header>
            <utrecht-table-row>
              <utrecht-table-header-cell
                scope="col"
                :aria-sort="sortField === 'officiele_titel' ? sortDir : 'none'"
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
                :aria-sort="sortField === 'creatiedatum' ? sortDir : 'none'"
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
              <utrecht-table-header-cell scope="col" colspan="3" class="gpp-woo-table-fixed-header"
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
                  <utrecht-icon icon="download" /> Download
                </utrecht-link>
              </utrecht-table-cell>
              <utrecht-table-cell>
                <gpp-woo-pdf-viewer-dialog
                  v-if="isPdfFile(bestandsnaam)"
                  :src="`${API_URL}/documenten/${uuid}/download`"
                  :title="officieleTitel"
                >
                  <template #default="{ open }">
                    <button
                      type="button"
                      class="utrecht-link-button utrecht-link-button--html-button gpp-woo-link-button gpp-woo-link--icon"
                      @click="open"
                    >
                      <utrecht-icon icon="eye" /> Bekijk
                    </button>
                  </template>
                </gpp-woo-pdf-viewer-dialog>
              </utrecht-table-cell>
              <utrecht-table-cell class="gpp-woo-file-meta"
                >{{ bestandsnaam.split(".").pop()?.toUpperCase()
                }}{{
                  bestandsomvang ? `, ${formatFileSize(bestandsomvang)}` : null
                }}</utrecht-table-cell
              >
            </utrecht-table-row>
          </utrecht-table-body>
        </utrecht-table>
      </gpp-woo-table-container>
    </section>
  </template>
</template>

<script setup lang="ts">
import { computed, ref, useId, watch } from "vue";
import { injectResources } from "@/resources";
import { useFetchApi } from "@/api/use-fetch-api";
import { useAllPages } from "@/composables/use-all-pages";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import SmallSpinner from "@/components/SmallSpinner.vue";
import UtrechtAlert from "@/components/UtrechtAlert.vue";
import UtrechtBadgeList, { type BadgeListItem } from "@/components/UtrechtBadgeList.vue";
import UtrechtIcon from "@/components/UtrechtIcon.vue";
import GppWooTableContainer from "@/components/GppWooTableContainer.vue";
import GppWooPdfViewerDialog from "@/components/GppWooPdfViewerDialog.vue";
import InzageProcedure from "./InzageProcedure.vue";
import { formatDate, formatFileSize, isPdfFile } from "@/helpers";
import type { Publicatie, PublicatieDocument } from "./types";
import { lijsten } from "@/stores/lijsten";

const API_URL = `/api/v2`;

const props = defineProps<{ uuid: string }>();

const documentenHeadingId = useId();
const publicatieGegevensId = useId();
const publicatieGegevensToggleId = useId();

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

// Publicatie gegevens collapsed by default when inzageProcedure is present
const isPublicatieGegevensExpanded = ref(false);

watch(
  loadingPublicatie,
  (isLoading) =>
    !isLoading && (isPublicatieGegevensExpanded.value = !publicatieData.value?.inzageProcedure)
);

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

.gpp-woo-toggle-button {
  --utrecht-button-focus-scale: 1.005;
  --utrecht-button-hover-scale: 1.005;
  --utrecht-button-column-gap: var(--utrecht-space-text-xs);
  --utrecht-button-max-inline-size: none;

  flex: 1;
  font: inherit;
  justify-content: space-between;
  margin-inline-end: calc(
    -1 *
      (
        var(--utrecht-button-padding-inline-end) +
          var(--utrecht-button-secondary-action-border-width)
      )
  );
  margin-inline-start: calc(
    -1 *
      (
        var(--utrecht-button-padding-inline-start) +
          var(--utrecht-button-secondary-action-border-width)
      )
  );
}

.utrecht-table {
  --utrecht-button-color: var(--utrecht-document-color);

  &--loading {
    opacity: 0.5;
    pointer-events: none;
  }

  &__header-cell-button {
    --_utrecht-button-line-height: 1;

    text-align: left;
  }
}

.gpp-woo-file-meta {
  white-space: nowrap;
}

.gpp-woo-link-button {
  --utrecht-button-min-block-size: auto;
  --utrecht-button-min-inline-size: auto;
  --utrecht-button-padding-block-start: 0;
  --utrecht-button-padding-block-end: 0;
  --utrecht-button-padding-inline-start: 0;
  --utrecht-button-padding-inline-end: 0;
}
</style>
