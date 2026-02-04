<template>
  <utrecht-fieldset class="afbeelding-fieldset">
    <utrecht-legend>
      {{ title }}

      <gpp-woo-info-popover v-if="helpText">
        <template #trigger="{ triggerProps }">
          <utrecht-button
            v-bind="triggerProps"
            appearance="primary-action-button"
            class="gpp-woo-info-popover__trigger"
            >?</utrecht-button
          >
        </template>

        <utrecht-paragraph class="gpp-woo-info-popover__content gpp-woo-pre-wrap">
          {{ helpText }}
        </utrecht-paragraph>
      </gpp-woo-info-popover>
    </utrecht-legend>

    <div class="afbeelding-fieldset__preview">
      <img
        :src="src"
        :alt="`${title} preview`"
        class="preview-img"
        :class="previewClass || undefined"
      />
    </div>

    <utrecht-form-field>
      <utrecht-form-label>
        <input
          ref="fileInput"
          type="file"
          :accept="accept"
          class="visually-hidden"
          @change="onFileSelected"
        />

        <utrecht-button
          type="button"
          :appearance="'secondary-action-button'"
          :disabled="isUploading"
          tabindex="-1"
        >
          {{ isUploading ? "Uploaden..." : "Vervangen" }}
        </utrecht-button>
      </utrecht-form-label>
    </utrecht-form-field>

    <utrecht-alert v-if="error" type="error">
      {{ error }}
    </utrecht-alert>

    <utrecht-alert v-if="success" type="ok">
      {{ success }}
    </utrecht-alert>
  </utrecht-fieldset>
</template>

<script setup lang="ts">
import { ref } from "vue";
import UtrechtAlert from "@/components/UtrechtAlert.vue";
import GppWooInfoPopover from "@/components/GppWooInfoPopover.vue";

defineProps<{
  title: string;
  src: string;
  accept: string;
  helpText?: string;
  previewClass?: string;
  isUploading: boolean;
  error: string | null;
  success: string | null;
}>();

const emit = defineEmits<{ fileSelected: [file: File] }>();

const fileInput = ref<HTMLInputElement>();

const onFileSelected = (event: Event) => {
  const input = event.target as HTMLInputElement;
  const file = input.files?.[0];

  if (file) emit("fileSelected", file);

  if (fileInput.value) fileInput.value.value = "";
};
</script>

<style lang="scss" scoped>
.afbeelding-fieldset {
  padding: 1rem;
  margin: 0;
  border: 1px solid #e5e5e5;
}

.afbeelding-fieldset__preview {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 5rem;
  padding: 1rem;
  margin-block: 1rem;
  background: #f5f5f5;
}

.preview-img {
  max-width: 100%;
  max-height: 5rem;
  height: auto;
  object-fit: contain;
}

.utrecht-form-label {
  display: inline-block;
  font-weight: 400;
  cursor: pointer;

  .utrecht-button {
    pointer-events: none;
  }

  &:focus-within .utrecht-button {
    outline-color: var(--utrecht-focus-outline-color);
    outline-offset: var(--utrecht-focus-outline-offset);
    outline-style: var(--utrecht-focus-outline-style);
    outline-width: var(--utrecht-focus-outline-width);
  }
}

.utrecht-alert {
  margin-block-start: 1rem;
}
</style>
