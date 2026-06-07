<template>
  <CatalogPageShell
    hide-title
    :stat-chips="statChips"
  >
    <div class="pa-4 pb-2">
      <p class="shell-list-hint mb-0">
        Manage <strong>users</strong> within your workspace — assign <strong>roles</strong> and control access.
      </p>
    </div>

    <v-divider />

    <div class="pa-4 pt-3 pb-3">
      <p class="text-body-1 font-weight-medium mb-0">
        {{ users.length }} user(s) in this tenant
      </p>
    </div>

    <v-divider />

    <div class="catalog-toolbar pa-3 d-flex flex-wrap align-center ga-2">
      <v-btn
        color="primary"
        prepend-icon="mdi-plus"
        variant="flat"
        @click="openCreate"
      >
        {{ t('users.newUser') }}
      </v-btn>
      <v-spacer class="d-none d-sm-block" />
      <v-text-field
        v-model="filterText"
        class="catalog-search"
        clearable
        density="compact"
        hide-details
        :label="t('users.search')"
        prepend-inner-icon="mdi-magnify"
        single-line
        variant="outlined"
      />
      <v-btn
        :loading="loading"
        prepend-icon="mdi-refresh"
        variant="text"
        @click="fetchAll"
      >
        {{ t('common.refresh') }}
      </v-btn>
    </div>

    <v-alert
      v-if="error"
      class="ma-3"
      density="compact"
      type="error"
      variant="tonal"
    >
      {{ error }}
    </v-alert>

    <v-data-table
      class="users-table"
      density="compact"
      :headers="headers"
      :items="filteredUsers"
      :items-per-page="15"
      :loading="loading"
      must-sort
      sort-asc-icon="mdi-menu-up"
      sort-desc-icon="mdi-menu-down"
    >
      <template #item.email="{ item }">
        <span class="text-body-2 font-weight-medium">{{ item.email }}</span>
      </template>
      <template #item.displayId="{ item }">
        <span class="text-body-2 font-mono">{{ item.displayId }}</span>
      </template>
      <template #item.roleName="{ item }">
        <v-chip size="small" variant="tonal">
          {{ item.roleName }}
        </v-chip>
      </template>
      <template #item.createdAt="{ item }">
        <span class="text-body-2">{{ formatDate(item.createdAt) }}</span>
      </template>
      <template #item.actions="{ item }">
        <v-btn
          :aria-label="t('common.edit')"
          icon="mdi-pencil"
          size="small"
          variant="text"
          @click="openEdit(item)"
        />
        <v-btn
          :aria-label="t('common.delete')"
          color="error"
          icon="mdi-delete"
          size="small"
          variant="text"
          @click="askDelete(item)"
        />
      </template>
    </v-data-table>

    <v-dialog
      v-model="dialogOpen"
      max-width="480"
      scroll-strategy="close"
    >
      <v-card>
        <v-card-title class="text-h6">
          {{ editing ? t('users.editUser') : t('users.newUser') }}
        </v-card-title>
        <v-card-text>
          <v-text-field
            v-model="formEmail"
            density="compact"
            hide-details="auto"
            :label="t('users.email')"
            :rules="[rules.required]"
            type="email"
            variant="outlined"
          />
          <v-text-field
            v-if="!editing"
            v-model="formPassword"
            class="mt-3"
            density="compact"
            hide-details="auto"
            :label="t('users.password')"
            :rules="[rules.required]"
            type="password"
            variant="outlined"
          />
          <v-select
            v-model="formRoleId"
            class="mt-3"
            density="compact"
            hide-details="auto"
            item-title="name"
            item-value="id"
            :items="roles"
            :label="t('users.role')"
            :loading="rolesLoading"
            :rules="[rules.requiredSelect]"
            variant="outlined"
          />
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn variant="text" @click="dialogOpen = false">
            {{ t('common.cancel') }}
          </v-btn>
          <v-btn
            color="primary"
            :loading="saving"
            variant="flat"
            @click="saveUser"
          >
            {{ t('common.save') }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-dialog v-model="deleteOpen" max-width="420">
      <v-card>
        <v-card-title>{{ t('users.deleteTitle') }}</v-card-title>
        <v-card-text>
          {{ t('users.deleteConfirm', { email: deleteTarget?.email ?? '' }) }}
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn variant="text" @click="deleteOpen = false">
            {{ t('common.cancel') }}
          </v-btn>
          <v-btn
            color="error"
            :loading="deleteLoading"
            variant="flat"
            @click="confirmDelete"
          >
            {{ t('common.delete') }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-snackbar v-model="snackOk" color="success" location="bottom">
      {{ snackText }}
    </v-snackbar>
    <v-snackbar v-model="snackErr" color="error" location="bottom">
      {{ snackText }}
    </v-snackbar>
  </CatalogPageShell>
</template>

<script lang="ts" setup>
  import type { Role } from '@/types/role'
  import type { User } from '@/types/user'

  import { computed, onMounted, ref } from 'vue'
  import { useI18n } from 'vue-i18n'

  import CatalogPageShell from '@/components/CatalogPageShell.vue'
  import { isApiError } from '@/plugins/api'
  import * as roleService from '@/services/roleService'
  import * as userService from '@/services/userService'

  const { t } = useI18n()

  const users = ref<User[]>([])
  const roles = ref<Role[]>([])
  const loading = ref(false)
  const rolesLoading = ref(false)
  const saving = ref(false)
  const deleteLoading = ref(false)
  const error = ref('')
  const filterText = ref('')
  const dialogOpen = ref(false)
  const deleteOpen = ref(false)
  const editing = ref<User | null>(null)
  const deleteTarget = ref<User | null>(null)
  const formEmail = ref('')
  const formPassword = ref('')
  const formRoleId = ref<string | null>(null)
  const snackOk = ref(false)
  const snackErr = ref(false)
  const snackText = ref('')

  const statChips = computed(() => [
    { label: `${users.value.length} ${t('users.title').toLowerCase()}`, variant: 'tonal' as const },
  ])

  const filteredUsers = computed(() => {
    const q = filterText.value.trim().toLowerCase()
    if (!q) {
      return users.value
    }
    return users.value.filter(u =>
      u.email.toLowerCase().includes(q)
      || u.displayId.toLowerCase().includes(q)
      || u.roleName.toLowerCase().includes(q),
    )
  })

  const headers = computed(() => [
    { title: t('users.email'), key: 'email', sortable: true },
    { title: t('users.displayId'), key: 'displayId', sortable: true, width: 140 },
    { title: t('users.role'), key: 'roleName', sortable: true, width: 140 },
    { title: t('users.createdAt'), key: 'createdAt', sortable: true, width: 160 },
    { title: '', key: 'actions', sortable: false, width: 96 },
  ])

  const rules = {
    required: (v: string) => !!v?.trim() || t('validation.required'),
    requiredSelect: (v: string | null) => !!v || t('validation.required'),
  }

  function formatDate (iso: string): string {
    try {
      return new Date(iso).toLocaleString()
    }
    catch {
      return iso
    }
  }

  function notify (ok: boolean, message: string): void {
    snackText.value = message
    snackOk.value = ok
    snackErr.value = !ok
  }

  async function fetchRoles (): Promise<void> {
    rolesLoading.value = true
    try {
      roles.value = await roleService.getAllRoles()
    }
    catch {
      roles.value = []
    }
    finally {
      rolesLoading.value = false
    }
  }

  async function fetchAll (): Promise<void> {
    loading.value = true
    error.value = ''
    try {
      users.value = await userService.getAllUsers()
    }
    catch (e) {
      error.value = isApiError(e) ? (e.detail ?? e.title) : t('users.loadError')
    }
    finally {
      loading.value = false
    }
  }

  function openCreate (): void {
    editing.value = null
    formEmail.value = ''
    formPassword.value = ''
    formRoleId.value = roles.value[0]?.id ?? null
    dialogOpen.value = true
  }

  function openEdit (user: User): void {
    editing.value = user
    formEmail.value = user.email
    formPassword.value = ''
    formRoleId.value = user.roleId
    dialogOpen.value = true
  }

  async function saveUser (): Promise<void> {
    const email = formEmail.value.trim()
    if (!email || !formRoleId.value) {
      return
    }
    if (!editing.value && !formPassword.value.trim()) {
      return
    }
    saving.value = true
    try {
      if (editing.value) {
        await userService.updateUser(editing.value.displayId, {
          email,
          roleId: formRoleId.value,
        })
        notify(true, t('users.updated'))
      }
      else {
        await userService.createUser({
          email,
          password: formPassword.value,
          roleId: formRoleId.value,
        })
        notify(true, t('users.createdSuccess'))
      }
      dialogOpen.value = false
      await fetchAll()
    }
    catch (e) {
      notify(false, isApiError(e) ? (e.detail ?? e.title) : t('users.saveError'))
    }
    finally {
      saving.value = false
    }
  }

  function askDelete (user: User): void {
    deleteTarget.value = user
    deleteOpen.value = true
  }

  async function confirmDelete (): Promise<void> {
    const target = deleteTarget.value
    if (!target) {
      return
    }
    deleteLoading.value = true
    try {
      await userService.deleteUser(target.displayId)
      deleteOpen.value = false
      notify(true, t('users.deleted'))
      await fetchAll()
    }
    catch (e) {
      notify(false, isApiError(e) ? (e.detail ?? e.title) : t('users.deleteError'))
    }
    finally {
      deleteLoading.value = false
    }
  }

  onMounted(() => {
    void fetchRoles()
    void fetchAll()
  })
</script>

<style scoped>
.catalog-toolbar {
  border-bottom: thin solid rgba(var(--v-border-color), var(--v-border-opacity));
}

.catalog-search {
  flex: 1 1 220px;
  max-width: 320px;
}
</style>
