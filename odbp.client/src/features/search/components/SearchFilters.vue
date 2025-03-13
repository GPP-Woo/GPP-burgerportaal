<template>
  <div>
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
        <utrecht-form-label for="update-date-from">Wijzigingsdatum vanaf</utrecht-form-label>

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
      v-model="model.resultType"
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
import { useModel } from "vue";
import SearchBuckets from "@/features/search/components/SearchBuckets.vue";
import type { SearchFormFields, Facets } from "../service";

const props = defineProps<{ modelValue: SearchFormFields; facets?: Facets }>();
const model = useModel(props, "modelValue");
</script>
