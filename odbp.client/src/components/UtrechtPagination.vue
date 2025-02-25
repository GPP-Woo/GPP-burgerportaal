<template>
  <nav class="utrecht-pagination">
    <span class="utrecht-pagination__before vorige">
      <router-link
        v-if="prev"
        :to="prev.href"
        class="utrecht-pagination__relative-link utrecht-pagination__relative-link--prev"
        rel="prev"
      >
        <span class="visually-hidden">Vorige</span>
        <span>&laquo;</span>
      </router-link>
    </span>
    <span role="group" class="utrecht-pagination__pages">
      <router-link
        v-for="link in links"
        :key="link.number"
        :class="[
          'utrecht-pagination__page-link',
          link.current && 'utrecht-pagination__page-link--current'
        ]"
        :to="link.href"
        :aria-current="link.current ? 'true' : undefined"
        :title="link.title"
      >
        {{ link.number }}
      </router-link>
    </span>
    <span class="utrecht-pagination__before volgende">
      <router-link
        v-if="next"
        :to="next.href"
        class="utrecht-pagination__relative-link utrecht-pagination__relative-link--next"
        rel="next"
      >
        <span class="visually-hidden">Volgende</span>
        <span>&raquo;</span>
      </router-link>
    </span>
  </nav>
</template>

<script setup lang="ts">
import type { RouteLocationRaw } from "vue-router";
type UtrechtPageLink = {
  href: RouteLocationRaw;
  title: string;
  current: boolean;
  number: number;
};

export type UtrechtPaginationProps = {
  links: UtrechtPageLink[];
  prev?: UtrechtPageLink;
  next?: UtrechtPageLink;
  currentIndex: number;
};

defineProps<UtrechtPaginationProps>();
</script>

<style lang="scss" scoped>
@import "@/assets/variables.scss";

.vorige {
  min-inline-size: 5ch;
  display: inline-block;
}

.volgende {
  min-inline-size: 5.5ch;
  display: inline-block;
  text-align: right;
}

.utrecht-pagination__page-link {
  min-inline-size: 2ch;
  text-align: center;

  &:not(.utrecht-pagination__page-link--current) {
    display: none;

    @media screen and (min-width: $breakpoint-md) {
      display: inline-block;
    }
  }
}
</style>
