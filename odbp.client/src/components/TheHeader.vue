<template>
  <utrecht-page-header>
    <utrecht-skip-link href="#main">Naar inhoud</utrecht-skip-link>

    <utrecht-skip-link href="#menu">Naar menu</utrecht-skip-link>

    <router-link
      :to="{ name: 'home' }"
      class="utrecht-link utrecht-link--html-a utrecht-link--box-content"
    >
      <figure v-if="svg" v-html="svg" class="utrecht-logo"></figure>

      <figure v-else-if="resources?.logoUrl" class="utrecht-logo">
        <img :src="resources.logoUrl" :alt="`Logo ${resources.name}`" crossorigin="anonymous" />
      </figure>
    </router-link>

    <utrecht-nav-bar />
  </utrecht-page-header>
</template>

<script lang="ts" setup>
import { computed } from "vue";
import UtrechtNavBar from "./UtrechtNavBar.vue";
import { injectResources } from "@/resources";

const resources = injectResources();

const svg = computed(() => {
  if (!resources?.logoUrl?.endsWith(`.svg`)) return;

  const svgTemplateId = btoa(resources.logoUrl);

  return (document.getElementById(svgTemplateId) as HTMLTemplateElement)?.innerHTML;
});
</script>
