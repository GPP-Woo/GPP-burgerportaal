<template>
  <section aria-label="PDF-documentviewer" class="pdf-viewer__container" @keydown="onKeydown">
    <progress
      v-if="progress && progress.loaded != progress.total"
      class="pdf-viewer__progress"
      :value="progress?.loaded"
      :max="progress?.total"
      aria-label="PDF laden"
    >
      PDF laden...
    </progress>

    <utrecht-alert v-if="error" type="error">
      Het PDF-document kon niet worden geladen. Probeer het bestand te downloaden.
    </utrecht-alert>

    <template v-if="pdf">
      <nav class="pdf-viewer__nav" aria-label="PDF paginanavigatie">
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

      <div class="pdf-viewer__page">
        <VuePDF :pdf="pdf" :page="page" text-layer annotation-layer />
      </div>
    </template>
  </section>
</template>

<script setup lang="ts">
import { ref } from "vue";
import { VuePDF, usePDF } from "@tato30/vue-pdf";
import "@tato30/vue-pdf/style.css";
import UtrechtAlert from "@/components/UtrechtAlert.vue";

const { src: pdfSource } = defineProps<{ src: string }>();

const page = ref(1);

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

const progress = ref<{ loaded: number; total: number } | null>(null);
const error = ref(false);

const { pdf, pages } = usePDF(pdfSource, {
  onProgress: (progressData) => (progress.value = progressData),
  onError: () => (error.value = true)
});
</script>

<style lang="scss" scoped>
.pdf-viewer {
  &__container {
    margin-block-start: calc(
      var(--utrecht-space-around, 0) * var(--utrecht-paragraph-margin-block-start, 0)
    );
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
    display: flex;
    justify-content: center;
    border: 1px solid var(--utrecht-color-grey-90);
  }
}
</style>
