<template>
  <utrecht-heading :level="2">Externe links</utrecht-heading>

  <simple-spinner v-if="isFetching" />

  <utrecht-alert v-else-if="error" type="error">
    <p>{{ error.message ?? "Er is een fout opgetreden." }}</p>

    <utrecht-button type="button" :appearance="'primary-action-button'" @click="get()"
      >Ga terug</utrecht-button
    >
  </utrecht-alert>

  <form v-else-if="links" class="utrecht-form" @submit.prevent="submit">
    <link-fieldset
      title="URL Website organisatie"
      v-model:link="links.websiteUrl"
      :help-text="helpWebsiteUrl"
    />

    <link-fieldset
      title="URL Toegankelijkheidsverklaring"
      v-model:link="links.a11yUrl"
      :help-text="helpA11yUrl"
    />

    <link-fieldset
      title="URL Privacy-verklaring"
      v-model:link="links.privacyUrl"
      :help-text="helpPrivacyUrl"
    />

    <link-fieldset
      title="URL Contact-pagina"
      v-model:link="links.contactUrl"
      :help-text="helpContactUrl"
    />

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
  </form>
</template>

<script setup lang="ts">
import { useCloned } from "@vueuse/core";
import { useFetchApi } from "@/api";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import UtrechtAlert from "@/components/UtrechtAlert.vue";
import LinkFieldset from "@/components/beheer/LinkFieldset.vue";

const helpWebsiteUrl = `Geef hier de URL aan van de website van de organisatie. In het menu van het burgerportaal wordt hiernaar verwezen.`;
const helpA11yUrl = `Geef hier de URL naar de toegankelijkheidsverklaring, bijvoorbeeld op https://www.toegankelijkheidsverklaring.nl/register. De link wordt op het burgerportaal onderaan in de voetbalk van iedere pagina getoond.`;
const helpPrivacyUrl = `Geef hier de URL naar de privacy-verklaring, bijvoorbeeld op de website van de organisatie. De link wordt op het burgerportaal onderaan in de voetbalk van iedere pagina getoond.`;
const helpContactUrl = `Geef hier de URL naar de contact-pagina op de website van de organisatie. De link wordt op het burgerportaal onderaan in de voetbalk van iedere pagina getoond.`;

type Links = {
  websiteUrl: string;
  privacyUrl: string;
  contactUrl: string;
  a11yUrl: string;
};

const {
  data,
  isFetching,
  error,
  get: getLinks,
  put: putLinks
} = useFetchApi(() => "/api/beheer/links", {
  async onFetchError(ctx) {
    try {
      ctx.error = await ctx.response?.json();
      return ctx;
    } catch {
      return ctx;
    }
  }
}).json<Links>();

const { cloned: links, isModified, sync } = useCloned(data);

const get = () => getLinks().execute();

const submit = () => putLinks(links).execute();
</script>
