<template>
  <utrecht-fieldset>
    <utrecht-legend class="visually-hidden">Zoeken en sorteren</utrecht-legend>

    <gpp-woo-search-field v-model="model.query" @submit="$emit('submit')" />

    <utrecht-form-field>
      <utrecht-form-label for="sort-select">Sorteren</utrecht-form-label>

      <div>
        <utrecht-select
          id="sort-select"
          v-model="model.sort"
          :options="Object.values(sortOptions)"
          @change="$emit('submit')"
        />

        <gpp-woo-icon icon="sort" />
      </div>
    </utrecht-form-field>
  </utrecht-fieldset>
</template>

<script setup lang="ts">
import { useModel } from "vue";
import GppWooIcon from "@/components/GppWooIcon.vue";
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

:has(> #sort-select) {
  position: relative;
  max-inline-size: var(--utrecht-form-control-max-inline-size);

  > :last-child {
    display: flex;
    position: absolute;
    inset-inline-end: 0;
    block-size: 100%;
    inline-size: 0.5rem;
    padding-inline-end: var(
      --utrecht-select-padding-inline-end,
      var(--utrecht-form-control-padding-inline-end)
    );
    pointer-events: none;
  }
}
</style>
