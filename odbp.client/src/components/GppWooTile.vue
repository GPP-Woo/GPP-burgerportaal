<template>
  <section>
    <figure>
      <img v-if="image" :src="image" :alt="`Afbeelding ${title}`" crossorigin="anonymous" />

      <span v-else>Geen afbeelding</span>
    </figure>

    <utrecht-heading :level="level ?? 3">
      <router-link :to="link" class="utrecht-link utrecht-link--html-a">
        {{ title }}
      </router-link>
    </utrecht-heading>

    <utrecht-paragraph v-if="description" class="gpp-woo-pre-wrap">{{
      description
    }}</utrecht-paragraph>
  </section>
</template>

<script setup lang="ts">
export type Tile = {
  link: string;
  title: string;
  description?: string;
  image?: string;
  level?: 2 | 3 | 4;
};

defineProps<Tile>();
</script>

<style lang="scss" scoped>
[class^="utrecht-heading"] {
  --utrecht-space-around: 1;
}

.utrecht-paragraph {
  --utrecht-space-around: 1;
}

figure {
  display: flex;
  aspect-ratio: 16/9;
  border: 1px solid var(--utrecht-color-grey-80);
  margin: 0;

  img {
    width: 100%;
    object-fit: cover;
  }

  span {
    margin-inline-start: 1rem;
    margin-block-start: 1rem;
  }
}

section {
  position: relative;

  .utrecht-link:after {
    content: "";
    display: block;
    position: absolute;
    inset: 0;
  }
}
</style>
