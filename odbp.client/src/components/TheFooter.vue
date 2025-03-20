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

    <div v-if="versionInfo" class="gpp-woo-page-footer__version-info">
      <span v-if="versionInfo.semanticVersion">Versie: {{ versionInfo.semanticVersion }}</span>
      <span v-if="versionInfo.gitSha">Commit: {{ versionInfo.gitSha }}</span>
    </div>
  </utrecht-page-footer>
</template>

<script setup lang="ts">
import UtrechtIcon from "@/components/UtrechtIcon.vue";
import { injectResources, type Resources } from "@/resources";
import { useLoader } from "@/composables/use-loader";

const resources = injectResources();
const { data: versionInfo } = useLoader<{ semanticVersion?: string; gitSha?: string }>(() =>
  fetch("/api/environment/version").then((r) => (r.ok ? r.json() : undefined))
);

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

.gpp-woo-page-footer {
  position: relative;

  &__version-info {
    font-size: 8px;
    position: absolute;
    left: 50%;
    bottom: 0;
    translate: -50% -100%;
    display: flex;
    gap: 1rem;
  }
}
</style>
