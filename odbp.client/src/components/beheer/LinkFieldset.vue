<template>
  <utrecht-form-field>
    <utrecht-form-label :for="`link-${linkId}`">
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

        <utrecht-paragraph class="gpp-woo-info-popover__content gpp-woo-pre-wrap">{{
          helpText
        }}</utrecht-paragraph>
      </gpp-woo-info-popover>
    </utrecht-form-label>

    <utrecht-textbox
      :id="`link-${linkId}`"
      v-model.trim="link"
      placeholder="https://"
      pattern="https://.*"
      :aria-describedby="`linkError-${linkId}`"
    />

    <span :id="`linkError-${linkId}`" class="form-error"
      >De opgegeven waarde is geen valide URL. Let op dat de URL moet beginnen met https://.</span
    >
  </utrecht-form-field>
</template>

<script setup lang="ts">
import { useId } from "vue";
import GppWooInfoPopover from "@/components/GppWooInfoPopover.vue";

defineProps<{ title: string; helpText?: string }>();

const link = defineModel<string>("link", { required: true });

const linkId = useId();
</script>
