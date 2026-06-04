import { defineStore } from 'pinia'
import { computed, ref } from 'vue'

import {
  $api,
  AUTH_STORAGE_KEY,
  clearLegacyAuthLocalStorage,
  type PersistedAuth,
} from '@/plugins/api'
import { getJwtClaim } from '@/utils/jwt'

interface LoginResponse {
  token: string
  email: string
  role: string
  userDisplayId: string
  tenantDisplayId: string
  workspaceSlug: string
}

function persistAuth (payload: PersistedAuth) {
  sessionStorage.setItem(AUTH_STORAGE_KEY, JSON.stringify(payload))
}

function clearPersistedAuth () {
  sessionStorage.removeItem(AUTH_STORAGE_KEY)
}

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(null)
  const email = ref<string | null>(null)
  const role = ref<string | null>(null)
  const workspaceSlug = ref<string | null>(null)
  const userDisplayId = ref<string | null>(null)
  const tenantDisplayId = ref<string | null>(null)
  const tenantId = ref<string | null>(null)

  const isAuthenticated = computed(() => token.value !== null)

  function restoreSession () {
    clearLegacyAuthLocalStorage()
    try {
      const raw = sessionStorage.getItem(AUTH_STORAGE_KEY)
      if (!raw) {
        return
      }
      const parsed = JSON.parse(raw) as PersistedAuth
      if (!parsed?.token || typeof parsed.token !== 'string') {
        clearPersistedAuth()
        return
      }
      token.value = parsed.token
      email.value = typeof parsed.email === 'string' ? parsed.email : null
      role.value = typeof parsed.role === 'string' ? parsed.role : null
      workspaceSlug.value = typeof parsed.workspaceSlug === 'string' ? parsed.workspaceSlug : null
      userDisplayId.value = typeof parsed.userDisplayId === 'string' ? parsed.userDisplayId : null
      tenantDisplayId.value = typeof parsed.tenantDisplayId === 'string' ? parsed.tenantDisplayId : null
      tenantId.value = typeof parsed.tenantId === 'string'
        ? parsed.tenantId
        : getJwtClaim(parsed.token, 'tenant_id')
    }
    catch {
      clearPersistedAuth()
    }
  }

  async function login (loginEmail: string, password: string, loginWorkspaceSlug: string) {
    const body = {
      workspaceSlug: loginWorkspaceSlug.trim().toLowerCase(),
      email: loginEmail.trim(),
      password,
    }
    const res = await $api<LoginResponse>('/api/auth/login', {
      method: 'POST',
      body,
    })
    const resolvedTenantId = getJwtClaim(res.token, 'tenant_id')

    token.value = res.token
    email.value = res.email
    role.value = res.role
    workspaceSlug.value = res.workspaceSlug
    userDisplayId.value = res.userDisplayId
    tenantDisplayId.value = res.tenantDisplayId
    tenantId.value = resolvedTenantId
    persistAuth({
      token: res.token,
      email: res.email,
      role: res.role,
      workspaceSlug: res.workspaceSlug,
      userDisplayId: res.userDisplayId,
      tenantDisplayId: res.tenantDisplayId,
      tenantId: resolvedTenantId ?? '',
    })
    const { default: r } = await import('@/router')
    const redirect = r.currentRoute.value.query.redirect
    const target
      = typeof redirect === 'string' && redirect.startsWith('/') ? redirect : '/dashboard'
    await r.push(target)
  }

  async function logout () {
    token.value = null
    email.value = null
    role.value = null
    workspaceSlug.value = null
    userDisplayId.value = null
    tenantDisplayId.value = null
    tenantId.value = null
    clearPersistedAuth()
    const { default: r } = await import('@/router')
    await r.push('/login')
  }

  return {
    token,
    email,
    role,
    workspaceSlug,
    userDisplayId,
    tenantDisplayId,
    tenantId,
    isAuthenticated,
    restoreSession,
    login,
    logout,
  }
})
