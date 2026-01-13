<template>
  <h1>GPP-Woo Burgerportaal (beheer)</h1>

  <p>Aan deze functionaliteit wordt nog gewerkt.</p>

  <p v-if="!loading">{{ data?.welcome }}</p>
</template>

<script setup lang="ts">
import { useLoader } from "@/composables/use-loader";

const { data, loading } = useLoader<{ welcome: string }>(() =>
  fetch("/api/beheer/homepage", { headers: { "is-api": "true" } }).then((r) =>
    r.ok ? r.json() : null
  )
);
</script>

<style lang="scss" scoped></style>
