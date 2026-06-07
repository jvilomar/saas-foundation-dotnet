import { $api } from '@/plugins/api'
import type { CreateRoleRequest, Role, UpdateRoleRequest } from '@/types/role'

const base = '/api/roles'

export async function getAllRoles (): Promise<Role[]> {
  return $api<Role[]>(base)
}

export async function getRoleById (id: string): Promise<Role> {
  return $api<Role>(`${base}/${id}`)
}

export async function createRole (request: CreateRoleRequest): Promise<Role> {
  return $api<Role>(base, { method: 'POST', body: request })
}

export async function updateRole (id: string, request: UpdateRoleRequest): Promise<Role> {
  return $api<Role>(`${base}/${id}`, { method: 'PUT', body: request })
}

export async function deleteRole (id: string): Promise<void> {
  await $api(`${base}/${id}`, { method: 'DELETE' })
}
