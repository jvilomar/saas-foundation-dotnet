export const RoleNames = {
  SuperAdmin: 'SuperAdmin',
  TenantAdmin: 'TenantAdmin',
  User: 'User',
} as const

export function hasAnyRole (userRole: string | null | undefined, allowed: string[]): boolean {
  return userRole != null && allowed.length > 0 && allowed.includes(userRole)
}

export function canManageUsers (userRole: string | null | undefined): boolean {
  return hasAnyRole(userRole, [RoleNames.SuperAdmin, RoleNames.TenantAdmin])
}

export function canManageRoles (userRole: string | null | undefined): boolean {
  return hasAnyRole(userRole, [RoleNames.SuperAdmin])
}
