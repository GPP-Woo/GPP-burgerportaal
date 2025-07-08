<template>
  <utrecht-page-header>
    <utrecht-skip-link href="#main">Naar inhoud</utrecht-skip-link>

    <utrecht-skip-link href="#menu">Naar menu</utrecht-skip-link>

    <component
      :is="$route.name !== 'home' ? 'router-link' : 'span'"
      :to="$route.name !== 'home' ? { name: 'home' } : undefined"
      class="utrecht-link--box-content"
      :class="{
        'utrecht-link utrecht-link--html-a': $route.name !== 'home'
      }"
    >
      <figure v-if="svg" v-html="svg" class="utrecht-logo"></figure>

      <figure v-else-if="resources?.logoUrl" class="utrecht-logo">
        <img :src="resources.logoUrl" :alt="`Logo ${resources.name}`" crossorigin="anonymous" />
      </figure>
    </component>

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

<style lang="scss" scoped>
@use "@/assets/variables";

.utrecht-page-header {
  grid-template-columns: 1fr;
  align-items: end;
  gap: var(--gpp-woo-header-gap);
  padding-inline-end: var(--gpp-woo-header-padding-inline-end);
  padding-inline-start: var(--gpp-woo-header-padding-inline-start);

  @media screen and (min-width: #{variables.$breakpoint-md}) {
    & {
      grid-template-columns: 1fr auto;
    }
  }
}
</style>
