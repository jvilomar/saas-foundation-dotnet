<template>
  <v-navigation-drawer
    app
    class="no-print app-drawer"
    location="start"
    permanent
    :rail="collapsed"
    width="260"
  >
    <div
      class="app-drawer__brand"
      :class="collapsed ? 'app-drawer__brand--rail' : 'app-drawer__brand--expanded'"
    >
      <v-btn
        :aria-label="collapsed ? t('nav.expandMenu') : t('nav.collapseMenu')"
        class="app-drawer__toggle"
        :icon="collapsed ? 'mdi-chevron-right' : 'mdi-chevron-left'"
        variant="text"
        @click="collapsed = !collapsed"
      />
      <RouterLink
        :aria-label="t('nav.goHome')"
        class="app-drawer__link text-decoration-none"
        to="/dashboard"
      >
        <v-icon color="primary" icon="mdi-cloud-outline" size="36" />
        <span v-if="!collapsed" class="app-drawer__name text-primary font-weight-bold">
          {{ t('app.name') }}
        </span>
      </RouterLink>
    </div>

    <v-divider />

    <v-list class="pt-2" density="comfortable" nav>
      <v-list-item
        :active="route.name === 'dashboard'"
        link
        prepend-icon="mdi-view-dashboard-outline"
        :title="t('nav.home')"
        to="/dashboard"
      />
      <v-list-item
        v-if="showUsersNav"
        :active="route.name === 'users'"
        link
        prepend-icon="mdi-account-group-outline"
        :title="t('nav.users')"
        to="/users"
      />
      <v-list-item
        v-if="showRolesNav"
        :active="route.name === 'roles'"
        link
        prepend-icon="mdi-shield-account-outline"
        :title="t('nav.roles')"
        to="/roles"
      />
    </v-list>

    <template #append>
      <v-divider />
      <v-list density="comfortable" nav>
        <v-list-item
          link
          prepend-icon="mdi-logout"
          :title="t('nav.logout')"
          @click="onLogout"
        />
      </v-list>
    </template>
  </v-navigation-drawer>

  <v-app-bar app border="b" class="no-print" flat>
    <v-app-bar-title>{{ pageTitle }}</v-app-bar-title>
    <v-spacer />
    <v-menu location="bottom end">
      <template #activator="{ props: menuProps }">
        <v-btn v-bind="menuProps" aria-label="User menu" icon variant="text">
          <v-avatar color="primary" size="32">
            <span class="text-caption font-weight-bold text-white">{{ userInitials }}</span>
          </v-avatar>
        </v-btn>
      </template>
      <v-list density="compact" min-width="240">
        <v-list-item>
          <v-list-item-title class="font-weight-medium">
            {{ authStore.email ?? 'User' }}
          </v-list-item-title>
          <v-list-item-subtitle v-if="authStore.role">
            {{ authStore.role }}
          </v-list-item-subtitle>
        </v-list-item>
        <v-divider class="my-1" />
        <v-list-item
          prepend-icon="mdi-logout"
          :title="t('nav.logout')"
          @click="onLogout"
        />
      </v-list>
    </v-menu>
  </v-app-bar>

  <v-main>
    <router-view />
  </v-main>
</template>

<script lang="ts" setup>
  import { computed, ref, watch } from 'vue'
  import { useI18n } from 'vue-i18n'
  import { RouterLink, useRoute } from 'vue-router'

  import { useAuthStore } from '@/stores/authStore'
  import { canManageRoles, canManageUsers } from '@/utils/roleAccess'

  const { t } = useI18n()
  const authStore = useAuthStore()
  const route = useRoute()
  const collapsed = ref(false)

  const showUsersNav = computed(() => canManageUsers(authStore.role))
  const showRolesNav = computed(() => canManageRoles(authStore.role))

  const userInitials = computed(() => {
    const e = (authStore.email ?? '').trim()
    if (!e) {
      return '?'
    }
    const local = e.split('@')[0] ?? e
    const parts = local.split(/[.\s_-]+/).filter(Boolean)
    if (parts.length >= 2) {
      return (parts[0]![0]! + parts[1]![0]!).toUpperCase()
    }
    return local.slice(0, 2).toUpperCase()
  })

  const pageTitle = computed(() => {
    const key = route.meta.titleKey
    return typeof key === 'string' ? t(key) : t('app.name')
  })

  function onLogout () {
    void authStore.logout()
  }

  watch(
    () => route.meta.titleKey,
    titleKey => {
      const page = typeof titleKey === 'string' ? t(titleKey) : t('app.name')
      document.title = `${page} · ${t('app.title')}`
    },
    { immediate: true },
  )
</script>

<style scoped>
.app-drawer__brand {
  display: flex;
  align-items: center;
  padding: 12px;
  gap: 8px;
  min-height: 64px;
}

.app-drawer__brand--expanded {
  position: relative;
  justify-content: center;
}

.app-drawer__brand--expanded .app-drawer__toggle {
  position: absolute;
  left: 4px;
  top: 50%;
  transform: translateY(-50%);
}

.app-drawer__brand--rail {
  flex-direction: column;
}

.app-drawer__link {
  display: flex;
  align-items: center;
  gap: 10px;
  color: inherit;
}

.app-drawer__name {
  font-size: 1rem;
  letter-spacing: 0.02em;
}
</style>
