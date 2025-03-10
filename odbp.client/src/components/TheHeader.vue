<template>
  <utrecht-page-header>
    <utrecht-skip-link href="#main">Naar inhoud</utrecht-skip-link>

    <utrecht-skip-link href="#menu">Naar menu</utrecht-skip-link>

    <router-link
      :to="{ name: 'home' }"
      class="utrecht-link utrecht-link--html-a utrecht-link--box-content"
    >
      <figure v-if="svgId" class="utrecht-logo">
        <svg>
          <use :xlink:href="`#${svgId}`"></use>
        </svg>
      </figure>

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

const svgId = computed(() =>
  resources?.logoUrl?.endsWith(`.svg`) ? btoa(resources.logoUrl) : null
);
</script>
