<template>
  <utrecht-button type="button" :appearance="'primary-action-button'" @click="open">
    <utrecht-icon icon="eye" />
    Bekijk document
  </utrecht-button>

  <dialog
    ref="dialogRef"
    class="pdf-viewer-dialog"
    aria-label="PDF-documentviewer"
    @click="onBackdropClick"
  >
    <div class="pdf-viewer-dialog__header">
      <p class="pdf-viewer-dialog__title">{{ title }}</p>

      <utrecht-button
        type="button"
        :appearance="'secondary-action-button'"
        class="pdf-viewer-dialog__close"
        autofocus
        @click="close"
      >
        <utrecht-icon icon="xmark" />
        <span class="visually-hidden">Sluiten</span>
      </utrecht-button>
    </div>

    <div class="pdf-viewer-dialog__body" @keydown="onKeydown">
      <template v-if="progress && progress.loaded != progress.total">
        <progress
          class="pdf-viewer-dialog__progress"
          :value="progress.loaded"
          :max="progress.total"
          aria-label="PDF laden"
        >
          PDF laden...
        </progress>

        <p>Document wordt geladen: {{ (progress.loaded / progress.total * 100).toFixed(0) }}%</p>
      </template>

      <utrecht-alert v-if="error" type="error">
        Het PDF-document kon niet worden geladen. Probeer het bestand te downloaden.
      </utrecht-alert>

      <template v-if="pdf">
        <nav class="pdf-viewer-dialog__nav" aria-label="PDF paginanavigatie">
          <utrecht-button
            type="button"
            :disabled="page <= 1"
            :appearance="'secondary-action-button'"
            @click="prevPage"
          >
            « Vorige
          </utrecht-button>

          <span aria-live="polite">Pagina {{ page }} van {{ pages }}</span>

          <utrecht-button
            type="button"
            :disabled="page >= pages"
            :appearance="'secondary-action-button'"
            @click="nextPage"
          >
            Volgende »
          </utrecht-button>
        </nav>

        <div class="pdf-viewer-dialog__page">
          <VuePDF :pdf="pdf" :page="page" text-layer annotation-layer fit-parent />
        </div>
      </template>
    </div>
  </dialog>
</template>

<script setup lang="ts">
import { ref } from "vue";
import { VuePDF, usePDF } from "@tato30/vue-pdf";
import "@tato30/vue-pdf/style.css";
import UtrechtAlert from "@/components/UtrechtAlert.vue";
import UtrechtIcon from "@/components/UtrechtIcon.vue";

const { src, title } = defineProps<{ src: string; title?: string }>();

const dialogRef = ref<HTMLDialogElement>();

const page = ref(1);

const progress = ref<{ loaded: number; total: number } | null>(null);
const error = ref(false);

const pdfSrc = ref("");

const { pdf, pages } = usePDF(pdfSrc, {
  onProgress: (progressData) => (progress.value = progressData),
  onError: () => (error.value = true)
});

function open() {
  if (!pdfSrc.value) {
    pdfSrc.value = src;
  }

  dialogRef.value?.showModal();
}

function close() {
  dialogRef.value?.close();
}

function onBackdropClick(e: MouseEvent) {
  if (e.target === dialogRef.value) close();
}

function prevPage() {
  if (page.value > 1) page.value--;
}

function nextPage() {
  if (pages.value && page.value < pages.value) page.value++;
}

function onKeydown(e: KeyboardEvent) {
  if (e.key === "ArrowLeft") {
    prevPage();
    e.preventDefault();
  } else if (e.key === "ArrowRight") {
    nextPage();
    e.preventDefault();
  }
}
</script>

<style lang="scss" scoped>
.pdf-viewer-dialog {
  &[open] {
    display: flex;
  }

  flex-direction: column;
  inset: 0;
  margin: auto;
  padding: 0;
  border: none;
  border-radius: 0.25rem;
  inline-size: min(96vw, 50rem);
  min-block-size: 12rem;
  max-block-size: 96vh;
  box-shadow: var(--gpp-woo-info-popover-box-shadow);

  &::backdrop {
    background-color: rgba(0, 0, 0, 0.5);
  }

  &__header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    gap: 1rem;
    padding: 1rem;
    background-color: var(--utrecht-button-primary-action-background-color);
    color: var(--utrecht-button-primary-action-color, #fff);
  }

  &__title {
    margin: 0;
    font-weight: bold;
    color: inherit;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }

  &__close {
    flex-shrink: 0;
  }

  &__body {
    flex: 1;
    overflow: auto;
    padding: 1rem;
  }

  &__progress {
    block-size: 0.5rem;
    inline-size: 100%;
    border-radius: 0.25rem;
    appearance: none;
    border: none;
    overflow: hidden;

    &::-webkit-progress-bar {
      background-color: var(--utrecht-color-grey-90);
    }

    &::-webkit-progress-value {
      background-color: var(--utrecht-button-primary-action-background-color);
    }

    &::-moz-progress-bar {
      background-color: var(--utrecht-button-primary-action-background-color);
    }
  }

  &__nav {
    display: flex;
    justify-content: space-between;
    align-items: center;
    gap: 1rem;
    margin-block-end: 0.5rem;
  }

  &__page {
    border: 1px solid var(--utrecht-color-grey-90);
  }
}
</style>
