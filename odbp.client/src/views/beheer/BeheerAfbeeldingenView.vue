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
      :help-text="helpLogo"
      :is-uploading="state.logo.isUploading"
      :error="state.logo.error"
      :success="state.logo.success"
      @fileSelected="(file) => uploadImage(`logo`, file)"
    />

    <afbeelding-fieldset
      class="afbeelding-fieldset--favicon"
      title="Favicon"
      :src="faviconSrc"
      accept=".ico,.svg,.png"
      :help-text="helpFavicon"
      :is-uploading="state.favicon.isUploading"
      :error="state.favicon.error"
      :success="state.favicon.success"
      @fileSelected="(file) => uploadImage(`favicon`, file)"
    />

    <afbeelding-fieldset
      class="afbeelding-fieldset--image"
      title="Sfeerfoto"
      :src="imageSrc"
      accept=".svg,.png,.jpg,.jpeg,.gif,.webp"
      :help-text="helpImage"
      preview-class="preview-image"
      :is-uploading="state.image.isUploading"
      :error="state.image.error"
      :success="state.image.success"
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

const helpLogo = `Het logo wordt in het burgerportaal linksboven op iedere pagina getoond. De beschikbare ruimte voor het logo wordt bepaald door de NL Design System tokens van de organisatie. Het logo wordt automatisch geschaald binnen deze ruimte. Een liggend (landscape) logo werkt over het algemeen het beste. De volgende bestandsformaten worden ondersteund: SVG, PNG, JPG, GIF, WebP. Maximale bestandsgrootte: 2 MB.`;
const helpFavicon = `Het favicon wordt door webbrowsers getoond in de URL-balk en/of op het browser-tabblad. Het wordt zeer klein weergegeven (vaak 16×16 of 32×32 pixels), dus gebruik bij voorkeur een eenvoudig, herkenbaar icoon. De volgende bestandsformaten worden ondersteund: ICO, SVG, PNG. Maximale bestandsgrootte: 512 KB.`;
const helpImage = `De sfeerfoto wordt in het burgerportaal op iedere pagina bovenaan onder de menubalk getoond. De afbeelding wordt over de volledige breedte van de pagina weergegeven en automatisch bijgesneden (niet vervormd). De zichtbare hoogte en breedte hangen af van de schermgrootte en de inhoud die over de foto heen wordt getoond. Gebruik een liggende (landscape) afbeelding met voldoende resolutie (minimaal 1920 pixels breed aanbevolen) voor een goed resultaat op grote schermen. De volgende bestandsformaten worden ondersteund: SVG, PNG, JPG, GIF, WebP. Maximale bestandsgrootte: 5 MB.`;

type ImageType = "logo" | "favicon" | "image";

type UploadState = {
  isUploading: boolean;
  error: string | null;
  success: string | null;
};

type Afbeeldingen = Record<ImageType, string | null>;

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

  if (file.name.endsWith(".svg") && !(await isValidSvg(file))) {
    state[type].error =
      "Upload mislukt: controleer of het bestand een geldig SVG-bestand is en een viewBox-attribuut bevat.";
    state[type].isUploading = false;

    return;
  }

  try {
    const body = new FormData();

    body.append("file", file);

    const response = await fetch(`/api/beheer/afbeeldingen/${type}`, { method: "POST", body });

    const data = await response.json();

    if (!response.ok) {
      throw new Error(data.message || "Upload mislukt.");
    }

    afbeeldingen.value[type] = data.fileName;

    state[type].success = "Upload gelukt.";

    setTimeout(() => (state[type].success = null), 3000);
  } catch (err) {
    state[type].error = err instanceof Error ? err.message : "Upload mislukt.";
  } finally {
    state[type].isUploading = false;
  }
};

async function isValidSvg(file: File): Promise<boolean> {
  return new Promise((resolve) => {
    const reader = new FileReader();

    reader.onload = () => {
      const parser = new DOMParser();
      const doc = parser.parseFromString(reader.result as string, "image/svg+xml");
      const svg = doc.querySelector("svg");

      // check SVG parses and has viewBox attribute (basic check for valid SVG)
      resolve(svg?.hasAttribute("viewBox") ?? false);
    };

    reader.onerror = () => resolve(false);

    reader.readAsText(file);
  });
}
</script>

<style lang="scss" scoped>
@use "@/assets/variables";

form {
  display: grid;
  grid-template-areas:
    "logo"
    "favicon"
    "image";
  gap: 2rem;

  @media screen and (min-width: #{variables.$breakpoint-md}) {
    grid-template-columns: 1fr 1fr;
    grid-template-areas:
      "logo favicon"
      "image image";
  }
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
