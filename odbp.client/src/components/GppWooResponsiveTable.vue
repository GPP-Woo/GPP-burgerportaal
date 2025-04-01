<template>
  <div class="gpp-woo-responsive-table">
    <slot></slot>
  </div>
</template>

<style lang="scss" scoped>
.gpp-woo-responsive-table {
  overflow-x: auto;

  :deep(table) {
    min-width: var(--gpp-woo-responsive-table-min-width);

    th[scope="row"] {
      inline-size: var(--gpp-woo-responsive-table-header-inline-size);
    }
  }
}
</style>
