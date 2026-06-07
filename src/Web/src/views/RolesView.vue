<template>
  <CatalogPageShell
    hide-title
    :stat-chips="statChips"
  >
    <div class="pa-4 pb-2">
      <p class="shell-list-hint mb-0">
        Manage application <strong>roles</strong> and <strong>permissions</strong> for your organization.
      </p>
    </div>

    <v-divider />

    <div class="pa-4 pt-3 pb-3">
      <p class="text-body-1 font-weight-medium mb-0">
        {{ roles.length }} role(s) · {{ systemCount }} system
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
        {{ t('roles.newRole') }}
      </v-btn>
      <v-spacer class="d-none d-sm-block" />
      <v-text-field
        v-model="filterText"
        class="catalog-search"
        clearable
        density="compact"
        hide-details
        :label="t('roles.search')"
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
      class="roles-table"
      density="compact"
      :headers="headers"
      :items="filteredRoles"
      :items-per-page="15"
      :loading="loading"
      must-sort
      sort-asc-icon="mdi-menu-up"
      sort-desc-icon="mdi-menu-down"
    >
      <template #item.name="{ item }">
        <span class="text-body-2 font-weight-medium">{{ item.name }}</span>
      </template>
      <template #item.isSystem="{ item }">
        <v-chip
          :color="item.isSystem ? 'primary' : 'default'"
          label
          size="small"
          variant="tonal"
        >
          {{ item.isSystem ? t('roles.system') : t('roles.custom') }}
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
          :disabled="item.isSystem"
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
          {{ editing ? t('roles.editRole') : t('roles.newRole') }}
        </v-card-title>
        <v-card-text>
          <v-text-field
            v-model="formName"
            density="compact"
            hide-details="auto"
            :label="t('roles.name')"
            :rules="[rules.required]"
            variant="outlined"
          />
          <v-textarea
            v-model="formDescription"
            class="mt-3"
            density="compact"
            hide-details="auto"
            :label="t('roles.description')"
            rows="2"
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
            @click="saveRole"
          >
            {{ t('common.save') }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-dialog v-model="deleteOpen" max-width="420">
      <v-card>
        <v-card-title>{{ t('roles.deleteTitle') }}</v-card-title>
        <v-card-text>
          {{ t('roles.deleteConfirm', { name: deleteTarget?.name ?? '' }) }}
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

  import { computed, onMounted, ref } from 'vue'
  import { useI18n } from 'vue-i18n'

  import CatalogPageShell from '@/components/CatalogPageShell.vue'
  import { isApiError } from '@/plugins/api'
  import * as roleService from '@/services/roleService'

  const { t } = useI18n()

  const roles = ref<Role[]>([])
  const loading = ref(false)
  const saving = ref(false)
  const deleteLoading = ref(false)
  const error = ref('')
  const filterText = ref('')
  const dialogOpen = ref(false)
  const deleteOpen = ref(false)
  const editing = ref<Role | null>(null)
  const deleteTarget = ref<Role | null>(null)
  const formName = ref('')
  const formDescription = ref('')
  const snackOk = ref(false)
  const snackErr = ref(false)
  const snackText = ref('')

  const systemCount = computed(() => roles.value.filter(r => r.isSystem).length)

  const statChips = computed(() => [
    { label: `${roles.value.length} ${t('roles.title').toLowerCase()}`, variant: 'tonal' as const },
    { label: `${systemCount.value} ${t('roles.system').toLowerCase()}`, variant: 'tonal' as const },
  ])

  const filteredRoles = computed(() => {
    const q = filterText.value.trim().toLowerCase()
    if (!q) {
      return roles.value
    }
    return roles.value.filter(r =>
      r.name.toLowerCase().includes(q)
      || (r.description ?? '').toLowerCase().includes(q),
    )
  })

  const headers = computed(() => [
    { title: t('roles.name'), key: 'name', sortable: true },
    { title: t('roles.description'), key: 'description', sortable: true },
    { title: t('roles.type'), key: 'isSystem', sortable: true, width: 120 },
    { title: t('roles.createdAt'), key: 'createdAt', sortable: true, width: 160 },
    { title: '', key: 'actions', sortable: false, width: 96 },
  ])

  const rules = {
    required: (v: string) => !!v?.trim() || t('validation.required'),
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

  async function fetchAll (): Promise<void> {
    loading.value = true
    error.value = ''
    try {
      roles.value = await roleService.getAllRoles()
    }
    catch (e) {
      error.value = isApiError(e) ? (e.detail ?? e.title) : t('roles.loadError')
    }
    finally {
      loading.value = false
    }
  }

  function openCreate (): void {
    editing.value = null
    formName.value = ''
    formDescription.value = ''
    dialogOpen.value = true
  }

  function openEdit (role: Role): void {
    editing.value = role
    formName.value = role.name
    formDescription.value = role.description ?? ''
    dialogOpen.value = true
  }

  async function saveRole (): Promise<void> {
    const name = formName.value.trim()
    if (!name) {
      return
    }
    saving.value = true
    try {
      if (editing.value) {
        await roleService.updateRole(editing.value.id, {
          name,
          description: formDescription.value.trim() || null,
        })
        notify(true, t('roles.updated'))
      }
      else {
        await roleService.createRole({
          name,
          description: formDescription.value.trim() || null,
        })
        notify(true, t('roles.createdSuccess'))
      }
      dialogOpen.value = false
      await fetchAll()
    }
    catch (e) {
      notify(false, isApiError(e) ? (e.detail ?? e.title) : t('roles.saveError'))
    }
    finally {
      saving.value = false
    }
  }

  function askDelete (role: Role): void {
    deleteTarget.value = role
    deleteOpen.value = true
  }

  async function confirmDelete (): Promise<void> {
    const target = deleteTarget.value
    if (!target) {
      return
    }
    deleteLoading.value = true
    try {
      await roleService.deleteRole(target.id)
      deleteOpen.value = false
      notify(true, t('roles.deleted'))
      await fetchAll()
    }
    catch (e) {
      notify(false, isApiError(e) ? (e.detail ?? e.title) : t('roles.deleteError'))
    }
    finally {
      deleteLoading.value = false
    }
  }

  onMounted(() => {
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
