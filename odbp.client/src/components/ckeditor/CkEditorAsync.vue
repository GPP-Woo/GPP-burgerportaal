<template>
  <ckeditor :editor="ClassicEditor" :config="config" v-model="modelValue" v-bind="$attrs" />
</template>

<script setup lang="ts">
import { useModel } from "vue";
import type { EditorConfig } from "./ckeditor-exports";

const props = defineProps<{ modelValue?: string }>();
const modelValue = useModel(props, "modelValue");

// we don't want to import directly from ckeditor
// see ./ckeditor-exports for an explanation of this workaround
const { ClassicEditor, Ckeditor, ...plugins } = await import("./ckeditor-exports");

const config: EditorConfig = {
  plugins: Object.values(plugins),
  toolbar: ["heading", "|", "link", "bulletedList", "numberedList"],
  licenseKey: "GPL",
  heading: {
    options: [
      {
        model: "heading1",
        view: { name: "h1", classes: "utrecht-heading-1" },
        title: "Heading 1",
        class: "ck-heading_heading1"
      },
      {
        model: "heading2",
        view: { name: "h2", classes: "utrecht-heading-2" },
        title: "Heading 2",
        class: "ck-heading_heading2"
      }
    ]
  }
};
</script>

<style src="ckeditor5/ckeditor5.css"></style>
