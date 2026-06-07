import { createRouter, createWebHistory } from 'vue-router'

import AppLayout from '@/layouts/AppLayout.vue'
import { useAuthStore } from '@/stores/authStore'
import Dashboard from '@/views/Dashboard.vue'
import Login from '@/views/Login.vue'
import RolesView from '@/views/RolesView.vue'
import UsersView from '@/views/UsersView.vue'
import { RoleNames, hasAnyRole } from '@/utils/roleAccess'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/login',
      name: 'login',
      component: Login,
      meta: { requiresAuth: false, titleKey: 'login.title' },
    },
    {
      path: '/',
      component: AppLayout,
      meta: { requiresAuth: true },
      children: [
        { path: '', redirect: { name: 'dashboard' } },
        {
          path: 'dashboard',
          name: 'dashboard',
          component: Dashboard,
          meta: { titleKey: 'dashboard.title' },
        },
        {
          path: 'users',
          name: 'users',
          component: UsersView,
          meta: {
            titleKey: 'users.title',
            requiredRoles: [RoleNames.SuperAdmin, RoleNames.TenantAdmin],
          },
        },
        {
          path: 'roles',
          name: 'roles',
          component: RolesView,
          meta: {
            titleKey: 'roles.title',
            requiredRoles: [RoleNames.SuperAdmin],
          },
        },
      ],
    },
    {
      path: '/:pathMatch(.*)*',
      redirect: '/login',
    },
  ],
})

router.beforeEach(to => {
  const auth = useAuthStore()
  const needsAuth = to.matched.some(record => record.meta.requiresAuth === true)

  if (needsAuth && !auth.isAuthenticated) {
    return { path: '/login', query: { redirect: to.fullPath } }
  }

  if (to.name === 'login' && auth.isAuthenticated) {
    return { path: '/dashboard' }
  }

  const requiredRoles = to.matched
    .map(record => record.meta.requiredRoles)
    .filter((roles): roles is string[] => Array.isArray(roles))
    .flat()

  if (requiredRoles.length > 0 && !hasAnyRole(auth.role, requiredRoles)) {
    return { path: '/dashboard', query: { forbidden: '1' } }
  }

  return true
})

export default router
