<template>
  <slot :open="openDialog">
    <utrecht-button type="button" :appearance="'primary-action-button'" @click="openDialog">
      <utrecht-icon icon="eye" /> Bekijk document
    </utrecht-button>
  </slot>

  <dialog
    ref="dialogRef"
    class="gpp-woo-pdf-viewer-dialog"
    :aria-labelledby="headingId"
    @click="onBackdropClick"
    @close="onDialogClose"
    @keydown="onKeydown"
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

    <div class="gpp-woo-pdf-viewer-dialog__body">
      <template v-if="loading">
        <gpp-woo-progress :percent="progressPercent" />

        <p>Document wordt geladen: {{ progressPercent ?? 0 }}%</p>
      </template>

      <utrecht-alert v-else-if="error" type="error">
        Het PDF-document kon niet worden geladen. Probeer het bestand te downloaden.
      </utrecht-alert>

      <template v-else>
        <nav class="gpp-woo-pdf-viewer-dialog__nav" aria-label="PDF paginanavigatie">
          <utrecht-button
            type="button"
            :disabled="page <= 1"
            :appearance="'secondary-action-button'"
            @click="prevPage"
          >
            <utrecht-icon icon="angle-left" />

            <span class="visually-hidden">Vorige pagina</span>
          </utrecht-button>

          <span aria-live="polite">Pagina {{ page }} van {{ pages }}</span>

          <utrecht-button
            type="button"
            :disabled="page >= pages"
            :appearance="'secondary-action-button'"
            @click="nextPage"
          >
            <utrecht-icon icon="angle-right" />

            <span class="visually-hidden">Volgende pagina</span>
          </utrecht-button>
        </nav>

        <div ref="pageWrapperRef" class="gpp-woo-pdf-viewer-dialog__page">
          <VuePDF ref="vuePdfRef" :pdf="pdf" :page="page" text-layer annotation-layer fit-parent />
        </div>
      </template>
    </div>
  </dialog>
</template>

<script setup lang="ts">
import { computed, onUnmounted, ref, shallowRef, useId } from "vue";
import { useDebounceFn, useResizeObserver } from "@vueuse/core";
import { VuePDF } from "@tato30/vue-pdf";
import "@tato30/vue-pdf/style.css";
// pdfjs-dist: transitive dependency, exact-pinned by @tato30/vue-pdf, so no risk of
// duplicate installs. Used directly (bypassing usePDF) in loadDocument() below.
import * as PDFJS from "pdfjs-dist";
import type { OnProgressParameters, PDFDocumentLoadingTask } from "pdfjs-dist";
import UtrechtAlert from "@/components/UtrechtAlert.vue";
import UtrechtIcon from "@/components/UtrechtIcon.vue";
import GppWooProgress from "@/components/GppWooProgress.vue";

const props = defineProps<{ src: string; title?: string }>();

const headingId = useId();

const dialogRef = ref<HTMLDialogElement>();
const pageWrapperRef = ref<HTMLDivElement>();
const vuePdfRef = ref<InstanceType<typeof VuePDF>>();

const pdf = shallowRef<PDFDocumentLoadingTask>();
const pages = ref(0);
const page = ref(1);

const loading = ref(false);
const error = ref(false);
const progress = ref<OnProgressParameters | null>(null);

const progressPercent = computed(() =>
  progress.value?.total ? Math.round(progress.value.percent) : undefined
);

// loadDocument sets pdf.value synchronously (not via usePDF, which only assigns it
// once loaded) so destroy() always has a valid task, even mid-load.
function loadDocument(url: string) {
  pdf.value?.destroy();

  // disableRange: backend chain doesn't support HTTP Range requests yet, skip
  // pdf.js' probe request and go straight to streaming.
  const task = PDFJS.getDocument({ url, disableRange: true });

  task.onProgress = (progressData: OnProgressParameters) => (progress.value = progressData);

  task.promise.then(
    (doc) => {
      pages.value = doc.numPages;
      loading.value = false;
    },
    () => {
      task.destroy();
      error.value = true;
      loading.value = false;
    }
  );

  pdf.value = task;
}

// fit-parent only recalculates scale when VuePDF re-renders (page/scale/rotation
// change), not on viewport/container resize, so trigger it manually.
useResizeObserver(
  pageWrapperRef,
  useDebounceFn(() => vuePdfRef.value?.reload(), 200)
);

const openDialog = () => {
  dialogRef.value?.showModal();
  // always reload instead of reusing: don't want every viewed doc kept in memory
  // when run as multiple instances on overview pages.
  loading.value = true;
  error.value = false;
  progress.value = null;
  loadDocument(props.src);
};

const closeDialog = () => dialogRef.value?.close();

const onDialogClose = () => pdf.value?.destroy();

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

// covers case of navigating away while the dialog is still open.
onUnmounted(() => pdf.value?.destroy());
</script>

<style lang="scss" scoped>
.gpp-woo-pdf-viewer-dialog {
  --utrecht-button-padding-inline-start: var(--utrecht-button-padding-block-start);
  --utrecht-button-padding-inline-end: var(--utrecht-button-padding-block-end);

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
    background-color: var(--gpp-woo-dialog-header-background-color);
    color: var(--gpp-woo-dialog-header-color);
  }

  &__title {
    display: block;
    font-weight: bold;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }

  &__nav {
    display: flex;
    justify-content: space-between;
    align-items: center;
    gap: var(--gpp-woo-dialog-spacing-small);
    text-align: center;
    margin-block-end: var(--gpp-woo-dialog-spacing-small);
  }

  &__body {
    flex: 1;
    overflow: auto;
    padding: var(--gpp-woo-dialog-spacing-default);
  }
}
</style>
