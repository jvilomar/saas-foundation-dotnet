import type { App } from 'vue'
import { createPinia } from 'pinia'

import router from '@/router'
import { useAuthStore } from '@/stores/authStore'

import { $api } from './api'
import i18n from './i18n'
import vuetify from './vuetify'

export function registerPlugins (app: App) {
  app.use(vuetify)
  app.use(createPinia())
  app.use(i18n)
  app.config.globalProperties.$api = $api
  app.use(router)
  useAuthStore().restoreSession()
}

export { $api } from './api'
