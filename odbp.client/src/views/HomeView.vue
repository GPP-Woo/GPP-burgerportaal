<template>
  <article v-html="html" class="utrecht-article"></article>
</template>

<script lang="ts" setup>
import { computed } from "vue";
import { injectResources } from "@/resources";
import { sanitizeHtml } from "@/helpers";

const resources = injectResources();

const html = computed(() => (resources?.welcome ? sanitizeHtml(resources.welcome) : ""));
</script>
