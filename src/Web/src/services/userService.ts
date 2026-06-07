import { $api } from '@/plugins/api'
import type { CreateUserRequest, UpdateUserRequest, User } from '@/types/user'

const base = '/api/users'

export async function getAllUsers (): Promise<User[]> {
  return $api<User[]>(base)
}

export async function getUserById (displayId: string): Promise<User> {
  return $api<User>(`${base}/${displayId}`)
}

export async function createUser (request: CreateUserRequest): Promise<User> {
  return $api<User>(base, { method: 'POST', body: request })
}

export async function updateUser (displayId: string, request: UpdateUserRequest): Promise<User> {
  return $api<User>(`${base}/${displayId}`, { method: 'PUT', body: request })
}

export async function deleteUser (displayId: string): Promise<void> {
  await $api(`${base}/${displayId}`, { method: 'DELETE' })
}
