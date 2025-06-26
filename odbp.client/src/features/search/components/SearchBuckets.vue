<template>
  <utrecht-fieldset v-if="buckets?.length" role="group" class="gpp-woo-search-buckets">
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

        <span class="gpp-woo-search-buckets__name">
          {{
            bucket.naam in resultOptions
              ? resultOptions[bucket.naam as ResultType].label
              : bucket.naam
          }}

          <gpp-woo-info-popover v-if="bucket.omschrijving">
            <template #trigger="{ triggerProps }">
              <utrecht-button
                v-bind="triggerProps"
                appearance="primary-action-button"
                class="gpp-woo-info-popover__trigger"
              >
                <utrecht-icon icon="question" />
              </utrecht-button>
            </template>

            <utrecht-paragraph class="gpp-woo-info-popover__content gpp-woo-pre-wrap">{{
              bucket.omschrijving
            }}</utrecht-paragraph>
          </gpp-woo-info-popover>
        </span>

        <span class="gpp-woo-search-buckets__count">({{ bucket.count }})</span>
      </utrecht-form-label>
    </utrecht-form-field>
  </utrecht-fieldset>
</template>

<script setup lang="ts">
import { useModel } from "vue";
import { resultOptions, type Bucket, type ResultType, type ResultTypeBucket } from "../service";
import GppWooInfoPopover from "@/components/GppWooInfoPopover.vue";
import UtrechtIcon from "@/components/UtrechtIcon.vue";

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
  --utrecht-space-around: var(--gpp-woo-search-buckets-fieldset-space-around);
}

.utrecht-form-field--checkbox {
  display: block;
}

.utrecht-form-label--checkbox {
  display: flex;
  align-items: flex-start;
  column-gap: var(--gpp-woo-search-buckets-form-label-column-gap);

  .gpp-woo-search-buckets__name {
    flex: 1;
  }
}

.gpp-woo-info-popover__trigger {
  --utrecht-icon-size: var(--gpp-woo-search-buckets-info-icon-size);
  --utrecht-button-min-block-size: var(--gpp-woo-search-buckets-info-button-size);
  --utrecht-button-min-inline-size: var(--gpp-woo-search-buckets-info-button-size);
  --utrecht-button-inline-size: var(--gpp-woo-search-buckets-info-button-size);

  padding: 0;
  border: none;
  border-radius: 50%;
  vertical-align: top;
}

.gpp-woo-info-popover__content {
  --utrecht-paragraph-margin-block-start: 0;
  --utrecht-paragraph-margin-block-end: 0;

  cursor: text;
}
</style>
