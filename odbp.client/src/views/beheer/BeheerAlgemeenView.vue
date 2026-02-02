<template>
  <utrecht-heading :level="2">Afbeeldingen</utrecht-heading>

  <simple-spinner v-if="isFetching" />

  <utrecht-alert v-else-if="error" type="error">
    <p>{{ error.message ?? "Er is een fout opgetreden." }}</p>

    <utrecht-button type="button" :appearance="'primary-action-button'" @click="get()"
      >Opnieuw proberen</utrecht-button
    >
  </utrecht-alert>

  <form v-else>
    <afbeelding-fieldset
      class="afbeelding-fieldset--logo"
      title="Logo organisatie"
      :src="logoSrc"
      accept=".svg,.png,.jpg,.jpeg,.gif,.webp"
      help-text="Het logo wordt weergegeven in de header van het portaal. Ondersteunde formaten: SVG, PNG, JPG, GIF, WebP. Maximale bestandsgrootte: 2 MB."
      :is-uploading="state.logo.isUploading"
      :error-message="state.logo.error"
      :success-message="state.logo.success"
      @fileSelected="(file) => uploadImage(`logo`, file)"
    />

    <afbeelding-fieldset
      class="afbeelding-fieldset--favicon"
      title="Favicon"
      :src="faviconSrc"
      accept=".ico,.svg,.png"
      help-text="Het favicon is het kleine pictogram dat in de browser-tab wordt getoond. Ondersteunde formaten: ICO, SVG, PNG. Maximale bestandsgrootte: 512 KB."
      :is-uploading="state.favicon.isUploading"
      :error-message="state.favicon.error"
      :success-message="state.favicon.success"
      @fileSelected="(file) => uploadImage(`favicon`, file)"
    />

    <afbeelding-fieldset
      class="afbeelding-fieldset--image"
      title="Sfeerfoto"
      :src="imageSrc"
      accept=".svg,.png,.jpg,.jpeg,.gif,.webp"
      help-text="De sfeerfoto wordt weergegeven onder de menubalk op de homepage. Ondersteunde formaten: SVG, PNG, JPG, GIF, WebP. Maximale bestandsgrootte: 5 MB."
      preview-class="preview-image"
      :is-uploading="state.image.isUploading"
      :error-message="state.image.error"
      :success-message="state.image.success"
      @fileSelected="(file) => uploadImage(`image`, file)"
    />
  </form>
</template>

<script setup lang="ts">
import { computed, reactive, ref, watch } from "vue";
import { useFetchApi } from "@/api";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import UtrechtAlert from "@/components/UtrechtAlert.vue";
import AfbeeldingFieldset from "@/components/beheer/AfbeeldingFieldset.vue";

type ImageType = "logo" | "favicon" | "image";

type Afbeeldingen = Record<ImageType, string | null>;

type UploadState = {
  isUploading: boolean;
  error: string | null;
  success: string | null;
};

const afbeeldingen = ref<Afbeeldingen>({ logo: null, favicon: null, image: null });

const {
  data,
  isFetching,
  error,
  get: getAfbeeldingen
} = useFetchApi(() => "/api/beheer/afbeeldingen").json<Afbeeldingen>();

watch(data, (value) => (afbeeldingen.value = value ?? afbeeldingen.value));

const get = () => getAfbeeldingen().execute();

const logoSrc = computed(() => `/api/afbeeldingen/${afbeeldingen.value?.logo ?? "logo"}`);
const faviconSrc = computed(() => `/api/afbeeldingen/${afbeeldingen.value?.favicon ?? "favicon"}`);
const imageSrc = computed(() => `/api/afbeeldingen/${afbeeldingen.value?.image ?? "image"}`);

const state = reactive<Record<ImageType, UploadState>>({
  logo: { isUploading: false, error: null, success: null },
  favicon: { isUploading: false, error: null, success: null },
  image: { isUploading: false, error: null, success: null }
});

const uploadImage = async (type: ImageType, file: File) => {
  state[type].isUploading = true;
  state[type].error = null;
  state[type].success = null;

  try {
    const formData = new FormData();

    formData.append("file", file);

    const response = await fetch(`/api/beheer/afbeeldingen/${type}`, {
      method: "POST",
      body: formData
    });

    const data = await response.json();

    if (!response.ok) {
      throw new Error(data.message || "Upload mislukt");
    }

    afbeeldingen.value[type] = data.fileName;

    state[type].success = "Upload gelukt";

    setTimeout(() => (state[type].success = null), 3000);
  } catch (err) {
    state[type].error = err instanceof Error ? err.message : "Upload mislukt";
  } finally {
    state[type].isUploading = false;
  }
};
</script>

<style lang="scss" scoped>
form {
  display: grid;
  grid-template-columns: 1fr 1fr;
  grid-template-areas:
    "logo favicon"
    "image image";
  gap: 2rem;
}

.afbeelding-fieldset {
  &--logo {
    grid-area: logo;
  }

  &--favicon {
    grid-area: favicon;
  }

  &--image {
    grid-area: image;
  }
}

:deep(.preview-image) {
  inline-size: 100%;
  max-height: 12rem;
  object-fit: cover;
}
</style>
