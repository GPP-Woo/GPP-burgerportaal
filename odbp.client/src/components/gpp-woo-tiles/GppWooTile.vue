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

    <utrecht-paragraph v-if="description" class="gpp-woo-pre-wrap">
      {{ truncatedDescription }}

      <router-link
        v-if="isTruncated"
        :to="link"
        :title="title"
        class="utrecht-link utrecht-link--html-a gpp-woo-link--icon"
        >Lees meer<utrecht-icon icon="chevron-right" />
      </router-link>
    </utrecht-paragraph>
  </section>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { truncate } from "@/helpers";
import UtrechtIcon from "@/components/UtrechtIcon.vue";

export type Tile = {
  link: string;
  title: string;
  description?: string;
  maxDescriptionLength?: number;
  image?: string;
  level?: 2 | 3 | 4;
};

const props = defineProps<Tile>();

const truncatedDescription = computed(() =>
  truncate(props.description, props.maxDescriptionLength)
);

const isTruncated = computed(
  () =>
    props.description &&
    props.maxDescriptionLength &&
    props.description.length > props.maxDescriptionLength
);
</script>

<style lang="scss" scoped>
section {
  position: relative;
}

[class^="utrecht-heading"] {
  --utrecht-space-around: 1;

  .utrecht-link:after {
    content: "";
    display: block;
    position: absolute;
    inset: 0;
  }
}

.utrecht-paragraph {
  --utrecht-space-around: 1;
  --utrecht-link-icon-size: 0.75rem;
}

figure {
  display: flex;
  border: 1px solid var(--utrecht-color-grey-80);
  margin: 0;

  img {
    inline-size: 100%;
    aspect-ratio: 16/9;
    object-fit: cover;
  }

  span {
    margin-inline-start: 1rem;
    margin-block-start: 1rem;
  }
}
</style>
