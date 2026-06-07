<template>
  <div class="catalog-page bg-grey-lighten-4">
    <v-container class="pa-4 pa-md-6 pb-6" fluid>
      <div
        v-if="headingRowVisible"
        class="d-flex flex-wrap align-center ga-2 mb-3"
      >
        <h2
          v-if="showPageTitle"
          class="text-h6 font-weight-medium text-truncate mb-0"
        >
          {{ pageTitle }}
        </h2>
        <v-spacer />
        <div
          v-if="statChips?.length"
          class="d-flex flex-wrap align-center ga-2"
        >
          <v-chip
            v-for="(chip, i) in statChips"
            :key="i"
            label
            size="small"
            :variant="chip.variant ?? 'tonal'"
          >
            {{ chip.label }}
          </v-chip>
        </div>
      </div>

      <v-card class="catalog-main-card" elevation="1" rounded="lg">
        <slot />
      </v-card>
    </v-container>
  </div>
</template>

<script lang="ts" setup>
  import { computed } from 'vue'

  export interface CatalogStatChip {
    label: string
    variant?: 'flat' | 'text' | 'elevated' | 'tonal' | 'outlined' | 'plain'
  }

  const props = withDefaults(
    defineProps<{
      pageTitle?: string
      statChips?: CatalogStatChip[]
      hideTitle?: boolean
    }>(),
    {
      pageTitle: '',
      hideTitle: false,
    },
  )

  const showPageTitle = computed(
    () => !props.hideTitle && props.pageTitle.trim() !== '',
  )

  const headingRowVisible = computed(
    () =>
      showPageTitle.value
      || (props.statChips != null && props.statChips.length > 0),
  )
</script>

<style scoped>
.catalog-page {
  min-height: calc(100vh - 64px);
}

.catalog-main-card :deep(.v-data-table) {
  background: transparent;
}

.catalog-main-card :deep(.shell-list-hint) {
  font-size: 15px !important;
  line-height: 1.5;
  letter-spacing: 0.02em;
  color: #1b5e20 !important;
  font-weight: 500;
}

.catalog-main-card :deep(.shell-list-hint strong) {
  color: #0d3d10 !important;
  font-weight: 700;
}
</style>

<style>
.shell-list-hint {
  font-size: 15px;
  line-height: 1.5;
  letter-spacing: 0.02em;
  color: #1b5e20;
  font-weight: 500;
}

.shell-list-hint strong {
  color: #0d3d10;
  font-weight: 700;
}
</style>
