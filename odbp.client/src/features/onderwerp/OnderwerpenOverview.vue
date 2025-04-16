<template>
  <utrecht-heading :level="1">Onderwerpen</utrecht-heading>

  <utrecht-heading :level="2">Gepromoot</utrecht-heading>
  
  <ul>
    <li v-for="{ uuid, officieleTitel, omschrijving } in promoted" :key="uuid">
      <router-link :to="{ name: 'onderwerp', params: { uuid } }" :title="officieleTitel">
        <h2>{{ officieleTitel }}</h2>
      </router-link>

      <utrecht-paragraph>{{ omschrijving }}</utrecht-paragraph>
    </li>
  </ul>

  <utrecht-heading :level="2">Alle onderwerpen</utrecht-heading>

  <ul>
    <li v-for="{ uuid, officieleTitel, omschrijving } in lijsten?.onderwerpen" :key="uuid">
      <router-link :to="{ name: 'onderwerp', params: { uuid } }" :title="officieleTitel">
        <h2>{{ officieleTitel }}</h2>
      </router-link>

      <utrecht-paragraph>{{ omschrijving }}</utrecht-paragraph>
    </li>
  </ul>
</template>

<script setup lang="ts">
import { lijsten } from "@/stores/lijsten";
import { computed } from "vue";

const promoted = computed(() => lijsten.value?.onderwerpen.filter((o) => o.promoot));
</script>

<style lang="scss" scoped>
ul {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(20rem, 1fr));
  grid-gap: 1rem;
}
</style>
