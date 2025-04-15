<template>
  <div role="region" class="utrecht-table__container gpp-woo-table__container">
    <slot></slot>
  </div>
</template>

<style lang="scss" scoped>
.gpp-woo-table__container {
  overflow-inline: auto;

  &:focus-visible {
    outline-color: var(--utrecht-focus-outline-color);
    outline-offset: var(--utrecht-focus-outline-offset);
    outline-style: var(--utrecht-focus-outline-style);
    outline-width: var(--utrecht-focus-outline-width);
  }

  :deep(table) {
    min-width: var(--gpp-woo-table-container-table-min-width);

    th[scope="row"] {
      inline-size: var(--gpp-woo-table-container-header-inline-size);
    }

    .gpp-woo-table-fixed-header {
      min-inline-size: var(--gpp-woo-table-container-header-inline-size);
    }
  }
}
</style>
