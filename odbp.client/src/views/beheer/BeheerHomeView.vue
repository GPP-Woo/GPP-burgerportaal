<template>
  <h1>GPP-Woo Burgerportaal (beheer)</h1>

  <simple-spinner v-if="isFetching" />

  <utrecht-alert v-else-if="error" type="error">
    <p>Er is een fout opgetreden.</p>
  </utrecht-alert>

  <form v-else-if="homepage" class="utrecht-form" @submit.prevent="submit">
    <utrecht-fieldset>
      <utrecht-form-field>
        <utrecht-form-label for="welcome">Welkomsttekst</utrecht-form-label>

        <utrecht-textarea id="welcome" v-model="homepage.welcome" rows="10"></utrecht-textarea>
      </utrecht-form-field>

      <utrecht-form-field>
        <utrecht-button type="submit" :appearance="'primary-action-button'" :disabled="isFetching"
          >Opslaan</utrecht-button
        >
      </utrecht-form-field>
    </utrecht-fieldset>
  </form>
</template>

<script setup lang="ts">
import { useFetchApi } from "@/api";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import UtrechtAlert from "@/components/UtrechtAlert.vue";

const {
  data: homepage,
  isFetching,
  error,
  put
} = useFetchApi(() => "/api/beheer/homepage").json<{
  welcome: string;
}>();

const submit = async () => await put(homepage).execute();
</script>

<style lang="scss" scoped></style>
