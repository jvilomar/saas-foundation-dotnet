<template>
  <v-main class="login-surface">
    <div class="login-center">
      <v-card border class="login-card" flat rounded="lg">
        <v-progress-linear
          v-if="isSubmitting"
          absolute
          color="primary"
          height="3"
          indeterminate
        />

        <div class="login-card__header text-center pa-6">
          <v-icon color="primary" icon="mdi-cloud-lock-outline" size="48" />
          <div class="text-h5 font-weight-bold mt-3">
            {{ t('login.title') }}
          </div>
          <div class="text-body-2 text-medium-emphasis mt-1">
            {{ t('login.subtitle') }}
          </div>
        </div>

        <v-divider />

        <v-card-text class="pa-6">
          <v-alert
            v-if="errorMsg && !isSubmitting && !isBootstrapping"
            class="mb-4"
            density="comfortable"
            type="error"
            variant="tonal"
          >
            {{ errorMsg }}
          </v-alert>

          <div
            v-if="isBootstrapping || isSubmitting"
            :aria-busy="true"
            :aria-label="isSubmitting ? t('login.signingIn') : t('login.loadingForm')"
          >
            <p v-if="isSubmitting" class="text-body-2 text-medium-emphasis mb-4">
              {{ t('login.connecting') }}
            </p>
            <v-skeleton-loader class="mb-4" type="text@2" />
            <v-skeleton-loader class="mb-4" type="text@2" />
            <v-skeleton-loader type="button" width="140" />
          </div>

          <v-form
            v-else
            ref="loginForm"
            @submit.prevent="onSubmit"
          >
            <v-text-field
              v-model="workspaceSlug"
              class="mb-3"
              density="comfortable"
              :hint="t('login.workspaceHint')"
              :label="t('login.workspace')"
              name="workspace"
              persistent-hint
              :rules="[rules.required, rules.slug]"
              variant="outlined"
            />

            <v-text-field
              v-model="email"
              autocomplete="username"
              autofocus
              class="mb-3"
              density="comfortable"
              :label="t('login.email')"
              name="email"
              type="email"
              :rules="[rules.required]"
              variant="outlined"
            />

            <v-text-field
              v-model="password"
              :append-inner-icon="showPassword ? 'mdi-eye-off' : 'mdi-eye'"
              autocomplete="current-password"
              class="mb-4"
              density="comfortable"
              :label="t('login.password')"
              name="password"
              :rules="[rules.required]"
              :type="showPassword ? 'text' : 'password'"
              variant="outlined"
              @click:append-inner="showPassword = !showPassword"
            />

            <div class="d-flex align-center justify-space-between flex-wrap ga-3">
              <v-checkbox
                v-model="rememberEmail"
                color="primary"
                density="comfortable"
                hide-details
                :label="t('login.rememberEmail')"
              />
              <v-btn
                color="primary"
                min-width="140"
                size="large"
                type="submit"
                variant="flat"
              >
                {{ t('login.submit') }}
              </v-btn>
            </div>
          </v-form>
        </v-card-text>
      </v-card>
    </div>
  </v-main>
</template>

<script lang="ts" setup>
  import type { VForm } from 'vuetify/components'
  import { nextTick, onMounted, ref } from 'vue'
  import { useI18n } from 'vue-i18n'

  import { isApiError } from '@/plugins/api'
  import { useAuthStore } from '@/stores/authStore'

  const REMEMBER_EMAIL_KEY = 'saas_remember_email'
  const REMEMBER_WORKSPACE_KEY = 'saas_remember_workspace'

  const { t } = useI18n()
  const authStore = useAuthStore()

  const loginForm = ref<VForm | null>(null)
  const email = ref('')
  const password = ref('')
  const workspaceSlug = ref('')
  const errorMsg = ref<string | null>(null)
  const isSubmitting = ref(false)
  const isBootstrapping = ref(true)
  const showPassword = ref(false)
  const rememberEmail = ref(true)

  const slugPattern = /^[a-z0-9]+(?:-[a-z0-9]+)*$/

  const rules = {
    required: (v: string) => (v != null && String(v).trim().length > 0) || t('validation.required'),
    slug: (v: string) => slugPattern.test(String(v).trim().toLowerCase()) || t('validation.invalidSlug'),
  }

  onMounted(async () => {
    try {
      const savedEmail = localStorage.getItem(REMEMBER_EMAIL_KEY)
      const savedWorkspace = localStorage.getItem(REMEMBER_WORKSPACE_KEY)
      if (savedEmail) {
        email.value = savedEmail
      }
      if (savedWorkspace) {
        workspaceSlug.value = savedWorkspace
      }
    }
    finally {
      await nextTick()
      isBootstrapping.value = false
    }
  })

  async function onSubmit () {
    errorMsg.value = null
    const form = loginForm.value
    const { valid } = (await form?.validate()) ?? { valid: false }
    if (!valid) {
      return
    }

    isSubmitting.value = true
    try {
      await authStore.login(
        email.value.trim(),
        password.value,
        workspaceSlug.value.trim().toLowerCase(),
      )
      if (rememberEmail.value) {
        localStorage.setItem(REMEMBER_EMAIL_KEY, email.value.trim())
        localStorage.setItem(REMEMBER_WORKSPACE_KEY, workspaceSlug.value.trim().toLowerCase())
      }
      else {
        localStorage.removeItem(REMEMBER_EMAIL_KEY)
        localStorage.removeItem(REMEMBER_WORKSPACE_KEY)
      }
    }
    catch (error: unknown) {
      if (isApiError(error)) {
        errorMsg.value = error.detail ?? error.title
      }
      else if (error instanceof Error) {
        const m = error.message
        errorMsg.value = /fetch|Load failed|Failed to fetch|NetworkError/i.test(m)
          ? t('login.errorNetwork')
          : m
      }
      else {
        errorMsg.value = t('login.errorGeneric')
      }
    }
    finally {
      isSubmitting.value = false
    }
  }
</script>

<style scoped>
.login-surface {
  min-height: 100dvh;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 24px;
  background: rgb(var(--v-theme-background));
}

.login-center {
  width: 100%;
  max-width: 440px;
}

.login-card {
  position: relative;
  overflow: hidden;
}
</style>
