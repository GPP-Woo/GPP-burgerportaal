<template>
  <utrecht-form-field>
    <utrecht-form-label for="search-field">Zoekterm</utrecht-form-label>

    <div class="gpp-woo-search-field">
      <utrecht-textbox
        id="search-field"
        placeholder="Hier zoeken"
        v-model="query"
        type="search"
        autocomplete="off"
        spelcheck="false"
        enterkeyhint="search"
        @search="$emit('submit')"
      />

      <utrecht-button type="submit" :appearance="'primary-action-button'">Zoeken</utrecht-button>
    </div>
  </utrecht-form-field>
</template>

<script setup lang="ts">
import { useModel } from "vue";

const props = defineProps<{ modelValue: string }>();
const query = useModel(props, "modelValue");
</script>

<style lang="scss" scoped>
.gpp-woo-search-field {
  display: flex;
  column-gap: var(--gpp-woo-search-field-column-gap);
}
</style>
