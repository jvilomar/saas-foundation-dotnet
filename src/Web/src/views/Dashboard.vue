<template>
  <v-container class="pa-4" fluid>
    <v-snackbar
      v-model="forbiddenSnackbar"
      color="warning"
      location="top"
      timeout="4000"
    >
      {{ t('dashboard.forbidden') }}
    </v-snackbar>

    <v-row>
      <v-col cols="12">
        <v-card rounded="lg" variant="outlined">
          <v-card-text class="pa-6">
            <div class="text-h5 font-weight-bold mb-2">
              {{ t('dashboard.welcome') }}{{ authStore.email ? `, ${authStore.email}` : '' }}
            </div>
            <p class="text-body-1 text-medium-emphasis mb-4">
              {{ t('dashboard.subtitle') }}
            </p>
            <v-divider class="mb-4" />
            <v-list density="compact">
              <v-list-item>
                <v-list-item-title>{{ t('dashboard.signedInAs') }}</v-list-item-title>
                <v-list-item-subtitle>{{ authStore.email ?? '—' }}</v-list-item-subtitle>
              </v-list-item>
              <v-list-item>
                <v-list-item-title>{{ t('dashboard.role') }}</v-list-item-title>
                <v-list-item-subtitle>{{ authStore.role ?? '—' }}</v-list-item-subtitle>
              </v-list-item>
              <v-list-item>
                <v-list-item-title>{{ t('dashboard.workspace') }}</v-list-item-title>
                <v-list-item-subtitle>{{ authStore.workspaceSlug ?? '—' }}</v-list-item-subtitle>
              </v-list-item>
              <v-list-item>
                <v-list-item-title>{{ t('dashboard.tenantId') }}</v-list-item-title>
                <v-list-item-subtitle class="font-mono text-caption">
                  {{ authStore.tenantDisplayId ?? '—' }}
                </v-list-item-subtitle>
              </v-list-item>
              <v-list-item>
                <v-list-item-title>{{ t('dashboard.userId') }}</v-list-item-title>
                <v-list-item-subtitle class="font-mono text-caption">
                  {{ authStore.userDisplayId ?? '—' }}
                </v-list-item-subtitle>
              </v-list-item>
            </v-list>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts" setup>
  import { onMounted, ref } from 'vue'
  import { useI18n } from 'vue-i18n'
  import { useRoute } from 'vue-router'

  import { useAuthStore } from '@/stores/authStore'

  const { t } = useI18n()
  const route = useRoute()
  const authStore = useAuthStore()
  const forbiddenSnackbar = ref(false)

  onMounted(() => {
    if (route.query.forbidden === '1') {
      forbiddenSnackbar.value = true
    }
  })
</script>
