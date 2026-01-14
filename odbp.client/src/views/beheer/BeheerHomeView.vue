<template>
  <utrecht-heading :level="2" :id="headingId">Homepage</utrecht-heading>

  <simple-spinner v-if="isFetching" />

  <utrecht-alert v-else-if="error" type="error">
    <p>Er is een fout opgetreden.</p>
  </utrecht-alert>

  <form v-else-if="homepage" class="utrecht-form" @submit.prevent="submit">
    <utrecht-fieldset :labelledby="headingId">
      <utrecht-form-field>
        <utrecht-form-label for="welcome">Welkomsttekst</utrecht-form-label>

        <ck-editor v-model="homepage.welcome" id="welcome" />
      </utrecht-form-field>

      <utrecht-form-field class="form-actions">
        <utrecht-button
          type="button"
          :appearance="'secondary-action-button'"
          :disabled="!isModified"
          @click="sync()"
          >Annuleren</utrecht-button
        >

        <utrecht-button type="submit" :appearance="'primary-action-button'"
          >Publiceren</utrecht-button
        >
      </utrecht-form-field>
    </utrecht-fieldset>
  </form>
</template>

<script setup lang="ts">
import { useId } from "vue";
import { useCloned } from "@vueuse/core";
import { useFetchApi } from "@/api";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import UtrechtAlert from "@/components/UtrechtAlert.vue";
import CkEditor from "@/components/ckeditor";

type HomepageBeheer = { welcome: string };

const headingId = useId();

const { data, isFetching, error, put } = useFetchApi(
  () => "/api/beheer/homepage"
).json<HomepageBeheer>();

const { cloned: homepage, isModified, sync } = useCloned(data);

const submit = async () => await put(homepage.value).execute();
</script>

<style lang="scss" scoped>
.utrecht-form-label {
  display: block;
  margin-block-end: 0.5rem;
}

.form-actions {
  display: flex;
  justify-content: space-between;
}
</style>
