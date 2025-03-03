<template>
  <utrecht-fieldset v-if="buckets?.length">
    <utrecht-legend>{{ legend }}</utrecht-legend>

    <utrecht-form-field v-for="{ uuid, naam, count } in buckets" :key="uuid" type="checkbox">
      <utrecht-form-label type="checkbox" :checked="model.includes(uuid)">
        <input
          type="checkbox"
          class="utrecht-checkbox utrecht-checkbox--html-input utrecht-checkbox--custom"
          v-model="model"
          :value="uuid"
          @change="$emit('change')"
        />
        {{ naam }} ({{ count }})
      </utrecht-form-label>
    </utrecht-form-field>
  </utrecht-fieldset>
</template>

<script setup lang="ts">
import { useModel } from "vue";
import type { Bucket } from "../service";

const props = defineProps<{
  legend: string;
  buckets?: Readonly<Bucket[]>;
  modelValue: string[];
}>();

const model = useModel(props, "modelValue");
</script>

<style lang="scss" scoped>
.utrecht-form-field--checkbox {
  display: block;
}

.utrecht-form-label--checkbox {
  display: flex;
  align-items: flex-start;
  column-gap: 0.5rem;
}
</style>
