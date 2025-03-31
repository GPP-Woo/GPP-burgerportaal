<template>
    <onderwerp-details :uuid="uuid" />
  </template>
  
  <script setup lang="ts">
  import OnderwerpDetails from "@/features/search/OnderwerpDetails.vue";
  
  defineProps<{ uuid: string }>();
  </script>
  