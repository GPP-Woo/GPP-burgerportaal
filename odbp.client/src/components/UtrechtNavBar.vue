<template>
  <nav class="utrecht-nav-bar" aria-label="Hoofdmenu">
    <div class="utrecht-nav-bar__content">
      <ul role="list" class="utrecht-nav-list" id="menu">
        <li v-for="{ name, label } in filteredLinks" :key="name" class="utrecht-nav-list__item">
          <component
            :is="$route.name !== name ? 'router-link' : 'span'"
            :to="$route.name !== name ? { name } : undefined"
            class="utrecht-nav-list__link"
            :class="{
              'utrecht-link utrecht-link--html-a': $route.name !== name
            }"
          >
            {{ label }}

            <span v-if="$route.name === name" class="visually-hidden">(active link)</span>
          </component>
        </li>

        <li v-if="resources?.websiteUrl" class="utrecht-nav-list__item">
          <utrecht-link
            external
            :href="resources.websiteUrl"
            class="utrecht-nav-list__link gpp-woo-link--icon"
          >
            Naar {{ resources.organisationLabel }}

            <span class="visually-hidden">(externe link)</span>

            <utrecht-icon icon="external" />
          </utrecht-link>
        </li>
      </ul>
    </div>
  </nav>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { injectResources } from "@/resources";
import UtrechtIcon from "@/components/UtrechtIcon.vue";
import { lijsten } from "@/stores/lijsten";

const resources = injectResources();

const links = [
  { name: "home", label: "Home" },
  { name: "zoeken", label: "Zoeken" },
  { name: "onderwerpen", label: "Onderwerpen" }
] as const;

// Hide link to Onderwerpen if no onderwerpen
const filteredLinks = computed(() =>
  lijsten.value?.onderwerpen.length ? links : links.filter(({ name }) => name !== "onderwerpen")
);
</script>

<style lang="scss" scoped>
.gpp-woo-link--icon {
  --utrecht-link-icon-size: 0.75rem;
}
</style>
