<template>
  <div role="region" class="gpp-woo-responsive-table">
    <slot></slot>
  </div>
</template>

<style lang="scss" scoped>
.gpp-woo-responsive-table {
  overflow-x: auto;

  &:focus-visible {
    outline-color: var(--utrecht-focus-outline-color);
    outline-offset: var(--utrecht-focus-outline-offset);
    outline-style: var(--utrecht-focus-outline-style);
    outline-width: var(--utrecht-focus-outline-width);
  }

  :deep(table) {
    min-width: var(--gpp-woo-responsive-table-min-width);

    th[scope="row"] {
      inline-size: var(--gpp-woo-responsive-table-header-inline-size);
    }
  }
}
</style>
