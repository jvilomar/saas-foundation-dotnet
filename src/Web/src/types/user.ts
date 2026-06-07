export interface User {
  displayId: string
  email: string
  tenantId: string
  roleId: string
  roleName: string
  createdAt: string
  lastModifiedAt: string | null
}

export interface CreateUserRequest {
  email: string
  password: string
  roleId: string
}

export interface UpdateUserRequest {
  email: string
  roleId: string
}
