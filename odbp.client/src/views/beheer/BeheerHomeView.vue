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
          v-model="homepage.videoUrl"
          placeholder="https://"
          :pattern="videoUrlPattern"
        />
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

const { data, isFetching, error, put } = useFetchApi(
  () => "/api/beheer/homepage"
).json<HomepageBeheer>();

const { cloned: homepage, isModified, sync } = useCloned(data);

const submit = async () => {
  if (homepage.value?.videoUrl !== data.value?.videoUrl) console.log('validate video');
  
  await put(homepage).execute();
};
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
}

.gpp-woo-info-popover__trigger {
  --utrecht-button-min-block-size: var(--gpp-woo-popover-trigger-button-size);
  --utrecht-button-min-inline-size: var(--gpp-woo-popover-trigger-button-size);
  --utrecht-button-inline-size: var(--gpp-woo-popover-trigger-button-size);

  font-size: var(--gpp-woo-popover-trigger-font-size);
  block-size: var(--gpp-woo-popover-trigger-button-size);
  padding: 0;
  border: none;
  border-radius: 50%;
  vertical-align: top;
}

.gpp-woo-info-popover__content {
  --utrecht-paragraph-margin-block-start: 0;

  cursor: text;
}
</style>
