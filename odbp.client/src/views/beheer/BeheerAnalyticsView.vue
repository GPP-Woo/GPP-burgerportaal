<template>
  <utrecht-heading :level="2">Analytics</utrecht-heading>

  <utrecht-alert v-if="success" type="ok">
    {{ success }}
  </utrecht-alert>

  <simple-spinner v-if="isFetching" />

  <utrecht-alert v-else-if="error && !data" type="error">
    <p>{{ errorMessages[0] }}</p>

    <utrecht-button type="button" :appearance="'primary-action-button'" @click="get()"
      >Ga terug</utrecht-button
    >
  </utrecht-alert>

  <form v-else-if="analytics" class="utrecht-form" @submit.prevent="submit">
    <utrecht-alert v-if="error" type="error">
      <utrecht-paragraph v-for="message in errorMessages" :key="message">{{
        message
      }}</utrecht-paragraph>
    </utrecht-alert>

    <utrecht-form-field>
      <utrecht-form-label for="analytics-tool">Analytics-tool</utrecht-form-label>

      <utrecht-select id="analytics-tool" v-model="analytics.tool" :options="toolOptions" />
    </utrecht-form-field>

    <template v-if="analytics.tool === 'Matomo'">
      <utrecht-form-field>
        <utrecht-form-label for="analytics-url">URL van de Matomo-instantie</utrecht-form-label>

        <utrecht-textbox
          id="analytics-url"
          v-model.trim="analytics.url"
          placeholder="https://"
          pattern="https://.*"
          required
          aria-describedby="analytics-url-error"
        />

        <span id="analytics-url-error" class="form-error"
          >De opgegeven waarde is geen valide URL. Let op dat de URL moet beginnen met
          https://.</span
        >
      </utrecht-form-field>

      <utrecht-form-field>
        <utrecht-form-label for="analytics-identifier">Site-ID</utrecht-form-label>

        <utrecht-textbox
          id="analytics-identifier"
          v-model.trim="analytics.identifier"
          pattern="[0-9]{1,9}"
          required
          aria-describedby="analytics-identifier-error"
        />

        <span id="analytics-identifier-error" class="form-error"
          >Het site-ID is verplicht en moet een getal zijn.</span
        >
      </utrecht-form-field>
    </template>

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
import { computed, ref } from "vue";
import { useCloned } from "@vueuse/core";
import { useFetchApi } from "@/api";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import UtrechtAlert from "@/components/UtrechtAlert.vue";

type Analytics = {
  tool: "None" | "Matomo";
  url: string;
  identifier: string;
};

const toolOptions = [
  { label: "Geen", value: "None" },
  { label: "Matomo", value: "Matomo" }
];

const DEFAULT_ERROR = "Er is een fout opgetreden.";

const success = ref<string | null>(null);

const {
  data,
  isFetching,
  error,
  get: getAnalytics,
  put: putAnalytics
} = useFetchApi(() => "/api/beheer/analyticsa", {
  async onFetchError(ctx) {
    try {
      ctx.error = await ctx.response?.json();
      return ctx;
    } catch {
      return ctx;
    }
  }
}).json<Analytics>();

const { cloned: analytics, isModified, sync } = useCloned(data);

const errorMessages = computed(() => {
  const { errors, message } = (error.value ?? {}) as {
    errors?: Record<string, string[]>;
    message?: string;
  };

  const messages = Object.values(errors ?? {}).flat();

  return messages.length ? messages : [message ?? DEFAULT_ERROR];
});

const get = () => getAnalytics().execute();

const submit = () =>
  putAnalytics(analytics)
    .execute()
    .then(() => {
      if (!error.value) {
        success.value = "Publiceren gelukt.";

        setTimeout(() => (success.value = null), 3000);
      }
    });
</script>
