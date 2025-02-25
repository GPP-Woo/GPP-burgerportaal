<template>
  <utrecht-form-field>
    <utrecht-form-label for="search-field">Zoekterm</utrecht-form-label>

    <div class="search-bar">
      <utrecht-textbox
        id="search-field"
        aria-placeholder="Hier zoeken"
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
.search-bar {
  display: flex;
}
</style>
