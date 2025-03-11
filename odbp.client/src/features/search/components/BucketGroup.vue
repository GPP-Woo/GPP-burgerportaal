<template>
  <utrecht-fieldset v-if="buckets?.length" role="group" class="gpp-woo-buckets-fieldset">
    <utrecht-legend>{{ legend }}</utrecht-legend>

    <utrecht-form-field v-for="(bucket, index) in buckets" :key="index" type="checkbox">
      <utrecht-form-label type="checkbox" :checked="model.includes(getBucketRef(bucket))">
        <input
          type="checkbox"
          class="utrecht-checkbox utrecht-checkbox--html-input utrecht-checkbox--custom"
          v-model="model"
          :value="getBucketRef(bucket)"
          @change="$emit('change')"
        />

        <span class="bucket-name">{{
          bucket.naam in resultOptions
            ? resultOptions[bucket.naam as ResultType].label
            : bucket.naam
        }}</span>

        <span class="bucket-count">({{ bucket.count }})</span>
      </utrecht-form-label>
    </utrecht-form-field>
  </utrecht-fieldset>
</template>

<script setup lang="ts">
import { useModel } from "vue";
import { resultOptions, type Bucket, type ResultType, type ResultTypeBucket } from "../service";

const props = defineProps<{
  legend: string;
  buckets?: Readonly<Bucket[] | ResultTypeBucket[]>;
  modelValue: string[];
}>();

const model = useModel(props, "modelValue");

const getBucketRef = (bucket: Bucket | ResultTypeBucket) =>
  "uuid" in bucket ? bucket.uuid : bucket.naam;
</script>

<style lang="scss" scoped>
.utrecht-form-fieldset {
  --utrecht-space-around: var(--gpp-woo-buckets-fieldset-space-around);
}

.utrecht-form-field--checkbox {
  display: block;
}

.utrecht-form-label--checkbox {
  display: flex;
  align-items: flex-start;
  column-gap: var(--gpp-woo-buckets-form-label-column-gap);

  .bucket-name {
    flex: 1;
  }
}
</style>
