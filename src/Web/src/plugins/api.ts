import { ofetch } from 'ofetch'

import { getJwtClaim } from '@/utils/jwt'

export const AUTH_STORAGE_KEY = 'saas_auth'

export interface PersistedAuth {
  token: string
  email: string
  role: string
  workspaceSlug: string
  userDisplayId: string
  tenantDisplayId: string
  tenantId: string
}

export interface ApiError extends Error {
  title: string
  detail?: string
  status: number
}

export function clearLegacyAuthLocalStorage (): void {
  try {
    localStorage.removeItem(AUTH_STORAGE_KEY)
  }
  catch {
    /* storage blocked */
  }
}

function readPersistedAuth (): { token: string | null, tenantId: string | null } {
  try {
    const raw = sessionStorage.getItem(AUTH_STORAGE_KEY)
    if (!raw) {
      return { token: null, tenantId: null }
    }
    const parsed = JSON.parse(raw) as Partial<PersistedAuth>
    const token = typeof parsed.token === 'string' ? parsed.token : null
    const tenantId = typeof parsed.tenantId === 'string' && parsed.tenantId
      ? parsed.tenantId
      : (token ? getJwtClaim(token, 'tenant_id') : null)
    return { token, tenantId }
  }
  catch {
    return { token: null, tenantId: null }
  }
}

function problemFromBody (
  data: unknown,
  statusText = '',
): Pick<ApiError, 'title' | 'detail'> {
  const fallbackTitle = statusText || 'Error'
  if (!data || typeof data !== 'object') {
    return { title: fallbackTitle }
  }
  const d = data as Record<string, unknown>
  const title = typeof d.title === 'string' ? d.title : fallbackTitle
  let detail: string | undefined
  if (typeof d.detail === 'string') {
    detail = d.detail
  }
  if (typeof d.message === 'string' && detail === undefined) {
    detail = d.message
  }
  return { title, detail }
}

function resolveApiBaseUrl (): string {
  const fromEnv = import.meta.env.VITE_API_BASE_URL
  if (fromEnv != null && fromEnv !== '') {
    return fromEnv
  }
  if (import.meta.env.DEV) {
    return ''
  }
  return ''
}

export const $api = ofetch.create({
  baseURL: resolveApiBaseUrl(),
  onRequest ({ options }) {
    const { token, tenantId } = readPersistedAuth()
    const headers = new Headers(options.headers as HeadersInit)
    if (token) {
      headers.set('Authorization', `Bearer ${token}`)
    }
    if (tenantId) {
      headers.set('X-Tenant-ID', tenantId)
    }
    options.headers = headers
  },
  async onResponseError ({ response }) {
    const status = response.status
    const { title, detail } = problemFromBody(response._data, response.statusText)
    const err = new Error(detail ?? title) as ApiError
    err.name = 'ApiError'
    err.title = title
    err.detail = detail
    err.status = status
    throw err
  },
})

export function isApiError (e: unknown): e is ApiError {
  return e instanceof Error && 'status' in e && typeof (e as ApiError).status === 'number'
}
