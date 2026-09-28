<template>
  <utrecht-spotlight-section type="info">
    <utrecht-article>
      <utrecht-heading :level="2">Terinzagelegging</utrecht-heading>

      <utrecht-paragraph class="gpp-woo-pre-wrap">{{
        inzageProcedure.toelichting
      }}</utrecht-paragraph>

      <utrecht-paragraph v-if="status === 'scheduled'">
        <em
          >De inzagetermijn is nog niet gestart. Deze loopt vanaf
          {{ formatDate(inzageProcedure.datumBeginInzagetermijn) }} tot en met
          {{ formatDate(inzageProcedure.datumEindeInzagetermijn) }}.
        </em>
      </utrecht-paragraph>

      <utrecht-paragraph v-else-if="status === 'open'">
        <em>
          U kunt de documenten hieronder inzien. U kunt hierop een
          {{ inzageProcedure.beschikbaarRechtsmiddel }} indienen tot en met
          {{ formatDate(inzageProcedure.datumEindeInzagetermijn) }}.
        </em>
      </utrecht-paragraph>

      <utrecht-paragraph v-else><em>De reactietermijn is helaas verstreken.</em></utrecht-paragraph>

      <ul role="list" class="utrecht-link-list utrecht-link-list--html-ul">
        <li v-if="inzageProcedure.urlBekendmaking" class="utrecht-link-list__item">
          <utrecht-link external :href="inzageProcedure.urlBekendmaking" class="gpp-woo-link--icon">
            <utrecht-icon icon="angle-right" class="utrecht-link-list__item--style-type" />

            Bekijk de bekendmaking

            <span class="visually-hidden">(externe link)</span>

            <utrecht-icon icon="external" />
          </utrecht-link>
        </li>

        <li
          v-if="status === 'open' && inzageProcedure.urlReactieformulier"
          class="utrecht-link-list__item"
        >
          <utrecht-link
            external
            :href="inzageProcedure.urlReactieformulier"
            class="gpp-woo-link--icon"
          >
            <utrecht-icon icon="angle-right" class="utrecht-link-list__item--style-type" />

            Dien {{ inzageProcedure.beschikbaarRechtsmiddel }} in

            <span class="visually-hidden">(externe link)</span>

            <utrecht-icon icon="external" />
          </utrecht-link>
        </li>
      </ul>
    </utrecht-article>
  </utrecht-spotlight-section>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { formatDate, todayIsoDate } from "@/helpers";
import UtrechtSpotlightSection from "@/components/UtrechtSpotlightSection.vue";
import UtrechtIcon from "@/components/UtrechtIcon.vue";
import type { InzageProcedure } from "./types";

const { inzageProcedure } = defineProps<{ inzageProcedure: InzageProcedure }>();

const status = computed<"scheduled" | "open" | "closed">(() => {
  const today = todayIsoDate();

  if (today < inzageProcedure.datumBeginInzagetermijn) return "scheduled";
  if (today > inzageProcedure.datumEindeInzagetermijn) return "closed";
  return "open";
});
</script>

<style lang="scss" scoped>
.utrecht-spotlight-section {
  --utrecht-heading-2-margin-block-start: 0;
  --utrecht-link-list-margin-block-start: var(--utrecht-paragraph-margin-block-start, 0);
}

.utrecht-link-list {
  &__item--style-type {
    --utrecht-icon-size: 1rem;
  }
}
</style>
