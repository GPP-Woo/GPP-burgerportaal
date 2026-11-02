<template>
  <utrecht-fieldset>
    <utrecht-legend class="visually-hidden">Zoeken en sorteren</utrecht-legend>

    <gpp-woo-search-field v-model="model.query" @submit="$emit('submit')" />

    <utrecht-form-field>
      <utrecht-form-label for="sort-select">Sorteren</utrecht-form-label>

      <utrecht-select
        id="sort-select"
        v-model="model.sort"
        :options="Object.values(sortOptions)"
        @change="$emit('submit')"
      />
    </utrecht-form-field>
  </utrecht-fieldset>
</template>

<script setup lang="ts">
import { useModel } from "vue";
import GppWooSearchField from "@/components/GppWooSearchField.vue";
import { sortOptions, type SearchFormFields } from "@/features/search/service";

const props = defineProps<{ modelValue: SearchFormFields }>();
const model = useModel(props, "modelValue");
</script>

<style lang="scss" scoped>
.utrecht-form-fieldset {
  > :first-child {
    display: flex;
    flex-wrap: wrap;
    column-gap: var(--gpp-woo-search-bar-fieldset-column-gap);

    > .utrecht-form-field {
      flex: 1 0 auto;
    }
  }
}

.utrecht-select--html-select {
  background-image: url("@/assets/icon-sort.svg");
}
</style>
