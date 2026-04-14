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
      <gpp-woo-logo
        :logo-url="resources?.logoUrl"
        :organisation-name="resources?.organisationName"
      />
    </component>

    <slot name="nav-bar"></slot>
  </utrecht-page-header>
</template>

<script lang="ts" setup>
import { injectResources } from "@/resources";
import GppWooLogo from "@/components/GppWooLogo.vue";

const resources = injectResources();
</script>

<style lang="scss" scoped>
@use "@/assets/variables";

.utrecht-page-header {
  grid-template-columns: 1fr;
  gap: var(--gpp-woo-header-gap);
  align-items: var(--gpp-woo-header-align-items);
  padding-inline-end: var(--gpp-woo-header-padding-inline-end);
  padding-inline-start: var(--gpp-woo-header-padding-inline-start);

  @media screen and (min-width: #{variables.$breakpoint-md}) {
    & {
      grid-template-columns: 1fr auto;
    }
  }
}
</style>
