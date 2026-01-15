<template>
  <Suspense>
    <div class="wrapper" ref="wrapperRef">
      <textarea
        :value="modelValue"
        v-bind="$attrs"
        @focus="editableElement?.focus()"
        tabindex="-1"
      ></textarea>

      <ck-editor-async v-model="modelValue" v-bind="$attrs" />
    </div>

    <template #fallback>
      <simple-spinner />
    </template>
  </Suspense>
</template>

<script lang="ts">
export default {
  inheritAttrs: false
};
</script>

<script setup lang="ts">
import { computed, ref, useModel } from "vue";
import SimpleSpinner from "@/components/SimpleSpinner.vue";
import CkEditorAsync from "./CkEditorAsync.vue";

const props = defineProps<{ modelValue?: string }>();
const modelValue = useModel(props, "modelValue");

const wrapperRef = ref<HTMLDivElement>();

const editableElement = computed(() => {
  const el = wrapperRef.value?.querySelector("[contenteditable=true]");

  if (el && el instanceof HTMLElement) return el;

  return undefined;
});
</script>

<style lang="scss" scoped>
.wrapper {
  --ck-content-font-family: var(--utrecht-document-font-family);
  --ck-content-font-size: var(--utrecht-document-font-size);

  display: grid;
  grid-template-areas: "stacked";

  :deep(> *) {
    grid-area: stacked;
  }

  > textarea {
    opacity: 0;
  }
}
</style>
