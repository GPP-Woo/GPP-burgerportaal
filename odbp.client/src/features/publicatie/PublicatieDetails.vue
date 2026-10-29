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
            <utrecht-table-row v-for="row in publicatieRows" :key="row.label">
              <template v-if="row.type === 'text' ? row.text : row.items.length">
                <utrecht-table-header-cell scope="row">{{ row.label }}</utrecht-table-header-cell>
                <utrecht-table-cell class="gpp-woo-pre-wrap">
                  <gpp-woo-link-list v-if="row.type === 'links'" :links="row.items" />

                  <utrecht-badge-list v-else-if="row.type === 'badges'" :badges="row.items" />

                  <template v-else>{{ row.text }}</template>
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
                <utrecht-button
                  class="utrecht-table__header-cell-button"
                  @click="toggleSort('officiele_titel')"
                >
                  <utrecht-icon :icon="sortField === 'officiele_titel' ? sortIcon : 'sort-none'" />
                  Officiële titel
                </utrecht-button>
              </utrecht-table-header-cell>
              <utrecht-table-header-cell
                scope="col"
                class="gpp-woo-table-fixed-header"
                :aria-sort="sortField === 'creatiedatum' ? sortDir : 'none'"
              >
                <utrecht-button
                  class="utrecht-table__header-cell-button"
                  @click="toggleSort('creatiedatum')"
                >
                  <utrecht-icon :icon="sortField === 'creatiedatum' ? sortIcon : 'sort-none'" />
                  Datum document
                </utrecht-button>
              </utrecht-table-header-cell>
              <utrecht-table-header-cell
                scope="col"
                colspan="3"
                class="gpp-woo-table-fixed-header"
                ><span class="utrecht-button utrecht-table__header-cell-button"
                  >Bestand</span
                ></utrecht-table-header-cell
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
import { computed, ref, useId } from "vue";
import { injectResources } from "@/resources";
import { useFetchApi } from "@/api/use-fetch-api";
import { usePageTitle } from "@/composables/use-page-title";
import { useAllPages } from "@/composables/use-all-pages";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import SmallSpinner from "@/components/SmallSpinner.vue";
import UtrechtAlert from "@/components/UtrechtAlert.vue";
import GppWooLinkList, { type LinkListItem } from "@/components/GppWooLinkList.vue";
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

usePageTitle(() => publicatieData.value?.officieleTitel);

const {
  data: documenten,
  loading: loadingDocumenten,
  error: documentenError
} = useAllPages<PublicatieDocument>(
  computed(() => `${API_URL}/documenten/?publicatie=${props.uuid}&sorteer=${sortParam.value}`)
);

type PublicatieRow =
  | { label: string; type: "text"; text?: string }
  | { label: string; type: "links"; items: LinkListItem[] }
  | { label: string; type: "badges"; items: BadgeListItem[] };

const publicatieRows = computed<PublicatieRow[]>(() => [
  { label: "Officiële titel", type: "text", text: publicatieData.value?.officieleTitel },
  { label: "Verkorte titel", type: "text", text: publicatieData.value?.verkorteTitel },
  { label: "Omschrijving", type: "text", text: publicatieData.value?.omschrijving },
  {
    label: "Organisatie",
    type: "text",
    text:
      lijsten.value?.organisaties.find((o) => o.uuid === publicatieData.value?.publisher)?.naam ||
      "onbekend"
  },
  {
    label: "Onderwerpen",
    type: "links",
    items:
      publicatieData.value?.onderwerpen.map((uuid) => ({
        naam: lijsten.value?.onderwerpen.find((o) => o.uuid === uuid)?.naam || "onbekend",
        url: `/onderwerpen/${uuid}`
      })) ?? []
  },
  {
    label: "Informatiecategorieën",
    type: "badges",
    items:
      publicatieData.value?.informatieCategorieen.map((uuid) => ({
        naam: lijsten.value?.informatiecategorieen.find((c) => c.uuid === uuid)?.naam || "onbekend"
      })) ?? []
  },
  {
    label: "Datum in werking",
    type: "text",
    text: formatDate(publicatieData.value?.datumBeginGeldigheid)
  },
  {
    label: "Datum buiten werking",
    type: "text",
    text: formatDate(publicatieData.value?.datumEindeGeldigheid)
  },
  {
    label: "Kenmerken",
    type: "text",
    text: publicatieData.value?.kenmerken.map((i) => i.kenmerk).join(", ")
  },
  {
    label: "Gepubliceerd op",
    type: "text",
    text: formatDate(publicatieData.value?.gepubliceerdOp)
  },
  {
    label: "Laatst gewijzigd op",
    type: "text",
    text: formatDate(publicatieData.value?.laatstGewijzigdDatum)
  }
]);
</script>

<style lang="scss" scoped>
.utrecht-heading-2 {
  display: flex;
  align-items: center;
  gap: 1ch;
}

.utrecht-table {
  &--loading {
    opacity: 0.5;
    pointer-events: none;
  }

  &__header-cell-button {
    --_utrecht-button-line-height: 1;

    text-align: left;

    &:is(span) {
      &:hover,
      &:active {
        cursor: auto;
        user-select: text;
        color: var(--_utrecht-button-color);
        background-color: var(--_utrecht-button-background-color);
        border-color: var(--_utrecht-button-border-color);
      }
    }
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
