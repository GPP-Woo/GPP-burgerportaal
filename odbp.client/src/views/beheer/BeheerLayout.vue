<template>
  <the-header>
    <template #nav-bar>
      <nav class="utrecht-nav-bar" aria-label="Beheermenu">
        <div class="utrecht-nav-bar__content">
          <ul role="list" class="utrecht-nav-list" id="menu">
            <li
              v-for="[name, label] in Object.entries(links)"
              :key="name"
              class="utrecht-nav-list__item"
            >
              <component
                :is="$route.name !== name ? 'router-link' : 'span'"
                :to="$route.name !== name ? { name } : undefined"
                class="utrecht-nav-list__link gpp-woo-link--icon"
                :class="{
                  'utrecht-link utrecht-link--html-a': $route.name !== name
                }"
              >
                {{ label }}

                <utrecht-icon icon="pen" />

                <span v-if="$route.name === name" class="visually-hidden">(active link)</span>
              </component>
            </li>

            <li class="utrecht-nav-list__item">
              <utrecht-link href="/api/logoff" class="utrecht-nav-list__link gpp-woo-link--icon">
                Uitloggen

                <utrecht-icon icon="logout" />
              </utrecht-link>
            </li>

            <li class="utrecht-nav-list__item">
              <span v-if="user" class="utrecht-nav-list__link gpp-woo-link--icon">
                <utrecht-icon icon="user" />

                {{ user.fullName }}
              </span>
            </li>
          </ul>
        </div>
      </nav>
    </template>
  </the-header>

  <section class="utrecht-page utrecht-page-content">
    <utrecht-heading :level="1">GPP-Woo Burgerportaal (beheer)</utrecht-heading>

    <router-view />
  </section>

  <the-footer />
</template>

<script setup lang="ts">
import TheHeader from "@/components/TheHeader.vue";
import TheFooter from "@/components/TheFooter.vue";
import UtrechtIcon from "@/components/UtrechtIcon.vue";
import { useLoader } from "@/composables/use-loader";
import { fetchAuthUser, type AuthUser } from "@/api/auth";

const { data: user } = useLoader<AuthUser | null>(() => fetchAuthUser());

const links = {
  "beheer-home": "Homepage",
  "beheer-afbeeldingen": "Afbeeldingen"
} as const;
</script>

<style lang="scss" scoped>
.utrecht-nav-list__item:last-child {
  border-inline-start: 1px solid;
}

.utrecht-page {
  box-sizing: border-box;
  inline-size: 100%;
}
</style>

<style lang="scss">
.gpp-woo-link--icon {
  --utrecht-icon-size: 1rem;
}

.gpp-woo-info-popover__trigger {
  --utrecht-button-min-block-size: var(--gpp-woo-popover-trigger-button-size);
  --utrecht-button-min-inline-size: var(--gpp-woo-popover-trigger-button-size);
  --utrecht-button-inline-size: var(--gpp-woo-popover-trigger-button-size);

  font-size: var(--gpp-woo-popover-trigger-font-size);
  font-weight: 400;
  block-size: var(--gpp-woo-popover-trigger-button-size);
  padding: 0;
  border: none;
  border-radius: 50%;
  vertical-align: top;
}

.gpp-woo-info-popover__content {
  --utrecht-paragraph-margin-block-start: 0;

  cursor: text;
}
</style>
