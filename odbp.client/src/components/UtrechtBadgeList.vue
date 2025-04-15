<template>
  <ul class="utrecht-badge-list">
    <li v-for="(badge, index) in badges" :key="index">
      <router-link
        v-if="badge.url"
        :to="badge.url"
        class="utrecht-data-badge gpp-woo-data-badge-link"
        >{{ badge.naam }}</router-link
      >

      <span v-else class="utrecht-data-badge">{{ badge.naam }}</span>
    </li>
  </ul>
</template>

<script setup lang="ts">
export type BadgeListItem = { naam: string; url?: string };

defineProps<{ badges: BadgeListItem[] }>();
</script>

<style lang="scss" scoped>
.utrecht-badge-list {
  list-style: none;
  margin: 0;
  padding: 0;

  .utrecht-data-badge {
    display: block;
  }

  .gpp-woo-data-badge-link {
    --utrecht-data-badge-background-color: var(--gpp-woo-data-badge-link-background-color);

    text-decoration: underline;

    &:focus-visible {
      outline-color: var(--utrecht-focus-outline-color);
      outline-offset: var(--utrecht-focus-outline-offset);
      outline-style: var(--utrecht-focus-outline-style);
      outline-width: var(--utrecht-focus-outline-width);
    }
  }
}
</style>
