<template>
  <utrecht-heading :level="2" :id="headingId">Homepage</utrecht-heading>

  <simple-spinner v-if="isFetching" />

  <utrecht-alert v-else-if="error" type="error">
    <p>{{ error.message ?? "Er is een fout opgetreden." }}</p>

    <utrecht-button type="button" :appearance="'primary-action-button'" @click="get()"
      >Ga terug</utrecht-button
    >
  </utrecht-alert>

  <form v-else-if="homepage" class="utrecht-form" @submit.prevent="submit">
    <utrecht-fieldset :labelledby="headingId">
      <utrecht-form-field>
        <utrecht-form-label for="welcome">Welkomsttekst</utrecht-form-label>

        <ck-editor v-model="homepage.welcome" id="welcome" />
      </utrecht-form-field>

      <utrecht-form-field>
        <utrecht-form-label for="videoUrl">
          Promotie- of instructievideo

          <gpp-woo-info-popover>
            <template #trigger="{ triggerProps }">
              <utrecht-button
                v-bind="triggerProps"
                appearance="primary-action-button"
                class="gpp-woo-info-popover__trigger"
                >?</utrecht-button
              >
            </template>

            <utrecht-paragraph class="gpp-woo-info-popover__content gpp-woo-pre-wrap"
              >De video wordt op de homepage naast de welkomsttekst getoond. De waarde moet een
              YouTube of Vimeo embed URL zijn, bijvoorbeeld
              <em>https://www.youtube.com/embed/dQw4w9WgXcQ</em> of
              <em>https://player.vimeo.com/video/123456789</em>.
            </utrecht-paragraph>

            <utrecht-paragraph class="gpp-woo-info-popover__content gpp-woo-pre-wrap"
              ><strong>Let op:</strong> Cross-Origin-Embedder-Policy wordt uitgeschakeld wanneer
              deze variabele is ingesteld. De embedder is verantwoordelijk voor juiste
              toegankelijkheid van video content, inclusief ondertiteling en toetsenbordnavigatie.
            </utrecht-paragraph>
          </gpp-woo-info-popover>
        </utrecht-form-label>

        <utrecht-textbox
          id="videoUrl"
          v-model.trim="homepage.videoUrl"
          placeholder="https://"
          :pattern="videoUrlPattern"
          aria-describedby="videoUrlError"
        />

        <span id="videoUrlError" class="form-error"
          >De waarde moet een geldig YouTube of Vimeo embed URL zijn.</span
        >
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
import CkEditor from "@/components/ckeditor";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import UtrechtAlert from "@/components/UtrechtAlert.vue";
import GppWooInfoPopover from "@/components/GppWooInfoPopover.vue";

const videoUrlPattern =
  "https://((www\\.)?(youtube\\.com|youtube-nocookie\\.com)/embed/[\\w\\-]+|player\\.vimeo\\.com/video/\\d+).*";

type HomepageBeheer = { welcome: string; videoUrl: string };

const headingId = useId();

const {
  data,
  isFetching,
  error,
  get: getHomepage,
  put: putHomepage
} = useFetchApi(() => "/api/beheer/homepage", {
  async onFetchError(ctx) {
    try {
      ctx.error = await ctx.response?.json();
      return ctx;
    } catch {
      return ctx;
    }
  }
}).json<HomepageBeheer>();

const { cloned: homepage, isModified, sync } = useCloned(data);

const get = () => getHomepage().execute();

const submit = () => putHomepage(homepage).execute();
</script>

<style lang="scss" scoped>
.utrecht-form {
  --utrecht-form-field-margin-block-end: 1rem;
  --utrecht-form-field-margin-block-start: 1rem;

  .utrecht-form-label {
    display: block;
    margin-block-end: 0.5rem;
  }

  .form-actions {
    display: flex;
    justify-content: space-between;
  }

  .form-error {
    display: block;
    color: var(--utrecht-color-invalid);
    margin-block-start: 0.5rem;
  }

  .utrecht-textbox {
    & + .form-error {
      display: none;
    }

    &:user-invalid {
      & + .form-error {
        display: block;
      }
    }
  }
}
</style>
