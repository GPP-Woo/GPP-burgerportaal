<template>
  <utrecht-heading :level="2">
    <utrecht-button
      v-if="!isLargeViewportWidth"
      type="button"
      :aria-controls="panelId"
      :aria-expanded="isExpanded"
      appearance="primary-action-button"
      @click="isExpanded = !isExpanded"
    >
      Filters

      <utrecht-icon :icon="!isExpanded ? `chevron-down` : `chevron-up`" />
    </utrecht-button>

    <template v-else>Filters</template>
  </utrecht-heading>

  <div
    v-bind="$attrs"
    :id="panelId"
    :hidden="!isExpanded"
    :aria-hidden="!isLargeViewportWidth ? !isExpanded : undefined"
    class="gpp-woo-search-filters"
  >
    <utrecht-fieldset>
      <utrecht-legend class="visually-hidden">Registratiedatum</utrecht-legend>

      <utrecht-form-field>
        <utrecht-form-label for="registration-date-from">Registratiedatum vanaf</utrecht-form-label>

        <utrecht-textbox
          id="registration-date-from"
          v-model="model.registratiedatumVanaf"
          type="date"
          @blur="$emit('submit')"
          @change="$emit('submit')"
        />
      </utrecht-form-field>

      <utrecht-form-field>
        <utrecht-form-label for="registration-date-until"
          >Registratiedatum tot en met</utrecht-form-label
        >

        <utrecht-textbox
          id="registration-date-until"
          v-model="model.registratiedatumTot"
          type="date"
          @blur="$emit('submit')"
          @change="$emit('submit')"
        />
      </utrecht-form-field>
    </utrecht-fieldset>

    <utrecht-fieldset>
      <utrecht-legend class="visually-hidden">Wijzigingsdatum</utrecht-legend>

      <utrecht-form-field>
        <utrecht-form-label for="updated-date-from">Wijzigingsdatum vanaf</utrecht-form-label>

        <utrecht-textbox
          id="updated-date-from"
          v-model="model.laatstGewijzigdDatumVanaf"
          type="date"
          @blur="$emit('submit')"
          @change="$emit('submit')"
        />
      </utrecht-form-field>

      <utrecht-form-field>
        <utrecht-form-label for="updated-date-until">Wijzigingsdatum tot en met</utrecht-form-label>

        <utrecht-textbox
          id="updated-date-until"
          v-model="model.laatstGewijzigdDatumTot"
          type="date"
          @blur="$emit('submit')"
          @change="$emit('submit')"
        />
      </utrecht-form-field>
    </utrecht-fieldset>

    <search-buckets
      legend="Type informatie"
      :buckets="facets?.resultTypes"
      v-model="model.resultTypes"
      @change="$emit('submit')"
    />

    <search-buckets
      legend="Organisaties"
      :buckets="facets?.publishers"
      v-model="model.publishers"
      @change="$emit('submit')"
    />

    <search-buckets
      legend="Informatiecategorieën"
      :buckets="facets?.informatieCategorieen"
      v-model="model.informatieCategorieen"
      @change="$emit('submit')"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, useId, useModel, watch } from "vue";
import { useMediaQuery } from "@vueuse/core";
import UtrechtIcon from "@/components/UtrechtIcon.vue";
import SearchBuckets from "@/features/search/components/SearchBuckets.vue";
import type { SearchFormFields, Facets } from "../service";

const props = defineProps<{ modelValue: SearchFormFields; facets?: Facets }>();
const model = useModel(props, "modelValue");

const panelId = useId();

const breakpoint = getComputedStyle(document.documentElement).getPropertyValue("--breakpoint-md");
const isLargeViewportWidth = useMediaQuery(`(min-width: ${breakpoint})`);

// initial expand or collapse depending on vw
const isExpanded = ref(isLargeViewportWidth.value);

// expand panel when screen widens and moves beyond breakpoint
watch(isLargeViewportWidth, (value) => value && (isExpanded.value = true));
</script>

<style lang="scss" scoped>
.utrecht-heading-2 {
  align-self: center;
}

.utrecht-button {
  --utrecht-button-focus-scale: 1;
  --utrecht-button-hover-scale: 1;
  --utrecht-button-inline-size: 100%;
  --utrecht-button-min-inline-size: 100%;
  --utrecht-button-font-weight: normal;

  justify-content: space-between;
}

.utrecht-form-field {
  --utrecht-form-control-max-inline-size: 100%;
}
</style>
