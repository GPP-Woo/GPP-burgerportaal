<template>
  <utrecht-button type="button" :appearance="'primary-action-button'" @click="openDialog">
    <utrecht-icon icon="eye" /> Bekijk document
  </utrecht-button>

  <dialog
    ref="dialogRef"
    class="gpp-woo-pdf-viewer-dialog"
    :aria-labelledby="headingId"
    @click="onBackdropClick"
  >
    <div class="gpp-woo-pdf-viewer-dialog__header">
      <span class="gpp-woo-pdf-viewer-dialog__title" :id="headingId">{{ title }}</span>

      <utrecht-button
        type="button"
        :appearance="'secondary-action-button'"
        class="gpp-woo-pdf-viewer-dialog__close"
        autofocus
        @click="closeDialog"
      >
        <utrecht-icon icon="xmark" />

        <span class="visually-hidden">Sluiten</span>
      </utrecht-button>
    </div>

    <div class="gpp-woo-pdf-viewer-dialog__body" @keydown="onKeydown">
      <utrecht-alert v-if="error" type="error">
        Het PDF-document kon niet worden geladen. Probeer het bestand te downloaden.
      </utrecht-alert>

      <template v-else-if="pdf">
        <nav class="gpp-woo-pdf-viewer-dialog__nav" aria-label="PDF paginanavigatie">
          <utrecht-button
            type="button"
            :aria-label="'Vorige pagina'"
            :disabled="page <= 1"
            :appearance="'secondary-action-button'"
            @click="prevPage"
            >« Vorige</utrecht-button
          >

          <span aria-live="polite">Pagina {{ page }} van {{ pages }}</span>

          <utrecht-button
            type="button"
            :aria-label="'Volgende pagina'"
            :disabled="page >= pages"
            :appearance="'secondary-action-button'"
            @click="nextPage"
            >Volgende »</utrecht-button
          >
        </nav>

        <div class="gpp-woo-pdf-viewer-dialog__page">
          <VuePDF :pdf="pdf" :page="page" text-layer annotation-layer fit-parent />
        </div>
      </template>

      <template v-else>
        <gpp-woo-progress :loaded="progress?.loaded" :total="progress?.total" />

        <p>
          Document wordt geladen:
          {{ !progress ? 0 : ((progress.loaded / progress.total) * 100).toFixed(0) }}%
        </p>
      </template>
    </div>
  </dialog>
</template>

<script setup lang="ts">
import { onUnmounted, ref, useId } from "vue";
import { VuePDF, usePDF } from "@tato30/vue-pdf";
import "@tato30/vue-pdf/style.css";
import UtrechtAlert from "@/components/UtrechtAlert.vue";
import UtrechtIcon from "@/components/UtrechtIcon.vue";
import GppWooProgress from "@/components/GppWooProgress.vue";

const { src, title } = defineProps<{ src: string; title?: string }>();

const headingId = useId();

const dialogRef = ref<HTMLDialogElement>();

const page = ref(1);

const error = ref(false);
const progress = ref<{ loaded: number; total: number } | null>(null);

const pdfSrc = ref("");

const { pdf, pages } = usePDF(pdfSrc, {
  onProgress: (progressData) => (progress.value = progressData),
  onError: () => {
    error.value = true;
    progress.value = null;
    pdfSrc.value = "";
  }
});

const openDialog = () => {
  if (!pdfSrc.value) pdfSrc.value = src;

  dialogRef.value?.showModal();
};

const closeDialog = () => dialogRef.value?.close();

const onBackdropClick = (e: MouseEvent) => {
  if (e.target === dialogRef.value) closeDialog();
};

const prevPage = () => {
  if (page.value > 1) page.value--;
};

const nextPage = () => {
  if (pages.value && page.value < pages.value) page.value++;
};

function onKeydown(e: KeyboardEvent) {
  if (e.key === "ArrowLeft") {
    prevPage();
    e.preventDefault();
  } else if (e.key === "ArrowRight") {
    nextPage();
    e.preventDefault();
  }
}

onUnmounted(() => pdf.value?.destroy());
</script>

<style lang="scss" scoped>
@use "@/assets/variables";

.gpp-woo-pdf-viewer-dialog {
  &[open] {
    display: flex;
  }

  flex-direction: column;
  inset: 0;
  margin: auto;
  padding: 0;
  border: none;
  inline-size: var(--gpp-woo-dialog-max-inline-size);
  max-block-size: var(--gpp-woo-dialog-max-block-size);
  border-radius: var(--gpp-woo-dialog-border-radius);

  &::backdrop {
    background-color: var(--gpp-woo-dialog-backdrop-background-color);
  }

  &__header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    gap: var(--gpp-woo-dialog-spacing-default);
    padding: var(--gpp-woo-dialog-spacing-default);
    background-color: var(--utrecht-button-primary-action-background-color);
    color: var(--utrecht-button-primary-action-color);
  }

  &__title {
    display: block;
    font-weight: bold;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }

  &__close {
    padding-inline-start: var(--utrecht-button-padding-block-start);
    padding-inline-end: var(--utrecht-button-padding-block-end);
  }

  &__nav {
    display: flex;

    justify-content: space-between;
    align-items: center;
    margin-block-end: var(--gpp-woo-dialog-spacing-small);
  }

  &__body {
    flex: 1;
    overflow: auto;
    padding: var(--gpp-woo-dialog-spacing-default);
  }
}
</style>
