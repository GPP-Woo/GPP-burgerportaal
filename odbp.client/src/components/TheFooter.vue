<template>
  <utrecht-page-footer class="gpp-woo-page-footer">
    <div class="utrecht-page-footer__content">
      <div class="utrecht-page-footer__navigation">
        <ul role="list" class="utrecht-link-list utrecht-link-list--html-ul">
          <template v-for="[key, value] in listItems" :key="key">
            <li v-if="resources?.[key]" class="utrecht-link-list__item">
              <utrecht-link external :href="resources[key]" class="utrecht-link-list__link">
                <span class="utrecht-link-list__link-text">{{ value }}</span>

                <span class="visually-hidden">(externe link)</span>

                <utrecht-icon icon="external" />
              </utrecht-link>
            </li>
          </template>
        </ul>
      </div>
    </div>
  </utrecht-page-footer>
</template>

<script setup lang="ts">
import UtrechtIcon from "@/components/UtrechtIcon.vue";
import { injectResources, type Resources } from "@/resources";

const resources = injectResources();

const listItems = new Map<keyof Resources, string>([
  ["a11yUrl", "Toegankelijkheid"],
  ["privacyUrl", "Privacy"],
  ["contactUrl", "Contact"]
]);
</script>

<style lang="scss" scoped>
.utrecht-link-list {
  flex-flow: row wrap;
  justify-content: center;
  column-gap: var(--gpp-woo-page-footer-link-list-column-gap);
}
</style>
