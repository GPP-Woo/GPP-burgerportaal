<template>
  <simple-spinner v-if="loading"></simple-spinner>

  <utrecht-alert v-else-if="error"
    >Helaas! Deze publicatie is niet (meer) beschikbaar! Mogelijk is deze verwijderd. Neem contact
    op met de gemeente voor nadere informatie.</utrecht-alert
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
        <utrecht-heading :level="2" :id="headingId">Documenten bij deze publicatie</utrecht-heading>

        <utrecht-table :aria-labelledby="headingId" class="utrecht-table--alternate-row-color">
          <utrecht-table-header>
            <utrecht-table-row>
              <utrecht-table-header-cell scope="col">Officiële titel</utrecht-table-header-cell>
              <utrecht-table-header-cell scope="col" class="gpp-woo-table-fixed-header"
                >Datum document</utrecht-table-header-cell
              >
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
import { computed, useId } from "vue";
import { useFetchApi } from "@/api/use-fetch-api";
import { useAllPages } from "@/composables/use-all-pages";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
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

const loading = computed(() => loadingPublicatie.value || loadingDocumenten.value);
const error = computed(() => !!publicatieError.value || !!documentenError.value);

const {
  data: publicatieData,
  isFetching: loadingPublicatie,
  error: publicatieError
} = useFetchApi(() => `${API_URL}/publicaties/${props.uuid}`).json<Publicatie>();

const {
  data: documenten,
  loading: loadingDocumenten,
  error: documentenError
} = useAllPages<PublicatieDocument>(`${API_URL}/documenten/?publicatie=${props.uuid}`);

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
      ["Geregistreerd op", formatDate(publicatieData.value?.registratiedatum)],
      ["Laatst gewijzigd op", formatDate(publicatieData.value?.laatstGewijzigdDatum)]
    ])
);
</script>
