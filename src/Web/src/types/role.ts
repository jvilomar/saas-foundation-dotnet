export interface Role {
  id: string
  name: string
  description: string | null
  isSystem: boolean
  createdAt: string
  lastModifiedAt: string | null
}

export interface CreateRoleRequest {
  name: string
  description?: string | null
}

export interface UpdateRoleRequest {
  name: string
  description?: string | null
}
